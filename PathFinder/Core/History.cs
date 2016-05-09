using System.Collections.Generic;

namespace PathFinder.Core {

    /// <summary>
    /// A history list to keep track of all state changes made to nodes in a grid
    /// </summary>
    internal class History {
        // List of all changes that were made
        private List<NodeChange> changes;

        // Grid being used for these nodes
        private Grid grid;

        // Current index in the history
        private int idx = 0;

        // Properties about current index
        public bool AtBeginning => idx <= 0;
        public bool AtEnd => idx >= changes.Count;

        /// <summary>
        /// Create a blank history
        /// </summary>
        /// <param name="g">Grid to use for accessing nodes</param>
        public History(Grid g) {
            changes = new List<NodeChange>();
            grid = g;
        }

        /// <summary>
        /// Add a state change to the history
        /// </summary>
        /// <param name="n">Affected node</param>
        /// <param name="next">The new state of the node</param>
        public void Push(Node n, NodeState next) {
            if (n.IsStart || n.IsEnd) return;
            changes.Add(new NodeChange(n.X, n.Y, next));
        }

        /// <summary>
        /// Clear all history and optionally reset the grid
        /// </summary>
        /// <param name="clearPath">Whether the grid display should be reset in the process</param>
        public void Clear(bool clearPath = true) {
            Reset(clearPath);
            changes.Clear();
        }

        /// <summary>
        /// Rewind to the beginning of the history and optionally clear the grid
        /// </summary>
        /// <param name="clearPath">Whether the grid display should be reset in the process</param>
        public void Reset(bool clearPath = true) {
            // Nothing to do if the history is blank
            if (changes.Count == 0) return;

            // Return to the start of the history
            idx = 0;
            if (clearPath) grid.ClearPath();
        }

        /// <summary>
        /// Attempt to take a step in the history
        /// </summary>
        /// <returns>True if a step was taken</returns>
        public bool Step() {
            // Quit if the end has already been reached
            if (AtEnd) return false;

            // Load the state change
            NodeChange nc = changes[idx++];

            // Save the current state of the node for stepping backwards
            nc.PrevState = grid[nc.Y, nc.X].State;

            // Apply the state change to the grid
            grid[nc.Y, nc.X].State = nc.NextState;
            return true;
        }

        public bool StepBack() {
            // Quit if the beginning has already been reached
            if (AtBeginning) return false;

            // Step back once and revert the state of the node
            NodeChange nc = changes[--idx];
            grid[nc.Y, nc.X].State = nc.PrevState;
            return true;
        }
    }

    /// <summary>
    /// Helper class to contain a single step in the history
    /// </summary>
    internal class NodeChange {
        public int X { get; }
        public int Y { get; }
        public NodeState NextState { get; }
        public NodeState PrevState { get; set; }

        public NodeChange(int x, int y, NodeState nextState) {
            X = x;
            Y = y;
            NextState = nextState;
        }
    }
}
