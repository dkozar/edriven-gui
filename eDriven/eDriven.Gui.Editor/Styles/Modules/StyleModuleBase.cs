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
using System.Collections.Generic;
using eDriven.Core.Reflection;
using eDriven.Gui.Styles;
using UnityEngine;

namespace eDriven.Gui.Editor.Styles
{
    internal abstract class StyleModuleBase : IStyleModule
    {
#if DEBUG
// ReSharper disable UnassignedField.Global
        /// <summary>
        /// Debug mode
        /// </summary>
        public static bool DebugMode;

// ReSharper restore UnassignedField.Global
#endif

        public bool Enabled = true;

        /// <summary>
        /// The module identifier<br/>
        /// It relates to the identifier serialized with style declaration and is used for lookup<br/>
        /// It is important not to change it once the style declarations are being serialized
        /// </summary>
        public string Id;

        /// <summary>
        /// Style name
        /// </summary>
        public string Description;

        /// <summary>
        /// Type icon
        /// </summary>
        public Texture Icon;

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
        public bool ProcessEditModeChanges;

        /// <summary>
        /// A flag indicating that changes in play mode will be processed
        /// </summary>
        public bool ProcessPlayModeChanges;

        public abstract IComponentTraverser Traverser { get; }
        public abstract List<TypeDescriptor> GetComponentDescriptors();
        public abstract Dictionary<string, MemberDescriptor> GetStyleDescriptors(Type type);
        public abstract Texture GetComponentIcon(Type type);
        public abstract Texture GetStyleIcon(Type type);

        public void ReflectAttribute()
        {
            var attributes = CoreReflector.GetClassAttributes<StyleModuleAttribute>(GetType());
            if (attributes.Count == 0)
                throw new Exception("Style module must be decorated with StyleModuleAttribute");

            StyleModuleAttribute attribute = attributes[0];
            Id = attribute.Id;
            Description = attribute.Description;
            Icon = attribute.Icon;
            AllowMultipleClients = attribute.AllowMultipleClients;
            AllowSubjectOmmision = attribute.AllowSubjectOmmision;
            ProcessEditModeChanges = attribute.ProcessEditModeChanges;
            ProcessPlayModeChanges = attribute.ProcessPlayModeChanges;
        }

        public void UpdateStyles(Selector selector, DictionaryDelta delta)
        {
#if DEBUG
            if (DebugMode)
            {
                Debug.Log(string.Format("##### UpdateStyles [{0}] #####", selector));
            }
#endif
            // ReSharper disable once UnusedVariable
            /* Important */
            var connector = StylingOverlayConnector.Instance;

            // process styles - live
            if (EditorSettings.LiveStyling)
                Gui.ProcessStyles();

            if (!Application.isPlaying && ProcessEditModeChanges ||
                Application.isPlaying && ProcessPlayModeChanges)
            {
                Traverser.UpdateStyles(selector, delta);
            }
        }

        /*public void InitStyles(CSSSelector selector, StyleTable styles)
        {
#if DEBUG
            if (DebugMode)
            {
                Debug.Log(string.Format("##### InitStyles [{0}] #####", selector));
            }
#endif
            /**
             * Important: this has to be done here, and not before
             * This is the first modification of the stylesheet, so we need to initialize the styling overlay stage
             * #1#

            // ReSharper disable once UnusedVariable
            var connector = StylingOverlayConnector.Instance;

            // process styles - live
            if (EditorSettings.LiveStyling)
                Gui.ProcessStyles();

            if (!Application.isPlaying && ProcessEditModeChanges ||
                Application.isPlaying && ProcessPlayModeChanges)
            {
                Traverser.InitStyles(selector, styles);
            }
        }*/
    }
}