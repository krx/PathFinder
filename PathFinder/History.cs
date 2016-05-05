using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinder {
    class History {
        private List<NodeChange> changes;
        private Grid grid;
        private int idx = 0;

        public History(Grid g) {
            changes = new List<NodeChange>();
            grid = g;
        }

        public void Push(Node n, NodeState next) {
            changes.Add(new NodeChange(n.X, n.Y, next));
        }

        public void Clear(bool clearPath = true) {
            if (changes.Count > 0) {
                changes.Clear();
                idx = 0;
                if (clearPath) grid.ClearPath();
            }
        }

        public void Reset() {
            if (changes.Count > 0) {
                idx = 0;
                grid.ClearPath();
            }
        }

        public bool Step() {
            if (idx >= changes.Count) return false;
            NodeChange nc = changes[idx++];
            grid[nc.Y, nc.X].State = nc.NextState;
            return true;
        }
    }

    class NodeChange {
        public int X { get; }
        public int Y { get; }
        public NodeState NextState { get; }

        public NodeChange(int x, int y, NodeState nextState) {
            X = x;
            Y = y;
            NextState = nextState;
        }
    }
}
