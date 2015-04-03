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

using System.Collections.Generic;
using eDriven.Core.Signals;
using eDriven.Gui.Editor.Dialogs;
using eDriven.Gui.Editor.Hierarchy;
using eDriven.Gui.Editor.Processing;
using UnityEditor;
using UnityEngine;
using Object=UnityEngine.Object;

namespace eDriven.Gui.Editor.Persistence
{
	/// <summary>
	/// Class managing the persistence
	/// A facade dealing with 2 kinds of persistance:
	/// 1. Persisting values of touched components
	/// 2. Persisting hierarchy
	/// 
	/// Persistence means saving the values changed in play mode<br/>
	/// When play mode is stopped, normally all the changes done when in play mode are being lost<br/>
	/// That is because Unity destroys all the objects on the scene and "resurrects" them<br/>
	/// Luckily, the data could be persisted by manually saving it in our custom data structure, just before the play mode is fully stopped<br/>
	/// We could then reapply this data to newly created objects in the scene. We just got to find the right object.<br/>
	/// Each Unity object (Component, Transform, GameObject) has the Unique identifier called the instance ID<br/>
	/// - Instance ID is readable by component.GetInstanceID()<br/>
	/// - Component having the particular instance ID could be found using the EditorUtility.InstanceIDToObject(id)<br/>
	/// - InstanceID is an integer<br/>
	/// Changed values must be applied to objects "resurrected" by Unity, after the play mode is stopped<br/>
	/// We could easily find objects existing prior to play mode, because Unity re-creates them giving them the same instance ID that they had in the play mode<br/>
	/// Objects being created in play mode are destroyed. So we have to re-create them from scratch, using:<br/>
	/// GameObject go = new GameObject();<br/>
	/// ComponentAdapter adapter = (ComponentAdapter)go.AddComponent(ComponentType);<br/>
	/// This of course means that we should save the adapter type that has to be created
	/// </summary>
	internal class PersistenceManager
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

		private static PersistenceManager _instance;

		/// <summary>
		/// Singleton class for handling focus
		/// </summary>
		private PersistenceManager()
		{
			// Constructor is protected
		}

		/// <summary>
		/// Singleton instance
		/// </summary>
		public static PersistenceManager Instance
		{
			get
			{
				if (_instance == null)
				{
#if DEBUG
					if (DebugMode)
						Debug.Log(string.Format("Instantiating PersistenceManager instance"));
#endif
					_instance = new PersistenceManager();
					//Initialize();
				}

				return _instance;
			}
		}

		#endregion

		#region Signals

		/// <summary>
		/// Fires when a new component added for monitoring
		/// </summary>
		public readonly Signal MonitoredObjectAddedSignal = new Signal();
		public readonly Signal ChangesAppliedSignal = new Signal();
		public readonly Signal ChangesAbandonedSignal = new Signal();

		#endregion

		#region Composition

		private readonly ComponentMonitor _componentMonitor = new ComponentMonitor();
		private readonly HierarchyMonitor _hierarchyMonitor = new HierarchyMonitor();

		#endregion

		#region Hierarchy delta

		public Delta Delta
		{
			get
			{
				return _hierarchyMonitor.Delta;
			}
			internal set
			{
				_hierarchyMonitor.Delta = value;
			}
		}

		#endregion

		#region Monitored objects

		public Dictionary<int, PersistedComponent> MonitoredObjects
		{
			get
			{
				return _componentMonitor.MonitoredObjects;
			}
		}

		public void Watch(Object component)
		{
			_componentMonitor.Watch(component);
			MonitoredObjectAddedSignal.Emit(component);
		}

		public void Unwatch(int componentId)
		{
			_componentMonitor.Unwatch(componentId);
		}

		#endregion

		#region Check for changes

		/// <summary>
		/// Returning true if there are pending changes<br/>
		/// This method is being called just after the play mode is stopped, and before the actual ApplyChanges call,<br/>
		/// giving the user a chance to cancel the apply call
		/// </summary>
		/// <returns></returns>
		public bool HasChanges
		{
		    get
		    {
                return _componentMonitor.HasChanges ||
                   _hierarchyMonitor.HasChanges() ||
                   ParentChildLinker.Instance.Changes.Count > 0;
		    }
		}

		#endregion

		#region Cache changes

		/// <summary>
		/// A flag indicating that another level has been loaded, so only the changes to the first one will be saved
		/// </summary>
		private bool _cachedOnSceneChange;

		public void CacheValues(bool cacheAllObjects, bool cachingOnSceneChange)
		{
			_cachedOnSceneChange = cachingOnSceneChange;

			/**
			 * 1. Apply hierarchy delta
			 * */
			_hierarchyMonitor.ApplyDelta();

			/**
			 * 2. Persist component values
			 * */
			_componentMonitor.Persist(cachingOnSceneChange);
		}

		#endregion

		#region Apply

		/// <summary>
		/// Applies changes to newly created ("resurrected") objects<br/>
		/// Thes method is being called AFTER the play mode is stopped<br/>
		/// </summary>
		protected internal void ApplyChanges()
		{
			/**
			 * 1. If no changes, do nothing
			 */
			if (!HasChanges)
			{
#if DEBUG
				if (DebugMode)
				{
					Debug.Log("No changes found.");
				}
#endif
				return;
			}

			/**
			 * 2. There are pending changes
			 * Depending of the AutoSave setting, show dialog
			 * */
			if (EditorSettings.AutoSave)
				DoApplyChanges();
			else
			{
				string title = "Save changes?";
				string msg = "Do you want to save changes made while in play mode?";

				if (AddEventHandlerPersistedData.Saved)
				{
					if (AddEventHandlerPersistedData.PostCompileProcessingMode)
					{
						title += " (creating script)";
//                        msg += @"
//
//NOTE: the application is stopping because of the the newly created script that should be compiled.";
					}
					else
					{
						title += " (adding script)";
//                        msg += @"
//
//NOTE: the application is stopping because of the the script that should be added.";
					}
				}

				else if (_cachedOnSceneChange)
				{
					title += " (1st scene only)";

					msg +=
						@"

NOTE: only the values from a starting scene will be saved.

This is because you loaded another scene dynamically.";
				}

				if (EditorUtility.DisplayDialog(title, msg, "Yes", "No"))
				{
					DoApplyChanges();
				}
				else
				{
					AbandonChanges();
				}
			}
		}

		/// <summary>
		/// Private method, containing logic of applying changes to monitored components
		/// </summary>
		private void DoApplyChanges()
		{
			/**
			 * Apply hierarchy changes
			 * */
			_hierarchyMonitor.ApplyChanges();

			/**
			 * Apply value changes
			 * */
			_componentMonitor.ApplyChanges();

			/**
			 * 5. Process order
			 * Important: This should be processed AFTER the persisted object applied whatever value
			 * This is because the linker has its own logic of applying the values (he has to look for newly created adapters etc.)
			 * */
			ParentChildLinker.Instance.Process();
			ParentChildLinker.Instance.Reset();

			/**
			 * 6. Clear dictionaries
			 * This dictionary has been filled in the 1st step (PersistedActions) 
			 * */
			/*TransformRegistry.Instance.Clear();
			ComponentRegistry.Instance.Clear();*/

			//Cleanup();

			//Debug.Log("### PersistenceManager: Changes applied ###");

			EditorState.Instance.HierarchyChange();

			/**
			 * 7. Signalize
			 * */
			ChangesAppliedSignal.Emit();
		}

		#endregion

		#region Abandon

		/// <summary>
		/// Abandons changes on all objects
		/// </summary>
		internal void AbandonChanges()
		{
#if DEBUG
			if (DebugMode)
			{
				Debug.Log(string.Format(@"Abandoning changes"));
			}
#endif
			_hierarchyMonitor.AbandonChanges();
			_componentMonitor.AbandonChanges();

			ParentChildLinker.Instance.Reset();

			//Cleanup();

			/**
			 * 7. Signalize
			 * */
			ChangesAbandonedSignal.Emit();
		}

		#endregion

		/*#region Cleanup

		private void Cleanup()
		{
			_hierarchyMonitor.Cleanup();
			_componentMonitor.AbandonChanges();

			/**
			 * 5. Clear AdapterInstanceIdToObjectDictionary
			 * #1#
			ComponentRegistry.Instance.Clear();
			TransformRegistry.Instance.Clear();

			/**
			 * 6. Reset delta
			 * #1#
			Delta.Reset();

			ParentChildLinker.Instance.Changes.Clear();
		}

		#endregion*/

	}
}