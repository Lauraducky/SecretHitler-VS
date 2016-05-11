using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretHitler {
    static class MessageHandler {
        static Dictionary<string, NetworkPlayer> players;
        static volatile bool handlingMessages = false;

        public static List<bool> votes = new List<bool>();
        public static volatile string nomination = null;
        public static volatile string card = null; //Should be of type POLICY, but no can do

        /*********************
         * MESSAGES RECEIVED *
         ********************/
        public static void handleMessages() {
            handlingMessages = true;
            while (handlingMessages) {
                foreach (NetworkPlayer player in players.Values) {
                    while (!player.messageQueue.IsEmpty) {
                        string response = null;
                        if(player.messageQueue.TryDequeue(out response)){
                            parseResponse(response);
                        }
                    }
                }
            }
        }

        private static void parseResponse(string response) {
            int index = response.IndexOf(' ');
            string command = response.Substring(0, index);
            string args = response.Substring(index + 1);

            switch (command) {
                case "VOTE":
                    getVotes(args);
                    break;
                case "NOMINATE":
                    nomination = args;
                    break;
                case "POLICY":
                    card = args;
                    break;
            }
        }
        
        //VOTE <ya/nein> ***RESPONSE
        public static void getVotes(string vote) {
            if(vote == "ya") {
                votes.Add(true);
            } else if (vote == "nein") {
                votes.Add(false);
            }
        }

        //NOMINATE <KILL/CHECK/CHANCELLOR/PRESIDENT> <nominee> ***RESPONSE
        //POLICY <DISCARD/ENACT> <card> ***RESPONSE

        //CLEAR RESPONSES
        public static void clearVotes() {
            votes = new List<bool>();
        }
    }
}
