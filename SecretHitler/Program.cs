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
        static Dictionary<string, Player> players;
        static Queue<TcpClient> connections;

        public static void startServer() {
            Thread listenThread = new Thread(listenForClients);
            listenThread.Start();
            handleConnections();
        }

        static void listenForClients() {
            TcpListener server = null;
            connections = new Queue<TcpClient>();
            try {
                // Set the TcpListener on port 67250.
                int port = 33333;
                IPAddress localAddr = Dns.GetHostEntry("localhost").AddressList[0];

                // TcpListener server = new TcpListener(port);
                server = new TcpListener(localAddr, port);

                // Start listening for client requests.
                server.Start();

                // Enter the listening loop.
                while (acceptingConnections) {
                    Console.Write("Waiting for a clients... ");
                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Connected!");
                    connections.Enqueue(client);
                }
            } catch (SocketException e) {
                Console.WriteLine("SocketException: {0}", e);
            } finally {
                // Stop listening for new clients.
                server.Stop();
            }
        }

        static void handleConnections() {

        }
    }
}
