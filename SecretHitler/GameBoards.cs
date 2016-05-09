using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretHitler {
    public class GameBoards {
        public GameBoard FascistBoard;
        public GameBoard LiberalBoard;

        public GameBoards(GAMESIZE size) {
            LiberalBoard = new GameBoard(5);
            LiberalBoard.AddPower(5, POWER.Win);

            FascistBoard = new GameBoard(6);

            FascistBoard.AddPower(6, POWER.Win);
            FascistBoard.AddPower(5, POWER.Assassination);
            FascistBoard.AddPower(4, POWER.Assassination);

            if (size == GAMESIZE.Small) {
                FascistBoard.AddPower(3, POWER.PolicyCheck);
            } else {
                FascistBoard.AddPower(3, POWER.SpecialElection);
                FascistBoard.AddPower(2, POWER.LoyaltyCheck);

                if(size == GAMESIZE.Large) {
                    FascistBoard.AddPower(1, POWER.LoyaltyCheck);
                }
            }
        }

        public POWER playCard(POLICY card) {
            if (card == POLICY.Liberal) {
                return LiberalBoard.playCard();
            } else {
                return FascistBoard.playCard();
            }
        }

        public string SpacesLeft {
            get {
                return FascistBoard.SpacesLeft + " spaces left on Fascist board and " + LiberalBoard.SpacesLeft +
                    " spaces left on Liberal board.";
            }
        }
    }

    public class GameBoard {
        private Dictionary<int, POWER> powers;
        private int cardsPlayed;
        private int boardLength;

        public GameBoard(int boardLength) {
            this.boardLength = boardLength;
            powers = new Dictionary<int, POWER>();
            cardsPlayed = 0;
        }

        public POWER playCard() {
            cardsPlayed++;
            if(powers.ContainsKey(cardsPlayed)) {
                return powers[cardsPlayed];
            }
            return POWER.None;
        }

        public void AddPower(int position, POWER power) {
            powers[position] = power;
        }

        public int CardsPlayed {
            get {
                return cardsPlayed;
            }
            set {
                cardsPlayed = value;
            }
        }

        public string SpacesLeft {
            get {
                return (boardLength - cardsPlayed).ToString();
            }
        }
    }
}
