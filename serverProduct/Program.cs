using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ServerApp
{
    class Program
    {
        static int numberOfClient = 0;
        static int nextClientId = 1;

        static void Main(string[] args)
        {
            string host = "127.0.0.1";
            int port = 1500;
            Console.WriteLine("Server App");
            IPAddress localAddr = IPAddress.Parse(host);
            TcpListener server = new TcpListener(localAddr, port);
            server.Start();

            Console.WriteLine("************************");
            Console.WriteLine("Waiting for clients...");

            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                int clientId = nextClientId++; // Gán ID client
                Console.WriteLine($"Client connected. [CLIENT {clientId}] Number of client connected: {++numberOfClient}");

                // Tạo luồng mới để xử lý client
                Thread thread = new Thread(() =>
                {
                    var clientHandler = new ClientHandler(client, clientId);
                    clientHandler.HandleClient();
                    Console.WriteLine($"Number of client connected: {--numberOfClient}");
                });
                thread.Start();
            }
        }
    }
}