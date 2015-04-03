using System.Collections.Generic;
using eDriven.Core.Reflection;
using eDriven.Gui.Components;
using Component = eDriven.Gui.Components.Component;

namespace eDriven.Gui.States
{
	/// <summary>
	/// SetProperty class is used in style overrides
	/// </summary>
	public class SetProperty : OverrideBase
	{
		#region Properties

		/// <summary>
		/// The target
		/// </summary>
		public object Target;

		/// <summary>
		/// Property name
		/// </summary>
		public string Name;

		/// <summary>
		/// New value
		/// </summary>
		public object Value;

		#endregion

		/// <summary>
		/// Storage for the old property value
		/// </summary>
		private object _oldValue;

		/**
		*  
		*  Storage for the old related property values, if used.
		*/
		private List<string> _relatedProps;

		/**
		*  
		*  Storage for the old related property values, if used.
		*/
		private List<object> _oldRelatedValues;

		private string _pseudonym;

		/**
		 *  The cached target for which we applied our override.
		 *  We keep track of the applied target while applied since
		 *  our target may be swapped out in the owning document and 
		 *  we want to make sure we roll back the correct (original) 
		 *  element. 
		 *
		 *  
		 */
		private object _appliedTarget;

		private MemberProxy _mp;

		// ReSharper disable InconsistentNaming

		/**
		 *  
		 *  This is a table of pseudonyms.
		 *  Whenever the property being overridden is found in this table,
		 *  the pseudonym is saved/restored instead.
		 */
		private static readonly Dictionary<string, string> PSEUDONYMS = new Dictionary<string, string>
		{
			{"Width", "ExplicitWidth"},
			{"Height", "ExplicitHeight"},
			{"CurrentState", "CurrentStateDeferred"}
		};

		/**
		 *  
		 *  This is a table of related properties.
		 *  Whenever the property being overridden is found in this table,
		 *  the related property is also saved and restored.
		 */
		private static readonly Dictionary<string, string[]> RELATED_PROPERTIES = new Dictionary<string, string[]>
		{
			{"ExplicitWidth", new [] {"PercentWidth"}},
			{"ExplicitHeight", new [] {"PercentHeight"}}
		};
		
		// ReSharper restore InconsistentNaming

		/// <summary>
		/// Sets the value of the property on target
		/// </summary>
		/// <param name="target"></param>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public SetProperty(object target, string name, object value)
		{
			Target = target;
			Name = name;
			Value = value;
		}

		/// <summary>
		/// Sets the value of the property on parent
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public SetProperty(string name, object value)
		{
			Name = name;
			Value = value;
		}

		/// <summary>
		/// 
		/// </summary>
		public override void Initialize()
		{
			
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="parent"></param>
		public override void Apply(Component parent)
		{
			//Debug.Log("SetProperty->Apply: Name: " + Name + "; Value: " + Value);

			_appliedTarget = GetTarget(Target, parent);
			_pseudonym = GetPseudonym(_appliedTarget, Name);
			
            _mp = new MemberProxy(_appliedTarget, _pseudonym); //_mp = GetMember(Target, parent, _pseudonym);
            if (!Applied)
			{
				_oldValue = _mp.GetValue();
			}
			_mp.SetValue(Value);
            
            // buggy caching (tries to do Color to Rotation)
            //MemberWrapper mw = new MemberWrapper(_appliedTarget.GetType(), _pseudonym);
            /*MemberWrapper mw = GlobalMemberCache.Instance.Get(_appliedTarget.GetType(), _pseudonym);
            if (null == mw) { 
                mw = new MemberWrapper(_appliedTarget.GetType(), _pseudonym);
                GlobalMemberCache.Instance.Put(_appliedTarget.GetType(), _pseudonym, mw);
            }
            if (!Applied)
            {
                _oldValue = mw.GetValue(_appliedTarget);
            }
            mw.SetValue(_appliedTarget, Value);*/

			_relatedProps = GetRelatedProperties(_appliedTarget, _pseudonym);
			if (_relatedProps.Count > 0)
			{
				if (null == _oldRelatedValues)
					_oldRelatedValues = new List<object>(); // lazily
				else
					_oldRelatedValues.Clear();

				foreach (string property in _relatedProps) {
					_oldRelatedValues.Add(new MemberProxy(_appliedTarget, property).GetValue());
				}
			}

			Applied = true;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="parent"></param>
		public override void Remove(Component parent)
		{
			//Debug.Log("SetProperty->Remove: Name: " + Name + "; Value: " + Value);

			//_appliedTarget = GetTarget(_appliedTarget, parent);
			_pseudonym = GetPseudonym(_appliedTarget, Name);

			// Special case for width and height. Restore the "width" and
			// "height" properties instead of explicitWidth/explicitHeight
			// so they can be kept in sync.
			if ((Name == "Width" || Name == "Height") && null != (_oldValue as float?))
			{
				_pseudonym = Name;
			}

			_mp = new MemberProxy(_appliedTarget, _pseudonym); // buggy caching (tries to do Color to Rotation)
            _mp.SetValue(_oldValue);
            /*MemberWrapper mw = new MemberWrapper(_appliedTarget.GetType(), _pseudonym);
            mw.SetValue(_appliedTarget, _oldValue);*/

			_oldValue = null;

			//_relatedProps = GetRelatedProperties(_appliedTarget, _pseudonym);
			if (null != _relatedProps)
			{
				for (var i = 0; i < _relatedProps.Count; i++)
				{
					_mp = new MemberProxy(_appliedTarget, _relatedProps[i]);
					_mp.SetValue(_oldRelatedValues[i]);
				}
			}

			Applied = false;
			_appliedTarget = null;
		}

		public override string ToString()
		{
			return string.Format("SetProperty [Target: {0}, Name: {1}, Value: {2}]", Target, Name, Value);
		}

		#region Helper

		/**
		 * Utility function to return the pseudonym of the property
		 * name if it exists on the object
		 */
		private string GetPseudonym(object obj, string name)
		{
			if (!(obj is InvalidationManagerClient))
				return name;

			if (PSEUDONYMS.ContainsKey(name))
			{
				return PSEUDONYMS[name];
			}

			return name;
		}

		private List<string> GetRelatedProperties(object obj, string propName)
		{
			if (!(obj is InvalidationManagerClient))
				return new List<string>();

			var related = RELATED_PROPERTIES.ContainsKey(propName) ? 
				new List<string>(RELATED_PROPERTIES[propName]) : 
				new List<string>();

			return related;
		}

		#endregion

	}
}