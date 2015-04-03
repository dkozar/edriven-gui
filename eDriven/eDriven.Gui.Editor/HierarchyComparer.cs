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
using eDriven.Gui.Editor.Hierarchy;
using UnityEngine;

namespace eDriven.Gui.Editor
{
    /// <summary>
    /// A class for comparing 2 hierarchies
    /// </summary>
    internal static class HierarchyComparer
    {
        /// <summary>
        /// Compares 2 hierarchies for changes:<br/>
        /// 1. Additions<br/>
        /// 2. Removals<br/>
        /// 3. TopLevelRemovals<br/>
        /// 4. Moves<br/>
        /// 5. Renamings<br/>
        /// <br/>
        /// Detailed:<br/>
        /// 1. Additions: When something is added to a hierarchy while in play mode, we have to add it again after the play mode stopped<br/>
        /// We have to do through each added component in chield hierarchy and add it (TODO: what to do with prefabs?)<br/>
        /// 3. Removals: we have to remove persisted data from the list IMMEDIATELLY while still in play mode<br/>
        /// 2. TopLevelRemovals: We have to remove only the TOP components or containers when play mode STOPPED.<br/>
        /// Meaning, if we removed the panel with children while in play mode it will be recreated <br/>
        /// after the play mode is stopped, including all the children. We should remove only the parent.<br/>
        /// </summary>
        /// <param name="oldNode"></param>
        /// <param name="newNode"></param>
        /// <returns></returns>
        public static Delta Compare(Node oldNode, Node newNode)
        {
            Delta delta = new Delta();
            //Process(oldNode, newNode, ref delta);

            var oldList = oldNode.ToList();
            var newList = newNode.ToList();

            /**
             * 1. Raw removals = all removed transforms (including children)
             * */
            var removals = FindMissing(oldList, newList);
            //Debug.Log("### Removals: " + removals.Count);

            /**
             * 2. Top level removals
             * Ignore children if parent in the list -> this is giving us top containers being removed
             * */
            var topLevelRemovals = removals.FindAll(delegate(Node node) // all nodes NOT having the parent node in the list
            {
                //return true; // TEMP
                return !removals.Contains(node.ParentNode);
            });
            
            // Important: we want to process removals bottom-up (from children to parent)
            removals.Reverse();

            /**
             * 3. Delta
             * */
            delta.Removals = removals;
            delta.TopLevelRemovals = topLevelRemovals;

            /**
             * ############ ADDITIONS ############
             * */

            /**
             * 1. Raw additions = all added transforms (including children)
             * */
            var additions = FindMissing(newList, oldList);
            
            /**
             * 2. Top level additions
             * Ignore children if parent in the list -> this is giving us top containers being added
             * */
            var topLevelAdditions = additions.FindAll(delegate(Node node) // all nodes NOT having the parent node in the list
            {
                /**
                 * Note: there was an error before the 20130612
                 * Instead of only the top level additions, all the additions were processed
                 * So it turned out that each sub-collection of the added prefab is being re-scanned, thus overwriting the order saved with a prefab
                 * Only the top-level additions needs to be processed
                 */

                //return true; // TEMP
                return !additions.Contains(node.ParentNode); // 20130612
            });

            /**
             * 3. Delta
             * */
            delta.Additions = additions;
            delta.TopLevelAdditions = topLevelAdditions;

            /**
             * ############ MOVES ############
             * */

            /**
             * 1. Building the list of old list candidates (all nodes in the old list that are NOT REMOVED)
             * */
            var candidatesInTheOldList = oldList.FindAll(delegate(Node node)
                                  {
                                      return !removals.Contains(node);
                                  });

            /**
             * 2. Building the list of new list candidates (all the nodes in the new list that are NOT ADD)
             * */
            var moves = newList.FindAll(delegate(Node node)
            {
                Node oldSnapshot = candidatesInTheOldList.Find(delegate(Node on)
                                                               {
                                                                   return on.Transform == node.Transform;
                                                               });

                var changed = !additions.Contains(node) && node.HasChangedParent(oldSnapshot);

                if (changed)
                {
                    node.OldParentTransform = oldSnapshot.ParentTransform;
                }

                return changed;
            });

            /**
             * 3. Delta
             * */
            delta.Moves = moves;

            /**
             * ############ RENAMINGS ############
             * */

            delta.Renamings = FindRenamed(oldList, newList);

            return delta;
        }

        /// <summary>
        /// Returns nodes missing in old
        /// </summary>
        /// <param name="allNodes"></param>
        /// <param name="subNodes"></param>
        private static List<Node> FindMissing(IEnumerable<Node> allNodes, ICollection<Node> subNodes)
        {
            List<Node> missing = new List<Node>();
            foreach (Node node in allNodes)
            {
                if (!subNodes.Contains(node))
                {
                    missing.Add(node);
                }
            }
            return missing;
        }

        /// <summary>
        /// Returns renamed nodes missing
        /// </summary>
        /// <param name="oldNodes"></param>
        /// <param name="newNodes"></param>
        private static List<Node> FindRenamed(List<Node> oldNodes, IEnumerable<Node> newNodes)
        {
            List<Node> renamed = new List<Node>();
            foreach (Node newNode in newNodes)
            {
                if (oldNodes.Contains(newNode))
                {
                    var newTransform = newNode.Transform;
                    var node = oldNodes.Find(delegate(Node oldNode)
                                      {
                                          return oldNode.Transform == newTransform;
                                      });
                    if (newNode.Name != node.Name)
                    {
                        renamed.Add(newNode);
                    }
                }
            }
            return renamed;
        }
    }
}