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
using eDriven.Gui.Designer.Adapters;
using eDriven.Gui.Editor.Hierarchy;
using UnityEngine;
using Object = UnityEngine.Object;

namespace eDriven.Gui.Editor.Persistence
{
	internal class HierarchyMonitor 
	{
#if DEBUG
// ReSharper disable UnassignedField.Global
		/// <summary>
		/// Debug mode
		/// </summary>
		public static bool DebugMode;

// ReSharper restore UnassignedField.Global
#endif

		#region Members

		/// <summary>
		/// Actions, later re-generated
		/// </summary>
		private readonly List<PersistedAction> _actions = new List<PersistedAction>();

		/// <summary>
		/// Persisting transform name changes
		/// </summary>
		private readonly List<PersistedNameChange> _nameChanges = new List<PersistedNameChange>();

		private readonly List<int> _removedTransformIds = new List<int>();

		#endregion

		#region Delta

		private Delta _delta = new Delta();
		public Delta Delta
		{
			get
			{
				return _delta;
			}
			internal set
			{
				_delta = value;
			}
		}

		#endregion

		/// <summary>
		/// Persists the transform addition or re-parenting action<br/>
		/// - a new transform is being created each time the new component is added to stage
		/// - the transform could be reparented using drag & drop in the hierarchy view
		/// </summary>
		/// <param name="transform"></param>
		private void RecordAction(Transform transform)
		{
			//Debug.Log("RecordAction: " + transform);

			ComponentAdapter adapter = transform.GetComponent<ComponentAdapter>();
			PersistedAction action = new PersistedAction(adapter);
            
			_actions.Add(action);
#if DEBUG
			if (DebugMode)
			{
				Debug.Log("Action recorded: " + action);
			}
#endif
		}

		/// <summary>
		/// Persists the game object name change<br/>
		/// The transform name could be changed by clicking the game object in the hierarchy and pressing F2 for rename
		/// </summary>
		/// <param name="transform"></param>
		// ReSharper disable SuggestBaseTypeForParameter
		private void RecordNameChange(Transform transform)
		// ReSharper restore SuggestBaseTypeForParameter
		{
			PersistedNameChange persistedNameChange = new PersistedNameChange(transform);
			_nameChanges.Add(persistedNameChange);
#if DEBUG
			if (DebugMode)
			{
				Debug.Log("Transform name cached: " + persistedNameChange);
			}
#endif
		}

		/// <summary>
		/// Sets the transform for removal after the resurrection<br/>
		/// Using instance ID because the transform has already been destroyed
		/// </summary>
		/// <param name="transformId"></param>
		internal void RemoveTransform(int transformId)
		{
			_removedTransformIds.Add(transformId); //transform.GetInstanceID());

#if DEBUG
			if (DebugMode)
			{
				Debug.Log(string.Format("Removing persisted transform [{0}]. {1} pending.", transformId, _removedTransformIds.Count));
			}
#endif
			// disregard game object renaming for a removed transform
			_nameChanges.RemoveAll(delegate(PersistedNameChange nameChange)
			{
				return nameChange.TransformId == transformId; // transform.GetInstanceID();
			});
		}

		/// <summary>
		/// Returning true if there are pending changes<br/>
		/// This method is being called just after the play mode is stopped, and before the actual ApplyChanges call,<br/>
		/// giving the user a chance to cancel the apply call
		/// </summary>
		/// <returns></returns>
		public bool HasChanges()
		{
#if DEBUG
			if (DebugMode)
			{
				if (_actions.Count > 0)
					Debug.Log(_actions.Count + " transform changes");
				if (_nameChanges.Count > 0)
					Debug.Log(_nameChanges.Count + " name changes");
				if (_removedTransformIds.Count > 0)
					Debug.Log(_removedTransformIds.Count + " transform removals");
				if (ParentChildLinker.Instance.HasChanges())
					Debug.Log(ParentChildLinker.Instance.Changes.Count + " order changes");
			}
#endif
			return _actions.Count > 0 ||
				_nameChanges.Count > 0 ||
				_removedTransformIds.Count > 0;
		}

		internal void ApplyDelta()
		{
#if DEBUG
			if (DebugMode)
			{
				Debug.Log(@"Accumulated delta:
				" + _delta);
			}
#endif

			//            Debug.Log(@"Accumulated delta:
			//" + Delta);

			/**
			 * 1. Consolidate nodes that are added or renamed and then removed
			 * This type of nodes should be removed from the additions and renamings
			 * */
			_delta.Removals.ForEach(delegate(Node removedNode)
			{
				_delta.TopLevelAdditions.RemoveAll(delegate(Node node)
				{
					return node.AdapterId == removedNode.AdapterId;
				});
				_delta.Renamings.RemoveAll(delegate(Node node)
				{
					return node.AdapterId == removedNode.AdapterId;
				});
			});

			/**
			 * 2. Handle removals
			 * a) Persist transform removal (only the top ones)
			 * b) Remove persisted adapter (all - including children)
			 * */
			_delta.TopLevelRemovals.ForEach(delegate(Node node)
			{
				RemoveTransform(node.TransformId);
			});
			_delta.Removals.ForEach(delegate(Node node)
			{
				PersistenceManager.Instance.Unwatch(node.AdapterId);
			});

			/**
			 * 3. Handle additions
			 * Record transform addition
			 * */
			/*foreach (Node node in _delta.TopLevelAdditions)
			{
				RecordAction(node.Transform);
			}*/
			_delta.Additions.ForEach(delegate(Node node)
			{
				// TODO: handle the prefab case using the TopLevelAdditions (??)
				/*var prefab = PrefabUtility.GetPrefabType(node.Adapter.gameObject);
				if (PrefabType.None != prefab)
					Debug.Log("Was prefab: " + prefab);*/
				RecordAction(node.Transform);
			});

			/**
			 * 4. Handle moves
			 * */
			_delta.Moves.ForEach(delegate(Node node)
			{
				RecordAction(node.Transform);
			});

			/**
			 * 5. Handle renamings
			 * */
			_delta.Renamings.ForEach(delegate(Node node)
			{
				RecordNameChange(node.Transform);
			});

			_delta.Reset();
		}

		/// <summary>
		/// Applies changes to newly created ("resurrected") objects<br/>
		/// Thes method is being called AFTER the play mode is stopped<br/>
		/// </summary>
		protected internal void ApplyChanges()
		{
#if DEBUG
			if (DebugMode)
			{
				Debug.Log(string.Format(@"Applying changes:
----------------------
Additions/moves: {0}
TopLevelRemovals: {1}
Renamings: {2}", _actions.Count, _removedTransformIds.Count, _nameChanges.Count));
			}
#endif

			/**
			 * 1. Handle actions (if component added in play mode or re-parented)
			 * */
			foreach (PersistedAction action in _actions)
			{
				action.Apply();
			}
			_actions.Clear();

			/**
			 * 2. Handle game object name changes
			 * Important: This has to be run after handling actions, because the 1st step
			 * adds values to PersistedComponent.AdapterInstanceIdToObjectDictionary used by this step
			 * */
			foreach (PersistedNameChange nameChange in _nameChanges)
			{
				nameChange.Apply();
			}
			_nameChanges.Clear();

			/**
			 * 4. Destroy objects removed while in play mode
			 * This has to be done since UnityEditor resurrects all objects being present before the play mode has been run
			 * */
			foreach (int removedTransformId in _removedTransformIds)
			{
				Transform transform = TransformRegistry.Instance.Get(removedTransformId, true);
				if (null == transform)
				{
					throw new Exception("Transform is null: " + removedTransformId);
				}

				//if (null != transform)
				//{
				transform.parent = null; // reparent
				Object.DestroyImmediate(transform.gameObject); // DestroyImmediate because we are in edit mode
				//}
			}
			_removedTransformIds.Clear();
		}

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
			Cleanup();
		}

		public void Cleanup()
		{
			/**
			 * 1. Clear actions
			 * */
			_actions.Clear();

			/**
			 * 2. Clear name changes
			 * */
			_nameChanges.Clear();

			/**
			 * 4. Clear removed objects
			 * */
			_removedTransformIds.Clear();

			/**
			 * 5. Clear AdapterInstanceIdToObjectDictionary
			 * */
			ComponentRegistry.Instance.Clear();
			TransformRegistry.Instance.Clear();

			/**
			 * 6. Reset delta
			 * */
			Delta.Reset();
		}
	}
}