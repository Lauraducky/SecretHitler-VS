using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretHitler {
    class ConsolePlayer {
        public string name;
        public ROLE role;
        public bool IsDead;
        public bool PrevPresident;
        public bool PrevChancellor;

        public void setRole(ROLE role) {
            this.role = role;
        }

        public void resetStatus() {
            PrevChancellor = false;
            PrevPresident = false;
        }

        public ConsolePlayer(string name, ROLE role) {
            this.name = name;
            this.role = role;

            IsDead = false;
            PrevPresident = false;
            PrevChancellor = false;
        }

        public bool pollVote(string president, string chancellor) {
            Console.WriteLine("[" + name + "]");
            Console.Write(president + " nominates " + chancellor + " for chancellor. Please vote: ");
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

        public string nominatePlayer(List<string> candidates, string action) {
            Console.WriteLine("[" + name + "]");
            Console.Write("Candidates for " + action + ": ");
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

        public POLICY chooseCard(List<POLICY> cards, string action) {
            Console.WriteLine("[" + name + "]");
            Console.WriteLine("Please choose a card to " + action + ".");
            for (int i = 0; i < cards.Count; i++) {
                if (i != 0) Console.Write(", ");
                Console.Write(cards[i].ToString());
            }
            Console.WriteLine();

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

        public void notifyParty(string player, string party) {
            Console.WriteLine("[" + name + "]");
            Console.WriteLine(player + " is in the " + party + " party.");
        }

        public void notifyCards(List<POLICY> cards) {
            Console.WriteLine("[" + name + "]");
            Console.Write("Next 3 cards: ");
            for (int i = 0; i < cards.Count; i++) {
                if (i != 0) Console.Write(", ");
                Console.Write(cards[i].ToString());
            }
            Console.WriteLine();
        }

        public void notifyStart(string president) {
            Console.WriteLine("Starting new round with " + president + " as president.");
        }

        public string Party {
            get {
                if (role == ROLE.Hitler || role == ROLE.Fascist) {
                    return "Fascist";
                }
                return "Liberal";
            }
        }
    }
}
