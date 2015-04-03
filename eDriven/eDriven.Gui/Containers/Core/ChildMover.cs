using System.Collections.Generic;
using System.Text;
using eDriven.Gui.Components;
using UnityEngine;

namespace eDriven.Gui.Containers
{
    internal class ChildMover
    {
#if DEBUG
// ReSharper disable UnassignedField.Global
        /// <summary>
        /// Debug mode
        /// </summary>
        public static bool DebugMode;

// ReSharper restore UnassignedField.Global
#endif

        private readonly DisplayObjectContainer _docFrom;
        private readonly DisplayObjectContainer _docTo;
        //private readonly Container _ctFrom;
        //private readonly Container _ctTo;
        private readonly int _numberOfChildrenToMove;

        public ChildMover(DisplayObjectContainer from, DisplayObjectContainer to, int numberOfChildren)
        {
            _docFrom = from;
            _docTo = to;

            //_ctFrom = _docFrom as Container;
            //_ctTo = _docTo as Container;

            _numberOfChildrenToMove = numberOfChildren;
        }

        public void Move()
        {

#if DEBUG
            if (DebugMode)
            {
                Debug.Log(string.Format(@"Moving: {0}
", DescribeMoves()));
            }
#endif

            // TODO: CreatingContentPane pane has already been set from the outside (the caller of this method)
            // that is because this flag has to be set not only for mover operations, but also for "manual" aditions
            // so we must rethink if this is to be used also here (it's redundant)

            List<DisplayListMember> childrenToTransfer = new List<DisplayListMember>();

            for (int i = 0; i < _numberOfChildrenToMove; i++)
            {
                childrenToTransfer.Add(_docFrom.QChildren[i]);
            }

            foreach (DisplayListMember child in childrenToTransfer)
            {
                /**
                 * Note: we have to use container's AddChild/RemoveChild stuff, because else the control won't initialize properly and its Visible will be false! // 20130507
                 * */
                if (null != _docFrom)
                    _docFrom.RemoveChild(child);

                if (null != _docTo)
                    _docTo.AddChild(child);
            }

#if DEBUG
            if (DebugMode)
            {
                Debug.Log(string.Format(@"Moved: {0}
", DescribeMoves()));
            }
#endif

            if (null != _docFrom)
                _docFrom.InvalidateDrawingList();
            if (null != _docTo)
                _docTo.InvalidateDrawingList();
        }

        /// <summary>
        /// Describes the collection
        /// </summary>
        /// <param name="children">Collection to describe</param>
        /// <returns></returns>
        private static string DescribeCollection(ICollection<DisplayListMember> children)
        {
            StringBuilder sb = new StringBuilder();
            int count = 0;
            if (children.Count == 0)
            {
                sb.Append("   - none -");
            }
            else
            {
                foreach (DisplayListMember child in children)
                {
                    if (count < children.Count - 1)
                        sb.AppendLine("   - " + child);
                    else
                        sb.Append("   - " + child);
                    count++;
                }
            }
            return sb.ToString();
        }

        private string DescribeMoves()
        {
            return string.Format(@"{3} -> [{0}] -> {4}
Source: {1} [{3} children]
Destination: {2} [{4} children]
Source children: 
{5}
Destination children: 
{6}", _numberOfChildrenToMove, 
    _docFrom, _docTo, 
    _docFrom.QChildren.Count, _docTo.QChildren.Count, 
    DescribeCollection(_docFrom.QChildren), DescribeCollection(_docTo.QChildren));
        }
    }
}