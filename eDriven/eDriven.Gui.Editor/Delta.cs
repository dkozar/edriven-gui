#region License

/*
 
Copyright (c) 2010-2014 Danko Kozar

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
 
*/

#endregion License

using System.Collections.Generic;
using System.Text;
using eDriven.Gui.Editor.Hierarchy;

namespace eDriven.Gui.Editor
{
    internal class Delta
    {
        /// <summary>
        /// Components added while in play mode<br/>
        /// Used for recreating the component tree AFTER the play mode stopped (instantiation)
        /// </summary>
        public List<Node> Additions = new List<Node>();

        /// <summary>
        /// Top components added while in play mode<br/>
        /// Used for recreating the component tree AFTER the play mode stopped (parent ordering)
        /// </summary>
        public List<Node> TopLevelAdditions = new List<Node>();

        /// <summary>
        /// Components removed while in play mode (including children)<br/>
        /// Used for removing persisted objects from the list BEFORE the play mode fully stopped
        /// </summary>
        public List<Node> Removals = new List<Node>();

        /// <summary>
        /// Components removed while in play mode (only top-level components)<br/>
        /// Used for removing resurrected objects from the scene AFTER the play mode stopped
        /// </summary>
        public List<Node> TopLevelRemovals = new List<Node>();

        /// <summary>
        /// Components that changed position while in play mode<br/>
        /// Used for re-parenting AFTER play mode stopped
        /// </summary>
        public List<Node> Moves = new List<Node>();

        /// <summary>
        /// Components that changed game object name<br/>
        /// Used for renaming AFTER play mode stopped
        /// </summary>
        public List<Node> Renamings = new List<Node>();

        private static StringBuilder _sb;

        /// <summary>
        /// Accumulates the change
        /// </summary>
        /// <param name="delta"></param>
        public void Accumulate(Delta delta)
        {
            Additions.AddRange(delta.Additions);
            TopLevelAdditions.AddRange(delta.TopLevelAdditions);
            Removals.AddRange(delta.Removals);
            TopLevelRemovals.AddRange(delta.TopLevelRemovals);
            Moves.AddRange(delta.Moves);
            Renamings.AddRange(delta.Renamings);
        }

        /// <summary>
        /// Resets the delta
        /// </summary>
        public void Reset()
        {
            Additions.Clear();
            TopLevelAdditions.Clear();
            Removals.Clear();
            TopLevelRemovals.Clear();
            Moves.Clear();
            Renamings.Clear();
        }

        /// <summary>
        /// Used for figuring out if any hierarchy change will be applied
        /// This is important because 1 or 2 hierarchy changes are being applied, depending if this is true or false
        /// </summary>
        /// <returns></returns>
        public bool HasChanges
        {
            get
            {
                return Additions.Count > 0 ||
                   TopLevelAdditions.Count > 0 ||
                   Removals.Count > 0 ||
                   TopLevelRemovals.Count > 0 ||
                   Moves.Count > 0 ||
                   Renamings.Count > 0;
            }
        }

        public override string ToString()
        {
            _sb = new StringBuilder(string.Format(@"[Additions: {0}, TopLevelAdditions: {1}, Removals: {2}, TopLevelRemovals: {3}, Moves: {4}, Renamings: {5}]
------------------------------------------------------------------------------------------------------------------------

", Additions.Count, TopLevelAdditions.Count, Removals.Count, TopLevelRemovals.Count, Moves.Count, Renamings.Count));

            if (Additions.Count > 0)
                Describe(ref Additions, "Additions");
            if (TopLevelAdditions.Count > 0)
                Describe(ref TopLevelAdditions, "TopLevelAdditions");
            if (Removals.Count > 0)
                Describe(ref Removals, "Removals");
            if (TopLevelRemovals.Count > 0)
                Describe(ref TopLevelRemovals, "TopLevelRemovals");
            if (Moves.Count > 0)
                Describe(ref Moves, "Moves");
            if (Renamings.Count > 0)
                Describe(ref Renamings, "Renamings");

            return _sb.ToString();
        }

        public static void Describe(ref List<Node> collection, string title)
        {
            _sb.Append(string.Format(@"{0}:
", title));  // ------------------------------------------------------------------------------------------------------------------------
            foreach (Node node in collection)
            {
                //_sb.AppendLine(string.Format("    -> {0}", node));
                _sb.AppendLine(string.Format("    {0}", node));
            }
            _sb.AppendLine();
        }
    }
}