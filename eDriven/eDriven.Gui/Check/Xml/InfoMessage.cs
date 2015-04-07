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

#if TRIAL

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
    [XmlRoot("info", Namespace = "http://edriven.dankokozar.com/gui/trial")]
    public class InfoMessage
// ReSharper restore ClassNeverInstantiated.Global
    {
        /// <summary>
        /// Message ID
        /// </summary>
        [XmlElement("id")]
        public int Id;

        /// <summary>
        /// Show in editor
        /// </summary>
        [XmlElement("title")]
        public string Title;

        /// <summary>
        /// Show in build
        /// </summary>
        [XmlElement("message")]
        public string Message;

        /// <summary>
        /// Logo URL
        /// </summary>
        [XmlElement("url")]
        public string Url;

        /// <summary>
        /// Cache buster?
        /// </summary>
        [XmlElement("cache_buster")]
        public bool CacheBuster = true;

        [XmlElement("mode")]
        [DefaultValue(MessageMode.Both)]
        public MessageMode Mode = MessageMode.Both;

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

        public override string ToString()
        {
            return string.Format(@"Id: {0}
Title: {1}
Message: {2}
Url: {3}
CacheBuster: {4}
Mode: {5}", Id, Title, Message, Url, CacheBuster, Mode);
        }
    }
}

#endif