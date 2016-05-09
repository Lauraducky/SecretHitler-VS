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

        public override bool pollVote(string president, string chancellor) {
            return true;
        }

        public override string nominatePlayer(List<string> candidates, string action) {
            return candidates[0];
        }

        public override POLICY chooseCard(List<POLICY> cards, string action) {
            return cards[0];
        }

        public override void notifyParty(string player, string party) {

        }

        public override void notifyCards(List<POLICY> cards) {

        }
    }
}
