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

using UnityEngine;

namespace eDriven.Animation
{
    public class Sequence : Composite
    {
#if DEBUG
        /// <summary>
        /// Debug on
        /// </summary>
        public new static bool DebugMode;
#endif

        private IAsyncAction _currentTween;

        public Sequence()
        {
        }

        public Sequence(params IAsyncAction[] tweens)
            : base(tweens)
        {
        }

        private int _currentIndex;

        public override int Count
        {
            get
            {
                return base.Count;
            }
            set
            {
                if (value == base.Count)
                    return;

                //_currentIndex++;

                // IMPORTANT: This is the last step!!
                base.Count = value;
            }
        }

        public override void Reset()
        {
            base.Reset();

            _currentIndex = 0;
        }

        public override string ToString()
        {
            return string.Format("Sequence {0}{1}[Delay: {2}][Target: {3}]", string.IsNullOrEmpty(Name) ? string.Empty : string.Format(@"""{0}"" ", Name), base.ToString(), Delay, Target ?? "-");
        }

        protected override void Launch()
        {
            _currentIndex = 0;

            if (0 == Children.Count)
            {
#if DEBUG
                if (DebugMode)
                    Debug.Log("Sequence: No tweens defined");
#endif
                return;
            }

#if DEBUG
            if (DebugMode)
                Debug.Log("Sequence Launch");
#endif

            //ITween first = null;
            _currentTween = Children[_currentIndex];

            TweenBase tb = _currentTween as TweenBase;
            if (null == tb)
                return;

            // add my delay to the first tween
            //tb.Delay += Delay;

            //tb.Play();
            if (null != Target && null == tb.Target)
                tb.Play(Target);
            else
                tb.Play();
        }

        private bool _ticked;

        public override bool Tick()
        {
            base.Tick();

            if (!IsLaunched)
                return false;

            _ticked = true;
            
            while (_ticked && _currentIndex < Children.Count)
            {
                //Debug.Log("_currentIndex: " + _currentIndex);

                _currentTween = Children[_currentIndex];
                //Debug.Log("_currentTween: " + _currentTween);

                //if (null != _currentTween)
                _ticked = _currentTween.Tick();

                if (_ticked)
                {
                    //Debug.Log("===== _ticked: " + _ticked);
                    Count--;
                    _currentIndex++;
                }
            }

            return Count == 0;
        }

        public override object Clone()
        {
            Sequence sequence = new Sequence {Name = Name}; //(Sequence) MemberwiseClone();
            sequence.Add(CloneTweens().ToArray());
            return sequence;
        }
    }
}