using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace GrowBoxServer
{
    class Program
    {
        static void Main(string[] args)
        {
            SocketServer ss = new SocketServer(new System.Net.IPAddress(new byte[] { 0, 0, 0, 0 }), 45555);

            ss.StartListening();

            while (Console.ReadLine() != "quit") ;
            Environment.Exit(0);
        }
    }
}
