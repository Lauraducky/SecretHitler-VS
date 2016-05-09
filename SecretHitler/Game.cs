using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretHitler {
    class Game {
        private PoliciesDeck policies = new PoliciesDeck();
        private GameBoards boards;
        private Dictionary<string, Player> players;
        private int numPlayers;
        private GAMESIZE size;

        private List<string> roster;
        private int nextPresident;

        private string president;
        private string chancellor;

        private bool specialElection = false;
        private string specialCandidate;

        public Game (Dictionary<string, Player> players) {
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
            players = new Dictionary<string, Player>();

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

            foreach(string name in temp) {
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

            List<string> candidates = new List<string>();
            foreach (KeyValuePair<string, Player> pair in players) {
                if(!pair.Value.IsDead && !pair.Value.PrevChancellor) {
                    if (size != GAMESIZE.Small) {
                        if (!pair.Value.PrevPresident)
                            candidates.Add(pair.Key);
                    } else {
                        candidates.Add(pair.Key);
                    }
                }
            }
            candidates.Remove(presCandidate);

            string chanCandidate = players[presCandidate].nominatePlayer(candidates, "Chancellorship");

            Console.WriteLine("Voting phase. Vote 'ya' for yes, and 'nein' for no.");

            int voteCount = 0;
            foreach(Player current in players.Values) {
                if (!current.IsDead) {
                    if (current.pollVote(presCandidate, chanCandidate)) {
                        voteCount++;
                    } else {
                        voteCount--;
                    }
                }
            }

            if(voteCount > 0) {
                Console.WriteLine("Vote passed.");
                president = presCandidate;
                chancellor = chanCandidate;
                choosePolicy();
            } else {
                Console.WriteLine("Vote failed.");
                electGovt();
            }
        }

        private void choosePolicy() {
            List<POLICY> cards = policies.drawCards(3);
            POLICY discard = players[president].chooseCard(cards, "DISCARD");
            cards.Remove(discard);
            policies.discard(discard);
            
            POLICY toPlay = players[chancellor].chooseCard(cards, "ENACT");
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
            foreach (Player current in players.Values) {
                current.resetStatus();
            }
            players[president].PrevPresident = true;
            players[chancellor].PrevChancellor = true;

            electGovt();
        }

        private void resolvePower(POWER power) {
            List<string> candidates = new List<string>();
            foreach (KeyValuePair<string, Player> pair in players) {
                if(!pair.Value.IsDead) {
                    candidates.Add(pair.Key);
                }
            }
            candidates.Remove(president);

            switch (power) {
                case POWER.Assassination:
                    string target = players[president].nominatePlayer(candidates, "KILL");
                    Console.WriteLine(target + " was killed by " + president + ".");
                    players[target].IsDead = true;
                    break;
                case POWER.LoyaltyCheck:
                    string player = players[president].nominatePlayer(candidates, "CHECK");
                    players[president].notifyParty(player, players[player].Party);
                    break;
                case POWER.PolicyCheck:
                    players[president].notifyCards(policies.showTopThree());
                    break;
                case POWER.SpecialElection:
                    specialCandidate = players[president].nominatePlayer(candidates, "PRESIDENT");
                    specialElection = true;
                    break;
            }
        }
    }
}
