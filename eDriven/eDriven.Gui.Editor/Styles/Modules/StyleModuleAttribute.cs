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

using System;
using eDriven.Gui.Editor.Rendering;
using UnityEngine;

namespace eDriven.Gui.Editor.Styles
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class StyleModuleAttribute : Attribute
    {
        /// <summary>
        /// The descriptor provider identifier<br/>
        /// It relates to the identifier serialized with style declaration and is used for lookup<br/>
        /// It is important not to change it once the style declarations are being serialized
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Style name
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Type icon
        /// </summary>
        public Texture Icon { get; private set; }

        /// <summary>
        /// A flag indicating if this module alows collecting styleable properties from multiple classes<br/>
        /// If not, the collected data in the 2nd tab of the StyleDeclaration dialog will be reset 
        /// upon the component change in the 1st tab<br/>
        /// False by default
        /// </summary>
        public bool AllowMultipleClients;

        /// <summary>
        /// A flag indicating that with this module the subject could be ommited, 
        /// so the target could be specified with using only the Class or Id
        /// </summary>
        public bool AllowSubjectOmmision;

        /// <summary>
        /// A flag indicating that changes in edit mode will be processed
        /// </summary>
        public bool ProcessEditModeChanges = true;

        /// <summary>
        /// A flag indicating that changes in play mode will be processed
        /// </summary>
        public bool ProcessPlayModeChanges = true;

        public StyleModuleAttribute(string id, string description, string typeIcon)
        {
            Id = id;
            Description = description;
            Icon = (Texture)Resources.Load(TextureCache.EditorIconsPath + typeIcon);
        }
    }
}