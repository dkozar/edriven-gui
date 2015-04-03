using eDriven.Core.Events;
using eDriven.Core.Geom;
using eDriven.Core.Managers;
using eDriven.Gui.Components;
using eDriven.Gui.Cursor;
using eDriven.Gui.Plugins;
using eDriven.Gui.Reflection;
using eDriven.Gui.Styles;
using Event=eDriven.Core.Events.Event;

namespace eDriven.Gui.Containers
{
    [Style(Name = "skinClass", Default = typeof(WindowSkin))]
    
    public class Window : Panel
    {
        #region Skin parts

        private readonly DoubleGroup _moveAreaDoubleGroup = new DoubleGroup();

        ///<summary>Header group
        ///</summary>
        [SkinPart(Required = false)]
        public Group MoveArea
        {
            get { return _moveAreaDoubleGroup.Part; }
            internal set { _moveAreaDoubleGroup.Part = value; }
        }

        #endregion

        #region Lifecycle methods

        protected override void PartAdded(string partName, object instance)
        {
            base.PartAdded(partName, instance);

            if (instance == MoveArea)
            {
                //Debug.Log("MoveArea added: " + TitleLabel);
                MoveArea.AddEventListener(MouseEvent.MOUSE_DOWN, MoveAreaMouseDownHandler);
            }
        }

        protected override void PartRemoved(string partName, object instance)
        {
            base.PartRemoved(partName, instance);
            
            if (instance == MoveArea)
            {
                //Debug.Log("MoveArea added: " + TitleLabel);
                MoveArea.RemoveEventListener(MouseEvent.MOUSE_DOWN, MoveAreaMouseDownHandler);
            }
        }

        private Point _offset;

        private int _cursorId;

        private void MoveAreaMouseDownHandler(Event e)
        {
            //Debug.Log("Mouse down: " + e.Target);

            //if (e.Target != MoveArea)
            //    return;

            //Debug.Log("MoveAreaMouseDownHandler: " + e.Target);
            // Only allow dragging of pop-upped windows
            if (Enabled && IsPopUp)
            {
                e.CancelAndStopPropagation();
                // Calculate the mouse's offset in the window
                //offsetX = event.stageX - x;
                //offsetY = event.stageY - y;

                BringToFront();

                MouseEvent me = (MouseEvent) e;
                _offset = me.GlobalPosition.Subtract(Transform.Position);

                var sm = SystemEventDispatcher.Instance;
                sm.AddEventListener(MouseEvent.MOUSE_MOVE, MoveAreaMouseMoveHandler, EventPhase.CaptureAndTarget);
                sm.AddEventListener(MouseEvent.MOUSE_UP, MoveAreaMouseUpHandler, EventPhase.CaptureAndTarget);
                _cursorId = CursorManager.Instance.SetCursor(CursorType.Move);
            }
        }

        private void MoveAreaMouseMoveHandler(Event e)
        {
            MouseEvent me = (MouseEvent)e;
            var newPositon = me.GlobalPosition.Subtract(_offset);
            Move(newPositon.X, newPositon.Y);
            //Transform.Apply();
        }

        private void MoveAreaMouseUpHandler(Event e)
        {
            var sm = SystemEventDispatcher.Instance;
            sm.RemoveEventListener(MouseEvent.MOUSE_MOVE, MoveAreaMouseMoveHandler, EventPhase.CaptureAndTarget);
            sm.RemoveEventListener(MouseEvent.MOUSE_UP, MoveAreaMouseUpHandler, EventPhase.CaptureAndTarget);
            CursorManager.Instance.RemoveCursor(_cursorId);
        }

        #endregion

        private Resizable _resizablePlugin;
        
        private bool _resizableChanged;
        private bool _resizable;
        /// <summary>
        /// True if the window is resizable
        /// </summary>
        public bool Resizable
        {
            get
            {
                return _resizable;
            }
            set
            {
                if (value == _resizable)
                    return;

                _resizable = value;
                _resizableChanged = true;
                InvalidateProperties();
            }
        }

        private bool _draggableChanged;
        private bool _draggable;
        public bool Draggable
        {
            get
            {
                return _draggable;
            }
            set
            {
                if (value == _draggable)
                    return;

                _draggable = value;
                _draggableChanged = true;
                InvalidateProperties();
            }
        }

        protected override void CommitProperties()
        {
            base.CommitProperties();

            if (_resizableChanged)
            {
                _resizableChanged = false;
                if (_resizable)
                {
                    if (null == _resizablePlugin)
                    {
                        _resizablePlugin = new Resizable { ShowOverlay = false };
                        Plugins.Add(_resizablePlugin);
                        if (Initialized)
                            _resizablePlugin.Initialize(this);
                    }
                    _resizablePlugin.Enabled = true;
                }
                else
                {
                    if (null != _resizablePlugin)
                        _resizablePlugin.Enabled = _draggable;
                }

                //if (null == _resizeHandle) // instantiate
                //{
                //    _resizeHandle = new Button { Width = 10, Height = 10, IncludeInLayout = false};
                //    _resizeHandle.X = Width - _resizeHandle.Width;
                //    _resizeHandle.Y = Height - _resizeHandle.Height;
                //    AddChild(_resizeHandle);
                //    _resizeHandle.ValidateNow();
                //}

                //_resizeHandle.Visible = _resizable;
            }
        }
    }
}