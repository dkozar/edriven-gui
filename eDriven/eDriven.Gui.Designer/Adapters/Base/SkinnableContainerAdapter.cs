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
using eDriven.Gui.Components;
using eDriven.Gui.Reflection;
using UnityEngine;
using Component = eDriven.Gui.Components.Component;

namespace eDriven.Gui.Designer.Adapters
{
    public abstract class SkinnableContainerAdapter : GroupAdapter
    {
#if DEBUG
// ReSharper disable UnassignedField.Global
        /// <summary>
        /// Debug mode
        /// </summary>
        public static bool DebugMode;

// ReSharper restore UnassignedField.Global
#endif

        public override Type ComponentType
        {
            get { return null; }
        }

        public override Component NewInstance()
        {
            return null;
        }

        /// <summary>
        /// Needs to be overriden in adapters
        /// </summary>
        /// <returns></returns>
        public virtual Type DefaultSkinClass
        {
            get { return null; }
        }

        [Saveable]
        public string SkinClass;

        /// <summary>
        /// Applies changes
        /// </summary>
        /// <param name="component"></param>
        public override void Apply(Component component)
        {
            base.Apply(component);

            #region Skin

            if (component is SkinnableComponent && !string.IsNullOrEmpty(SkinClass) &&
                (null == component.SkinClass || SkinClass != component.SkinClass.FullName))
            {
                if (!GlobalTypeDictionary.Instance.ContainsKey(SkinClass)) {
                    Debug.LogError(string.Format(@"Couldn't find reflected class [{0}] on adapter [{1}]. Maybe you have changed a class name or namespace? 
Click this message to select the problematic component.", SkinClass, this), this);
                    throw new Exception(string.Format(@"Couldn't find reflected class"));
                }

                var type = GlobalTypeDictionary.Instance[SkinClass];
                if (component.SkinClass != type)
                {
                    component.SkinClass = type;
                    SkinChanged(GetType(), SkinClass);
                }
            }

            #endregion
        }

        /// <summary>
        /// Remembers the last used skin
        /// </summary>
        /// <param name="skinnableAdapterType"></param>
        /// <param name="skinClass"></param>
        public static void SkinChanged(Type skinnableAdapterType, string skinClass)
        {
#if DEBUG
            if (DebugMode)
            {
                Debug.Log("Skin changed to: " + skinClass);
            }
#endif
            SkinnableComponentAdapter.LastUsedSkins[skinnableAdapterType] = skinClass;
        }

        /// <summary>
        /// Applies the last used skin or the default skin
        /// </summary>
        /// <param name="lastUsed">True to apply the last used skin fror this component type</param>
        public void ApplySkin(bool lastUsed = false)
        {
            // 1. first look for the last used skin
            if (lastUsed)
            {
                var skinClass = GetLastUsedSkin(GetType());
                if (null != skinClass)
                {
                    SkinClass = skinClass;
                }
            }

            // 2. if not found, get the default skin
            if (null == SkinClass && null != DefaultSkinClass)
            {
                SkinClass = DefaultSkinClass.FullName;
            }
        }

        /// <summary>
        /// Tryes to get the last used skin or a default skin as a fallback
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private string GetLastUsedSkin(Type type)
        {
            if (SkinnableComponentAdapter.LastUsedSkins.ContainsKey(type))
                return SkinnableComponentAdapter.LastUsedSkins[type];
            return null;
        }
    }
}
