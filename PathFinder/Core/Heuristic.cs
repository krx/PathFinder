using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinder {

    delegate double HeuristicFunc(Node n, Node end);

    class Heuristic {
        public static HeuristicFunc Manhattan = (n, end) => Math.Abs(end.X - n.X) + Math.Abs(end.Y - n.Y);

        public static HeuristicFunc Euclidean = (n, end) => {
            int dx = Math.Abs(end.X - n.X),
                dy = Math.Abs(end.Y - n.Y);
            return Math.Sqrt(dx * dx + dy * dy);
        };

        public static HeuristicFunc Chebyshev = (n, end) => Math.Max(Math.Abs(end.X - n.X), Math.Abs(end.Y - n.Y));
    }
}
