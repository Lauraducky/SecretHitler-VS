using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SecretHitler {
    class NetworkPlayer : Player {
        TcpClient client;
        NetworkStream stream;

        //CLIENT -> SERVER
        //POWER <powername> <args>

        public NetworkPlayer(TcpClient client, string name) {
            this.client = client;
            this.name = name;

            stream = client.GetStream();

            IsDead = false;
            PrevPresident = false;
            PrevChancellor = false;
        }

        private void sendMessage(string message) {
            byte[] msg = Encoding.ASCII.GetBytes(message + ":");
            stream.Write(msg, 0, msg.Length);
            stream.Flush();
        }

        //ROLE <rolename> [hitler [fascist1, fascist2...]]
        public override void setRole(ROLE role) {
            this.role = role;
            sendMessage("ROLE " + role.ToString());
        }

        //VOTE <president> <chancellor>
        //VOTE <ya/nein> ***RESPONSE
        public override bool pollVote(string president, string chancellor) {
            sendMessage("VOTE " + president + " " + chancellor);

            //TODO: Get response
            string answer = Console.ReadLine();

            while (answer != "ya" && answer != "nein") {
                Console.Write("Please vote: ");
                answer = Console.ReadLine();
            }
            if (answer == "ya") {
                return true;
            }
            return false;
        }

        //NOMINATE <KILL/CHECK/CHANCELLOR/PRESIDENT> <candidates>
        //NOMINATE <KILL/CHECK/CHANCELLOR/PRESIDENT> <nominee> ***RESPONSE
        public override string nominatePlayer(List<string> candidates, string action) {
            string candidateString = string.Join(" ", candidates);

            sendMessage("NOMINATE " + action + " " + candidateString);

            //TODO: Get response
            for (int i = 0; i < candidates.Count; i++) {
                if (i != 0) Console.Write(", ");
                Console.Write(candidates[i]);
            }
            Console.WriteLine();

            Console.Write("Please nominate a player: ");
            string nominee = Console.ReadLine();

            while (!candidates.Contains(nominee)) {
                Console.Write("Please nominate a player: ");
                nominee = Console.ReadLine();
            }

            return nominee;
        }

        //POLICY <DISCARD/ENACT> <card1> <card2> [card3]
        //POLICY <DISCARD/ENACT> <card> ***RESPONSE
        public override POLICY chooseCard(List<POLICY> cards, string action) {
            string cardString = string.Join(" ", cards);
            sendMessage("POLICY " + action + " " + cardString);

            //TODO: Get Response

            Console.Write("Chosen card: ");
            string card = Console.ReadLine();
            POLICY chosen;

            bool success = Enum.TryParse(card, true, out chosen);

            while (!success || !cards.Contains(chosen)) {
                Console.Write("Chosen card: ");
                card = Console.ReadLine();
                success = Enum.TryParse(card, true, out chosen);
            }
            return chosen;
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
