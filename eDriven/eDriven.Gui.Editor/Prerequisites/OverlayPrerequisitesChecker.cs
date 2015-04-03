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

using eDriven.Gui.Mappers;
using UnityEditor;

namespace eDriven.Gui.Editor.Prerequisites
{
    internal static class OverlayPrerequisitesChecker
    {
        internal static void Check()
        {
            MapperInfo info;

            //SceneInitializer.Init();

            FontMapper mapper = SceneInitializer.GetFontMapper(out info);
            if (!mapper)
            {
                EditorSettings.InspectorEnabled = false;
                EditorUtility.DisplayDialog("eDriven.Gui Info", @"Inspector overlay needs a default font for drawing, but no default font mapper found in the scene.

Inspector overlay will be disabled until the mapper is created in the scene.", "OK");

                EditorSettings.InspectorEnabled = false;
                return;
            }

            if (null == mapper.Font)
            {
                Selection.activeGameObject = mapper.gameObject;
                EditorGUIUtility.PingObject(mapper);

                EditorSettings.InspectorEnabled = false;
                EditorUtility.DisplayDialog("eDriven.Gui Info", string.Format(@"Inspector overlay needs a default font for drawing.

The default mapper has been found, but with no font attached.

Inspector overlay will be disabled until the font is attached."), "OK");
                //The mapper ""{0}"" has been found, but with no font attached. // , CreateFontMapperCommand.FontMapperId

                EditorSettings.InspectorEnabled = false;
            }
        }
    }
}
