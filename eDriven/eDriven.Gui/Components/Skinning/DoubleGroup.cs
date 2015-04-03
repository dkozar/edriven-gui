using eDriven.Gui.Containers;

namespace eDriven.Gui.Components
{
    /// <summary>
    /// Class handling the placeholder <--> part operations
    /// </summary>
    internal class DoubleGroup
    {
        private Group _part;
        /// <summary>
        /// The part being injected by the skin
        /// </summary>
        public Group Part
        {
            get
            {
                _modified = true;
                if (null == _part)
                {
                    _modified = true;
                    if (null == _placeholder)
                        _placeholder = new Group {Id = "placeholder"};

                    return _placeholder;
                }
                return _part;
            }
            internal set
            {
                //Debug.Log("ControlBarGroup part set");
                _part = value;
            }
        }

        private Group _placeholder;
        /// <summary>
        /// Temporary placeholder group created if the part has not yet been added
        /// </summary>
// ReSharper disable UnusedMember.Global
        public Group Placeholder
// ReSharper restore UnusedMember.Global
        {
            get
            {
                return _placeholder;
            }
        }

        private bool _modified;
        /// <summary>
        /// Flag indicating that the group has been retrieved via the getter<br/>
        /// For instance, it is set to true when the group is being referenced when adding the children from the outside
        /// </summary>
        public bool Modified
        {
            get { return _modified; }
        }

        ///<summary>Moves all the children of a placeholder group to added part
        ///</summary>
        public void MoveToReal()
        {
            if (!_modified || null == _placeholder)
                return;

            //if (null != _placeholder.Layout)
            //    _part.Layout = _placeholder.Layout;

            var mover = new ChildMover(_placeholder, _part, _placeholder.NumberOfChildren);
            mover.Move();
        }

        ///<summary>Moves all the children from the removed part to a placeholder group
        ///</summary>
        public void MoveToPlaceholder()
        {
            if (!_modified || null == _part)
                return;
            var mover = new ChildMover(_part, _placeholder, _part.NumberOfChildren);
            mover.Move();
        }
    }
}