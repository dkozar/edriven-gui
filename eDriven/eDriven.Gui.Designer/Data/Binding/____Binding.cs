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

//namespace eDriven.Gui.Data.Binding
//{
//    public class Binding : IDisposable
//    {
//        private readonly Bindable _source;
//        private readonly object _destination;
//        private readonly MemberInfo _memberInfo;

//        private static readonly object[] Idx = new object[] {};

//        public Binding(Bindable source, object destination, string member)
//        {
//            _source = source;
//            _destination = destination;

//            MemberInfo[] infos = _destination.GetType().GetMember(member);
//            if (infos.Length > 0)
//            {
//                _memberInfo = infos[0];
//                _source.Signal.Connect(OnChange);
//            }
//        }

//        private void OnChange(object[] parameters)
//        {
//            if (_memberInfo is PropertyInfo)
//                ((PropertyInfo)_memberInfo).SetValue(_destination, _source.Get(), Idx);
//            else if (_memberInfo is FieldInfo)
//                ((FieldInfo)_memberInfo).SetValue(_destination, _source.Get());
//        }

//        public void Dispose()
//        {
//            _source.Signal.Disconnect(OnChange);
//        }
//    }
//}
