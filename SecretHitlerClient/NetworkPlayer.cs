using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretHitlerClient {
    class NetworkPlayer {
        public enum ROLE { Liberal, Fascist, Hitler }

        ROLE myRole;
        string myName;
        List<string> fascists = null;
        string hitler = null;

        public NetworkPlayer(string myName) {
            this.myName = myName;
        }

        public string receiveMessage(string message) {
            string response = null;
            if (message == "PASSED") {
                Console.WriteLine("Vote passed");
            } else if (message == "FAILED") {
                Console.WriteLine("Vote failed");
            } else if (message == "END") {
                Console.WriteLine("End round.");
                Console.WriteLine("-----");
            } else if (message == "FINISHED") {
                Console.WriteLine("End of game.");
                return "EXIT";
            } else {
                int index = message.IndexOf(' ');
                string command = message.Substring(0, index);
                string args = message.Substring(index + 1);
                response = handleCommand(command, args);
            }
            return response;
        }

        private string handleCommand(string command, string args) {
            string response = null;
            switch (command) {
                case "ROLE":
                    updateRole(args);
                    break;
                case "START":
                    //START <president>
                    Console.WriteLine("Start Round");
                    Console.WriteLine(args + " is now the President.");
                    break;
                case "VOTE":
                    response = promptVote(args);
                    break;
                case "POLICY":
                    response = promptPolicy(args);
                    break;
                case "NOTIFY":
                    notifyPlayer(args);
                    break;
                case "NOMINATE":
                    response = promptNominate(args);
                    break;
            }
            return response;
        }
        
        //NOMINATE <KILL/CHECK/CHANCELLOR/PRESIDENT> <candidates>
        //NOMINATE <KILL/CHECK/CHANCELLOR/PRESIDENT> <nominee> ***RESPONSE
        private string promptNominate(string message) {
            char[] delim = { ' ' };
            string action;
            int index = message.IndexOf(' ');
            string command = message.Substring(0, index);
            string[] candidates = message.Substring(index + 1).Split(delim);

            action = command;
            if (command == "KILL") {
                action = "assassination";
            } else if (command == "CHECK") {
                action = "loyalty check";
            } else if (command == "CHANCELLOR") {
                action = "Chancellorship";
            } else if (command == "PRESIDENT") {
                action = "special election";
            }

            string person = nominatePlayer(new List<string>(candidates), action);

            return "NOMINATE " + command + " " + person;
        }

        public string nominatePlayer(List<string> candidates, string action) {
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

        //NOTIFY <CARDS/PLAYER> <args>
        private void notifyPlayer(string message) {
            int index = message.IndexOf(' ');
            string type = message.Substring(0, index);
            string info = message.Substring(index + 1);

            if (type == "PLAYER") {
                index = info.IndexOf(' ');
                //TODO: Finish this
            }
        }

        //VOTE <president> <chancellor>
        //VOTE <ya/nein> ***RESPONSE
        private string promptVote(string message) {
            char[] delim = { ' ' };
            string[] players = message.Split(delim);
            string president = players[0];
            string chancellor = players[1];

            Console.Write(president + " nominates " + chancellor + " for chancellor. Please vote: ");
            string answer = Console.ReadLine();

            while (answer != "ya" && answer != "nein") {
                Console.Write("Please vote: ");
                answer = Console.ReadLine();
            }
            return "VOTE " + answer;
        }

        //POLICY <DISCARD/ENACT> <card1> <card2> [card3]
        //POLICY <DISCARD/ENACT> <card> ***RESPONSE
        private string promptPolicy(string message) {
            char[] delim = { ' ' };
            int index = message.IndexOf(' ');
            string action = message.Substring(0, index);
            action = action.ToLower();
            string[] cards = message.Substring(index + 1).Split(delim);

            string chosen = chooseCard(new List<string>(cards), action);

            return "POLICY " + action.ToUpper() + " " + chosen;
        }

        public string chooseCard(List<string> cards, string action) {
            Console.WriteLine("Please choose a card to " + action + ".");
            for (int i = 0; i < cards.Count; i++) {
                if (i != 0) Console.Write(", ");
                Console.Write(cards[i].ToString());
            }
            Console.WriteLine();

            Console.Write("Chosen card: ");
            string card = Console.ReadLine();

            while (!cards.Contains(card)) {
                Console.Write("Chosen card: ");
                card = Console.ReadLine();
            }
            return card;
        }

        //ROLE <rolename> [hitler [fascist1, fascist2...]]
        private void updateRole(string args) {
            char[] delim = { ' ' };
            string[] argsSplit = args.Split(delim);

            switch (argsSplit[0]) {
                case "Hitler":
                    myRole = ROLE.Hitler;
                    break;
                case "Fascist":
                    myRole = ROLE.Fascist;
                    break;
                default:
                    myRole = ROLE.Liberal;
                    break;
            }
            Console.WriteLine("Your role is: " + myRole.ToString());

            if (myRole == ROLE.Fascist || myRole == ROLE.Hitler) {
                fascists = new List<string>();
                hitler = argsSplit[1];
                for (int i = 2; i < argsSplit.Length; i++) {
                    fascists.Add(argsSplit[i]);
                }

                Console.WriteLine("Hitler: " + hitler);
                Console.Write("Fascists: ");
                foreach (string fascist in fascists) Console.Write(fascist + ", ");
                Console.WriteLine(" ");
            }
        }
    }
}
