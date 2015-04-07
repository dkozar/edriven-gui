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
using eDriven.Core.Managers;
using UnityEngine;

#if TRIAL
using eDriven.Core;
using eDriven.Animation.Check;
#endif

namespace eDriven.Animation
{
    public class TweenRegistry
    {
#if DEBUG
        public static bool DebugMode; // = true;
#endif
        public static bool LogConnect;
        public static bool LogDisconnect;

        #region Singleton

        private static TweenRegistry _instance;

        private TweenRegistry()
        {
            // Constructor is protected!
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static TweenRegistry Instance
        {
            get
            {
                if (_instance == null)
                {
                    //#if DEBUG
                    //                    if (DebugMode)
                    //                        Debug.Log(string.Format("Getting TweenRegistry instance"));
                    //#endif
                    _instance = new TweenRegistry();
                    //_instance.Initialize();
                }

                return _instance;
            }
        }

        #endregion

        private readonly List<IAnimation> _tweens = new List<IAnimation>();

        private bool _shouldHandle;

        private readonly List<IAnimation> _toRemove = new List<IAnimation>();

        ///<summary>
        /// Pre render slot
        ///</summary>
        ///<param name="parameters"></param>
        public void Receive(params object[] parameters)
        {
            _shouldHandle = false;
            _tweens.ForEach(delegate(IAnimation anim)
            {
                IAsyncAction tween = anim as IAsyncAction;
                //Debug.Log("Tick " + anim);
                if (null != tween && tween.Tick())
                {
                    _toRemove.Add(anim);
                    if (null != anim.Callback) // commented out 20130420 because of the Callback being fired twice (see ComboBox mask anim) => // 20131107: remove it from THERE
                    {
                        //Debug.Log("Executing Callback on: " + tween);
                        anim.Callback(anim); //IMPORTANT: Composites should also be registered and have callbacks!
                    }
                }
            });

            _toRemove.ForEach(delegate(IAnimation tween)
            {
                //Debug.Log("Removing tween: " + tween);
                _tweens.Remove(tween);
            });

            if (_toRemove.Count > 0)
                _shouldHandle = true;

            _toRemove.Clear();

            if (_shouldHandle)
                HandleConnection();
        }

        ///<summary>
        /// Adds a tween
        ///</summary>
        ///<param name="tween"></param>
        public void Add(IAnimation tween)
        {
            //CancelSimilar(tween);

            //Debug.Log("ADD: " + tween);

            // add new tween
            _tweens.Add(tween);

            //tween.Play();

            HandleConnection();

#if DEBUG
            if (DebugMode)
                Debug.Log(string.Format("===== TweenRegistry: Tween added [now running {0}] =====\nAdded => {1}", _tweens.Count, tween));
#endif

#if TRIAL
            /* RELEASE HACK CHECK */
            Acme2 acme = (Acme2)Framework.GetComponent<Acme2>(true);
            if (null == acme || !acme.gameObject.activeInHierarchy || !acme.enabled)
                return;
#endif
        }

        ///<summary>
        /// Returns the tween tweening the specified property on target
        ///</summary>
        ///<param name="target"></param>
        ///<param name="property"></param>
        ///<returns></returns>
        ///<exception cref="Exception"></exception>
        public IAnimation GetTween(object target, string property)
        {
            Debug.Log(string.Format(@"GetTween ({0}, ""{1}""", target, property));
            List<IAnimation> tweens = _tweens.FindAll(delegate(IAnimation t)
            {
                return t.Target == target
                       && t.Property == property;
            });

            if (tweens.Count > 1)
                Debug.LogWarning(string.Format(@"There's more than one tween affecting the property ""{0}""", property));

            if (tweens.Count >= 1)
                return tweens[0];

            return null;
        }

        /// <summary>
        /// Disposes and removes similar tweens
        /// Similar tweens are tweens twenning the same target by the one of affected properties
        /// </summary>
        /// <param name="tween">A tween to check on</param>
        public void CancelSimilar(IAnimation tween)
        {
            Debug.Log("CancelSimilar");
            List<IAnimation> tweens = _tweens.FindAll(delegate(IAnimation t)
            {
                return t.Target == tween.Target
                       && t.Property == tween.Property;
            });

            if (tweens.Count > 0)
            {
                //Debug.Log("### FOUND " + tweens.Count + " SIMILAR TWEENS ###");

                // dispose tweens on the same target/property
                tweens.ForEach(delegate(IAnimation t)
                {
                    t.Dispose();
                });

                // remove tweens on the same target/property
                _tweens.RemoveAll(tweens.Contains);
            }

            HandleConnection();
        }

        /// <summary>
        /// Removes the tween
        /// </summary>
        /// <param name="tween"></param>
        public void Remove(IAnimation tween)
        {
            //Debug.Log("Remove: " + tween);

            //if (!_tweens.Contains(tween))
            //    Debug.LogWarning(string.Format("Tween {0} not managed by TweenRegistry", tween));

            //else
            //{
            _tweens.Remove(tween);
#if DEBUG
            if (DebugMode)
                Debug.Log(string.Format("===== TweenRegistry: Tween removed [now running {0}] =====\nRemoved => {1}", _tweens.Count, tween));
#endif

            HandleConnection();
            //}
        }

        /// <summary>
        /// Removes the tween
        /// </summary>
        /// <param name="target"></param>
        /// <param name="property"></param>
        public void Remove(object target, string property)
        {
            _tweens.RemoveAll(delegate(IAnimation t)
            {
                return t.Target == target && t.Property == property;
            });
            HandleConnection();
        }

        /// <summary>
        /// Removes multiple tweens
        /// </summary>
        /// <param name="target"></param>
        public void RemoveTweens(object target)
        {
            _tweens.RemoveAll(delegate(IAnimation t)
            {
                return t.Target == target;
            });
            HandleConnection();
        }

        /// <summary>
        /// Removes multiple tweens
        /// </summary>
        /// <param name="tweens"></param>
        public void RemoveTweens(params ITween[] tweens)
        {
            List<IAnimation> twns = new List<IAnimation>(tweens);
            _tweens.RemoveAll(twns.Contains);
            HandleConnection();
        }

        /// <summary>
        /// Removes multiple tweens
        /// </summary>
        /// <param name="target"></param>
        /// <param name="properties"></param>
        public void RemoveTweens(object target, params string[] properties)
        {
            List<string> props = new List<string>(properties);
            _tweens.RemoveAll(delegate(IAnimation t)
            {
                return t.Target == target && props.Contains(t.Property);
            });
            HandleConnection();
        }

        /// <summary>
        /// Finds a tween
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public IAnimation Find(Predicate<IAnimation> match)
        {
            return _tweens.Find(match);
        }

        /// <summary>
        /// Finds all tweens
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public List<IAnimation> FindAll(Predicate<IAnimation> match)
        {
            return _tweens.FindAll(match);
        }

        private void HandleConnection()
        {
            //Debug.Log("Tween count: " + _tweens.Count);

            if (!SystemManager.Instance.RenderSignal.HasSlot(Receive)
                && _tweens.Count > 0)
            {
#if DEBUG
                if (DebugMode || LogConnect)
                    Debug.Log("===== TweenRegistry connecting =====");
#endif

                SystemManager.Instance.RenderSignal.Connect(Receive);
            }

            else if (SystemManager.Instance.RenderSignal.HasSlot(Receive)
                && _tweens.Count == 0)
            {
                //Debug.Log("_tweens.Count: " + _tweens.Count);
#if DEBUG
                if (DebugMode || LogDisconnect)
                    Debug.Log("===== TweenRegistry disconnecting =====");
#endif
                SystemManager.Instance.RenderSignal.Disconnect(Receive);
            }
        }

        public override string ToString()
        {
            return string.Format("TweenRegistry [Active tweens: {0}]", _tweens.Count);
        }
    }
}
