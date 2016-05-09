using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SecretHitler {
    public class PoliciesDeck {
        const int libCards = 6;
        const int fasCards = 11;

        private List<POLICY> discardPile;
        private List<POLICY> drawPile;

        public PoliciesDeck() {
            drawPile = new List<POLICY>();
            discardPile = new List<POLICY>();

            for (int i = 0; i < libCards; i++) {
                drawPile.Add(POLICY.Liberal);
            }

            for (int i = 0; i < fasCards; i++) {
                drawPile.Add(POLICY.Fascist);
            }
            drawPile.Shuffle();
        }

        public List<POLICY> drawCards(int numCards) {
            if (drawPile.Count < numCards) {
                collectDiscard();
            }

            List<POLICY> result = new List<POLICY>();
            for (int i = 0; i < numCards; i++) {
                result.Add(drawPile[0]);
                drawPile.RemoveAt(0);
            }
            return result;
        }

        public List<POLICY> showTopThree() {
            if (drawPile.Count < 3) {
                collectDiscard();
            }

            List<POLICY> result = new List<POLICY>();
            for (int i = 0; i < 3; i++) {
                result.Add(drawPile[i]);
            }
            return result;
        }

        private void collectDiscard() {
            foreach (POLICY card in discardPile) {
                drawPile.Add(card);
            }
            discardPile = new List<POLICY>();
            drawPile.Shuffle();
        }

        public void discard(POLICY card) {
            discardPile.Add(card);
        }
    }

    public static class ThreadSafeRandom {
        [ThreadStatic]
        private static System.Random Local;

        public static System.Random ThisThreadsRandom {
            get { return Local ?? (Local = new System.Random(unchecked(Environment.TickCount * 31 + Thread.CurrentThread.ManagedThreadId))); }
        }
    }

    static class MyExtensions {
        public static void Shuffle<T>(this IList<T> list) {
            int n = list.Count;
            while (n > 1) {
                n--;
                int k = ThreadSafeRandom.ThisThreadsRandom.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
