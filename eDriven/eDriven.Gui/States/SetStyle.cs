using System.Collections.Generic;
using eDriven.Core.Reflection;
using eDriven.Gui.Components;
using eDriven.Gui.Styles;
using Component = eDriven.Gui.Components.Component;

namespace eDriven.Gui.States
{
	/// <summary>
	/// SetStyle class is used in style overrides
	/// </summary>
	public class SetStyle : OverrideBase
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

        /// <summary>
        /// True if old value was set as an inline style
        /// </summary>
        private bool _wasInline;

		// ReSharper disable InconsistentNaming

        ///**
        // *  
        // *  This is a table of pseudonyms.
        // *  Whenever the property being overridden is found in this table,
        // *  the pseudonym is saved/restored instead.
        // */
        //private static Dictionary<string, string> PSEUDONYMS = new Dictionary<string, string>
        //{
        //    {"Width", "ExplicitWidth"},
        //    {"Height", "ExplicitHeight"},
        //    {"CurrentState", "CurrentStateDeferred"}
        //};

		/**
		 *  
		 *  This is a table of related properties.
		 *  Whenever the property being overridden is found in this table,
		 *  the related property is also saved and restored.
		 */
		private static readonly Dictionary<string, string[]> RELATED_PROPERTIES = new Dictionary<string, string[]>
		{
            /* note: keys are small caps (they are style names), properties are big caps! */
			{"left", new [] {"X"}},
			{"right", new [] {"X"}},
			{"top", new [] {"Y"}},
			{"bottom", new [] {"Y"}}
		};
		
		// ReSharper restore InconsistentNaming

		/// <summary>
		/// Sets the value of the property on target
		/// </summary>
		/// <param name="target"></param>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public SetStyle(object target, string name, object value)
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
		public SetStyle(string name, object value)
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
            //Debug.Log("SetStyle->Apply: Name: " + Name + "; Value: " + Value);

			_appliedTarget = GetTarget(Target, parent);
			
            IStyleClient obj = ((IStyleClient)_appliedTarget);

			if (!Applied)
			{
                /**
                 * IMPORTANT: obj.StyleDeclaration.GetStyle(Name) returns UNDEFINED
                 * UNDEFINED is a flag indicating that the style ISN'T DEFINED in the style declaration (overrides)
                 * That's because the null is a valid style value, so it cannot return null because we couldn't be
                 * able to differentiate between the null (value) or undefined style
                 * */
                _wasInline = null != obj.StyleDeclaration && 
                    //StyleDeclaration.UNDEFINED != obj.StyleDeclaration.GetStyle(Name);
                    !StyleDeclaration.UNDEFINED.Equals(obj.StyleDeclaration.GetStyle(Name));

                // if it is the inline style remember its value, null otherwise (it doesn't metter, we won't use the old value in "Remove")
                _oldValue = _wasInline ? obj.GetStyle(Name) : null;
			}

			_relatedProps = GetRelatedProperties(_appliedTarget, Name);
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

            // Set new value
		    if (null == Value)
		    {
		        obj.ClearStyle(Name); // (??? why setting it to UNDEFINED? Does null for SetStyle object mean "clear property?")
		    }
		    else
		    {
		        obj.SetStyle(Name, Value);
		    }

            Applied = true;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="parent"></param>
		public override void Remove(Component parent)
		{
			//Debug.Log("SetStyle->Remove: Name: " + Name + "; Value: " + Value);

		    var obj = (IStyleClient) _appliedTarget;

            // ovo je fakat čudno, njihova logika ne radi (ili ima bug)
            // što znači "clearate" style - maknut ga iz style deklaracije ili ga staviti unutra s vrijednosti "undefined"???

            if (_wasInline)
            {
                // Restore the old value
                /*if (_oldValue == null)
                    obj.ClearStyle(Name);
                else*/
                obj.SetStyle(Name, _oldValue); // apply whatever the old value is
            }
            else
            {
                obj.ClearStyle(Name);
            }

            //obj.SetStyle(Name, _oldValue);

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
            return string.Format("SetStyle [Target: {0}, Name: {1}, Value: {2}]", Target, Name, Value);
		}

		#region Helper
        
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