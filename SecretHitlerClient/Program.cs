using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SecretHitlerClient {
    class Program {
        static string name;
        static NetworkPlayer player;

        static void Main(string[] args) {
            Console.Write("Please enter your name: ");
            name = Console.ReadLine();

            while (name.Contains(":") || name.Contains(",") || name.Contains(" ")) {
                Console.Write("Invalid name. Please choose another: ");
                name = Console.ReadLine();
            }

            player = new NetworkPlayer(name);

            Console.Write("Please enter the IP Address of the server: ");
            string address = Console.ReadLine();

            while (!connectServer(address)) {
                Console.Write("Failed connection. Try a different IP? ");
                address = Console.ReadLine();
            }

            Console.Write("Press ENTER to quit.");
            Console.ReadLine();
        }

        static bool connectServer(string ipAddress) {
            try {
                // Set the TcpListener on port 67250.
                int port = 33333;
                TcpClient client = new TcpClient(ipAddress, port);
                
                Console.WriteLine("Connected!");

                // Get a stream object for reading and writing
                NetworkStream stream = client.GetStream();

                byte[] msg = Encoding.ASCII.GetBytes(name);
                stream.Write(msg, 0, msg.Length);
                stream.Flush();
                handleMessages(stream);

                // Shutdown and end connection
                client.Close();
            } catch (SocketException e) {
                Console.WriteLine("SocketException: {0}", e);
                return false;
            }
            return true;
        }

        static void handleMessages(NetworkStream stream) {
            try {
                byte[] bytes = new byte[256];
                int bytesRead;
                string data = null;

                while ((bytesRead = stream.Read(bytes, 0, bytes.Length)) != 0) {
                    data = Encoding.ASCII.GetString(bytes, 0, bytesRead);
                    char[] delim = { ':' };
                    string[] messages = data.Split(delim);

                    for (int i = 0; i < messages.Length; i++) {
                        string response = player.receiveMessage(messages[i]);
                        if (!string.IsNullOrEmpty(response)) {
                            if(response == "END") {
                                return;
                            }

                            byte[] msg = Encoding.ASCII.GetBytes(name + " " + response + ":");
                            stream.Write(msg, 0, msg.Length);
                            stream.Flush();
                        }
                    }
                }
            } catch {
                Console.Write("Lost connection");
            }
        }
    }
}
