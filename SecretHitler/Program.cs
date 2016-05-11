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
        const int portNum = 33333;
        static bool acceptingConnections = true;
        static Dictionary<string, NetworkPlayer> players;

        delegate void mThread(ref bool isRunning);
        delegate void AccptTcpClnt(ref TcpClient client, TcpListener listener);

        public static void startServer() {
            players = new Dictionary<string, NetworkPlayer>();
            acceptingConnections = true;
            mThread t = new mThread(StartListening);
            Thread masterThread = new Thread(() => t(ref acceptingConnections));
            masterThread.IsBackground = true; //better to run it as a background thread
            masterThread.Start();
        }

        public static void AccptClnt(ref TcpClient client, TcpListener listener) {
            if (client == null)
                client = listener.AcceptTcpClient();
        }

        public static void StartListening(ref bool isRunning) {
            TcpListener listener = new TcpListener(new IPEndPoint(IPAddress.Any, portNum));

            try {
                listener.Start();

                TcpClient handler = null;
                while (isRunning) {
                    AccptTcpClnt t = new AccptTcpClnt(AccptClnt);

                    Thread tt = new Thread(() => t(ref handler, listener));
                    tt.IsBackground = true;
                    tt.Start(); //the AcceptTcpClient() is a blocking method, so we are invoking it    in a seperate thread so that we can break the while loop wher isRunning becomes false

                    while (isRunning && tt.IsAlive && handler == null)
                        Thread.Sleep(500); //change the time as you prefer


                    if (handler != null) {
                        //handle the accepted connection here
                        startPlayer(handler);
                        handler = null;
                    } else if (!isRunning && tt.IsAlive) {
                        tt.Abort();
                    }
                }
                listener.Stop();
            } catch (Exception e) {
                Console.WriteLine(e.ToString());
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
