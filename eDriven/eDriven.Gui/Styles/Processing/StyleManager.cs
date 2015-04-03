using System.Collections.Generic;
using System.Text;
using eDriven.Gui.Containers;
using UnityEngine;

namespace eDriven.Gui.Styles
{
	internal class StyleManager
	{
#if DEBUG
		// ReSharper disable UnassignedField.Global
		public static bool DebugMode;
		// ReSharper restore UnassignedField.Global
#endif

		#region Singleton

		private static StyleManager _instance;

		/// <summary>
		/// Singleton class for handling focus
		/// </summary>
		private StyleManager()
		{
			// Constructor is protected
		}

		/// <summary>
		/// Singleton instance
		/// </summary>
		public static StyleManager Instance
		{
			get
			{
				if (_instance == null)
				{
#if DEBUG
					if (DebugMode)
						Debug.Log(string.Format("Instantiating StyleManager instance"));
#endif
					_instance = new StyleManager();
					_instance.Initialize();
				}

				return _instance;
			}
		}

		private void Initialize()
		{
			//StyleSheetProcessor.Load();
		}

		#endregion

		#region Size invalidating styles

		/**
		 *  
		 *  Set of styles for which setStyle() causes
		 *  invalidateDisplayList() to be called on the component's parent.
		 *  The method registerParentDisplayListInvalidatingStyle() adds to this set
		 *  and isParentDisplayListInvalidatingStyle() queries this set.
		 */
		private readonly List<string> _sizeInvalidatingStyles = new List<string>
																	{
																		"marginTop",
																		"marginBottom",
																		"marginLeft",
																		"marginRight",
																		"paddingLeft",
																		"paddingRight",
																		"paddingTop",
																		"paddingBottom",
																		"horizontalAlign",
																		"horizontalGap",
																		//"horizontalSpacing",
																		"verticalAlign",
																		"verticalGap",
																		//"verticalSpacing",
																		"font"
																	};

		///<summary>
		///</summary>
		///<param name="styleName"></param>
		public void RegisterSizeInvalidatingStyle(string styleName)
		{
			if (!_sizeInvalidatingStyles.Contains(styleName))
				_sizeInvalidatingStyles.Add(styleName);
		}

		///<summary>
		/// Returns true if the queried style is size invalidating
		///</summary>
		///<param name="styleName"></param>
		///<returns></returns>
		public bool IsSizeInvalidatingStyle(string styleName)
		{
			if (_sizeInvalidatingStyles.Contains(styleName))
				return true;

			//if (Parent && Parent.isSizeInvalidatingStyle(styleName))
			//    return true;

			return false;
		}

		#endregion

		#region Parent size invalidating styles

		/**
		 *  
		 *  Set of styles for which setStyle() causes
		 *  invalidateDisplayList() to be called on the component's parent.
		 *  The method registerParentDisplayListInvalidatingStyle() adds to this set
		 *  and isParentDisplayListInvalidatingStyle() queries this set.
		 */
		private readonly List<string> _parentSizeInvalidatingStyles = new List<string>
																		  {
																			  "left", 
																			  "right",
																			  "top",
																			  "bottom",
																			  "horizontalCenter",
																			  "verticalCenter"
																		  };

		public void RegisterParentSizeInvalidatingStyle(string styleName)
		{
			if (!_parentSizeInvalidatingStyles.Contains(styleName))
				_parentSizeInvalidatingStyles.Add(styleName);
		}

		public bool IsParentSizeInvalidatingStyle(string styleName)
		{
			if (_parentSizeInvalidatingStyles.Contains(styleName))
				return true;

			//if (Parent && Parent.isParentSizeInvalidatingStyle(styleName))
			//    return true;

			return false;
		}

		#endregion

		#region Parent display list invalidating styles

		/**
		 *  
		 *  Set of styles for which setStyle() causes
		 *  invalidateDisplayList() to be called on the component's parent.
		 *  The method registerParentDisplayListInvalidatingStyle() adds to this set
		 *  and isParentDisplayListInvalidatingStyle() queries this set.
		 */
		private readonly List<string> _parentDisplayListInvalidatingStyles = new List<string>
																		{
																			"left", 
																			"right",
																			"top",
																			"bottom",
																			"horizontalCenter",
																			"verticalCenter"
																		};

		/**
		 *  Adds to the list of styles which may affect the appearance
		 *  or layout of the component's parent container.
		 *  When one of these styles is set with <code>setStyle()</code>,
		 *  the <code>invalidateDisplayList()</code> method is auomatically called on the component's
		 *  parent container to make it redraw and/or relayout its children.
		 *
		 *  Param: styleName The name of the style to register.
		 */
		public void RegisterParentDisplayListInvalidatingStyle(string styleName)
		{
			if (!_parentDisplayListInvalidatingStyles.Contains(styleName))
				_parentDisplayListInvalidatingStyles.Add(styleName);
		}

		/**
		 *  Tests to see if this style affects the component's parent container in
		 *  such a way as to require that the parent container redraws itself when this style changes.
		 *
		 *  <p>When one of these styles is set with <code>setStyle()</code>,
		 *  the <code>invalidateDisplayList()</code> method is auomatically called on the component's
		 *  parent container to make it redraw and/or relayout its children.</p>
		 *
		 *  Param: styleName The name of the style to test.
		 *
		 *  Returns: Returns <code>true</code> if the specified style is one
		 *  which may affect the appearance or layout of the component's
		 *  parent container.
		 */
		public bool IsParentDisplayListInvalidatingStyle(string styleName)
		{
			if (_parentDisplayListInvalidatingStyles.Contains(styleName))
				return true;

			//if (null != Parent && Parent.isParentDisplayListInvalidatingStyle(styleName))
			//    return true;

			return false;
		}

		#endregion

		#region Selectors / declarations

		private readonly Dictionary<string, StyleDeclaration> _selectors = new Dictionary<string, StyleDeclaration>();

		/**
		 *  @review
		 *  Returns an array of strings of all CSS selectors registered with the StyleManager.
		 *  Pass items in this array to the getStyleDeclaration function to get the corresponding StyleDeclaration.
		 *  Note that class selectors are prepended with a period.
		 *  
		 *  Returns: An array of all of the selectors
		 */ 
		///<summary>
		/// Gets the list of selectors (as strings)
		///</summary>
		internal List<string> Selectors // TEMP internal
		{
			get
			{
				List<string> theSelectors = new List<string>();
				foreach (var key in _selectors.Keys)
				{
					theSelectors.Add(key);
				}
				return theSelectors;
			}
		}

		#endregion

		/// <summary>
		/// Gets the style declaration
		/// </summary>
		/// <param name="selector"></param>
		/// <returns></returns>
		internal StyleDeclaration GetStyleDeclaration(string selector)
		{
			if (!_selectors.ContainsKey(selector))
				return null;

			return _selectors[selector];
		}

// ReSharper disable UnusedMember.Global
		///<summary>
		///</summary>
		///<param name="selector"></param>
		///<param name="styleDeclaration"></param>
		///<param name="update"></param>
		internal void SetStyleDeclaration(string selector, StyleDeclaration styleDeclaration, bool update)
// ReSharper restore UnusedMember.Global
		{
			//Debug.Log("SetStyleDeclaration: " + selector);

			styleDeclaration.SelectorRefCount++;
			//Debug.Log("    styleDeclaration.SelectorRefCount: " + styleDeclaration.SelectorRefCount);

			_selectors[selector] = styleDeclaration;

			//// Flush cache and start over.
			//_typeSelectorCache = new Dictionary<string, List<StyleDeclaration>>();

			//if (update)
			//    StyleDeclarationsChanged();

			// We also index by subject to help match advanced selectors
			string subject = styleDeclaration.Subject;
			if (null != selector)
			{
				if (null == styleDeclaration.Subject)
				{
					// If the styleDeclaration does not yet have a subject we
					// update its selector to keep it in sync with the provided
					// selector.
					styleDeclaration.SelectorString = selector;
					subject = styleDeclaration.Subject;
				}
				else if (selector != styleDeclaration.SelectorString)
				{
					// The styleDeclaration does not match the provided selector, so
					// we ignore the subject on the styleDeclaration and try to
					// determine the subject from the selector
					char firstChar = selector[0]; 
					if (firstChar == '.' || firstChar == ':' || firstChar == '#')
					{
						subject = "*";
					}
					else
					{
						// TODO: Support parsing Advanced CSS selectors for a 
						// subject...
						subject = selector;
					}

					// Finally, we update the styleDeclaration's selector to keep
					// it in sync with the provided selector.
					styleDeclaration.SelectorString = selector;
				}
			}

			//Debug.Log("subject: " + subject);

			if (subject != null)
			{
				List<StyleDeclaration> declarations = null;
				
				if (_subjects.ContainsKey(subject))
					declarations = _subjects[subject];

				if (declarations == null)
				{
					declarations = new List<StyleDeclaration> {styleDeclaration};
					_subjects[subject] = declarations;
				}
				else
				{
					declarations.Add(styleDeclaration);
				}
			}

			//Debug.Log(3);

			// Flush cache and start over.
			//_typeSelectorCache = new Dictionary<string, List<StyleDeclaration>>();

			if (update)
				StyleDeclarationsChanged();
		}

// ReSharper disable UnusedMember.Global
		///<summary>
		///</summary>
		///<param name="selector"></param>
		///<param name="update"></param>
		public void ClearStyleDeclaration(string selector, bool update)
// ReSharper restore UnusedMember.Global
		{
			StyleDeclaration styleDeclaration = GetStyleDeclaration(selector);

			if (null != styleDeclaration && styleDeclaration.SelectorRefCount > 0)
				styleDeclaration.SelectorRefCount--;

			if (_selectors.ContainsKey(selector))
				_selectors.Remove(selector);

			// Clear out matching decls from our selectors stored by subject
			int i;
			StyleDeclaration decl;

			if (null != styleDeclaration && string.IsNullOrEmpty(styleDeclaration.Subject))
			{
				List<StyleDeclaration> decls = _subjects[styleDeclaration.Subject];
				if (null != decls)
				{
					// Work from the back of the array so we can remove elements
					// as we go.
					for (i = decls.Count - 1; i >= 0; i--)
					{
						decl = decls[i];
						if (null != decl && decl.SelectorString == selector)
						{
							if (decls.Count == 1)
								_subjects.Remove(styleDeclaration.Subject);
							else
								decls.RemoveAt(i);
						}
					}
				}
			}
			else
			{
				// Without a subject, we start searching all declarations for this
				// selector, clear out matching selectors if found and then assume
				// this we can limit our search to this subject and stop looking.
				var matchingSubject = false;
				foreach (var decls2 in _subjects.Values)
				{
					if (null != decls2)
					{
						// Work from the back of the array so we can remove elements
						// as we go.
						for (i = decls2.Count - 1; i >= 0; i--)
						{
							decl = decls2[i];
							if (null != decl && decl.SelectorString == selector)
							{
								matchingSubject = true;
								if (decls2.Count == 1)
									_subjects.Remove(decl.Subject);
								else
									decls2.RemoveAt(i);
							}
						}

						if (matchingSubject)
							break;
					}
				}
			}

			if (update)
				StyleDeclarationsChanged();
		}

		private readonly List<string> _tempStringList = new List<string>();

		///<summary>
		/// Clears all style declarations (moja metoda!)
		///</summary>
		public void ClearStyleDeclarations(bool update)
		{
			_tempStringList.Clear();

			foreach (string selector in _selectors.Keys)
			{
				_tempStringList.Add(selector);
			}

			foreach (string selector in _tempStringList)
			{
				ClearStyleDeclaration(selector, false);
			}

			if (update)
				StyleDeclarationsChanged();

			/*List<string> keysToRemove = new List<string>();

			/**
			 * Delete only declarations created when loading from stylesheets
			 * (do not delete default, reflected declarations nor "global")
			 * #1#
			foreach (var pair in _selectors)
			{
				if (!pair.Value.IsReflected && pair.Key != "global")
					keysToRemove.Add(pair.Key);
			}

			Debug.Log(@"Keys to remove: 
" + ListUtil<string>.Format(keysToRemove));

			foreach (string key in keysToRemove)
			{
				_selectors.Remove(key);
			}

			Debug.Log("After removal: " + _selectors.Count);*/

			_selectors.Clear(); // NONO: this removes also the default declarations
		}

		/**
		 *  
		 *  After an entire selector is added, replaced, or removed,
		 *  this method updates all the DisplayList trees.
		 */
		///<summary>
		///</summary>
		public void StyleDeclarationsChanged()
		{
#if DEBUG
			if (DebugMode)
			{
				Debug.Log("##### StyleDeclarationsChanged #####");
			}
#endif
			var stageList = StageManager.Instance.StageList;
			var count = stageList.Count;
			
			for (int i = 0; i < count; i++)
			{
				Stage component = stageList[i];
				component.RegenerateStyleCache(true);
				component.NotifyStyleChangeInChildren(null, null, true);
			}
		}

		#region Type selector cache (not used because always Advanced)

		/*///<summary>
		///</summary>
		private Dictionary<string, List<StyleDeclaration>> _typeSelectorCache = new Dictionary<string, List<StyleDeclaration>>();

		/// <summary>
		/// Type selector cache is a cache for superclass type lookup, so we don't have to go through a whole proces each time
		/// </summary>
		public Dictionary<string, List<StyleDeclaration>> TypeSelectorCache
		{
			get
			{
				return _typeSelectorCache;
			}
		}*/

		#endregion

		#region Type hierarchy cache

		///<summary>
		///</summary>
		private readonly Dictionary<string, OrderedObject<bool>> _typeHierarchyCache = new Dictionary<string, OrderedObject<bool>>();

		/// <summary>
		/// Type selector cache is a cache for superclass type lookup, so we don't have to go through a whole proces each time
		/// </summary>
		public Dictionary<string, OrderedObject<bool>> TypeHierarchyCache
		{
			get
			{
				return _typeHierarchyCache;
			}
		}

		#endregion

		#region Styles root

		/**
		 *  
		 */
		private StyleTable _stylesRoot; // = new StyleTable();

		/**
		 *  
		 *  The root of all proto chains used for looking up styles.
		 *  This object is initialized once by initProtoChainRoots() and
		 *  then updated by calls to setStyle() on the global StyleDeclaration.
		 *  It is accessed by code that needs to construct proto chains,
		 *  such as the initProtoChain() method of Component.
		 */
		///<summary>
		/// Styles root
		///</summary>
		internal StyleTable StylesRoot
		{
			get
			{
				return _stylesRoot;
			}
			set
			{
				_stylesRoot = value;
			}
		}

		/**
		 *  
		 *  This method is called by code autogenerated by the MXML compiler,
		 *  after StyleManager.styles is popuplated with CSSStyleDeclarations.
		 */
		///<summary>
		///</summary>
		internal void InitProtoChainRoots()
		{
			//Debug.Log("### InitProtoChainRoots ###");
			if (null == _stylesRoot)
			{
				//_stylesRoot = _selectors["global"].AddStyleToProtoChain(new StyleTable(), null);
				StyleDeclaration style = GetMergedStyleDeclaration("global");
				if (style != null)
				{
					//Debug.Log("Global:::::: " + style);
					_stylesRoot = style.AddStyleToProtoChain(new StyleTable(), null);
				}
			}
		}

		#endregion

		//----------------------------------
		//  inheritingStyles
		//----------------------------------

		/**
		 *  
		 */
		private List<string> _inheritingStyles = new List<string>()
		{
			"backgroundColor"
		};

		/**
		 *  
		 *  Set of inheriting non-color styles.
		 *  This is not the complete set from CSS.
		 *  Some of the omitted we don't support at all,
		 *  others may be added later as needed.
		 *  The method registerInheritingStyle() adds to this set
		 *  and isInheritingStyle() queries this set.
		 */
		///<summary>
		///</summary>
		public List<string> InheritingStyles
		{
			get
			{
				return _inheritingStyles;
			}
			set
			{
				_inheritingStyles = value;
			}
		}

		/**
		 *  Adds to the list of styles that can inherit values
		 *  from their parents.
		 *
		 *  <p><b>Note:</b> Ensure that you avoid using duplicate style names, as name
		 *  collisions can result in decreased performance if a style that is
		 *  already used becomes inheriting.</p>
		 *
		 *  Param: styleName The name of the style that is added to the list of styles that can inherit values.
		 */
		///<summary>
		///</summary>
		///<param name="styleName"></param>
		public void RegisterInheritingStyle(string styleName)
		{
			if (!_inheritingStyles.Contains(styleName))
				_inheritingStyles.Add(styleName);
		}

		/// <summary>
		/// Returns true if the style is inheriting style (parent to child components)
		/// </summary>
		/// <param name="styleName"></param>
		/// <returns></returns>
		public bool IsInheritingStyle(string styleName)
		{
			return _inheritingStyles.Contains(styleName);
		}

		/// <summary>
		/// Returns true if this is a valid style value
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static bool IsValidStyleValue(object value)
		{
			// By convention, we don't allow style values to be undefined,
			// so we can check for this as the "not set" value.
			return !StyleDeclaration.UNDEFINED.Equals(value);
		}

		/**
		 *  
		 *  A map of selector "subjects" to an ordered map of selector Strings and
		 *  their associated CSSStyleDeclarations.
		 *  The subject is the right most simple type selector in a potential chain
		 *  of selectors.
		 */
		private readonly Dictionary<string, List<StyleDeclaration>> _subjects = new Dictionary<string, List<StyleDeclaration>>();

		///// <summary>
		///// The list of all types processed the first time each type has been initialized
		///// </summary>
		//public List<Type> ProcessedDefaultsForTypes = new List<Type>();

		/**
		 *  
		 *  After an entire selector is added, replaced, or removed,
		 *  this method updates all the DisplayList trees.
		 */

		/**
		 *  Retrieve all style declarations applicable to this subject. The subject
		 *  is the right most simple type selector in a selector chain.
		 * 
		 *  Param: subject The subject of the style declaration's selector.
		 */ 
		///<summary>
		///</summary>
		///<param name="subject"></param>
		///<returns></returns>
		internal List<StyleDeclaration> GetStyleDeclarations(string subject) // of StyleDeclaration
		{
			//Debug.Log("SM->GetStyleDeclarations: " + subject);
			//Debug.Log("Subjects: " + _subjects.Count);

			if (_subjects.ContainsKey(subject))
				return _subjects[subject];

			//Debug.Log(3);
			return null;

			//if (null == theSubjects)
			//    return null;

			//return theSubjects.Values;
		}

		internal StyleDeclaration GetMergedStyleDeclaration(string selector)
		{
			StyleDeclaration style = GetStyleDeclaration(selector);
			//StyleDeclaration parentStyle = null;
			
			// If we have a parent, get its style and merge them with our style.
			//if (parent)
			//    parentStyle = parent.getMergedStyleDeclaration(selector);

			if (null != style) // || null != parentStyle)
			{
				style = new CSSMergedStyleDeclaration(style, /*parentStyle*/null, 
								 /*style ? */style.SelectorString/* : parentStyle.SelectorString*/, false);
			}
					
			return style;
		}

		/*public List<StyleDeclaration> GetStyleDeclarationsForModule(string moduleID)
		{
			throw new System.NotImplementedException();
		}*/

		internal string Report()
		{
			List<string> keys = new List<string>();

			StringBuilder sb = new StringBuilder();
			foreach (var key in _selectors.Keys)
			{
				keys.Add(key);
			}
			keys.Sort();

			foreach (var key in keys)
			{
				sb.AppendLine(string.Format("{0}: {1}", key, _selectors[key]));
			}

			return sb.ToString();
		}

	    /*private const bool _hasAdvancedSelectors = true; // da, mi imamo ADVANCED selektore!!! :) Ovdje je bio bug

	    /**
		 *  Determines whether any of the selectors registered with the style
		 *  manager have been advanced selectors (descendant selector, id selector,
		 *  non-global class selector, pseudo selector).
		 #1#
		public bool HasAdvancedSelectors()
		{
			if (_hasAdvancedSelectors)
				return true;

			/*if (parent)
				return parent.hasAdvancedSelectors();#1#

			return false;
		}*/
	}
}