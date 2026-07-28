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
        private const int SimulationIntervalSeconds = 10;

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
                var result = await _deviceCommunicationService.ReadHoldingRegistersAsync(ipAddress);
                if (!result.Success || result.Data.Length < RegisterCount) return;

                var registers = result.Data.Take(RegisterCount).ToArray();
                bool isRunning = registers[0] == 1;
                var state = _states.GetOrAdd(ipAddress, _ => new SimulationState());

                // 0/1/3/4 的状态、模式和报警完全由 Modbus Slave 手动维护，不在此修改。
                SimulateTemperature(registers, isRunning);
                SimulateProcess(registers, isRunning);
                bool completedCycle = SimulateOperation(registers, isRunning, state);
                SimulateProduction(registers, isRunning, completedCycle, state);

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
                // 停机后温度缓慢冷却；9 号设定温度不修改。
                registers[5] = MoveTowards(registers[5], 300, 0, 1150, 15);
                registers[6] = MoveTowards(registers[6], 300, 0, 2250, 20);
                registers[7] = MoveTowards(registers[7], 300, 0, 2450, 20);
                registers[8] = MoveTowards(registers[8], 300, 0, 2550, 20);
                return;
            }

            // 温度均按 0.1 ℃ 存储。9 号为人工设定的模具温度，推荐设置 850（85.0 ℃）。
            int moldTarget = registers[9] == 0 ? 820 : registers[9] - 30;
            registers[5] = SimulateRunningValue(
                registers[5], moldTarget,
                recoveryThreshold: 760, normalMin: 780, normalMax: 900,
                hardMax: 1150, normalStep: 12, recoveryStep: 25, spikeSize: 140);

            registers[6] = SimulateRunningValue(
                registers[6], target: 1850,
                recoveryThreshold: 1500, normalMin: 1750, normalMax: 1950,
                hardMax: 2250, normalStep: 15, recoveryStep: 35, spikeSize: 180);

            registers[7] = SimulateRunningValue(
                registers[7], target: 2050,
                recoveryThreshold: 1700, normalMin: 1950, normalMax: 2150,
                hardMax: 2450, normalStep: 15, recoveryStep: 35, spikeSize: 180);

            registers[8] = SimulateRunningValue(
                registers[8], target: 2150,
                recoveryThreshold: 1800, normalMin: 2050, normalMax: 2250,
                hardMax: 2550, normalStep: 15, recoveryStep: 35, spikeSize: 180);
        }

        private static void SimulateProcess(ushort[] registers, bool isRunning)
        {
            if (!isRunning)
            {
                // 非运行状态下工艺参数逐步归零，而不是立即跳变。
                registers[10] = MoveTowards(registers[10], 0, 0, 1500, 40);
                registers[11] = MoveTowards(registers[11], 0, 0, 1100, 35);
                registers[12] = MoveTowards(registers[12], 0, 0, 18000, 350);
                registers[13] = MoveTowards(registers[13], 0, 0, 900, 35);
                registers[14] = MoveTowards(registers[14], 0, 0, 150, 6);
                return;
            }

            // 10~13 按 0.1 单位存储，14 为整数 rpm。
            registers[10] = SimulateRunningValue(
                registers[10], target: 1100,
                recoveryThreshold: 700, normalMin: 950, normalMax: 1250,
                hardMax: 1500, normalStep: 30, recoveryStep: 70, spikeSize: 170); // 注射压力：0~150 MPa

            registers[11] = SimulateRunningValue(
                registers[11], target: 700,
                recoveryThreshold: 450, normalMin: 600, normalMax: 800,
                hardMax: 1100, normalStep: 25, recoveryStep: 55, spikeSize: 140); // 保压压力：0~110 MPa

            registers[12] = SimulateRunningValue(
                registers[12], target: 12500,
                recoveryThreshold: 9000, normalMin: 11000, normalMax: 14000,
                hardMax: 18000, normalStep: 220, recoveryStep: 500, spikeSize: 1800); // 锁模力：0~1800 kN

            registers[13] = SimulateRunningValue(
                registers[13], target: 500,
                recoveryThreshold: 300, normalMin: 400, normalMax: 600,
                hardMax: 900, normalStep: 30, recoveryStep: 70, spikeSize: 160); // 注射速度：0~90 mm/s

            registers[14] = SimulateRunningValue(
                registers[14], target: 75,
                recoveryThreshold: 40, normalMin: 60, normalMax: 90,
                hardMax: 150, normalStep: 5, recoveryStep: 12, spikeSize: 35); // 螺杆转速：0~150 rpm
        }

        private static void SimulateProduction(
            ushort[] registers,
            bool isRunning,
            bool completedCycle,
            SimulationState state)
        {
            // 只有运行状态完成一个标准循环时，才产出新的制品。
            int total = registers[15];
            int qualified = registers[16];
            int defective = registers[17];
            int shiftOutput = registers[19];

            if (isRunning && completedCycle)
            {
                // 一个循环通常由模具多腔同时产出 1~4 件。
                int produced = Random.Shared.Next(1, 5);

                // 约 3% 的循环出现不良品，单个循环最多出现 2 件不良品。
                int defectiveIncrement = Random.Shared.NextDouble() < 0.03
                    ? Random.Shared.Next(1, Math.Min(produced, 2) + 1)
                    : 0;

                int qualifiedIncrement = produced - defectiveIncrement;

                total += produced;
                qualified += qualifiedIncrement;
                defective += defectiveIncrement;

                state.CompletedCyclesInShift++;

                // 每 6 个标准循环视为一个班次；第 6 个周期完成后班次产量清零。
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

            registers[15] = ClampToUshort(total);
            registers[16] = ClampToUshort(qualified);
            registers[17] = ClampToUshort(defective);
            registers[18] = total == 0
                ? (ushort)0
                : ClampToUshort((int)Math.Round(Math.Min(qualified, total) * 1000d / total));
            registers[19] = ClampToUshort(shiftOutput);
        }

        private static bool SimulateOperation(ushort[] registers, bool isRunning, SimulationState state)
        {
            // 21 号标准循环周期由 Modbus Slave 手动设置，不在此修改。
            ushort standardCycle = registers[21];

            if (isRunning && standardCycle > 0)
            {
                // 20 号按 0.1 秒存储。采集周期为 10 秒，因此每轮增加 100。
                int nextCycle = registers[20] + SimulationIntervalSeconds * 10;
                bool completedCycle = nextCycle >= standardCycle;
                registers[20] = completedCycle ? (ushort)0 : ClampToUshort(nextCycle);

                UpdateOperationTime(registers, isRunning, state);
                return completedCycle;
            }

            registers[20] = 0;
            UpdateOperationTime(registers, isRunning, state);
            return false;
        }

        private static void UpdateOperationTime(ushort[] registers, bool isRunning, SimulationState state)
        {
            // 22/23 的单位是分钟；每累计 60 秒，才给对应计时器加 1。
            if (isRunning)
            {
                state.RunningSeconds += SimulationIntervalSeconds;
                int minutes = state.RunningSeconds / 60;
                state.RunningSeconds %= 60;
                registers[22] = ClampToUshort(registers[22] + minutes);
            }
            else
            {
                state.DowntimeSeconds += SimulationIntervalSeconds;
                int minutes = state.DowntimeSeconds / 60;
                state.DowntimeSeconds %= 60;
                registers[23] = ClampToUshort(registers[23] + minutes);
            }

            int totalMinutes = registers[22] + registers[23];
            registers[24] = totalMinutes == 0
                ? (ushort)0
                : ClampToUshort((int)Math.Round(registers[22] * 1000d / totalMinutes)); // 0.1%
        }

        private static ushort SimulateRunningValue(
            ushort current,
            int target,
            int recoveryThreshold,
            int normalMin,
            int normalMax,
            int hardMax,
            int normalStep,
            int recoveryStep,
            int spikeSize)
        {
            // 低于恢复阈值时只允许上升，直到回到正常下限，模拟加热/建压过程。
            if (current < recoveryThreshold)
            {
                return ClampToUshort(Math.Min(current + Random.Shared.Next(recoveryStep / 2, recoveryStep + 1), normalMin));
            }

            int next = MoveTowards(current, target, 0, hardMax, normalStep);

            // 少量低值场景：下一轮会进入上面的恢复分支并连续升高。
            if (Random.Shared.NextDouble() < 0.01)
            {
                return ClampToUshort(Math.Max(0, recoveryThreshold - Random.Shared.Next(recoveryStep, recoveryStep * 3)));
            }

            // 约 1.5% 概率产生一次短时高值，为后续“过高报警”模块留出测试场景。
            if (Random.Shared.NextDouble() < 0.015)
            {
                next = Math.Min(next + Random.Shared.Next(spikeSize / 2, spikeSize + 1), hardMax);
            }

            return ClampToUshort(Math.Clamp(next, normalMin, hardMax));
        }

        private static ushort MoveTowards(ushort current, int target, int min, int max, int maxStep)
        {
            int correction = Math.Clamp(target - current, -maxStep / 2, maxStep / 2);
            int noise = Random.Shared.Next(-maxStep, maxStep + 1);
            int next = current + correction + noise;

            return ClampToUshort(Math.Clamp(next, min, max));
        }

        private static ushort ClampToUshort(int value)
        {
            return (ushort)Math.Clamp(value, ushort.MinValue, ushort.MaxValue);
        }

        private sealed class SimulationState
        {
            public int RunningSeconds { get; set; }
            public int DowntimeSeconds { get; set; }
            public int CompletedCyclesInShift { get; set; }
        }
    }
}
