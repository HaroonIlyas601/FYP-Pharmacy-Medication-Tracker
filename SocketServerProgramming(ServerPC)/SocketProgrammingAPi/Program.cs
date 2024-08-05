using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace server1
{
    class Program
    {
        static Socket serverSocket;
        static List<Socket> clientSockets = new List<Socket>();
        static int port = 9000;
        static IPAddress ip;
        static Thread acceptThread;
        static bool isRunning = true;

        static string GetIp()
        {
            string strHostName = Dns.GetHostName();
            IPHostEntry ipEntry = Dns.GetHostEntry(strHostName);
            IPAddress[] addr = ipEntry.AddressList;
            return addr[addr.Length - 1].ToString();
        }

        static void HandleClient(object client)
        {
            Socket acc = (Socket)client;
            while (isRunning)
            {
                try
                {
                    byte[] buffer = new byte[1024];
                    int rec = acc.Receive(buffer, 0, buffer.Length, 0);
                    if (rec == 0) // Check for client disconnection
                    {
                        throw new SocketException();
                    }
                    Array.Resize(ref buffer, rec);
                    string receivedMessage = Encoding.Default.GetString(buffer);
                    Console.WriteLine("Received message: " + receivedMessage);

                    // Echo the message back to all clients if it's a valid API request
                    if (IsValidApiRequest(receivedMessage))
                    {
                        BroadcastMessage(buffer);
                    }
                }
                catch
                {
                    // Handle client disconnection
                    //Console.WriteLine("Client disconnected.");
                    acc.Close();
                    clientSockets.Remove(acc);
                    break;
                }
            }
        }

        static bool IsValidApiRequest(string message)
        {
            // Add your logic here to validate the incoming API requests
            // For example, check if the message follows the expected format
            return message.StartsWith("GetProductById") ||
                   message.StartsWith("SearchProductsNearby") ||
                   message.StartsWith("GetProducts");
        }

        static void BroadcastMessage(byte[] buffer)
        {
            foreach (var socket in clientSockets)
            {
                if (socket.Connected)
                {
                    socket.Send(buffer, 0, buffer.Length, 0);
                }
            }
        }

        static void AcceptClients()
        {
            while (isRunning)
            {
                Console.WriteLine("Waiting for a connection...");
                Socket acc = serverSocket.Accept();
                clientSockets.Add(acc);
                Console.WriteLine("Client connected.");
                Thread clientThread = new Thread(HandleClient);
                clientThread.Start(acc);
            }
        }

        static void SendMessageFromServer()
        {
            while (isRunning)
            {
                string message = Console.ReadLine();
                if (message.Equals("exit", StringComparison.OrdinalIgnoreCase))
                {
                    isRunning = false;
                    serverSocket.Close();
                    break;
                }
                byte[] data = Encoding.Default.GetBytes("Server: " + message);
                BroadcastMessage(data);
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Your Local IP is: " + GetIp());
            ip = IPAddress.Parse(GetIp());
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(new IPEndPoint(ip, port));
            serverSocket.Listen(0);

            acceptThread = new Thread(AcceptClients);
            acceptThread.Start();

            SendMessageFromServer();
        }
    }
}