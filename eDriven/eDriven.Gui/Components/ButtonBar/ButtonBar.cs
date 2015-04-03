using eDriven.Core.Events;
using eDriven.Gui.Data;
using eDriven.Gui.Events;
using eDriven.Gui.Reflection;
using eDriven.Gui.Styles;

namespace eDriven.Gui.Components
{
    /// <summary>
    /// Button bar
    /// </summary>
    [Style(Name = "skinClass", Default = typeof(ButtonBarSkin))]

    public class ButtonBar : ButtonBarBase
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ButtonBar()
        {
            ItemRendererFunction = DefaultButtonBarItemRendererFunction;
        }

        /// <summary>
        /// First button
        /// </summary>
        [SkinPart(Required=false)]
        public IFactory FirstButton;

        /// <summary>
        /// Last button
        /// </summary>
        [SkinPart(Required = false)]
        public IFactory LastButton;

        /// <summary>
        /// Middle button (required)
        /// </summary>
        [SkinPart(Required = true)]
        public IFactory MiddleButton;

        public override IList DataProvider
        {
            get { return base.DataProvider; }
            set
            {
                if (null != base.DataProvider)
                    base.DataProvider.RemoveEventListener(CollectionEvent.COLLECTION_CHANGE, ResetCollectionChangeHandler);

                // not really a default handler, we just want it to run after the datagroup
                if (null != value)
                    value.AddEventListener(CollectionEvent.COLLECTION_CHANGE, ResetCollectionChangeHandler, EventPhase.TargetAndBubbling, EventPriority.DEFAULT_HANDLER);

                base.DataProvider = value;
            }
        }

        /**
         *  
         */
        private void ResetCollectionChangeHandler(Event e)
        {
            //var ce = e as CollectionEvent;
            //if (ce != null)
            //{
            //    if (ce.Kind == CollectionEventKind.ADD ||
            //        ce.Kind == CollectionEventKind.REMOVE)
            //    {
            //        //Debug.Log("ResetCollectionChangeHandler");

            //        // force reset here so first/middle/last skins
            //        // get reassigned
            //        if (null != DataGroup)
            //        {
            //            DataGroup.Layout.UseVirtualLayout = true;
            //            DataGroup.Layout.UseVirtualLayout = false;
            //        }
            //    }
            //}
        }

        protected override void SetCurrentCaretIndex(int value)
        {
            if (-1 == value)
                return;

            base.SetCurrentCaretIndex(value);
        }

        private IFactory DefaultButtonBarItemRendererFunction(object data)
        {
            var i = DataProvider.GetItemIndex(data);
            if (i == 0)
                return FirstButton ?? MiddleButton;

            var n = DataProvider.Length - 1;
            if (i == n)
                return LastButton ?? MiddleButton;

            return MiddleButton;
        }
    }
}
