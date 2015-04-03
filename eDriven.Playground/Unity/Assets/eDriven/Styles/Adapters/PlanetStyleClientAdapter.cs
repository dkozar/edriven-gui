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
using eDriven.Gui.Styles;

namespace Assets.eDriven.Styles.Adapters
{
    /// <summary>
    /// An example of a style client adapter<br/>
    /// The class should be written per component and extend StyleClientAdapterBase<br/>
    /// If style client adapter for the particular component not defined - it will fall back to the default one<br/>
    /// Note: The default adapter returns Component.gameObject.name as ID and an empty string as StyleName
    /// </summary>
    [StyleClientAdapter(typeof(Planet))]
    public class PlanetStyleClientAdapter : StyleClientAdapterBase
    {
        override public string Id
        {
            get
            {
                return Component.gameObject.name; // let's treat gameObject name as ID
            }
            set { throw new Exception("ID is read-only"); }
        }

        override public object StyleName
        {
            get
            {
                return "foo"; // you're in charge of defining what serves as a style name for your component
            }
            set { throw new Exception("Style name is read-only"); }
        }
    }
}
