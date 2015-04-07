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
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using eDriven.Core.Serialization;

namespace eDriven.Gui.Check
{
    /// <summary>
    /// The Application object
    /// </summary>
    [Serializable]
    [XmlRoot(ElementName = "configuration", Namespace = "http://edriven.dankokozar.com/gui/trial")]
// ReSharper disable ClassNeverInstantiated.Global
    public class Configuration
// ReSharper restore ClassNeverInstantiated.Global
    {
        /// <summary>
        /// Identifier
        /// </summary>
        [XmlAttribute("id")]
        public string Id;

        /// <summary>
        /// App blocked?
        /// </summary>
        [XmlElement("blocked")]
        [DefaultValue(false)]
        public bool Blocked;

        /// <summary>
        /// Info message
        /// </summary>
        [XmlElement("info")]
        public InfoMessage InfoMessage;

        /// <summary>
        /// Settings
        /// </summary>
        [XmlElement("settings")]
        public Settings Settings;

        /// <summary>
        /// Additional messages
        /// </summary>
        [XmlArray("messages")]
        [XmlArrayItem("message")]
        public List<InfoMessage> Messages;

        /// <summary>
        /// Append messages to an existing array?
        /// </summary>
        [XmlElement("append_messages")]
        [DefaultValue(true)]
        public bool AppendMessages = true;

        /// <summary>
        /// Logo
        /// </summary>
        [XmlElement("logo")]
        public LogoInfo LogoInfo;

        /// <summary>
        /// Properties
        /// </summary>
        [XmlArray("properties")]
        [XmlArrayItem("add")]
        public StringIndexedList<ConfigurationProperty> Properties = new StringIndexedList<ConfigurationProperty>();

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