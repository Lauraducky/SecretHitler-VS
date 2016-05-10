using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretHitler {
    class NetworkedGame {
        private PoliciesDeck policies = new PoliciesDeck();
        private GameBoards boards;
        private Dictionary<string, NetworkPlayer> players;
        private int numPlayers;
        private GAMESIZE size;

        private List<string> roster;
        private int nextPresident;

        private string president;
        private string chancellor;

        private bool specialElection = false;
        private string specialCandidate;

        public NetworkedGame(Dictionary<string, NetworkPlayer> players) {
            this.players = players;
            roster = new List<string>(players.Keys);
            roster.Shuffle();
            nextPresident = 0;

            numPlayers = players.Count();

            if (numPlayers < 7) {
                size = GAMESIZE.Small;
            } else if (numPlayers < 9) {
                size = GAMESIZE.Medium;
            } else {
                size = GAMESIZE.Large;
            }

            boards = new GameBoards(size);

            List<string> temp = new List<string>(players.Keys);
            players = new Dictionary<string, NetworkPlayer>();

            int numFascists;

            if (numPlayers % 2 == 0) {
                numFascists = numPlayers / 2 - 1;
            } else {
                numFascists = (numPlayers - 1) / 2;
            }

            int fascist = ThreadSafeRandom.ThisThreadsRandom.Next(temp.Count);
            players[temp[fascist]].setRole(ROLE.Hitler);
            temp.RemoveAt(fascist);

            for (int i = 1; i < numFascists; i++) {
                fascist = ThreadSafeRandom.ThisThreadsRandom.Next(temp.Count);
                players[temp[fascist]].setRole(ROLE.Fascist);
                temp.RemoveAt(fascist);
            }

            foreach (string name in temp) {
                players[name].setRole(ROLE.Liberal);
            }

            electGovt();
        }

        private void electGovt() {
            string presCandidate;
            if (specialElection) {
                presCandidate = specialCandidate;
                specialElection = false;
            } else {
                if (nextPresident >= numPlayers) {
                    nextPresident = 0;
                }
                presCandidate = roster[nextPresident];
                nextPresident++;
            }

            foreach (NetworkPlayer current in players.Values) {
                current.notifyStart(presCandidate);
            }

            List<string> candidates = new List<string>();
            foreach (KeyValuePair<string, NetworkPlayer> pair in players) {
                if (!pair.Value.IsDead && !pair.Value.PrevChancellor) {
                    if (size != GAMESIZE.Small) {
                        if (!pair.Value.PrevPresident)
                            candidates.Add(pair.Key);
                    } else {
                        candidates.Add(pair.Key);
                    }
                }
            }
            candidates.Remove(presCandidate);
            players[presCandidate].nominatePlayer(candidates, "Chancellorship");

            string chanCandidate = null;
            while (MessageHandler.nomination == null) ; //Wait for a response to come in
            chanCandidate = MessageHandler.nomination;
            MessageHandler.nomination = null;

            Console.WriteLine("Voting phase. Vote 'ya' for yes, and 'nein' for no.");

            if (runVote(presCandidate, chanCandidate)) {
                Console.WriteLine("Vote passed.");
                president = presCandidate;
                chancellor = chanCandidate;
                choosePolicy();
            } else {
                Console.WriteLine("Vote failed.");
                electGovt();
            }
        }

        private bool runVote(string presCandidate, string chanCandidate) {
            int polled = 0;
            foreach (NetworkPlayer current in players.Values) {
                if (!current.IsDead) {
                    current.pollVote(presCandidate, chanCandidate);
                    polled++;
                }
            }

            while (MessageHandler.votes.Count != polled) ; //Wait for responses
            List<bool> votes = new List<bool>(MessageHandler.votes);
            MessageHandler.clearVotes();

            int voteCount = 0;
            foreach (bool vote in votes) {
                if (vote) voteCount++;
            }

            if(voteCount > polled / 2) {
                return true;
            }
            return false;
        }

        private void choosePolicy() {
            List<POLICY> cards = policies.drawCards(3);
            players[president].chooseCard(cards, "DISCARD");

            POLICY discard;
            while (MessageHandler.card == null) ; //Wait for a response
            discard = (POLICY) Enum.Parse(typeof(POLICY), MessageHandler.card);
            MessageHandler.card = null;

            cards.Remove(discard);
            policies.discard(discard);
            players[chancellor].chooseCard(cards, "ENACT");

            POLICY toPlay;
            while (MessageHandler.card == null) ; //Wait for a response
            toPlay = (POLICY)Enum.Parse(typeof(POLICY), MessageHandler.card);
            MessageHandler.card = null;

            cards.Remove(toPlay);

            POWER current = boards.playCard(toPlay);
            policies.discard(cards[0]);

            Console.WriteLine(toPlay.ToString() + " policy was played. " + boards.SpacesLeft);
            if (current == POWER.Win) {
                Console.WriteLine(toPlay.ToString() + "s win.");
            } else if (current == POWER.None) {
                resolveRound();
            } else {
                resolvePower(current);
                resolveRound();
            }
        }

        private void resolveRound() {
            foreach (NetworkPlayer current in players.Values) {
                current.resetStatus();
            }
            players[president].PrevPresident = true;
            players[chancellor].PrevChancellor = true;

            electGovt();
        }

        private void resolvePower(POWER power) {
            List<string> candidates = new List<string>();
            foreach (KeyValuePair<string, NetworkPlayer> pair in players) {
                if (!pair.Value.IsDead) {
                    candidates.Add(pair.Key);
                }
            }
            candidates.Remove(president);

            switch (power) {
                case POWER.Assassination:
                    players[president].nominatePlayer(candidates, "KILL");
                    string target = getNominee();
                    Console.WriteLine(target + " was killed by " + president + ".");
                    players[target].IsDead = true;
                    break;
                case POWER.LoyaltyCheck:
                    players[president].nominatePlayer(candidates, "CHECK");
                    string player = getNominee();
                    players[president].notifyParty(player, players[player].Party);
                    break;
                case POWER.PolicyCheck:
                    players[president].notifyCards(policies.showTopThree());
                    break;
                case POWER.SpecialElection:
                    players[president].nominatePlayer(candidates, "PRESIDENT");
                    specialCandidate = getNominee();
                    specialElection = true;
                    break;
            }
        }

        private string getNominee() {
            while (MessageHandler.nomination == null) ; //Wait for a response
            string nomination = MessageHandler.nomination;
            MessageHandler.nomination = null;
            return nomination;
        }
    }
}
