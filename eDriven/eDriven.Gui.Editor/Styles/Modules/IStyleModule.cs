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
using eDriven.Gui.Styles;
using UnityEngine;

namespace eDriven.Gui.Editor.Styles
{
    /// <summary>
    /// Handles reflecting of styles and building collections needed for the StyleDeclaration dialog
    /// </summary>
    public interface IStyleModule
    {
        IComponentTraverser Traverser { get; }
        
        /// <summary>
        /// Gets all available class descriptors
        /// </summary>
        /// <returns></returns>
        List<TypeDescriptor> GetComponentDescriptors();

        /// <summary>
        /// Gets all the styles as string/type dictionary
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        Dictionary<string, MemberDescriptor> GetStyleDescriptors(Type type);

        /// <summary>
        /// Gets the icon for a specified component type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        Texture GetComponentIcon(Type type);

        /// <summary>
        /// Gets the icon for a specified style
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        Texture GetStyleIcon(Type type);
    }
}