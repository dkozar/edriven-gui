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

using UnityEditor;

namespace eDriven.Gui.Editor
{
    [InitializeOnLoad]
    internal class Startup
    {
        private static readonly int CheckCount; // sometimes it is being run 2 times when the UnityEditor is turned on

        static Startup()
        {
            if (CheckCount<5 && EditorApplication.timeSinceStartup < 10) // less than 10 seconds
            {
                CheckCount ++;

                //Debug.Log("Less than 10 seconds since startup: " + EditorApplication.timeSinceStartup);
                if (EditorSettings.CheckForUpdates)
                {
                    UpdateCheck.Instance.Run(true, true, true); // 1) check if update needed based on settings, 2) skip dialog if no new updates
                }
            }

            /*//if (EditorSettings.InspectorEnabled && Application.isEditor && Application.isPlaying/* && null == DesignerOverlay.Instance#1#)
            if (EditorSettings.InspectorEnabled && Application.isEditor &&
                (!EditorApplication.isPlaying && EditorApplication.isPlayingOrWillChangePlaymode))
            {
                //if (FontMapper.IsMapping(DipSwitches.DefaultFontMapperId))
                if (null != FontMapper.GetDefault())
                {
                    // ReSharper disable once UnusedVariable
                    DesignerOverlay overlay = (DesignerOverlay)Framework.GetComponent<DesignerOverlay>(true); // add if non-existing
                    //overlay.Font = FontMapper.GetWithFallback(DipSwitches.DefaultFontMapperId).Font; // set font
                }
            }*/
        }
    }
}