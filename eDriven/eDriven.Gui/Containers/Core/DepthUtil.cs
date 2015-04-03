using System;
using System.Collections.Generic;
using eDriven.Gui.Components;
using eDriven.Gui.Util;
using UnityEngine;

namespace eDriven.Gui.Containers
{
    /// <summary>
    /// This is the utility responsible for handling th edrawing list order<br/>
    /// The drawing list is different than the child list in a sense that it is ordered by Depth property of each child (integer)<br/>
    /// 
    /// </summary>
    internal static class DepthUtil
    {
#if DEBUG
// ReSharper disable UnassignedField.Global
        /// <summary>
        /// Debug mode
        /// </summary>
        public static bool DebugMode;

// ReSharper restore UnassignedField.Global
#endif

        private static DisplayListMember _tmp;

        /// <summary>
        /// This utility sorts the drawing list by request<br/>
        /// Takes the component as input and reads its child list internally<br/>
        /// Then it sorts it and saves into the drawing list<br/>
        /// Note: the original child list is never being modified (we want to stay robust here)
        /// </summary>
        /// <param name="doc"></param>
        public static void UpdateDrawingList(DisplayObjectContainer doc)
        {
#if DEBUG
            if (DebugMode)
            {
                Debug.Log(string.Format("         DepthUtil -> UpdateDrawingList: {0} -> [orderList: {1}, depthList: {2}]", doc, doc.QNumberOfChildren, doc.QDrawingList.Count));
            }
#endif
            UpdateDrawingList(doc.QChildren, doc.QDrawingList);
        }

        /// <summary>
        /// Brings the child to front in a depth list
        /// </summary>
        /// <param name="depthList"></param>
        /// <param name="child"></param>
        public static void BringToFront(List<DisplayListMember> depthList, DisplayListMember child)
        {
            //Debug.Log("BringToFront");

            var max = GetMaxDepth(depthList);
            //Debug.Log(string.Format(@"depthList.Count: {0}; max: {1}; child.Depth: {2}; child: {3}", depthList.Count, max, child.Depth, child));

            if (child.Depth > max)
                return;

            //Debug.Log(1);

            if (child.Depth == max && IsOnlyChildWithDepth(depthList, child, max))
                return;

            //Debug.Log(2);

            SetDepth(depthList, child, GetMaxDepth(depthList) + 1, true);
        }

        /// <summary>
        /// Pushes the child to back in a depth list
        /// </summary>
        /// <param name="depthList"></param>
        /// <param name="child"></param>
        public static void PushToBack(List<DisplayListMember> depthList, DisplayListMember child)
        {
            var min = GetMinDepth(depthList);
            //Debug.Log("min: " + min);

            if (child.Depth < min)
                return;

            if (child.Depth == min && IsOnlyChildWithDepth(depthList, child, min))
                return;

            SetDepth(depthList, child, GetMinDepth(depthList) - 1, true);
        }

        #region Helper

        /// <summary>
        /// Updates the supplied depth list using the order list<br/>
        /// Generally, it removes all the depth values, refills the list, and then sorts by the child Depth value
        /// </summary>
        /// <param name="orderList"></param>
        /// <param name="depthList"></param>
        // ReSharper disable SuggestBaseTypeForParameter
        private static void UpdateDrawingList(List<DisplayListMember> orderList, List<DisplayListMember> depthList)
        // ReSharper restore SuggestBaseTypeForParameter
        {
            //Debug.Log(string.Format("                  ! -> [orderList: {0}, depthList: {1}]", orderList.Count, depthList.Count));

            /**
             * 1. Empty the list
             * */
            depthList.Clear();

            int len = orderList.Count;

            /**
             * 2. Fill the list with values from the order list
             * */
            foreach (DisplayListMember displayListMember in orderList)
            {
                depthList.Add(displayListMember);
            }

            //Debug.Log("     depthList.Count: " + depthList.Count);

            if (len <= 1)
            {
#if DEBUG
                if (DebugMode)
                {
                    Debug.Log(string.Format("                       -> [orderList: {0}, depthList: {1}] *", orderList.Count, depthList.Count));
                }
#endif
                return; // don't bother
            }

            /**
             * 3. Sort by Depth
             * */
            for (int i = 1; i < len; i++)
            {
                for (int j = i; j > 0; j--)
                {
                    if (depthList[j].Depth < depthList[j - 1].Depth)
                    {
                        _tmp = depthList[j];
                        depthList[j] = depthList[j - 1];
                        depthList[j - 1] = _tmp;
                    }
                    //else
                    //    break;
                }
            }

            //Debug.Log(ListUtil<DisplayListMember>.Format(depthList));

#if DEBUG
            if (DebugMode)
            {
                Debug.Log(string.Format("                       -> [orderList: {0}, depthList: {1}]", orderList.Count, depthList.Count));
            }
#endif
        }

        /// <summary>
        /// Returns the minimum depth found in the list
        /// </summary>
        /// <param name="depthList"></param>
        /// <returns></returns>
        private static int GetMinDepth(ICollection<DisplayListMember> depthList)
        {
            if (depthList.Count == 0)
                return 0;

            int min = Int32.MaxValue;
            foreach (DisplayListMember child in depthList)
            {
                if (child.Depth < min)
                    min = child.Depth;
            }

            return min;
            //return depthList[0].Depth; // assuming the first element has the lowest Depth
        }

        /// <summary>
        /// Returns the maximum depth found in the list
        /// </summary>
        /// <param name="depthList"></param>
        /// <returns></returns>
        private static int GetMaxDepth(ICollection<DisplayListMember> depthList)
        {
            if (depthList.Count == 0)
                return 0;

            int max = Int32.MinValue;
            foreach (DisplayListMember child in depthList)
            {
                if (child.Depth > max)
                    max = child.Depth;
            }

            return max;
            //return depthList[depthList.Count - 1].Depth; // assuming the last element has the highest Depth
        }

        private static List<DisplayListMember> GetChildrenWithDepth(List<DisplayListMember> depthList, int depth)
        {
            return depthList.FindAll(delegate(DisplayListMember displayListMember)
            {
                return displayListMember.Depth == depth;
            });
        }

        private static bool IsOnlyChildWithDepth(List<DisplayListMember> depthList, DisplayListMember child, int depth)
        {
            var childrenWithSameDepth = GetChildrenWithDepth(depthList, depth);
            return childrenWithSameDepth.Count == 1 && childrenWithSameDepth[0] == child;
        }

        //private static int _prevDepth;

        /// <summary>
        /// Changes the depth of a single child within the list
        /// </summary>
        /// <param name="depthList"></param>
        /// <param name="child"></param>
        /// <param name="depth"></param>
        public static void SetDepth(List<DisplayListMember> depthList, DisplayListMember child, int depth)
        {
            SetDepth(depthList, child, depth, true);
        }

        /// <summary>
        /// Changes the depth of a single child within the list
        /// </summary>
        /// <param name="depthList"></param>
        /// <param name="child"></param>
        /// <param name="depth"></param>
        /// <param name="makeRoom"></param>
        private static void SetDepth(List<DisplayListMember> depthList, DisplayListMember child, int depth, bool makeRoom)
        {
            //Debug.Log("Setting depth: " + depth + " [" + child + "]");

            child.Depth = depth;

            // remove
            depthList.Remove(child);

            var index = depthList.FindIndex(delegate(DisplayListMember displayListMember)
            {
                return displayListMember.Depth >= depth;
            });

            if (index == -1) // not found, meaning all depths are lower than the supplied one
                index = depthList.Count;

            // insert at index
            depthList.Insert(index, child);

            var len = depthList.Count;

            // fix other depths

            // say depth = 1
            // and that we have depths [0, 0, 1, 1, 1, 2, 6, 7] and inserting into the 3rd place (between 0 and 1)
            // ... [0, 0, *1, 1, 1, 1, 2, 6, 7] ...
            // we have to get [0, 0, *1, 2, 2, 2, 3, 6, 7] - note that some values are shifted up, but the values of 6, 7 are not because there's no need to

            //_prevDepth = depth;

            if (makeRoom)
            {
                // needs room?
                if (depthList.Count == index + 1 || depthList[index + 1].Depth > depth)
                    return; // this is the last item or the next item has a greater depth

                for (int i = index + 1; i < len; i++) // from 3rd place to the end
                {
                    depthList[index].Depth += 1;
                    //var item2 = depthList[index]; // 2
                    //if (item2.Depth != _prevDepth)
                    //    _prevDepth = item2.Depth; // _prevDepth = 1

                    //if (item2.Depth == _prevDepth) // 1 == 1
                    //{
                    //    item2.Depth = _prevDepth + 1; // item2.Depth = 2
                    //}
                }
            }
        }

        #endregion
    }
}