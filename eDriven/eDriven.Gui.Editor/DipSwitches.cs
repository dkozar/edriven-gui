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

namespace eDriven.Gui.Editor
{
    internal static class DipSwitches
    {
        /// <summary>
        /// The font mapper used for Pplay mode overlay
        /// </summary>
        public const string DefaultFontMapperId = "default_font_mapper";

        /// <summary>
        /// The duration of the frame in the about box
        /// </summary>
        public const float AboutDuration = 5f;

        /// <summary>
        /// Minimal width of the eDriven.Gui Designer window 
        /// </summary>
        public const float MinWindowWidth = 470;

        /// <summary>
        /// Minimal height of the eDriven.Gui Designer window 
        /// </summary>
        public const float MinWindowHeight = 300;

        /// <summary>
        /// eDriven.Gui Designer window name
        /// </summary>
        public const string MainWindowName = "eDriven.Gui";

        /// <summary>
        /// eDriven.Gui View window name
        /// </summary>
        public const string RenderingDebugWindowName = "Rendering";
        
        /// <summary>
        /// Utility window?
        /// </summary>
        public const bool IsUtilityWindow = false;

        /// <summary>
        /// Control buttin min width (toolbox)
        /// </summary>
        public const float ControlButtonMinWidth = 190;

        /// <summary>
        /// When non-adapted component clicked in th egame view (Alert, pop-uped Dialog etc.)<br/>
        /// We might choose to deselect the current selection, or do nothing
        /// </summary>
        public const bool DeselectHierarchyOnNonAdaptedComponentSelection = false;
    }
}
