using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretHitler {
    class AIPlayer : Player {

        public AIPlayer() {
            IsDead = false;
            PrevPresident = false;
            PrevChancellor = false;
        }

        public override void setRole(ROLE role) {
            this.role = role;
        }

        public override void pollVote(string president, string chancellor) {
            messageQueue.Enqueue("VOTE ya");
        }

        public override void nominatePlayer(List<string> candidates, string action) {
            messageQueue.Enqueue("NOMINATE " + candidates[0]);
        }

        public override void chooseCard(List<POLICY> cards, string action) {
            messageQueue.Enqueue("POLICY " + action + " " + cards[0].ToString());
        }

        public override void notifyParty(string player, string party) {
            return;
        }

        public override void notifyCards(List<POLICY> cards) {
            return;
        }

        public override void notifyStart(string president) {
            return;
        }
    }
}
