using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace GrowBoxServer
{
    class SocketServer
    {
        IPEndPoint _localEndPoint;
        Task _listenTask;

        public SocketServer(IPAddress address, int port)
        {
            _localEndPoint = new IPEndPoint(address, port);
            _listenTask = new Task(Listen);
        }

        public void StartListening()
        {
            if (_listenTask.Status != TaskStatus.Running)
            {
                _listenTask.Start();
            }
        }

        public void StopListening()
        {
            if (_listenTask.Status == TaskStatus.Running)
            {
            }
        }

        private void Listen()
        {
            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            listener.Bind(_localEndPoint);
            listener.Listen(50);

            while (true)
            {
                Socket handler = listener.Accept();

                Console.WriteLine("Connected {0}", ((IPEndPoint)handler.RemoteEndPoint).Address.ToString());
                new Task(() => HandleConnection(handler)).Start();
            }
        }

        private void HandleConnection(Socket handler)
        {
            string remoteIp = ((IPEndPoint)handler.RemoteEndPoint).Address.ToString();
            // Create a new logger
            FileLogger fl = new FileLogger(remoteIp.Replace('.', '_') + ".log");
            //ConsoleLogger fl = new ConsoleLogger();
            fl.LogFormatter = (message) => string.Format("({1}) {0}\n", message.Message, DateTime.Now);

            while (handler.Connected && (handler.Available != 0 && handler.Poll(1000, SelectMode.SelectRead)))
            {
                string data = "";
                byte[] buffer = new byte[1024];

                int bytesRead;
                do
                {
                    bytesRead = handler.Receive(buffer);
                    data += Encoding.ASCII.GetString(buffer, 0, bytesRead);
                } while (bytesRead > 0);

                fl.Log(new LogMessage(data, LoggingLevel.NORMAL, LogginType.INFO));
            }
            Console.WriteLine("Disconnected {0}", ((IPEndPoint)handler.RemoteEndPoint).Address.ToString());
        }
    }
}
