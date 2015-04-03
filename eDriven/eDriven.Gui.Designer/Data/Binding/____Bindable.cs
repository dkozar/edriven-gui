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

//using eDriven.Core.Signals;
//using eDriven.Gui.Designer.Data;

//namespace eDriven.Gui.Data.Binding
//{
//    public class Bindable : IBindable
//    {
//        private object _source;
//        private Signal _signal;

//        private Bindable()
//        {
//        }

//        //public Bindable(object source)
//        //{
//        //    _source = source;
//        //}

//        public Bindable(string sourcePath)
//        {
//            string[] parts = sourcePath.Split('.');
//            string variable = parts[parts.Length - 1];

//        }

//        //public Bindable(ref int source)
//        //{
//        //    _source = source;
//        //}

//        //public Bindable(ref float source)
//        //{
//        //    _source = source;
//        //}

//        //public Bindable(ref string source)
//        //{
//        //    _source = source;
//        //}

//        //public static Bindable Create(ref int source)
//        //{
//        //    return new Bindable {_source = source};
//        //}

//        public object Get()
//        {
//            return _source;
//        }

//        public void Set(object value)
//        {
//            if (value == _source)
//                return;

//            _source = value;

//            if (null != _signal)
//                _signal.Emit();
//        }

//        public Signal Signal
//        {
//            get { 
//                if (null == _signal)
//                    _signal = new Signal();

//                return _signal;
//            }
//        }
//    }
//}