using eDriven.Core.Events;
using eDriven.Gui.Components;

namespace eDriven.Gui.Data
{
    public class RendererExistenceEvent : Event
    {
        public const string RENDERER_ADD = "rendererAdd";
        public const string RENDERER_REMOVE = "rendererRemove";

        #region Constructor

        public RendererExistenceEvent(string type) : base(type)
        {
        }

        public RendererExistenceEvent(string type, object target) : base(type, target)
        {
        }

        public RendererExistenceEvent(string type, bool bubbles) : base(type, bubbles)
        {
        }

        public RendererExistenceEvent(string type, bool bubbles, bool cancelable) : base(type, bubbles, cancelable)
        {
        }

        public RendererExistenceEvent(string type, object target, bool bubbles, bool cancelable) : base(type, target, bubbles, cancelable)
        {
        }

        #endregion

        public object Data;
        
        //----------------------------------
        //  index
        //----------------------------------

        public int Index;

        //----------------------------------
        //  renderer
        //----------------------------------

        public IVisualElement Renderer;

        //--------------------------------------------------------------------------
        //
        //  Overridden methods: Event
        //
        //--------------------------------------------------------------------------

        /**
         *  
         */
        override public object Clone()
        {
            return new RendererExistenceEvent(Type, Bubbles, Cancelable)
                        {
                            Renderer = Renderer,
                            Index = Index,
                            Data = Data
                        };
        }
    }
}