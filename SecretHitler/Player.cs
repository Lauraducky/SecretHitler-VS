using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretHitler {
    abstract class Player {
        protected string name;
        protected ROLE role;

        public Player() {
            IsDead = false;
            PrevPresident = false;
            PrevChancellor = false;
        }

        public virtual void setRole(ROLE role) {
            this.role = role;
        }

        public abstract void pollVote(string president, string chancellor);

        public abstract void nominatePlayer(List<string> candidates, string action);

        public abstract void chooseCard(List<POLICY> cards, string action);

        public abstract void notifyParty(string player, string party);

        public abstract void notifyCards(List<POLICY> cards);

        public abstract void notifyStart(string president);

        public void resetStatus() {
            PrevChancellor = false;
            PrevPresident = false;
        }

        public bool IsDead { get; set; }
        public bool PrevPresident { get; set; }
        public bool PrevChancellor { get; set; }

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
