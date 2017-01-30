using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CoursSocket
{
    class Program
    {
        static void Main(string[] args)
        {
            PrintIpAddresses();

            Task.Run(() => CreateServer(50001));

            Thread.Sleep(1000);

            SendRequest(50001);
            Console.ReadLine();
        }

        private static void PrintIpAddresses()
        {
            string name = Dns.GetHostName();
            try
            {
                var hostEntry = Dns.GetHostEntry(name);
                IPAddress[] addrs = hostEntry.AddressList;
                foreach (IPAddress addr in addrs)
                    Console.WriteLine("{0}/{1}", name, addr);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.ReadLine();
        }

        private static bool _isServerRunning = true;
        private static void CreateServer(ushort port)
        {
            var listener = TcpListener.Create(port);
            listener.Start();
            while (_isServerRunning)
            {
                var socketClient = listener.AcceptSocket();


                using (var writer = new BinaryWriter(new NetworkStream(socketClient)))
                {
                    writer.Write("Hello world!");
                }
                socketClient.Close();
            }

            listener.Stop();
        }

        private static void SendRequest(ushort port)
        {
            using (var client = new TcpClient())
            {

                client.Connect("localhost", port);

                using (var reader = new BinaryReader(client.GetStream()))
                {
                    while (client.Connected)
                    {
                        if (client.Available > 0)
                        {
                            Console.WriteLine(reader.ReadString());
                        }
                    }
                }
            }

        }
    }
}
