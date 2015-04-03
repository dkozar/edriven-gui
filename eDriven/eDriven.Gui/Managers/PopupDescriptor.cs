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

using eDriven.Gui.Components;
using eDriven.Gui.Containers;

namespace eDriven.Gui.Managers
{
    internal class PopupDescriptor
    {
        /// <summary>
        /// The owner of popup component
        /// </summary>
        internal DisplayObject Owner;
        
        /// <summary>
        /// The actual popup
        /// </summary>
        internal DisplayListMember Popup;
        
        /// <summary>
        /// The overlay if modal popup
        /// </summary>
        internal DisplayListMember Overlay;

        /// <summary>
        /// If modal popup, both overlay and popup are added to a container, which is added to stage
        /// This way, the manipulation of popups is more robust
        /// If popup not modal, the PopupRoor has the value of popup
        /// </summary>
        internal DisplayListMember PopupRoot;

        /// <summary>
        /// The stage this popup is added to
        /// </summary>
        internal Stage Stage;

        /// <summary>
        /// Is popup modal
        /// </summary>
        internal bool Modal;

        /// <summary>
        /// Is popup centered
        /// </summary>
        internal bool Centered;

        /// <summary>
        /// Should popup keep center position after the screen resize
        /// </summary>
        internal bool KeepCenter;

        /// <summary>
        /// Should popup be removed on mouse down outside
        /// </summary>
        internal bool RemoveOnMouseDownOutside;

        /// <summary>
        /// Should popup be removed on mouse wheel outside
        /// </summary>
        internal bool RemoveOnMouseWheelOutside;

        /// <summary>
        /// Should popup be removed on screen resize
        /// </summary>
        internal bool RemoveOnScreenResize;

        /// <summary>
        /// Should popup be focused when shown
        /// </summary>
        internal bool AutoFocus;

        /// <summary>
        /// Should previously open popup be focused when hidden
        /// </summary>
        internal bool FocusPreviousOnHide;

        public PopupDescriptor(DisplayObject owner)
        {
            Owner = owner;
        }

        public PopupDescriptor(DisplayObject owner, DisplayListMember overlay)
        {
            Owner = owner;
            Overlay = overlay;
        }

        public PopupDescriptor(DisplayObject owner, DisplayListMember overlay, DisplayListMember popupRoot)
        {
            Owner = owner;
            Overlay = overlay;
            PopupRoot = popupRoot;
        }
    }
}