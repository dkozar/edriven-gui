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
using System.Text;
using eDriven.Core.Reflection;
using eDriven.Core.Util;
using eDriven.Gui.Designer.Adapters;
using eDriven.Gui.Designer.Collections;
using eDriven.Gui.Designer.Util;
using eDriven.Gui.Reflection;
using eDriven.Gui.Styles;
using eDriven.Gui.Styles.Serialization;
using eDriven.Gui.Util;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using Object=UnityEngine.Object;

namespace eDriven.Gui.Editor.Persistence
{
	/// <summary>
	/// The class handling the object state changes<br/>
	/// It has the internal logic of:
	/// 1. saving the original values on component selection (cloning) 
	/// 2. saving the final values just before the play mode is stopped
	/// 3. figuring out the delta (a set of values that changed)
	/// 4. finding the "resurrected" objects to which to apply changed values
	/// 5. applying the changed values to each of the found objects
	/// The logic of persisting newly created objects, as well as hierarchy changes are not handled by this class
	/// </summary>
	internal class PersistedComponent : IDisposable
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
		/// Original object values
		/// </summary>
		/// <summary>
		/// Original target
		/// That is the target monitored when GUI element clicked<br/>
		/// Important: calues persisted by this object are in fact being reapplied to another object when ApplyChanges is being called<br/>
		/// They are never applied to the same object (referenced as _target while in play mode)<br/>
		/// The values will be applied to another ("resurrected") object
		/// This object will inevitably be destroyed on play mode stop<br/>
		/// </summary>
		private Object _target;
		
		/// <summary>
		/// The original instance ID<br/>
		/// This is the ID of the component being clicked in play mode<br/>
		/// This will be the ID of the "resurrected" component after the play mode being stopped IF AND ONLY IF it is a positive number<br/>
		/// If negative, this is the component added in play mode, and will not exist out of th eplay mode anymore<br/>
		/// However, we will recreate it before the ApplyChanges is called<br/>
		/// The reference to the newly created component will be passed via the static dictionary AdapterInstanceIdToObjectDictionary<br/>
		/// The key in this dictionary will be this instance ID (not the instance ID of the newly created component)<br/>
		/// By using this dictionary, the newly created component would then be found
		/// </summary>
		private readonly int _instanceId = -1;
		
		/// <summary>
		/// Monitored component type
		/// </summary>
		private readonly Type _type;

		/// <summary>
		/// Static dictionary of saveable members for each type
		/// </summary>
		private readonly static MemberInfoCache SaveableMembers = new MemberInfoCache();

		private readonly Dictionary<MemberInfo, object> _originalValues = new Dictionary<MemberInfo, object>();

		/// <summary>
		/// Saved values<br/>
		/// (final values while stopping the play mode)
		/// </summary>
		private readonly Dictionary<MemberInfo, object> _savedValues = new Dictionary<MemberInfo, object>();

		/// <summary>
		/// Changes<br/>
		/// A list of deltas (objects containing the old and new value for each property)
		/// </summary>
		private readonly List<ValueDelta> _changes = new List<ValueDelta>();

		/// <summary>
		/// Just a string builder allocated space
		/// </summary>
		private StringBuilder _sb;

		/// <summary>
		/// The flag set by HasChanges method
		/// </summary>
		private bool _hasChanges;

		#endregion

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="target">The target</param>
		internal PersistedComponent(Object target)
		{
			/**
			 * 1. Save metadata
			 * */
			_target = target;
			_type = _target.GetType();
			_instanceId = _target.GetInstanceID();

			var adapter = _target as ComponentAdapter;
			if (null != adapter)
				ComponentRegistry.Instance.Register(_instanceId, adapter);

#if DEBUG
			if (DebugMode)
			{
				_sb = new StringBuilder();
				foreach (MemberInfo memberInfo in _type.GetMembers())
				{
					var attributes = CoreReflector.GetMemberAttributes<SaveableAttribute>(memberInfo);
					//foreach (Attribute attr in memberInfo.GetCustomAttributes(typeof(SaveableAttribute), true))
					foreach (var attribute in attributes)
					{
						if (attribute.IsSaveable)
						{
							_sb.AppendLine(string.Format("    - {0}; {1}", memberInfo.Name, memberInfo));
						}
					}
				}

				Debug.Log(string.Format(@"[{0}] SaveableMembers: {1}
{2}", _target, SaveableMembers.Count(_type), _sb), _target);
			}
#endif

			/**
			 * 2. Adding the new member info to SaveableMembers dictionary
			 * This is done for each saveable type in the component
			 * Each member that must be persisted should be decorated with [Saveable]
			 * */
			foreach (MemberInfo memberInfo in _type.GetMembers())
			{
				var attributes = CoreReflector.GetMemberAttributes<SaveableAttribute>(memberInfo);
				foreach (SaveableAttribute attribute in attributes)
				{
					if (attribute.IsSaveable)
					{
						// doesn't add anything if it already exists
						SaveableMembers.Put(_type, memberInfo);
					}
				}
			}

			/**
			 * 3. Take a snapshot of the original values
			 * */
			TakeSnapshot(_originalValues);
		}

		/// <summary>
		/// Original target
		/// </summary>
		internal Object Target
		{
			get
			{
				if (null == _target)
				{
					// retarget
					Retarget();
				}

				if (null == _target)
					throw new Exception(string.Format("{0}: Target doesn't exist [{1}]", this, _instanceId) );

				return _target;
			}
		}

		/// <summary>
		/// Called when this persistence object persists the newly created script<br/>
		/// In that case, the adapter is recreated with new ID (after the play mode stop)
		/// and then registered to ComponentRegistry with old ID, so we can access it here
		/// </summary>
		internal void Retarget()
		{
			_target = ComponentRegistry.Instance.Get(_instanceId, true);
		}

		/// <summary>
		/// Saves current values
		/// </summary>
		internal void SaveValues()
		{
#if DEBUG
			if (DebugMode)
			{
				Debug.Log("SaveValues: " + _target);
			}
#endif
			TakeSnapshot(_savedValues);
		}

		/// <summary>
		/// Takes the snapshot of current values<br/>
		/// Saves the values to a supplied dictionary
		/// </summary>
		/// <param name="dictionary"></param>
		private void TakeSnapshot(IDictionary<MemberInfo, object> dictionary)
		{
			// clear dictionary
			dictionary.Clear();

			// gets all members for this type
			var members = SaveableMembers.Get(_type);

			if (null == members)
			{
				Debug.LogWarning(string.Format("Cannot find saveable members for type [{0}]", _type));
				return;
			}

#if DEBUG
			if (DebugMode)
			{
				_sb = new StringBuilder();
			}
#endif

			int count = 1;
			/**
			 * Loop through all members to get their values
			 * */
			foreach (MemberInfo memberInfo in members)
			{
				// read value
				var value = TypeUtil.GetValue(memberInfo, _target);
				
#if DEBUG
				if (DebugMode)
				{
					_sb.AppendLine(string.Format("{0}) {1}: {2}", count, memberInfo.Name, value));
				}
#endif

				// #####################################################################
				// # Handling the case of old eDriven.Gui collection type
				// # (converting on-the-fly)
				// #####################################################################
				if (value is IList<string> && !(value is SaveableStringCollection))
				{
					var pers = SaveableStringCollection.FromList((IList<string>)value);

#if DEBUG
	if (DebugMode)
	{
		Debug.Log(string.Format(@"{0}
Converted from IList<string> to SaveableStringCollection:
---------------------------------------------------------
{1}", memberInfo, pers));
	}
#endif
					dictionary[memberInfo] = pers;
				}
				// #####################################################################

				/**
				 * 1. This is a list
				 * Important: In order to save the current values, we have to break the connection to the adapter itself
				 * (or else the collection will be changed with new values and no change between old and new will be detected)
				 * */
// ReSharper disable once CanBeReplacedWithTryCastAndCheckForNull
				else if (value is List<ComponentAdapter>)
				{
					//Debug.Log("Clonning list: " + memberInfo.Name);
					dictionary[memberInfo] = ListUtil<ComponentAdapter>.Clone((List<ComponentAdapter>)value);
				}
				
				/**
				 * 2. Cloning cloneables (rather than comparing references)
				 * */
				else if (value is ICloneable)
				{
//                    Debug.Log(@"##### This is ICloneable #####
//" + value);
					ICloneable cloneable = value as ICloneable;
					dictionary[memberInfo] = cloneable.Clone();
				}

				/**
				 * 3. Normal values
				 * */
				else
				{
					dictionary[memberInfo] = value;
				}
				count++;
			}

#if DEBUG
	if (DebugMode)
			{
				Debug.Log(string.Format(@"Values read. Target: {0} [{1} saveable members]
			
{2}", _target, members.Count, _sb), _target);
			}
#endif
		}

		/// <summary>
		/// Checks if there are any changes<br/>
		/// This method is being called when play mode stopped, giving the user a chance to cancel the persistence
		/// </summary>
		/// <returns></returns>
		internal void ProcessChanges()
		{
			//Debug.Log("HasChanges? " + _target);
			_sb = new StringBuilder();
			_changes.Clear();
			_hasChanges = false;

			foreach (MemberInfo memberInfo in _savedValues.Keys)
			{
				/**
				 * 1. Read both the old and new value (i.e. the original and changed value)
				 * */
				var oldValue = _originalValues[memberInfo];
				var newValue = _savedValues[memberInfo];
#if DEBUG
				if (DebugMode)
				{
					//Debug.Log(string.Format("   {0} [{1}, {2}]", memberInfo, oldValue, newValue));
				}
#endif
				/**
				 * 2. If one of them is null, and the other is, then values are different
				 * */
				if (oldValue == null ^ newValue == null)
				{
					//Debug.Log("Value differs: " + newValue);
					RegisterChanges(memberInfo, oldValue, newValue);
					continue; // no need to compare actual values
				}

				/**
				 * 3. If old value isn't null, then the new value isn't null neither (because of the previous check)
				 * Doing the specific check for each type using its own Equals implementation
				 * */
				if (oldValue != null)
				{
					List<ComponentAdapter> oldList = oldValue as List<ComponentAdapter>;
					
					if (null != oldList) {
						// we have a List value
					
						List<ComponentAdapter> newList = newValue as List<ComponentAdapter>;

//                        Debug.Log(string.Format(@"***** Comparing 2 lists:
//--- old --- 
//{0}
//--- new ---
//{1}",
//    ComponentAdapterUtil.DescribeAdapterList(oldList),
//    ComponentAdapterUtil.DescribeAdapterList(newList)));

						bool hasChanges = null == newList;
						if (!hasChanges)
							// Note: if new element added to list, it's adapter is being destroyed by now
							hasChanges = ComponentAdapterUtil.ListContainsNullReference(oldList);
						if (!hasChanges)
							hasChanges = ComponentAdapterUtil.ListContainsNullReference(newList);
						if (!hasChanges)
							hasChanges = !ListUtil<ComponentAdapter>.Equals(oldList, newList);

						if (hasChanges)
						{
//                            Debug.Log(string.Format(@"  -> List changed: {0}.{1}", null == _target ? "_": _target.ToString(), memberInfo.Name));
//                            Debug.Log(@"+++++ newList: 
//" + ComponentAdapterUtil.DescribeAdapterList(newList, true));
							RegisterChanges(memberInfo, oldValue, newValue);
// ReSharper disable once RedundantJumpStatement
							continue;
						}
					}

					else if (!oldValue.Equals(newValue))
					{
#if DEBUG
						if (DebugMode)
						{
							Debug.Log("Changes: " + memberInfo.Name + ": " + newValue);
						}
#endif
						RegisterChanges(memberInfo, oldValue, newValue);
// ReSharper disable once RedundantJumpStatement
						continue;
					}
				}
			}

#if DEBUG
			if (DebugMode)
			{
				if (_hasChanges)
				{
					Debug.Log(string.Format(@"{0}: {1} changes detected:
			
{2}", null == _target ? "-" : _target.ToString(), _changes.Count, _sb), _target);
				}
				else
				{
					Debug.Log("NO CHANGES detected.");
				}
			}
#endif
			//return _hasChanges;
		}

	    public bool HasChanges
	    {
	        get
	        {
                return _hasChanges;
	        }
	    }

		/// <summary>
		/// Registers changes for the supplied member
		/// </summary>
		/// <param name="memberInfo">The member info for which to register changes</param>
		/// <param name="oldValue">Original value</param>
		/// <param name="newValue">Saved value</param>
		private void RegisterChanges(MemberInfo memberInfo, object oldValue, object newValue)
		{
			//Debug.Log(string.Format("   Registering change -> {0} from [{1}] to [{2}]", memberInfo.Name, oldValue, newValue));

			_sb.AppendLine(string.Format(" -> {0} from [{1}] to [{2}]", memberInfo.Name, oldValue, newValue));
			_changes.Add(new ValueDelta(memberInfo, oldValue, newValue));
			_hasChanges = true;
		}

		/// <summary>
		/// Applies changes in edit mode<br/>
		/// 1. Looks for an object to which to apply changes to
		/// 2. Applies the changes saved while in play mode.
		/// </summary>
		internal void ApplyChanges()
		{
			/**
			 * 1. Look for the object instance
			 * */
			try
			{
				/**
				 * 1. 
				 * */
				_target = ComponentRegistry.Instance.Get(_instanceId, true); // Beware - it couldn't be false!
				if (null == _target)
				{
					Debug.LogWarning("target is null"); // NOTE: The source of the bug is here somwhere! When this is written to log - it works! This line is not being executed if Linker works on this object!!
				}
			}
			catch (Exception ex) // catch (KeyNotFoundException)
			{
				throw new Exception(string.Format("Object with instance ID [{0}] not found", _instanceId), ex);
			}

#if DEBUG
			if (DebugMode)
			{
				Debug.Log(string.Format("Instance found. Applying {0} changes to {1}", _changes.Count, _target));
			}
#endif
			//Debug.Log("_changes: " + _changes.Count);

			/**
			 * 2. Loop through changes and apply them
			 * */
			foreach (ValueDelta delta in _changes)
			{
				// Applies a new value to the object
				// Note: although this line handles the child collections, it is being overwritten later by the ParentChildLinker
				// because they should be handled by ParentChildLinker exclusively
				// CONFIRMED: check the new content children below

				//Debug.Log("Processing change: " + delta.MemberInfo);

				if (delta.MemberInfo.GetType() == typeof(StyleSheet))
					Debug.Log("Processing styleseet: " + delta.MemberInfo);

				TypeUtil.SetValue(delta.MemberInfo, _target, delta.NewValue);
			}

			/**
			 * 3. Clear changes
			 * */
			_changes.Clear();
		}

		/// <summary>
		/// Abandons chances on this object
		/// </summary>
		public void AbandonChanges()
		{
			_changes.Clear();
		}

		/// <summary>
		/// Dispose
		/// </summary>
		public void Dispose()
		{
			_savedValues.Clear();
			_changes.Clear();
		}

		/// <summary>
		/// To string
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format("PersistedComponent [{0}, {1}]", _type.Name, _instanceId);
		}
	}

	/// <summary>
	/// Class representing a member value change
	/// </summary>
	internal class ValueDelta
	{
		public MemberInfo MemberInfo;
		public object OldValue;
		public object NewValue;

		public ValueDelta(MemberInfo memberInfo, object oldValue, object newValue)
		{
			MemberInfo = memberInfo;
			OldValue = oldValue;
			NewValue = newValue;
		}

		public override string ToString()
		{
			return string.Format("{0} -> OldValue: [{1}]; NewValue: [{2}]", MemberInfo.Name, OldValue, NewValue);
		}
	}
}