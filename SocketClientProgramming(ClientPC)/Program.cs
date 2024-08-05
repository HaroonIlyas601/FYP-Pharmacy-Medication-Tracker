using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Net.Http;
using System.Threading.Tasks;

namespace client1
{
    class Program
    {
        static string name = "Health Plus Pharmacy";
        static int port = 9000;
        static IPAddress ip;
        static Socket sck;
        static Thread recThread;
        static HttpClient httpClient = new HttpClient();

        static async Task<string> SendApiRequest(string endpoint)
        {
            // Check if endpoint is already a complete URL
            string url = endpoint.StartsWith("http") ? endpoint : $"https://localhost:44371/api/{endpoint}";
            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        static async void ReceiveData()
        {
            while (sck.Connected)
            {
                try
                {
                    Thread.Sleep(500); // Delays to prevent overwhelming the CPU
                    byte[] buffer = new byte[2048];
                    int rec = sck.Receive(buffer, 0, buffer.Length, 0);
                    if (rec == 0)
                    {
                        Console.WriteLine("Server has closed the connection.");
                        break;
                    }
                    Array.Resize(ref buffer, rec);
                    string receivedMessage = Encoding.Default.GetString(buffer);
                   Console.WriteLine($"Received message: {receivedMessage}");

                    // Ignore connection messages or any non-API messages
                    if (receivedMessage.StartsWith("<") && receivedMessage.EndsWith("> Connected"))
                    {
                        continue;
                    }

                    // Determine the endpoint based on the received message
                    string endpoint = ParseMessageToEndpoint(receivedMessage);
                   // Console.WriteLine($"Calling endpoint: {endpoint}");

                    // Make API call
                    string apiResponse = await SendApiRequest(endpoint);
                    Console.WriteLine("API Response: " + apiResponse);

                    // Send response back to server
                    byte[] responseBuffer = Encoding.Default.GetBytes(apiResponse);
                    sck.Send(responseBuffer, 0, responseBuffer.Length, 0);
                }
                catch (Exception ex)
                {
                  //  Console.WriteLine($"Exception: {ex.Message}");
                }
            }
            sck.Close();
            Environment.Exit(0); // Exit the program cleanly when the socket is closed
        }

        // Modify the ParseMessageToEndpoint method
        static string ParseMessageToEndpoint(string message)
        {
            // If the message is already a complete URL, return it as-is
            if (message.StartsWith("http"))
            {
                return message;
            }

            var parts = message.Split(':');
            if (parts[0] == "GetProductById")
            {
                return $"products/{parts[1]}";
            }
            else if (parts[0] == "SearchProductsNearby")
            {
                return $"products/search/nearby?q={parts[1]}&lat={parts[2]}&lon={parts[3]}&radius={parts[4]}";
            }
            else if (parts[0] == "GetProducts")
            {
                return "products";
            }
            // Add more cases as needed
            return message; // Default case, just return the message
        }

        static void Main(string[] args)
        {
            string serverIP = "10.8.157.1";
            ip = IPAddress.Parse(serverIP);

            string inputPort = "9000";
            if (!int.TryParse(inputPort, out port))
            {
                port = 9000; // Default port if parse fails
            }

            sck = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                sck.Connect(new IPEndPoint(ip, port));
                Console.WriteLine("Connected to server");

                recThread = new Thread(ReceiveData);
                recThread.Start();

                byte[] connectionMessage = Encoding.Default.GetBytes("<" + name + "> Connected");
                sck.Send(connectionMessage, 0, connectionMessage.Length, 0);

                while (sck.Connected)
                {
                    string message = Console.ReadLine();
                    byte[] sendData = Encoding.Default.GetBytes("<" + name + "> " + message);
                    sck.Send(sendData, 0, sendData.Length, 0);
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine("Unable to connect to server. Error: " + ex.Message);
                return; // Exit if connection fails
            }
        }
    }
}
