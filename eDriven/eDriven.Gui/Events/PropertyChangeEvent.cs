using eDriven.Core.Events;

namespace eDriven.Gui.Events
{
	public class PropertyChangeEvent : Event
	{
		public const string PROPERTY_CHANGE = "propertyChange";

		#region Constructor

		public PropertyChangeEvent(string type) : base(type)
		{
		}

		public PropertyChangeEvent(string type, object target) : base(type, target)
		{
		}

		public PropertyChangeEvent(string type, bool bubbles) : base(type, bubbles)
		{
		}

		public PropertyChangeEvent(string type, bool bubbles, bool cancelable) : base(type, bubbles, cancelable)
		{
		}

		public PropertyChangeEvent(string type, object target, bool bubbles, bool cancelable) : base(type, target, bubbles, cancelable)
		{
		}

		#endregion

		/**
		 *  Specifies the kind of change.
		 *  The possible values are <code>PropertyChangeEventKind.UPDATE</code>,
		 *  <code>PropertyChangeEventKind.DELETE</code>, and <code>null</code>.
		 */
		public string Kind;

		//----------------------------------
		//  newValue
		//----------------------------------

		/**
		 *  The value of the property after the change.
		 */
		public object NewValue;

		//----------------------------------
		//  oldValue
		//----------------------------------
	 
		/**
		 *  The value of the property before the change.
		 */
		public object OldValue;

		//----------------------------------
		//  property
		//----------------------------------

		/**
		 *  A String, QName, or int specifying the property that changed.
		 */
		public string Property;

		//----------------------------------
		//  source
		//----------------------------------

		/**
		 *  The object that the change occured on.
		 */
		public object Source;

		public static PropertyChangeEvent CreateUpdateEvent(object source, string property, object newValue, object oldValue)
		{
			PropertyChangeEvent e = new PropertyChangeEvent(PROPERTY_CHANGE)
										{
											Kind = PropertyChangeEventKind.UPDATE,
											OldValue = oldValue,
											NewValue = newValue,
											Source = source,
											Property = property
										};

			return e;
		}
	}
}
