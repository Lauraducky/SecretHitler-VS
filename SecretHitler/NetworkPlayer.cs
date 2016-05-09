using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SecretHitler {
    /**
     * Dedicated player class to send information to the player and receive
     * their response. Response handling to be done by another party.
     */
    class NetworkPlayer : Player {
        TcpClient client;
        NetworkStream stream;

        public NetworkPlayer(TcpClient client, string name) {
            this.client = client;
            this.name = name;

            stream = client.GetStream();

            IsDead = false;
            PrevPresident = false;
            PrevChancellor = false;

            messageQueue = new ConcurrentQueue<string>();

            Thread responseThread = new Thread(getResponses);
            responseThread.Start();
        }

        private void sendMessage(string message) {
            byte[] msg = Encoding.ASCII.GetBytes(message + ":");
            stream.Write(msg, 0, msg.Length);
            stream.Flush();
        }

        private void getResponses() {
            try {
                byte[] bytes = new byte[256];
                int bytesRead;
                string data = null;

                bytesRead = stream.Read(bytes, 0, bytes.Length);
                data = Encoding.ASCII.GetString(bytes, 0, bytesRead);
                char[] delim = { ':' };
                string[] messages = data.Split(delim);

                for (int i = 0; i < messages.Length; i++) {
                    messageQueue.Enqueue(messages[i]);
                }
            } catch {
                Console.Write("Lost connection");
            }
        }
        
        /********************
         * MESSAGES TO SEND *
         *******************/
        //ROLE <rolename> [hitler [fascist1, fascist2...]]
        public override void setRole(ROLE role) {
            this.role = role;
            sendMessage("ROLE " + role.ToString());
        }

        //VOTE <president> <chancellor>
        public override void pollVote(string president, string chancellor) {
            sendMessage("VOTE " + president + " " + chancellor);
        }

        //NOMINATE <KILL/CHECK/CHANCELLOR/PRESIDENT> <candidates>
        public override void nominatePlayer(List<string> candidates, string action) {
            string candidateString = string.Join(" ", candidates);
            sendMessage("NOMINATE " + action + " " + candidateString);
        }

        //POLICY <DISCARD/ENACT> <card1> <card2> [card3]
        public override void chooseCard(List<POLICY> cards, string action) {
            string cardString = string.Join(" ", cards);
            sendMessage("POLICY " + action + " " + cardString);
        }

        //NOTIFY <CARDS/PLAYER> <args>
        public override void notifyParty(string player, string party) {
            sendMessage("NOTIFY PLAYER " + party);
        }

        public override void notifyCards(List<POLICY> cards) {
            string cardString = string.Join(" ", cards);
            sendMessage("NOTIFY CARDS " + cardString);
        }

        //START <president>
        public override void notifyStart(string president) {
            sendMessage("START " + president);
        }
    }
}
