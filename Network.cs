using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Helpers
{
    public class Network
    {
        static public bool CheckNetworkStatus(string ip)
        {
            bool networkFlag = false;
            Ping p = new Ping();
            PingOptions pOption = new PingOptions();
            pOption.DontFragment = true;

            string data = "Test Data!";

            byte[] buffer = Encoding.ASCII.GetBytes(data);

            int timeout = 500; // Timeout 

            System.Net.NetworkInformation.PingReply reply = p.Send(ip, timeout, buffer, pOption);

            if (reply.Status == System.Net.NetworkInformation.IPStatus.Success)
            {
                networkFlag = true;
            }
            return networkFlag;
        }
        static public bool CanReceive(string m_host, decimal? m_port)
        {
            bool result = false;
            TcpClient tc = new TcpClient();

            //设置超时时间
            tc.SendTimeout = tc.ReceiveTimeout = 2000;
            try
            {
                int port = 0;
                if (m_port.HasValue) port = Convert.ToInt16(m_port);
                //尝试连接
                tc.Connect(m_host, port);
                if (tc.Connected)
                {
                    //如果连接上，证明此端口为开放状态
                    result = true;
                }
            }
            catch (SocketException)
            {
                result = false;
            }
            finally
            {
                tc.Close();
                tc = null;

            }
            return result;
        }

    }
}
