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
using eDriven.Gui.Data;
using eDriven.Gui.Reflection;
using UnityEngine;
using Component = eDriven.Gui.Components.Component;

namespace eDriven.Gui.Designer.Data
{
    [AddComponentMenu("eDriven/Gui/Data/BindableStringDataProvider (* in development)")]
    [Obfuscation(Exclude = true)]
    public class BindableStringDataProvider : MonoBehaviour
    {
        // ReSharper disable UnusedMember.Local
        [Obfuscation(Exclude = true)]
        void ComponentInstantiated(Component descriptor)
        // ReSharper restore UnusedMember.Local
        {
            DataGroup dataGroup = descriptor as DataGroup;
            if (null != dataGroup)
            {
                //Debug.Log("Setting dataprovider on combo: " + combo);
            }
            else
            {
                Debug.Log("No dataGroup found");
            }

            if (null != dataGroup)
            {
                List<object> list = new List<object>();

                foreach (string s in Data)
                {
                    list.Add(new ListItem(s, s));
                }

                dataGroup.DataProvider = new ArrayList(list);
            }
        }

        [Obfuscation(Exclude = true)]
// ReSharper disable UnusedMember.Local
        void OnDisable()
// ReSharper restore UnusedMember.Local
        {
    
        }

        [Saveable]
        [SerializeField]
        public string[] Data = { };
    }
}