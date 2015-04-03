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

using eDriven.Core.Managers;
using eDriven.Core.Signals;
using eDriven.Gui.Designer.Rendering;
using UnityEditor;

#if DEBUG
using UnityEngine;
#endif

namespace eDriven.Gui.Editor.Processing
{
    /**
    * Initialize the PlayModeStateChangeEmitter instance
    * We are subscribing to it's ChangesAppliedSignal to react when changes applied
    * so we could then pust the changes to views (events, children and layout view)
    * */

	/// <summary>
	/// A class handling the play mode state change
	/// The class subscribes to EditorApplication.playmodeStateChanged events
	/// It handles:
	/// 1. Play mode overlay
	/// 2. Caches values when stopping
	/// 3. Applies values when stopped
	/// 4. Emits the signal for refreshing the UI
	/// </summary>
	internal class PlayModeStateChangeEmitter
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

		private static PlayModeStateChangeEmitter _instance;

		/// <summary>
		/// Singleton class for handling focus
		/// </summary>
		private PlayModeStateChangeEmitter()
		{
			// Constructor is protected
		}

		/// <summary>
		/// Singleton instance
		/// </summary>
		internal static PlayModeStateChangeEmitter Instance
		{
			get
			{
				if (_instance == null)
				{
#if DEBUG
					if (DebugMode)
					{
						Debug.Log(string.Format("Instantiating PlayModeStateChangeEmitter instance"));
					}
#endif
					_instance = new PlayModeStateChangeEmitter();
					_instance.Initialize();
				}

				return _instance;
			}
		}

		#endregion

		/// <summary>
		/// A flag indicating that the previous state is PLAYING
		/// </summary>
		private bool _isPlaying;

		/// <summary>
		/// Signal for subscribing from the main display
		/// When receiveing the signal, the persisted changes should be "pushed" to displays
		/// </summary>
		public Signal ChangesAppliedSignal = new Signal();

		/// <summary>
		/// Play mode started
		/// </summary>
		public Signal PlayModeStartedSignal = new Signal();

		/// <summary>
		/// Play mode stopping
		/// </summary>
		public Signal PlayModeStoppingSignal = new Signal();

		/// <summary>
		/// Play mode stopped
		/// </summary>
		public Signal PlayModeStoppedSignal = new Signal();

		/// <summary>
		/// Scene change signal
		/// Used to cache changes from the 1st scene, if the second scene is about to be loaded
		/// </summary>
		public Signal SceneChangeSignal = new Signal();

		/// <summary>
		/// Level loaded signal
		/// Used to cancel the further persistance
		/// </summary>
		public Signal LevelLoadedSignal = new Signal();

		/// <summary>
		/// Level loaded signal
		/// Used to cancel the further persistance
		/// </summary>
		public Signal SelectionChangedSignal = new Signal();

		/// <summary>
		/// Initializes the Singleton instance
		/// </summary>
		private void Initialize()
		{
			//Debug.Log("=== PlayModeStateChangeEmitter INITIALIZE ===");
			//EditorApplication.playmodeStateChanged -= OnPlayModeStateChanged;
			EditorApplication.playmodeStateChanged += OnPlayModeStateChanged;
		}

		~PlayModeStateChangeEmitter()  // destructor
		{
// ReSharper disable once DelegateSubtraction
			EditorApplication.playmodeStateChanged -= OnPlayModeStateChanged;
		}

		/// <summary>
		/// PlayModeChanged fires 2 times:
		/// First before stopping the play mode, and then after stopping
		/// </summary>
		private void OnPlayModeStateChanged()
		{
#if DEBUG
	if (DebugMode)
	{
		Debug.Log("OnPlayModeStateChanged ***");
	}
#endif
			/**
			 * 1. Handle play mode overlay
			 * */
			Designer.DesignerState.IsPlaying = EditorApplication.isPlaying;
			if (_isPlaying != EditorApplication.isPlaying)
			{
				_isPlaying = EditorApplication.isPlaying;
				if (_isPlaying) {
					if (null != DesignerOverlay.Instance)
						DesignerOverlay.Instance.enabled = EditorSettings.InspectorEnabled;
#if DEBUG
					if (DebugMode)
					{
						Debug.Log("=== LevelLoadedSignal SUBSCRIBING ===");
					}
#endif
					SystemManager.Instance.SceneChangeSignal.Connect(SceneChangeSlot);
					SystemManager.Instance.LevelLoadedSignal.Connect(LevelLoadedSlot);
					PlayModeStartedSignal.Emit();
				}
			}

			/**
			 * 2. Cache values when stopping
			 * */
			if (EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode)
			{
				PlayModeStoppingSignal.Emit();
			}

			/**
			 * 3. Apply values when stopped
			 * */
			else if (!EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode) // process only the case when exiting the play mode complete
			{
				PlayModeStoppedSignal.Emit();
#if DEBUG
				if (DebugMode)
				{
					Debug.Log("=== LevelLoadedSignal UNSUBSCRIBING ===");
				}
#endif
				/**
				 * 4. Emit the signal
				 * */
				ChangesAppliedSignal.Emit();

				SystemManager.Instance.SceneChangeSignal.Disconnect(SceneChangeSlot);
				SystemManager.Instance.LevelLoadedSignal.Disconnect(LevelLoadedSlot);
			}
		}

		private void SceneChangeSlot(object[] parameters)
		{
			SceneChangeSignal.Emit();
		}

		private void LevelLoadedSlot(object[] parameters)
		{
			LevelLoadedSignal.Emit();
		}
	}
}