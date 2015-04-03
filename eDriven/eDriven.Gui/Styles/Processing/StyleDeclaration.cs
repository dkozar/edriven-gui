using System.Collections.Generic;
using eDriven.Gui.Components;

#if DEBUG
using UnityEngine;
#endif

namespace eDriven.Gui.Styles
{
	///<summary>
	///</summary>
	public class StyleDeclaration // : EventDispatcher // TODO: Go with signals
	{
		/// <summary>
		/// The undefined style value
		/// IMPORTANT: obj.StyleDeclaration.GetStyle(styleName) returns UNDEFINED in the case that the style isn't defined
		/// UNDEFINED is a flag indicating that the style ISN'T DEFINED in this style declaration
		/// That's because it cannot return null because the null is a valid style value, 
		/// so we couldn't be able to differentiate between the null (value) or style not being defined
		/// There's a helper method StyleManager.IsValidStyleValue(value) returning true if the value isn't UNDEFINED
		/// </summary>
// ReSharper disable InconsistentNaming
		internal static object UNDEFINED = new Undefined(); //"***_____UNDEFINED_____***";
// ReSharper restore InconsistentNaming

		///<summary>
		/// Constructor
		///</summary>
		/*public StyleDeclaration(object selector)
			: this(selector, false)
		{

		}*/

		///<summary>
		/// Constructor
		///</summary>
		public StyleDeclaration(object selector, bool autoRegisterWithStyleManager = true)
		{
			if (null != selector)
			{
				//StyleManager.Instance.SetStyleDeclaration(selector, this, false);
				if (selector is Selector)
				{
					Selector = selector as Selector;
				}
				else
				{
					// Otherwise, a legacy Flex 3 String selector was provided
					SelectorString = selector.ToString();
				}

				if (autoRegisterWithStyleManager)
					StyleManager.Instance.SetStyleDeclaration(SelectorString, this, false);     
			}
		}

		/**
		 *  
		 *  This Dictionary keeps track of all the style name/value objects
		 *  produced from this StyleDeclaration and already inserted into
		 *  prototype chains. Whenever this StyleDeclaration's overrides object
		 *  is updated by setStyle(), these clone objects must also be updated.
		 */
		private readonly List<StyleTable> _clones = new List<StyleTable>();

		/// <summary>
		/// Selector reference count
		/// </summary>
		internal int SelectorRefCount; // TEMP internal

		/// <summary>
		/// 
		/// </summary>
		internal virtual IStyleValuesFactory Set1 { get; set; } // DefaultValues

		/// <summary>
		/// 
		/// </summary>
		internal virtual IStyleValuesFactory Set2 { get; set; } // StylesheetValues
		
		/**
		 *  If the <code>setStyle()</code>s method is called on a Component or StyleDeclaration
		 *  at run time, this object stores the name/value pairs that were set;
		 *  they override the name/value pairs in the objects produced by
		 *  the  methods specified by the <code>defaultFactory</code> and 
		 *  <code>factory</code> properties.
		 */
		private StyleTable _overrides; // = new Dictionary<string, object>();

		///<summary>
		/// Style overrides
		///</summary>
		internal virtual StyleTable Overrides
		{
			get { return _overrides; }
			set { _overrides = value; }
		}

		/// <summary>
		/// Gets the style
		/// </summary>
		/// <param name="styleProp"></param>
		/// <returns></returns>
// ReSharper disable MemberCanBePrivate.Global
		public object GetStyle(string styleProp)
// ReSharper restore MemberCanBePrivate.Global
		{
			// First look in the overrides, in case setStyle()
			// has been called on this StyleDeclaration.
			if (null != _overrides)
			{
				// If the property exists in our overrides, but 
				// has 'undefined' as its value, it has been 
				// cleared from this stylesheet so return
				// undefined.
				/*if (_overrides.ContainsKey(styleProp) && UNDEFINED.Equals(_overrides[styleProp]))
				{
					return UNDEFINED;
				}*/

				if (_overrides.ContainsKey(styleProp))
				{
					var v = _overrides[styleProp];
					if (!UNDEFINED.Equals(v)) // must use !==
						return v;
				}
			}
			// not found in overrides

			if (null != Set2)
			{
				StyleTable o = Set2.Produce();
				object v = o.GetValue(styleProp);
				if (!UNDEFINED.Equals(v))
					return v;
			}
			// not found in stylesheet

			if (null != Set1)
			{
				StyleTable o = Set1.Produce();
				object v = o.GetValue(styleProp);
				if (!UNDEFINED.Equals(v))
					return v;
			}
			// not faund in default values

			// so return null
			return UNDEFINED;
		}

		/// <summary>
		/// Sets the style
		/// </summary>
		/// <param name="styleProp"></param>
		/// <param name="newValue"></param>
// ReSharper disable MemberCanBeProtected.Global
		public virtual void SetStyle(string styleProp, object newValue)
// ReSharper restore MemberCanBeProtected.Global
		{
			object oldValue = GetStyle(styleProp);
			bool regenerate = false;

			if (SelectorRefCount > 0 &&
				Set2 == null &&
				Set1 == null &&
				null == _overrides && 
				(oldValue != newValue))
			{
				regenerate = true;
			}

			if (!UNDEFINED.Equals(newValue))
			{
				SetLocalStyle(styleProp, newValue);
			}
			else
			{
				if (newValue == oldValue)
					return;
				SetLocalStyle(styleProp, newValue);
			}

			var stageList = StageManager.Instance.StageList;
			var count = stageList.Count;

			if (regenerate)
			{
				// Regenerate all the proto chains
				// for all objects in the application.
				for (int i = 0; i < count; i++)
				{
					var stage = stageList[i];
					stage.RegenerateStyleCache(true);
				}
			}

			for (int i = 0; i < count; i++)
			{
				var stage = stageList[i];
				stage.NotifyStyleChangeInChildren(styleProp, newValue, true);
			}
		}

		/// <summary>
		/// Private
		/// </summary>
		/// <param name="styleProp"></param>
		/// <param name="value"></param>
		internal void SetLocalStyle(string styleProp, object value)
		{
			//object oldValue = GetStyle(styleProp);

			if (UNDEFINED.Equals(value))
			{
				ClearStyleAttr(styleProp);
				return;
			}

			StyleTable o;

			if (null != Set1)
			{
				o = Set1.Produce();
				if (!o.ContainsKey(styleProp) || o[styleProp] != value) // Defaultni factory nema taj stil ili ga ima ali s različitom vrijednošću
				{
					/**
					 * Defaultni factory ima različitu vrijednost ovog stila, znači radi se o overrideu
					 * Kreirajmo overrides tablicu i u njoj setirajmo stil
					 * */
					if (null == _overrides)
						_overrides = new StyleTable();

					_overrides[styleProp] = value;
				}
				else if (null != _overrides && _overrides.ContainsKey(styleProp)) // Defaultni factory ima taj stil i to sa istom vrijednošću
				{
					/**
					 * Obrišimo ga u overrides tabeli
					 * */
					_overrides.Remove(styleProp);
				}
			}

			if (null != Set2)
			{
				o = Set2.Produce();
				if (!o.ContainsKey(styleProp) || o[styleProp] != value)
				{
					if (null == _overrides)
						_overrides = new StyleTable();

					_overrides[styleProp] = value;
				}
				else if (null != _overrides && _overrides.ContainsKey(styleProp))
				{
					_overrides.Remove(styleProp);
				}
			}

			if (null == Set1 && null == Set2) // Ne postoji niti jedan factory (ni metadata, ni stylesheet)
			{
				if (null == _overrides)
					_overrides = new StyleTable();
				_overrides[styleProp] = value;
			}

			UpdateClones(styleProp, value);
		}

		private void UpdateClones(string styleProp, object value)
		{
			foreach (var clone in _clones)
			{
				clone[styleProp] = value;
			}
		}

		//private void UpdateClones(string styleProp, object value)
		//{
		//    // Update all clones of this style sheet.
		//    foreach (var clone in _clones)
		//    {
		//        clone[styleProp] = value;

		//        //var cloneFilter:Object = clone[FILTERMAP_PROP];
		//        //if (cloneFilter)
		//        //{
		//            //if (cloneFilter[styleProp] != null)
		//            //{
		//            //    clone[cloneFilter[styleProp]] = value;      
		//            //}
		//        //}
		//        //else
		//        //{
		//        //    clone[styleProp] = value;
		//        //}
		//    }
		//}

		/// <summary>
		/// Clears the supplied style
		/// </summary>
		/// <param name="styleProp"></param>
		public void ClearStyle(string styleProp)
		{
			SetStyle(styleProp, UNDEFINED);
		}

		/**
		 *  
		 */
		internal StyleTable CreateProtoChainRoot(bool registerClone = true)
		{
			StyleTable root = new StyleTable();

			// If there's a defaultFactory for this style sheet,
			// then add the object it produces to the root.
			if (null != Set1)
			{
				root = root.CloneAndOverrideWith(Set1.Produce());
			}

			// If there's a factory for this style sheet,
			// then add the object it produces to the root.
			if (null != Set2)
			{
				root = root.CloneAndOverrideWith(Set2.Produce());
			}

			if (registerClone)
				_clones.Add(root);
			
			return root;
		}

#if DEBUG
// ReSharper disable UnassignedField.Global
		/// <summary>
		/// Debug mode
		/// </summary>
		public string DebugId;

// ReSharper restore UnassignedField.Global
#endif

		///<summary>
		///</summary>
		///<param name="chain">Replikativni Dictionary</param>
		///<param name="target"></param>
		///<returns></returns>
		internal virtual StyleTable AddStyleToProtoChain(StyleTable chain, object target/*, object filterMap*/)
		{
			//Debug.Log("AddStyleToProtoChain: chain: " + chain + "; target: " + target);

			bool nodeAddedToChain = false;
			//var originalChain = chain;

			//if (filterMap)
			//{
			//    chain = {};
			//}

#if DEBUG
			if (!string.IsNullOrEmpty(DebugId))
			{
				Debug.Log(DebugId + " 0. " + chain);
			}
#endif

			// If there's a defaultFactory for this style sheet,
			// then add the object it produces to the chain.
			if (null != Set1)
			{
				chain = chain.CloneAndOverrideWith(Set1.Produce());
				nodeAddedToChain = true;
			}

#if DEBUG
			if (!string.IsNullOrEmpty(DebugId))
			{
				Debug.Log(DebugId + " 1. " + chain);
			}
#endif

			// If there's a factory for this style sheet,
			// then add the object it produces to the chain.
			if (null != Set2)
			{
				chain = chain.CloneAndOverrideWith(Set2.Produce());
				nodeAddedToChain = true;
			}

#if DEBUG
			if (!string.IsNullOrEmpty(DebugId))
			{
				Debug.Log(DebugId + " 2. " + chain);
			}
#endif

			//Debug.Log("-- chain: " + chain);

			// If someone has called setStyle() on this StyleDeclaration,
			// then some of the values returned from the factory are
			// out-of-date. Overwrite them with the up-to-date values.
			if (null != _overrides)
			{
				// Before we add our overrides to the object at the head of
				// the chain, make sure that we added an object at the head
				// of the chain.
				if (null == Set1 && null == Set2)
				{
					chain = (StyleTable)chain.Clone();
					nodeAddedToChain = true;
				}

				foreach (string p in _overrides.Keys)
				{
					if (!UNDEFINED.Equals(_overrides[p]))
						chain[p] = _overrides[p];
					/*if (UNDEFINED.Equals(_overrides[p]))
						chain.Remove(p);
					else
						chain[p] = _overrides[p];*/
				}
			}

#if DEBUG
			if (!string.IsNullOrEmpty(DebugId))
			{
				Debug.Log(DebugId + " 3. " + chain);
			}
#endif

			#region _complex

			////if (filterMap)
			////{
			//    if (nodeAddedToChain)
			//    {
			//        //Debug.Log("nodeAddedToChain");
			//        //var filteredChain = new Dictionary<string, object>();
			//        //for (string i in chain)
			//        //{
			//        //    if (filterMap[i] != null)
			//        //    {
			//        //        filteredChain[filterMap[i]] = chain[i];
			//        //    }
			//        //} 

			//        //var f = new StyleTableValuesFactory(originalChain);

			//        chain = (StyleTable) originalChain.Clone();

			//        //chain = originalChain;

			//        //chain = filteredChain;
			//        //chain = f.Produce();

			//        //chain[FILTERMAP_PROP] = filterMap;
			//    }
			//    else
			//    {
			//        chain = originalChain;
			//    }
			////}

			#endregion

			if (nodeAddedToChain)
				_clones.Add(chain);

			return chain;
		}
		
// ReSharper disable once UnusedMember.Local
		private void ClearOverride(string styleProp)
		{
			if (null != _overrides && _overrides.ContainsKey(styleProp) && !UNDEFINED.Equals(_overrides[styleProp]))
				_overrides.Remove(styleProp);
		}

		/**
		 *  
		 */
		private void ClearStyleAttr(string styleProp)
		{
			// Put "undefined" into our overrides Array
			if (null == _overrides)
				_overrides = new StyleTable();

			_overrides[styleProp] = UNDEFINED;
			
			// Remove the property from all our clones
			foreach (var clone in _clones)
			{
				clone.Remove(styleProp);
			}
		}

		//----------------------------------
		//  selector
		//----------------------------------

		private Selector _selector;
		private string _selectorString;

		/// <summary>
		/// Selector string
		/// </summary>
		internal string SelectorString // TEMP internal
		{
			set
			{
				// For the legacy API, the first argument is either a simple
				// type selector or a universal class selector
				if (value[0] == '.')
				{
					CSSCondition condition = new CSSCondition(CSSConditionKind.Class, value.Substring(1));
					_selector = new Selector("", new List<CSSCondition> {condition});
				}
				else
				{
					_selector = new Selector(value);
				}

				_selectorString = value;
			}
			get
			{
				if (_selectorString == null && _selector != null)
					_selectorString = _selector.ToString();

				return _selectorString; 
			}
		}

		/**
		 *  This property is the base selector of a potential chain of selectors
		 *  and conditions that are used to match CSS style declarations to
		 *  components.
		 */
		///<summary>
		///</summary>
		internal Selector Selector // TEMP internal
		{
			get
			{
				return _selector; 
			}
			set
			{
				_selector = value;
				_selectorString = null;
			}
		}

		/**
		 *  Determines whether this style declaration applies to the given component
		 *  based on a match of the selector chain.
		 * 
		 *  Returns: true if this style declaration applies to the component, 
		 *  otherwise false.
		 */
		///<summary>
		///</summary>
		///<param name="client"></param>
		///<returns></returns>
		public bool MatchesStyleClient(IStyleClient client)
		{
			//return (_selector != null) ? _selector.MatchesStyleClient(client) : false;
			return (_selector != null) && _selector.MatchesStyleClient(client);
		}

		//----------------------------------
		//  specificity
		//----------------------------------

		/**
		 *  Determines the order of precedence when applying multiple style
		 *  declarations to a component. If style declarations are of equal
		 *  precedence, the last one wins.
		 */
		///<summary>
		///</summary>
		internal int Specificity // TEMP internal
		{
			get
			{
				return null != _selector ? _selector.Specificity : 0; 
			}
		}

		/**
		 *  The subject describes the name of a component that may be a potential
		 *  match for this style declaration. The subject is determined as right
		 *  most simple type selector in a potential chain of selectors.
		 */
		/// <summary>
		/// Subject is the right most type selector
		/// </summary>
		internal string Subject // TEMP internal
		{
			get
			{
				if (_selector != null)
				{
					// Check for an implicit universal selector which omits *
					// for the subject but includes conditions.
					if (_selector.Subject == string.Empty && null != _selector.Conditions)
						return "*";
					return _selector.Subject;
				}

				return null;
			}
		}

		public override string ToString()
		{
			return string.Format("Subject: {0}; Selector: {1};", Subject, SelectorString);
		}

		/*public override string ToString()
		{
			var table = new StyleTable();

			// If there's a defaultFactory for this style sheet,
			// then add the object it produces to the chain.
			if (null != Set1)
			{
				table = table.CloneAndOverrideWith(Set1.Produce());
			}
			
			// If there's a factory for this style sheet,
			// then add the object it produces to the chain.
			if (null != Set2)
			{
				table = table.CloneAndOverrideWith(Set2.Produce());
			}

			return string.Format(@"Subject: {0}; Selector: {1}; Specificity: {2}; {3}", Subject, SelectorString, Specificity, table);
		}*/

		//internal bool IsReflected { get; set; }

		internal string Module { get; set; }
	}
}