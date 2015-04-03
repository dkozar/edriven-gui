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

//using System;
//using System.Reflection;
//using eDriven.Gui.Designer.Data;
//using UnityEngine;

//namespace eDriven.Gui.Data.Binding
//{
//    public class BindingComponent : MonoBehaviour, IDisposable
//    {
//        public string Member;
//        public IBindable Source;
//        public string SourceProperty;

//        private IBindable _source;

//        private MemberInfo _memberInfo;
//        private ComponentAdapter _adapter;
//        private Components.Component _component;

//        private static readonly object[] Idx = new object[] {};

//        void Start()
//        {
//            Debug.Log("Start");
//            //if (string.IsNullOrEmpty(SourceProperty))
//            //{
//            //    Debug.LogError("SourceProperty is empty");
//            //    return;
//            //}

//            //if ((Source == null))
//            //{
//            //    Debug.LogError("Source is null");
//            //    return;
//            //}

//            //_source = Source;

//            if (string.IsNullOrEmpty(Member))
//            {
//                Debug.LogError("No Member defined");
//                return;
//            }

//            //if (null == Source)
//            //{
//            //    Debug.LogError("No Sources defined");
//            //    return;
//            //}

//            _adapter = GetComponent(typeof (ComponentAdapter)) as ComponentAdapter;
//            if (null == _adapter)
//            {
//                Debug.LogError("No ComponentAdapter found attached to this game object");
//                return;
//            }

//            //_component = _adapter.Component;
//            //if (null == _component)
//            //{
//            //    Debug.LogError("No Component found on descriptor");
//            //    return;
//            //}

//            return;

//            MemberInfo[] infos = _adapter.GetType().GetMember(Member);
//            if (infos.Length > 0)
//            {
//                _memberInfo = infos[0];
//                _source.Signal.Connect(OnChange);
//            }
//        }

//        private void OnChange(object[] parameters)
//        {
//            Debug.Log("OnChange");
//            if (_memberInfo is PropertyInfo)
//                ((PropertyInfo)_memberInfo).SetValue(_adapter, _source.Get(), Idx);
//            else if (_memberInfo is FieldInfo)
//                ((FieldInfo)_memberInfo).SetValue(_adapter, _source.Get());
//        }

//        public void Dispose()
//        {
//            _source.Signal.Disconnect(OnChange);
//        }
//    }
//}
