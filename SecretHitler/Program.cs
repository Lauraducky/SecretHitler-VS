using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SecretHitler {
    public enum GAMESIZE { Small, Medium, Large }
    public enum POLICY { Fascist, Liberal }
    public enum ROLE { Liberal, Fascist, Hitler };
    public enum POWER { None, PolicyCheck, SpecialElection, LoyaltyCheck, Assassination, Win };

    public enum PLAYERSTATES { Idle, Voting, SelectingChance, PresPolicy, ChanPolicy,
        Investigate, SpecialElection, PolicyPeek, Assassinate };

    class Program {
        private static Dictionary<string, Player> players = new Dictionary<string, Player>();

        static void Main(string[] args) {
            Server.startServer();
            //Game myGame = new Game(players);
            Console.ReadKey();
            Server.stopAcceptingConnections();
            NetworkedGame myGame = new NetworkedGame(Server.getPlayers());
        }

        static void testBoardsPolicies(GAMESIZE size) {
            PoliciesDeck policies = new PoliciesDeck();
            GameBoards boards = new GameBoards(size);

            POWER current = POWER.None;

            while (current != POWER.Win) {
                POLICY card = policies.drawCards(1)[0];
                Console.WriteLine("Played card: " + card.ToString());
                current = boards.playCard(card);
                Console.WriteLine("Current power: " + current.ToString());
            }
            if (boards.FascistBoard.CardsPlayed == 6)
                Console.WriteLine("Winner: Fascists");
            else Console.WriteLine("Winner: Liberals");
            Console.WriteLine("------");
        }
    }

    static class Server {
        static bool acceptingConnections = true;
        static Dictionary<string, NetworkPlayer> players;
        static Queue<TcpClient> connections;

        public static void startServer() {
            acceptingConnections = true;
            Thread listenThread = new Thread(listenForClients);
            listenThread.Start();
        }

        static void listenForClients() {
            players = new Dictionary<string, NetworkPlayer>();
            TcpListener server = null;
            connections = new Queue<TcpClient>();
            try {
                // Set the TcpListener on port 33333.
                int port = 33333;
                IPAddress localAddr = Dns.GetHostEntry("localhost").AddressList[0];

                // TcpListener server = new TcpListener(port);
                server = new TcpListener(IPAddress.Any, port);

                // Start listening for client requests.
                server.Start();

                // Enter the listening loop.
                while (acceptingConnections) {
                    Console.Write("Waiting for a client... ");
                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Connected!");
                    connections.Enqueue(client);
                    startPlayer(client);
                }
            } catch (SocketException e) {
                Console.WriteLine("SocketException: {0}", e);
            } finally {
                // Stop listening for new clients.
                server.Stop();
            }
        }

        static void startPlayer(TcpClient client) {
            string name = "";
            try {
                byte[] bytes = new byte[256];
                int bytesRead;
                NetworkStream stream = client.GetStream();
                bytesRead = stream.Read(bytes, 0, bytes.Length);
                name = Encoding.ASCII.GetString(bytes, 0, bytesRead);
            } catch {
                Console.WriteLine("Lost connection");
            }
            
            NetworkPlayer player = new NetworkPlayer(client, name);
            players.Add(name, player);
        }

        public static void stopAcceptingConnections() {
            acceptingConnections = false;
        }

        public static Dictionary<string, NetworkPlayer> getPlayers() {
            return new Dictionary<string, NetworkPlayer>(players);
        }
    }
}
