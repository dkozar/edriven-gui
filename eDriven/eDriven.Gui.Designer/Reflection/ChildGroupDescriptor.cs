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

using System.Collections.Generic;
using System.Reflection;
using eDriven.Gui.Components;
using eDriven.Gui.Containers;
using eDriven.Gui.Designer.Adapters;

namespace eDriven.Gui.Designer
{
    ///<summary>
    ///</summary>
    public class ChildGroupDescriptor
    {
        /// <summary>
        /// Member info
        /// </summary>
        public MemberInfo CollectionMemberInfo;

        /// <summary>
        /// Target container member info
        /// </summary>
        public MemberInfo TargetContainerMemberInfo;

        /// <summary>
        /// The attribute
        /// </summary>
        public ChildCollectionAttribute Attribute;

        /// <summary>
        /// Gets the target container if instantiated
        /// </summary>
        /// <param name="me"></param>
        /// <returns></returns>
        public IContentChildList GetTargetContainer(IContentChildList me)
        {
            if (null != TargetContainerMemberInfo && null != me)
            {
                return (Group)Core.Reflection.CoreReflector.GetMemberValue(
                    TargetContainerMemberInfo,
                    me
                );
            }
            return me;
        }

        ///<summary>
        ///</summary>
        ///<param name="containerAdapter"></param>
        ///<returns></returns>
        public List<ComponentAdapter> GetChildAdaptersCollection(ComponentAdapter containerAdapter)
        {
            if (null == CollectionMemberInfo || null == containerAdapter)
                return null;

            List<ComponentAdapter> collection = Core.Reflection.CoreReflector.GetMemberValue(CollectionMemberInfo, containerAdapter) as List<ComponentAdapter>;
            return collection;
        }
    }
}