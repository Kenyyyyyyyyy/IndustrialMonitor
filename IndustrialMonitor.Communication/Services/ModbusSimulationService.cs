using IndustrialMonitor.Communication.IServices;
using System.Collections.Concurrent;
using System.Threading;

namespace IndustrialMonitor.Communication.Services
{
    /// <summary>
    /// 为 Modbus Slave 生成注塑机工艺模拟数据。
    /// 寄存器定义与 Dashboard/Tools/DetailHelper 保持一致。
    /// </summary>
    public class ModbusSimulationService : IModbusSimulationService
    {
        private const int RegisterCount = 25;
        private const int BaseOutputPerCycle = 10;
        private const double MaxElapsedSeconds = 30;

        private readonly IDeviceCommunicationService _deviceCommunicationService;
        private readonly ConcurrentDictionary<string, SemaphoreSlim> _deviceLocks = new();
        private readonly ConcurrentDictionary<string, SimulationState> _states = new();

        public ModbusSimulationService(IDeviceCommunicationService deviceCommunicationService)
        {
            _deviceCommunicationService = deviceCommunicationService;
        }

        public async Task StartSimulation(string ipAddress)
        {
            if (string.IsNullOrWhiteSpace(ipAddress)) return;

            var deviceLock = _deviceLocks.GetOrAdd(ipAddress, _ => new SemaphoreSlim(1, 1));
            await deviceLock.WaitAsync();

            try
            {
                // 采集配置允许从任意 StartAddress 开始读取，但模拟规则固定使用
                // 0~24 号物理寄存器。不能把读取结果的数组下标直接当作寄存器地址。
                if (!_deviceCommunicationService.Connections.TryGetValue(ipAddress, out var connection)) return;

                var deviceConfig = connection.DeviceConfig;
                var registers = await Task.Run(() => connection.modbusMaster.ReadHoldingRegisters(
                    deviceConfig.SlaveId,
                    startAddress: 0,
                    numberOfPoints: RegisterCount));

                if (registers.Length < RegisterCount) return;
                bool isRunning = registers[0] == 1;
                var state = _states.GetOrAdd(ipAddress, _ => new SimulationState());
                double elapsedSeconds = state.GetElapsedSeconds();

                // 0/1/3/4 的状态、模式和报警完全由 Modbus Slave 手动维护，不在此修改。
                SimulateTemperature(registers, isRunning);
                SimulateProcess(registers, isRunning);
                int completedCycles = SimulateOperation(registers, isRunning, state, elapsedSeconds);
                SimulateProduction(registers, isRunning, completedCycles, state);

                // 只写 5~24，避免覆盖手动设置的设备状态寄存器。
                await _deviceCommunicationService.WriteRegisterAsync(ipAddress, 5, registers[5..25]);
            }
            finally
            {
                deviceLock.Release();
            }
        }

        private static void SimulateTemperature(ushort[] registers, bool isRunning)
        {
            if (!isRunning)
            {
                // 停机、待机、故障或未知状态时，实际温度持续下降；9 号设定温度不修改。
                registers[5] = DecreaseValue(registers[5], 15);
                registers[6] = DecreaseValue(registers[6], 20);
                registers[7] = DecreaseValue(registers[7], 20);
                registers[8] = DecreaseValue(registers[8], 20);
                return;
            }

            // 温度均按 0.1 ℃ 存储；9 号设定温度仍由 Modbus Slave 手动维护。
            registers[5] = SimulateBoundedValue(registers[5], 780, 900, 600, 1150, 12, 100);
            registers[6] = SimulateBoundedValue(registers[6], 1750, 1950, 1400, 2250, 15, 150);
            registers[7] = SimulateBoundedValue(registers[7], 1950, 2150, 1600, 2450, 15, 150);
            registers[8] = SimulateBoundedValue(registers[8], 2050, 2250, 1700, 2550, 15, 150);
        }

        private static void SimulateProcess(ushort[] registers, bool isRunning)
        {
            if (!isRunning)
            {
                // 非运行状态下工艺参数持续下降，直至归零。
                registers[10] = DecreaseValue(registers[10], 40);
                registers[11] = DecreaseValue(registers[11], 35);
                registers[12] = DecreaseValue(registers[12], 350);
                registers[13] = DecreaseValue(registers[13], 35);
                registers[14] = DecreaseValue(registers[14], 6);
                return;
            }

            // 10~13 按 0.1 单位存储，14 为整数 rpm。
            registers[10] = SimulateBoundedValue(registers[10], 950, 1250, 600, 1500, 30, 120);
            registers[11] = SimulateBoundedValue(registers[11], 600, 800, 350, 1100, 25, 100);
            registers[12] = SimulateBoundedValue(registers[12], 11000, 14000, 8000, 18000, 220, 1200);
            registers[13] = SimulateBoundedValue(registers[13], 400, 600, 200, 900, 30, 120);
            registers[14] = SimulateBoundedValue(registers[14], 60, 90, 30, 150, 5, 25);
        }

        private static void SimulateProduction(
            ushort[] registers,
            bool isRunning,
            int completedCycles,
            SimulationState state)
        {
            int total = registers[15];
            int qualified = registers[16];
            int defective = registers[17];
            int shiftOutput = registers[19];

            if (isRunning && completedCycles > 0)
            {
                double qualityPenalty = CalculateQualityPenalty(registers);

                for (int cycle = 0; cycle < completedCycles; cycle++)
                {
                    // 参数越偏离正常范围，周期产量越低且不良率越高。
                    int produced = Math.Max(0, (int)Math.Round(BaseOutputPerCycle * (1 - qualityPenalty)));
                    double defectRate = Math.Clamp(
                        0.02 + qualityPenalty * 0.55 + Random.Shared.NextDouble() * 0.02,
                        0.01,
                        0.85);
                    int defectiveIncrement = (int)Math.Round(produced * defectRate, MidpointRounding.AwayFromZero);
                    defectiveIncrement = Math.Clamp(defectiveIncrement, 0, produced);

                    total += produced;
                    qualified += produced - defectiveIncrement;
                    defective += defectiveIncrement;

                    state.CompletedCyclesInShift++;
                    if (state.CompletedCyclesInShift >= 6)
                    {
                        shiftOutput = 0;
                        state.CompletedCyclesInShift = 0;
                    }
                    else
                    {
                        shiftOutput += produced;
                    }
                }
            }

            registers[15] = ClampToUshort(total);
            registers[16] = ClampToUshort(qualified);
            registers[17] = ClampToUshort(defective);
            registers[18] = total == 0
                ? (ushort)0
                : ClampToUshort((int)Math.Round(Math.Min(qualified, total) * 1000d / total));
            registers[19] = ClampToUshort(shiftOutput);
        }

        private static int SimulateOperation(
            ushort[] registers,
            bool isRunning,
            SimulationState state,
            double elapsedSeconds)
        {
            // 21 号标准循环周期由 Modbus Slave 手动设置，不在此修改。
            ushort standardCycle = registers[21];

            if (isRunning && standardCycle > 0)
            {
                // 20/21 号按 0.1 秒存储，按真实经过时间推进循环。
                int elapsedTenths = (int)Math.Round(elapsedSeconds * 10, MidpointRounding.AwayFromZero);
                int nextCycle = registers[20] + elapsedTenths;
                int completedCycles = nextCycle / standardCycle;
                registers[20] = (ushort)(nextCycle % standardCycle);

                UpdateOperationTime(registers, true, state, elapsedSeconds);
                return completedCycles;
            }

            registers[20] = 0;
            UpdateOperationTime(registers, false, state, elapsedSeconds);
            return 0;
        }

        private static void UpdateOperationTime(
            ushort[] registers,
            bool isRunning,
            SimulationState state,
            double elapsedSeconds)
        {
            // 22/23 的单位是分钟；按设备保持连接期间真实经过的时间累计。
            if (isRunning)
            {
                state.RunningSeconds += elapsedSeconds;
                int minutes = (int)(state.RunningSeconds / 60);
                state.RunningSeconds %= 60;
                registers[22] = ClampToUshort(registers[22] + minutes);
            }
            else
            {
                state.DowntimeSeconds += elapsedSeconds;
                int minutes = (int)(state.DowntimeSeconds / 60);
                state.DowntimeSeconds %= 60;
                registers[23] = ClampToUshort(registers[23] + minutes);
            }

            int totalMinutes = registers[22] + registers[23];
            registers[24] = totalMinutes == 0
                ? (ushort)0
                : ClampToUshort((int)Math.Round(registers[22] * 1000d / totalMinutes)); // 0.1%
        }

        private static ushort SimulateBoundedValue(
            ushort current,
            int min,
            int max,
            int absoluteMin,
            int absoluteMax,
            int normalStep,
            int disturbanceSize)
        {
            int next;
            if (current < min)
            {
                double severity = Math.Clamp((min - current) / (double)(min - absoluteMin), 0, 1);
                bool moveUp = Random.Shared.NextDouble() < 0.60 + severity * 0.35;
                next = current + (moveUp ? 1 : -1) * Random.Shared.Next(1, normalStep + 1);
            }
            else if (current > max)
            {
                double severity = Math.Clamp((current - max) / (double)(absoluteMax - max), 0, 1);
                bool moveDown = Random.Shared.NextDouble() < 0.60 + severity * 0.35;
                next = current + (moveDown ? -1 : 1) * Random.Shared.Next(1, normalStep + 1);
            }
            else
            {
                next = current + Random.Shared.Next(-normalStep, normalStep + 1);
                if (Random.Shared.NextDouble() < 0.02)
                {
                    next += Random.Shared.Next(0, 2) == 0 ? -disturbanceSize : disturbanceSize;
                }
            }

            return ClampToUshort(Math.Clamp(next, absoluteMin, absoluteMax));
        }

        private static ushort DecreaseValue(ushort current, int maxStep)
        {
            if (current == 0) return 0;

            int decrease = Random.Shared.Next(1, maxStep + 1);
            return ClampToUshort(Math.Max(0, current - decrease));
        }

        private static double CalculateQualityPenalty(ushort[] registers)
        {
            var ranges = new (int Index, int Min, int Max, double Weight)[]
            {
                (5, 780, 900, 1.0), (6, 1750, 1950, 1.0), (7, 1950, 2150, 1.0),
                (8, 2050, 2250, 1.0), (10, 950, 1250, 1.2), (11, 600, 800, 1.1),
                (12, 11000, 14000, 1.3), (13, 400, 600, 0.9), (14, 60, 90, 0.8)
            };

            double weightedPenalty = 0;
            double totalWeight = 0;
            foreach (var range in ranges)
            {
                int value = registers[range.Index];
                int deviation = value < range.Min ? range.Min - value : Math.Max(0, value - range.Max);
                double normalizedDeviation = Math.Clamp(deviation / (double)(range.Max - range.Min), 0, 1);
                weightedPenalty += normalizedDeviation * range.Weight;
                totalWeight += range.Weight;
            }

            return totalWeight == 0 ? 0 : weightedPenalty / totalWeight;
        }

        private static ushort ClampToUshort(int value)
        {
            return (ushort)Math.Clamp(value, ushort.MinValue, ushort.MaxValue);
        }

        private sealed class SimulationState
        {
            public DateTime? LastSimulationAtUtc { get; set; }
            public double RunningSeconds { get; set; }
            public double DowntimeSeconds { get; set; }
            public int CompletedCyclesInShift { get; set; }

            public double GetElapsedSeconds()
            {
                DateTime now = DateTime.UtcNow;
                if (LastSimulationAtUtc is null)
                {
                    LastSimulationAtUtc = now;
                    return 0;
                }

                double elapsed = (now - LastSimulationAtUtc.Value).TotalSeconds;
                LastSimulationAtUtc = now;
                return Math.Clamp(elapsed, 0, MaxElapsedSeconds);
            }
        }
    }
}
