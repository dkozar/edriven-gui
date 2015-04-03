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
using eDriven.Animation;
using eDriven.Animation.Animation;
using eDriven.Core.Events;
using eDriven.Core.Geom;
using eDriven.Core.Managers;
using eDriven.Gui.Containers;
using eDriven.Gui.Stages;
using UnityEngine;
using Event=eDriven.Core.Events.Event;

namespace eDriven.Gui.Cursor
{
    /// <summary>
    /// Registres cursor manager stage
    /// Follows the mouse and changes the position of the cursor image
    /// Receives the animation package
    /// Plays the specified animation
    /// </summary>
    internal sealed class CursorAnimator : IAnimator, IDisposable
    {
#if DEBUG
        // ReSharper disable UnassignedField.Global
        public static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif

        private CursorManagerStage _stage;

        private CursorImage _cursorImage;

        //public bool HideNativeCursorOnShow = true;

        private readonly FrameAnimator _animator;

        private Point _offset = new Point();

        public CursorAnimator()
        {
            //// stage
            //InitStage();

            // internal animator
            _animator = new FrameAnimator();
            _animator.AnimationChangeSignal.Connect(AnimationChangeSlot);
            _animator.FrameChangeSignal.Connect(FrameChangeSlot);
        }

        private void InitStage()
        {
            _stage = CursorManagerStage.Instance;

            // cursor image
            _cursorImage = new CursorImage();
            _stage.AddChild(_cursorImage);
        }

        private void AnimationChangeSlot(object[] parameters)
        {
            //Debug.Log("AnimationChangeSlot");
            var anim = ((eDriven.Animation.Animation.Animation) parameters[0]);
            _offset = null != anim ? anim.Offset : new Point();

            ShowCurrentTexture();
        }

        private void FrameChangeSlot(object[] parameters)
        {
            ShowCurrentTexture();
        }

        public AnimationPackage Package
        {
            get
            {
                return _animator.Package;
            }
            set
            {
                _animator.Package = value;
            }
        }

        public void Play(string animationId)
        {
#if DEBUG
            if (DebugMode)
            {
                Debug.Log("CursorAnimator.Play: " + animationId);
            }
#endif
            _animator.Play(animationId);

            if (null != animationId)
            //if (string.IsNullOrEmpty(animationId))
            {
                // subscribe
                SystemEventDispatcher.Instance.AddEventListener(MouseEvent.MOUSE_MOVE, OnMouseMove);

                // hide system cursor
                //Screen.showCursor = false;

                // show custom cursor
                //_stage.Visible = true;

                ShowCurrentTexture();
            }
            else
            {
                Stop();
            }
        }

        public void Stop()
        {
#if DEBUG
            if (DebugMode)
            {
                Debug.Log("CursorAnimator.Stop");
            }
#endif
            _animator.Stop();
            SystemEventDispatcher.Instance.RemoveEventListener(MouseEvent.MOUSE_MOVE, OnMouseMove);

            // show system cursor
            //Screen.showCursor = true;

            HideCursor();

            // hide custom cursor
            //_stage.Visible = false;
        }

        public void Reset()
        {
#if DEBUG
            if (DebugMode)
            {
                Debug.Log("CursorAnimator.Reset");
            }
#endif
            _animator.Reset();
        }

        private void OnMouseMove(Event e)
        {
            //MouseEvent me = (MouseEvent)e;
            //Point position = me.GlobalPosition.Add(_offset);
            //_cursorImage.Move(position);
            ShowCurrentTexture();
        }
        
        public void Dispose()
        {
            _animator.Dispose();
            SystemEventDispatcher.Instance.RemoveEventListener(MouseEvent.MOUSE_MOVE, OnMouseMove);
        }

        private void ShowCurrentTexture()
        {
            //Debug.Log("_animator.Animation: " + _animator.Animation);
            //Debug.Log("_animator.Animation.CurrentFrame: " + _animator.Animation.CurrentFrame);
            if (null == _animator.Animation || null == _animator.Animation.CurrentFrame)
                return;

            var texture = _animator.Animation.CurrentFrame.Texture;
            var mode = CursorManager.RenderingMode;

            switch (mode)
            {
                case CursorRenderingMode.Auto:
                case CursorRenderingMode.ForceSoftware:
                    Point offset = (Point)_offset.Clone();
                    offset.Invert();
                    //Debug.Log("ShowCurrentTexture: " + texture + "; " + offset);
                    UnityEngine.Cursor.SetCursor((Texture2D)texture, offset.ToVector2(), 
                        mode == CursorRenderingMode.Auto ? CursorMode.Auto : CursorMode.ForceSoftware);
                    break;
                case CursorRenderingMode.Stage:
                    Screen.showCursor = false;
                    if (null == _stage) {
                        InitStage();
                    }
// ReSharper disable PossibleNullReferenceException
                    _stage.Visible = true;
// ReSharper restore PossibleNullReferenceException
                    _cursorImage.Texture = texture;
                    _cursorImage.Move(SystemManager.Instance.MousePosition.Add(_offset));
                    break;
            }
        }

        private void HideCursor()
        {
            var mode = CursorManager.RenderingMode;
            switch (mode)
            {
                case CursorRenderingMode.Auto:
                case CursorRenderingMode.ForceSoftware:
                    Point offset = (Point)_offset.Clone();
                    offset.Invert();
                    UnityEngine.Cursor.SetCursor(null, offset.ToVector2(), mode == CursorRenderingMode.Auto ? CursorMode.Auto : CursorMode.ForceSoftware);
                    break;
                case CursorRenderingMode.Stage:
                    Screen.showCursor = true;
                    if (null != _stage)
                        _stage.Visible = false;
                    break;
            }
        }
    }
}