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
using eDriven.Animation.Animation;
using eDriven.Core.Managers;
using eDriven.Core.Util;
using UnityEngine;

namespace eDriven.Gui.Cursor
{
    /// <summary>
    /// Cursor manager manages cursors
    /// By default, only the system cursor (arrow) is visible in Unity
    /// This class adds other cursor images / animations
    /// The animations are being loaded from the animation package
    /// In fact, a whole package is loaded at once and set to CursorAnimator
    /// Then, CursorAnimator is responsible for animating the selected animation from current package
    /// </summary>
    public sealed class CursorManager
    {
#if DEBUG
// ReSharper disable UnassignedField.Global
        /// <summary>
        /// Debug mode
        /// </summary>
        public static bool DebugMode;

// ReSharper restore UnassignedField.Global
#endif

        #region Singleton

        private static CursorManager _instance;

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static CursorManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new CursorManager();
                    _instance.Initialize();
                }

                return _instance;
            }
        }

        #endregion

        /// <summary>
        /// If true, cursor manager is enabled
        /// You can turn of cursor management here (for mobile etc.)
        /// </summary>
        public static bool Enabled = true; // TRUE by default

        /// <summary>
        /// Cursor rendering mode<br/>
        /// The cursor could be rendered in one of the three modes (2 Unity modes and 1 eDriven mode)
        /// </summary>
        public static CursorRenderingMode RenderingMode = CursorRenderingMode.Auto;

        /// <summary>
        /// The reference to stage
        /// </summary>
        /// <summary>
        /// Initialization routine
        /// </summary>
        private void Initialize()
        {
            // cursor stack
            _stack = new TypedPriorityStack<string>();

            // animator
            _cursorAnimator = new CursorAnimator();

            SystemManager.Instance.DisposingSignal.Connect(DisposingSlot, true);
        }

        private static void DisposingSlot(object[] parameters)
        {
            Debug.Log("CursorManager>DisposingSlot");
            _instance = null;
        }

        /// <summary>
        /// Default cursor package path inside the "Resources" folder
        /// </summary>
        public static string DefaultCursorPackagePath = "Cursors/default/package";

        private readonly ISyncLoader<AnimationPackage> _loader = new AnimationPackageLoader();

        private CursorAnimator _cursorAnimator;
        private AnimationPackage _package;
        private TypedPriorityStack<string> _stack;
        private string _currentPackagePath;
        private string _currentAnimId;

        #region Show / hide

        /// <summary>
        /// Inserts the cursor on th estack
        /// This doesn't mean that the supplied cursor will be shown
        /// It all depends of the stack content
        /// If there are cursors with the higher priority, the cursor with the highest priority will be shown
        /// The inserted cursor will be shown when all other cursors with higher or equal priority will be removed
        /// </summary>
        /// <param name="type">Cursor ID</param>
        /// <param name="priority">Cursor proirity: higher numbers have smaller priority</param>
        /// <returns></returns>
        public int SetCursor(string type, int priority)
        {
            //Debug.Log(string.Format("SetCursorPosition: {0}, {1}", type, priority));

            // lazy load
            if (null == _package)
            {
                if (string.IsNullOrEmpty(DefaultCursorPackagePath))
                    throw new Exception("DefaultCursorPackagePath not set");

                if (null == _package)
                    LoadPackage(DefaultCursorPackagePath);

                _cursorAnimator.Package = _package;
            }

            if (null != type)
            {
                var id = _stack.Insert(type, priority);
                
                if (Enabled)
                    _cursorAnimator.Play(_stack.Current);
#if DEBUG
                if (DebugMode)
                {
                    Debug.Log(string.Format(@"Cursor set: ""{0}"" [ID={1}]", type, id));
                }
#endif
                return id;
            }
            
            return -1;
        }

        /// <summary>
        /// Inserts the cursor on th estack
        /// This doesn't mean that the supplied cursor will be shown
        /// It all depends of the stack content
        /// If there are cursors with the higher priority, the cursor with the highest priority will be shown
        /// The inserted cursor will be shown when all other cursors with higher or equal priority will be removed
        /// </summary>
        /// <param name="type">Cursor ID</param>
        /// <returns></returns>
        public int SetCursor(string type)
        {
            return SetCursor(type, CursorPriority.Medium);
        }

        /// <summary>
        /// Removes the cursor specified by cursor ID from the stack
        /// </summary>
        /// <param name="id">Cursor ID</param>
        public void RemoveCursor(int id)
        {
            _stack.Remove(id);
#if DEBUG
            if (DebugMode)
            {
                Debug.Log(string.Format(@"Cursor removed: [ID={0}]", id));
            }
#endif
            if (Enabled)
                _cursorAnimator.Play(_stack.Current);
        }

        /// <summary>
        /// Removes all cursors from the stack and shows the system cursor
        /// </summary>
        public void RemoveAllCursors()
        {
            _stack.Clear();
            if (Enabled)
                _cursorAnimator.Play(_stack.Current);
        }

        /// <summary>
        /// Loads the animation package (specified by the XML file and additional graphics)
        /// </summary>
        /// <param name="packageConfigFilePath"></param>
        public void LoadPackage(string packageConfigFilePath)
        {
            // if path not changed, return
            if (_currentPackagePath == packageConfigFilePath)
                return;

            // remember current path
            _currentPackagePath = packageConfigFilePath;

            // remember current animation ID
            if (null != _stack.Current)
                _currentAnimId = _stack.Current;

            // dispose old package
            //if (null != _package)
            //    _package.Dispose();

            // load new package and cache it
            _package = _loader.Load(packageConfigFilePath);

            _cursorAnimator.Package = _package;

            // if animation playing, and there is the animation with the same ID in a new package
            // continue the same animation in another package
            if (null != _currentAnimId && null != _package.Animations[_currentAnimId])
            {
                // TODO: Totaly decouple animation descriptors and actual animations (textures), because of changing the package!!!
                _cursorAnimator.Play(_stack.Current);
            }
        }

        /// <summary>
        /// If you want cursor animation to be loaded on app start, call this method
        /// </summary>
        public void Preload()
        {
            if (null == _package)
                LoadPackage(DefaultCursorPackagePath);
        }

        public string Report()
        {
            if (null == _stack)
                return "No stack";

            return _stack.ToString();
        }

        #endregion
    }

    public class CursorPriority
    {
        public const int High = 1;
        public const int Medium = 50;
        public const int Low = 100;
    }

    /// <summary>
    /// The enumeration for the cursor rendering mode
    /// </summary>
    public enum CursorRenderingMode
    {
        /// <summary>
        /// Uses the Unity cursor with CursorMode.Auto
        /// </summary>
        Auto, 
        
        /// <summary>
        /// Uses the Unity cursor with CursorMode.ForceSoftware
        /// </summary>
        ForceSoftware,
        
        /// <summary>
        /// Uses the eDriven.Gui cursor rendering to a special stage
        /// </summary>
        Stage
    }
}