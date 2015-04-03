using System;
using eDriven.Core.Events;
using eDriven.Gui.Containers;
using eDriven.Core.Geom;
using eDriven.Gui.Util;
using Event=eDriven.Core.Events.Event;
using EventHandler=eDriven.Core.Events.EventHandler;

namespace eDriven.Gui.Components
{
    /// <summary>
    /// Display object that could be added to display list
    /// </summary>
    public abstract class DisplayListMember : DisplayObject, IVisualElement, IGlobalLocal
    {
        private string _id;
        /// <summary>
        /// Component identifier
        /// </summary>
        public virtual string Id
        {
            get { return _id; }
            set
            {
                _id = value;
                //_name = _id; // commented out 20130331
            }
        }

        /// <summary>
        /// A parent container<br/>
        /// Set by framework<br/>
        /// Has to be set for every component on stage except Stage
        ///</summary>
        public DisplayObjectContainer Parent { get; internal set; }

        /// <summary>
        /// The owner of this component
        /// </summary>
        public DisplayObject Owner { get; set; }

        private string _name;
        protected int _depth = -1;

        /// <summary>
        /// Name
        /// </summary>
        internal string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Stage of which the component is ancestor<br/>
        /// Set by framework in stage propagation<br/>
        ///</summary>
        public virtual Stage Stage { get; internal set; }

        protected DisplayListMember()
        {
            _name = /*_id ?? */NamingUtil.CreateUniqueName(this); // _id commented out 20130331, because the GuiInspector was showing hierarchies such as "...button1"
        }

        internal void PropagateStage()
        {
            if (Parent is Stage)
            { // if Owner is Stage...
                Stage = (Stage)Parent;
            }
            else // else look for parents Stage (Owner should already be InternalStructureBuilt)
            {
                if (Parent.Stage != null)
                {
                    Stage = Parent.Stage;
                }
            }
        }

        #region IGlobalLocal Members

        public virtual Point LocalToGlobal(Point p)
        {
            return p.Add(Transform.GlobalPosition);
        }

        public virtual Point GlobalToLocal(Point p)
        {
            return p.Subtract(Transform.GlobalPosition);
        }

        public virtual Rectangle LocalToGlobal(Rectangle r)
        {
            Rectangle r2 = (Rectangle)r.Clone();
            r2.Position = LocalToGlobal(r2.Position);
            return r2;
        }

        public virtual Rectangle GlobalToLocal(Rectangle r)
        {
            Rectangle r2 = (Rectangle)r.Clone();
            r2.Position = GlobalToLocal(r2.Position);
            return r2;
        }

        #endregion

        #region Event propagation

        /// <summary>
        /// Override of processing events
        /// </summary>
        /// <param name="e"></param>
        protected override void ProcessEvent(Event e)
        {
            if (e.Bubbles && e.Target is DisplayListMember)
                GuiEventProcessor.BubbleEvent(e);
            else
                base.ProcessEvent(e);
        }

        /// <summary>
        /// IMPORTANT
        /// Here we plug in the event traverser for GUI components
        /// </summary>
        /// <param name="eventType"></param>
        /// <returns></returns>
        public override bool HasBubblingEventListener(string eventType)
        {
            return GuiEventProcessor.HasBubblingEventListener(eventType, this);
        }

        /// <summary>
        /// AddEventListener Overload
        /// Assumes that useCapturePhase is false
        /// </summary>
        /// <param name="eventType">Event type</param>
        /// <param name="handler">Event handler (function)</param>
        
        public override void AddEventListener(string eventType, EventHandler handler)
        {
            // listening for Target and Bubbling phases by default
            base.AddEventListener(eventType, handler, EventPhase.Target | EventPhase.Bubbling);
        }

        public override void RemoveEventListener(string eventType, EventHandler handler)
        {
            // listening for Target and Bubbling phases by default
            base.RemoveEventListener(eventType, handler, EventPhase.Target | EventPhase.Bubbling);
        }

        #endregion

        public override string ToString()
        {
            //return NamingUtil.DisplayListMemberToString(this); // commented out 20130313
            string result = base.ToString();

            if (!string.IsNullOrEmpty(Id))
                result += string.Format(@" [Id=""{0}""]", Id);

            return result;
        }

        DisplayObjectContainer IVisualElement.Parent
        {
            get { return Parent; }
            set { Parent = value; }
        }

        /// <summary>
        /// Crutial for fixing ScrollView and TextField click-throughs
        /// </summary>
        public int Depth
        {
            get { return _depth; }
            set
            {
                _depth = value;
                //Debug.Log("Depth set to: " + _depth + " [" + this + "]");
                if (null != Parent)
                    Parent.InvalidateDrawingList();
            }
        }

        /// <summary>
        /// True for including in layout
        /// </summary>
        public virtual bool IncludeInLayout { get; set;}

        /// <summary>
        /// Bring to front
        /// </summary>
        public virtual void BringToFront()
        {
            if (null != Parent)
            {
                Parent.BringChildToFront(this);
            }
        }

        /// <summary>
        /// Send to back
        /// </summary>
        public virtual void SendToBack()
        {
            if (null != Parent)
            {
                Parent.PushChildToBack(this);
            }
        }

        #region Effects

        /// <summary>
        /// Note: this has to go here because the container should be able to run children effects
        /// And we don't want to use too much castinh in the code
        /// </summary>
        /// <param name="effectStyleName"></param>
        /// <returns></returns>
        public virtual bool TriggerEffect(string effectStyleName)
        {
            // override in subclass
            return false;
        }

        /// <summary>
        /// Note: this has to go here because the container should be able to run children effects
        /// And we don't want to use too much castinh in the code
        /// </summary>
        /// <param name="effectStyleName"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public virtual bool TriggerEffect(string effectStyleName, object target)
        {
            // override in subclass
            return false;
        }

        #endregion
    }

    public class DisplayListMemberException : Exception
    {
#pragma warning disable 1591
        public static string UndefinedParent = "Undefined Owner";
        public static string UndefinedParentStage = "Undefined Stage on Owner";
        public static string UndefinedSkin = "Undefined Skin ";
        
#pragma warning restore 1591

#pragma warning disable 1591
        public DisplayListMemberException()
#pragma warning restore 1591
        {

        }

        /// <summary>
        /// Constructor
        ///</summary>
        ///<param name="message"></param>
        public DisplayListMemberException(string message)
            : base(message)
        {

        }
    }
}