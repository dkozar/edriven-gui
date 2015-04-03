#region Copyright

/*
 
Copyright (c) Danko Kozar 2012. All rights reserved.
 
*/

#endregion Copyright

using System;
using eDriven.Animation;
using eDriven.Gui.Styles;

namespace eDriven.Gui.Components
{
    #region Style metadata

    /// <summary>
    /// Modal overlay which is covers the stage when using a modal dialog
    /// </summary>
    [Style(Name = "skinClass", Default = typeof(ModalOverlaySkin))]
    
    #endregion

    public sealed class ModalOverlay : SkinnableComponent
    {
        #region Static

        public static ITweenFactory AddedEffect;
        public static ITweenFactory RemovedEffect;
        
        /// <summary>
        /// Optional default skin class // TODO: Do it with styling (implement skinClass as string in a picker)
        /// </summary>
        public static Type DefaultSkin;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public ModalOverlay()
        {
            MouseEnabled = true;
            FocusEnabled = false;
            StopMouseWheelPropagation = true;

            //SetStyle("boxComponentStyle", ModalOverlayStyle.Instance);

            if (null != DefaultSkin)
                SetStyle("skinClass", DefaultSkin);

            if (null != AddedEffect)
                SetStyle("addedEffect", AddedEffect); // addedEffect!

            if (null != RemovedEffect)
                SetStyle("removedEffect", RemovedEffect);

            /*ResizeWithStyleBackground = false;
            ResizeWithContent = false;*/
        }
    }
}