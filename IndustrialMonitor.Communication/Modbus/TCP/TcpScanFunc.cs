using IndustrialMonitor.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialMonitor.Communication.Modbus.TCP
{
    public class TcpScanFunc
    {
        private ModBusTCP _modBusTCP;
        private TcpClient _tcpClient;

        public async Task<ObservableCollection<TcpResultModel>> TCPConnect(List<string> iplist)
        {
            var tasks = iplist.Select(ip => TCPConnectAsync(ip));
            var results = await Task.WhenAll(tasks);
            return new ObservableCollection<TcpResultModel>(results);
        }

        public async Task<TcpResultModel> TCPConnectAsync(string ip)
        {
            _tcpClient = new();
            _modBusTCP = new();
            try
            {
                _tcpClient = await _modBusTCP.ModbusTcpConnectAsync(ip, 502);

                TcpResultModel result = new TcpResultModel
                {
                    IpAddress = ip,
                    IsConnected = true,
                    TcpClient = _tcpClient,
                    StatusText = "在线"
                };
                return result;

            }
            catch (Exception ex)
            {
                TcpResultModel result = new TcpResultModel
                {
                    IpAddress = ip,
                    IsConnected = false,
                    TcpClient = null,
                    StatusText = "离线"

                };
                return result;
            }
            
        }

    }
}
