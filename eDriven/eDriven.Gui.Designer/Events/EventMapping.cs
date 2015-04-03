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
using eDriven.Core.Events;
using UnityEngine;

namespace eDriven.Gui.Designer
{
    [Serializable]
    public class EventMapping : ICloneable
    {
        [SerializeField]
        public string EventType;

        [SerializeField] 
        public EventPhase Phase; // = EventPhase.Target | EventPhase.Bubbling; // default

        [SerializeField]
        public string ScriptName;

        [SerializeField]
        public string MethodName;

        [SerializeField]
        public bool Enabled = true;

        //[SerializeField]
        //public MethodInfo MethodInfo;

        //public EventMapping(string eventType, MethodInfo methodInfo)
        //{
        //    EventType = eventType;
        //    ScriptName = methodInfo.ReflectedType.ToString();
        //    MethodName = methodInfo.Name;
        //    //MethodInfo = methodInfo;
        //}

        //void OnEnable()
        //{
        //    hideFlags = HideFlags.HideAndDontSave;
        //}

        public override string ToString()
        {
            return string.Format(@"[{0}] -> {1}.{2}
Phase: {3}", EventType, ScriptName, MethodName, Phase);
        }

        public bool Equals(EventMapping other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.EventType, EventType) && Equals(other.Phase, Phase) && Equals(other.ScriptName, ScriptName) && Equals(other.MethodName, MethodName) && other.Enabled.Equals(Enabled);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (EventMapping)) return false;
            return Equals((EventMapping) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = (EventType != null ? EventType.GetHashCode() : 0);
                result = (result*397) ^ Phase.GetHashCode();
                result = (result*397) ^ (ScriptName != null ? ScriptName.GetHashCode() : 0);
                result = (result*397) ^ (MethodName != null ? MethodName.GetHashCode() : 0);
                result = (result*397) ^ Enabled.GetHashCode();
                return result;
            }
        }

        public object Clone()
        {
            EventMapping mapping = new EventMapping();
            mapping.EventType = EventType;
            mapping.ScriptName = ScriptName;
            mapping.MethodName = MethodName;
            mapping.Enabled = Enabled;
            mapping.Phase = Phase;
            return mapping;
        }
    }
}