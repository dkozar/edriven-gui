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

using eDriven.Gui.Designer.Util;
using eDriven.Gui.Editor.Persistence;
using eDriven.Gui.Mappers;
using UnityEditor;
using UnityEngine;

namespace eDriven.Gui.Editor.Commands
{
    internal class CreateFontMapperCommand
    {
        public string FontMapperPath;

        //public const string FontMapperId = "default_font_mapper";

        public void Run()
        {
            GameObject go = GameObjectUtil.CreateGameObjectAtPath(FontMapperPath);

            /**
             * 1. FontMapper
             * */
            FontMapper m = (FontMapper)go.AddComponent(typeof(FontMapper));
            m.Id = DipSwitches.DefaultFontMapperId;
            m.Default = true; // is the default font at the same time

            EditorUtility.DisplayDialog("Info", string.Format(@"Creating the default font mapper at ""{0}""

Please attach the default font to the font mapper.

Then create the Stage again.", FontMapperPath), "OK");

            EditorGUIUtility.PingObject(m);
            Selection.objects = new Object[] { m };

            /**
             * 3. Re-scan the hierarchy
             * */
            HierarchyViewDecorator.Instance.ReScan();
        }
    }
}