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
using eDriven.Core.Data.Collections;
using eDriven.Gui.Components;
using eDriven.Gui.Data;
using eDriven.Gui.Designer.Adapters;
using eDriven.Gui.Reflection;
using UnityEngine;
using ArrayList = eDriven.Gui.Data.ArrayList;
using Component = eDriven.Gui.Components.Component;

namespace eDriven.Gui.Designer.Data
{
    [AddComponentMenu("eDriven/Gui/Data/StringDataProvider")]
    [Obfuscation(Exclude = true)]
    public class StringDataProvider : ComponentAdapterBase
    {
        /// <summary>
        /// The list of strings
        /// </summary>
        [Saveable]
        [SerializeField]
        public string[] Data = { };

        // ReSharper disable UnusedMember.Local
        void Update()
            // ReSharper restore UnusedMember.Local
        {

        }

// ReSharper disable UnusedMember.Local
        [Obfuscation(Exclude = true)]
        void InitializeComponent(Component component)
// ReSharper restore UnusedMember.Local
        {
            //_component = component;

            if (!enabled)
            {
                //Debug.Log("StringDataProvider not enabled");
                return;
            }

            Apply(component);
        }

        public override void Apply(Component component)
        {
            //Debug.Log("=== Apply ===", transform);

            DataGroup dataProviderClient = component as DataGroup;
            SkinnableDataContainer skinnableDataContainer = component as SkinnableDataContainer;
            if (null == dataProviderClient && null == skinnableDataContainer)
            {
                Debug.LogWarning("GUI component is not a DataGroup nor SkinnableDataContainer");
                return;
            }

            List<object> list = new List<object>();
            foreach (string s in Data)
            {
                list.Add(new ListItem(s, s));
            }

            if (null != dataProviderClient)
                dataProviderClient.DataProvider = new ArrayList(list);
            else
                skinnableDataContainer.DataProvider = new ArrayList(list);
        }
    }
}