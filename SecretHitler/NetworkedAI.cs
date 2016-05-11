using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretHitler {
    //A 'networked' AI that will work the way the program expects a human to
    //Base level functionality, no intelligence
    class NetworkedAI : Player {
        public override void chooseCard(List<POLICY> cards, string action) {
            throw new NotImplementedException();
        }

        public override void nominatePlayer(List<string> candidates, string action) {
            throw new NotImplementedException();
        }

        public override void notifyCards(List<POLICY> cards) {
            throw new NotImplementedException();
        }

        public override void notifyParty(string player, string party) {
            throw new NotImplementedException();
        }

        public override void notifyStart(string president) {
            throw new NotImplementedException();
        }

        public override void pollVote(string president, string chancellor) {
            throw new NotImplementedException();
        }
    }
}
