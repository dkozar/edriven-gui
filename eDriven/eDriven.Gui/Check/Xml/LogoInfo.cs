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

#if RELEASE

using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace eDriven.Gui.Check
{
    /// <summary>
    /// The Application object
    /// </summary>
    [Serializable]
// ReSharper disable ClassNeverInstantiated.Global
    [XmlRoot("logo", Namespace = "http://edriven.dankokozar.com/gui/trial")]
    public class LogoInfo
// ReSharper restore ClassNeverInstantiated.Global
    {
        /// <summary>
        /// Show in editor
        /// </summary>
        [XmlElement("show_in_editor")]
        [DefaultValue(true)]
        public bool ShowInEditor = true;

        /// <summary>
        /// Show in build
        /// </summary>
        [XmlElement("show_in_build")]
        [DefaultValue(true)]
        public bool ShowInBuild = true;

        /// <summary>
        /// Logo URL
        /// </summary>
        [XmlElement("url")]
        public string Url;

        /// <summary>
        /// Cache buster?
        /// </summary>
        [XmlElement("cache_buster")]
        [DefaultValue(false)]
        public bool CacheBuster;

        /// <summary>
        /// Logo placement
        /// </summary>
        [XmlElement("placement")]
        [DefaultValue(5)]
        public int Placement = 5;

        /// <summary>
        /// Alpha
        /// </summary>
        [XmlElement("alpha")]
        [DefaultValue(0.5f)]
        public float Alpha = 0.5f;

        /// <summary>
        /// Logo placement
        /// </summary>
        [XmlElement("duration")]
        [DefaultValue(1200)]
        public float Duration = 1200;

        #region Handling Unknowns

        private System.Xml.XmlNode[] _unknownNodes;
        private System.Xml.XmlAttribute[] _unknownAttributes;

        /// <summary>
        /// Deserialization stuff
        /// </summary>
        [XmlAnyElement]
        public System.Xml.XmlNode[] UnknownNodes
        {
            get { return _unknownNodes; }
            set { _unknownNodes = value; }
        }

        /// <summary>
        /// Deserialization stuff
        /// </summary>
        [XmlAnyAttribute]
        public System.Xml.XmlAttribute[] UnknownAttributes
        {
            get { return _unknownAttributes; }
            set { _unknownAttributes = value; }
        }

        #endregion
    }
}

#endif