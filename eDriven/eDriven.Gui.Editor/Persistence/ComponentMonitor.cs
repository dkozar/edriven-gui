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
using System.Text;
using eDriven.Gui.Designer;
using eDriven.Gui.Designer.Adapters;
using eDriven.Gui.Styles;
using UnityEngine;
using Object = UnityEngine.Object;

namespace eDriven.Gui.Editor.Persistence
{
	/// <summary>
	/// Monitors Unity component values for changes
	/// </summary>
	internal class ComponentMonitor
	{
#if DEBUG
// ReSharper disable UnassignedField.Global
		/// <summary>
		/// Debug mode
		/// </summary>
		public static bool DebugMode;

// ReSharper restore UnassignedField.Global
#endif

		/// <summary>
		/// The dictionary holding objects being clicked on in the hierarchy, being monitored for value changes
		/// </summary>
		private readonly Dictionary<int, PersistedComponent> _monitoredObjects = new Dictionary<int, PersistedComponent>();
		public Dictionary<int, PersistedComponent> MonitoredObjects
		{
			get { return _monitoredObjects; }
		}

		#region Monitoring

		/// <summary>
		/// Monitors the object for property changes<br/>
		/// Monitoring an object means putting it on the list which will be checked for changes after the play mode is stopped<br/>
		/// When put on the list, all the original properties of the object are being saved (cloned)<br/>
		/// When play mode stopped, properties are being read from all the monitored objects<br/>
		/// For each property, the original and current value are being compared for change<br/>
		/// and the changed value list is being made<br/>
		/// Changed values are being applied to an object "resurrected" by Unity, after the play mode is stopped<br/>
		/// Here we are monitoring eDriven.Gui COMPONENTS, not transforms or game objects<br/>
		/// (however, they could also be monitored in some other scenario)<br/>
		/// </summary>
		/// <param name="target">Target (component) to monitor</param>
		public void Watch(Object target)
		{
			/**
			 * 1. Get the instance ID because it is the key in the dictionary holding the monitored objects
			 * */
			int instanceId = target.GetInstanceID();

			//Debug.Log("* Monitoring: " + instanceId);

			/**
			 * 2. We need to check if the object is already being monitored
			 * This is important because we must not overwrite the original values each time the component is being clicked
			 * because this might lead to the loss of data (only changes from the last component click would be then be saved as the original values)
			 * For instance, take a look at this scenario:
			 * - this component click. Changing the text to "foo".
			 * - other component click.
			 * - this component click. Changing the color to green.
			 * In play mode, all the changes would be accumulated, and there would seem to be no problems.
			 * But after the play mode is stopped, and started again, we would discover that only the component color is being changed to green,
			 * and no text has been changed - due to second component click rewriting the "original" values, thus deleting the change to "foo"
			 * */
			if (_monitoredObjects.ContainsKey(instanceId))
				return;
#if DEBUG
			if (DebugMode)
			{
				ComponentAdapter componentAdapter = target as ComponentAdapter;
				if (null != componentAdapter)
					Debug.Log("Monitoring: " + GuiLookup.PathToString(componentAdapter.transform, " -> "), componentAdapter.transform);
			}
#endif
			/**
			 * 3. This is the first time we are monitoring this object
			 * Create a new PersistedComponent instance and add it to dictionary
			 * */
			_monitoredObjects[instanceId] = new PersistedComponent(target);
			//_currentInstanceId = target.GetInstanceID();
#if DEBUG
			if (DebugMode)
			{
				Debug.Log(string.Format("    Added [{0}] to monitored objects list. Total: {1}", target.name, _monitoredObjects.Count), target);
			}
#endif
			//MonitoredObjectAddedSignal.Emit(target);
		}

		/// <summary>
		/// Unmonitors components specified by its instance ID
		/// </summary>
		/// <param name="instanceId">Component instance ID</param>
		public void Unwatch(int instanceId)
		{
#if DEBUG
			if (DebugMode)
			{
				Debug.Log(string.Format("Removing monitored component [{0}]. {1} left.", instanceId, _monitoredObjects.Count));
			}
#endif
			if (_monitoredObjects.ContainsKey(instanceId))
				_monitoredObjects.Remove(instanceId);
		}

		#endregion

		/// <summary>
		/// Returns true if there is at least one object having the changed values
		/// </summary>
		public bool HasChanges
		{
			get
			{
				// NOTE: This loop has to be run in its entirety, because internally we are processing items
				foreach (PersistedComponent po in _monitoredObjects.Values)
				{
					po.ProcessChanges();
				}

				foreach (PersistedComponent po in _monitoredObjects.Values)
				{
					if (po.HasChanges)
					{
						return true;
					}
				}
				return false;
			}
		}

		///<summary>
		/// Persists values
		/// This method is being called JUST BEFORE the play mode is stopped
		/// It reads current values from all the monitored objects, compares them with the original values and stores the delta (value changes)
		/// These changes are then being applied by calling ApplyChanges AFTER the play mode is stopped
		/// </summary>
		/// <param name="cachingOnSceneChange">A flag indicating that another level has been loaded, so only the changes to the first one will be saved</param>
		public void Persist(bool cachingOnSceneChange)
		{
#if DEBUG
			if (DebugMode)
			{
				Debug.Log(string.Format("Persist [cachingOnSceneChange={0}; monitored objects: {1}]", cachingOnSceneChange, _monitoredObjects.Count));
			}
#endif
			/**
			 * 2. Apply values
			 * */
			foreach (PersistedComponent persistenceObject in _monitoredObjects.Values)
			{
				persistenceObject.SaveValues();
			}
		}

		public void ApplyChanges()
		{
#if DEBUG
			if (DebugMode)
			{
				StringBuilder sb = new StringBuilder();
				foreach (PersistedComponent persistedObject in MonitoredObjects.Values)
				{
					//Debug.Log("persistedObject.Target: " + persistedObject.Target);
					if (null == persistedObject.Target)
					{
						sb.AppendLine("Target is null");
					}
					else
					{
						var target = persistedObject.Target; // as ComponentAdapterBase; // it doesn't have to be ComponentAdapterBase (if it is a stylesheet for instance)
						if (target is ComponentAdapterBase)
						{
							var transform = (target as ComponentAdapterBase).transform;
							sb.AppendLine(GuiLookup.PathToString(transform, " -> "));
						}
						else if (target is eDrivenStyleSheet)
						{
							//var transform = (target as eDrivenStyleSheetBase).transform;
							sb.AppendLine("--- Stylesheet");
						}
					}
				}

				//                Debug.Log(string.Format(@"Applying changes to {0} components:
				//{1}", _monitoredObjects.Values.Count, sb));
			}
#endif
			/**
			 * 3. Handle value changes
			 * Each persistant object has the list of changes (delta between the original and final values)
			 * This changes are being applied by calling the ApplyChanges on each persisted object
			 * Internally, PersistedComponent handles the case of negative or positive instance IDs, 
			 * thus looking up for objects using the appropriate lookup
			 * */
			foreach (PersistedComponent persistedObject in _monitoredObjects.Values)
			{
				persistedObject.ApplyChanges();
			}
			_monitoredObjects.Clear();
		}

		public void AbandonChanges()
		{
			/**
			 * 3. Clear value changes
			 * */
			foreach (PersistedComponent persistenceObject in MonitoredObjects.Values)
			{
				persistenceObject.AbandonChanges();
			}
			_monitoredObjects.Clear();
		}
	}
}
