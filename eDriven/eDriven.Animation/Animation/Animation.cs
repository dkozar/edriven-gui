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
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using eDriven.Core.Geom;
using eDriven.Core.Serialization;
using UnityEngine;

namespace eDriven.Animation.Animation
{
    public interface IKeyframeAnimation
    {
        /// <summary>
        /// Gets and returns the next frame
        /// </summary>
        /// <returns></returns>
        Frame Next();

        void Reset();
    }

    [Serializable]
    public class Animation : PathDescriptor, IDisposable, IUnique, IKeyframeAnimation//, ICloneable
    {
        /// <summary>
        /// Animation ID
        /// </summary>
        [XmlAttribute("Id")]
        [DefaultValue("")]
        public string Id { get; set; }

        [XmlAttribute("FramePrefix")]
        [DefaultValue("")]
        public string FramePrefix;

        [XmlAttribute("FrameSuffix")]
        [DefaultValue("")]
        public string FrameSuffix;

        /// <summary>
        /// Overal frame duration (overridable by each frame)
        /// </summary>
        [XmlAttribute("FrameDuration")]
        [DefaultValue(0)]
        public float FrameDuration = 0.112f;

        /// <summary>
        /// Overal offset
        /// </summary>
        [XmlAttribute("OffsetX")]
        [DefaultValue(0)]
        public float OffsetX;

        /// <summary>
        /// Overal offset
        /// </summary>
        [XmlAttribute("OffsetY")]
        [DefaultValue(0)]
        public float OffsetY;

        /// <summary>
        /// Overal offset
        /// </summary>
        [XmlIgnore]
        public Point Offset
        {
            get
            {
                return new Point(OffsetX, OffsetY);
            }
            set
            {
                OffsetX = value.X;
                OffsetY = value.Y;
            }
        }

        /// <summary>
        /// Frames
        /// </summary>
        //[XmlArray("Frames")]
        [XmlArrayItem("Frame")]
        public List<Frame> Frames = new List<Frame>();

        #region Constructor

        /// <summary>
        /// Constructor (needed for deserialization)
        /// </summary>
        public Animation()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="frames"></param>
        /// <param name="path"></param>
        /// <param name="offset"></param>
        public Animation(string path, Point offset, params Frame[] frames)
        {
            if (frames.Length == 0)
            {
                throw new Exception("The length of the supplied resources is 0");
            }

            Frames = new List<Frame>(frames);

            if (null == path)
                Path = string.Empty;

            CurrentFrame = Frames[0];

            if (null != offset)
            {
                OffsetX = offset.X;
                OffsetY = offset.Y;
            }
        }

        /// <summary>
        /// Loads frame textures from descriptors
        /// </summary>
        public void LoadTextures(string path)
        {
            foreach (Frame frame in Frames)
            {
                try
                {
                    // concatenate package, animation and frame path
                    var fullPath = string.Format("{0}{1}{2}{3}{4}", path, Path, FramePrefix, frame.Path, FrameSuffix);
                    frame.Texture = (Texture)Resources.Load(fullPath);
                    //Debug.Log("fullPath: " + fullPath);
                }
                catch (Exception ex)
                {
                    Debug.LogError("Error converting the resource to texture: " + ex);
                }
            }
        }

        #endregion

        #region Frames

        private int _frame;

        /// <summary>
        /// Returns a current frame while playing
        /// </summary>
        [XmlIgnore]
        public Frame CurrentFrame { get; private set; }

        /// <summary>
        /// Gets and returns the next frame
        /// </summary>
        /// <returns></returns>
        public Frame Next()
        {
            _frame++;
            if (_frame >= Frames.Count)
                _frame = 0;

            CurrentFrame = Frames[_frame];

            return CurrentFrame;
        }

        public void Reset()
        {
            _frame = 0;
            CurrentFrame = (Frames.Count > 0) ? Frames[_frame] : null;
        }

        #endregion

        public void Dispose()
        {
            Frames.ForEach(delegate (Frame frame)
                               {
                                   frame.Dispose();
                               });
        }

        public override string ToString()
        {
            return string.Format(@"Animation [""{0}"", {1} frames]", Id, Frames.Count);
        }

        //public object Clone()
        //{
        //    Animation anim = (Animation) MemberwiseClone();

        //    foreach (Frame frame in Frames)
        //    {
        //        Frame f = new Frame {Texture = frame.Texture};
        //        anim.Frames.Add(f);
        //    }

        //    return anim;
        //}

        #region Handling Unknowns

        private System.Xml.XmlNode[] _unknownNodes;
        private System.Xml.XmlAttribute[] _unknownAttributes;

        [XmlAnyElement]
        public System.Xml.XmlNode[] UnknownNodes
        {
            get { return _unknownNodes; }
            set { _unknownNodes = value; }
        }

        [XmlAnyAttribute]
        public System.Xml.XmlAttribute[] UnknownAttributes
        {
            get { return _unknownAttributes; }
            set { _unknownAttributes = value; }
        }

        #endregion
    }
}