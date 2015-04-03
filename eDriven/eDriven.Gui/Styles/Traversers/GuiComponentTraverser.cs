using System.Collections.Generic;
using eDriven.Core.Signals;
using eDriven.Gui.Components;
using eDriven.Gui.Util;
using UnityEngine;
using Component = eDriven.Gui.Components.Component;

namespace eDriven.Gui.Styles
{
	///<summary>
	/// Contains methods for traversing component trees for styling purposes
	///</summary>
	public class GuiComponentTraverser : ComponentTraverser<Component>
	{
#if DEBUG
		// ReSharper disable UnassignedField.Global
		public static bool DebugMode;
		// ReSharper restore UnassignedField.Global
#endif

		#region Singleton

		private static GuiComponentTraverser _instance;

		/// <summary>
		/// Singleton class for handling focus
		/// </summary>
		private GuiComponentTraverser()
		{
			// Constructor is protected
		}

		/// <summary>
		/// Singleton instance
		/// </summary>
		public static GuiComponentTraverser Instance
		{
			get
			{
				if (_instance == null)
				{
#if DEBUG
					if (DebugMode)
						Debug.Log(string.Format("Instantiating GuiComponentTraverser instance"));
#endif
					_instance = new GuiComponentTraverser();
					_instance.Initialize();
				}

				return _instance;
			}
		}

		#endregion

		/// <summary>
		/// Initializes the Singleton instance
		/// </summary>
		private void Initialize()
		{

		}

		private static Signal _selectorSignal;
		///<summary>
		/// Emits the list of component references that are being processed by tweaking up style sheet in play mode
		///</summary>
		public static Signal SelectorSignal
		{
			get { return _selectorSignal ?? (_selectorSignal = new Signal()); }
		}

		///<summary>
		///</summary>
		override public List<Component> GetComponentsMatchingSelector(Selector selector)
		{
			//Debug.Log("##### GetComponentsMatchingSelector #####");
			var stageList = StageManager.Instance.StageList;
			var count = stageList.Count;

			List<Component> components = new List<Component>();

			for (int i = 0; i < count; i++)
			{
				Component component = stageList[i];
				FindComps(components, component, selector);
			}

			if (0 == components.Count)
			{
#if DEBUG
				if (DebugMode)
				{
					Debug.Log("No components found.");
				}
#endif
			}
			else
			{
#if DEBUG
				if (DebugMode)
				{
					Debug.Log(string.Format(@"Found {0} components: 
{1}", components.Count, ComponentUtil.DescribeComponentList(components, true)));
				}
#endif
			}

			return components;
		}

		public override void InitStyles(Selector selector, StyleTable styles)
		{
			var components = GetComponentsMatchingSelector(selector);
			foreach (Component component in components)
			{
				// 2. for additions, set the style
				foreach (KeyValuePair<string, object> pair in styles)
				{
					component.SetStyle(pair.Key, pair.Value);
				}
			}
		}

		public override void UpdateStyles(Selector selector, DictionaryDelta delta)
		{
			var components = GetComponentsMatchingSelector(selector);

			delta.Process();

			foreach (Component component in components)
			{
				// 1. for removals, clear the style
				foreach (string removal in delta.Removals.Keys)
				{
					//Debug.Log("Removing -> " + removal);
					component.ClearStyle(removal);
				}
				// 2. for additions, set the style
				foreach (KeyValuePair<string, object> addition in delta.Additions)
				{
					//Debug.Log("Adding -> " + addition + ", " + prop.Value);
					component.SetStyle(addition.Key, addition.Value); // ?? StyleDeclaration.UNDEFINED); // StyleDeclaration.UNDEFINED 20131122
				}
				// 3. for updates, set the style
				foreach (KeyValuePair<string, object> update in delta.Updates)
				{
					//Debug.Log("Setting -> " + update.Name + ", " + update.Value);
					component.SetStyle(update.Key, update.Value);
				}
			}

			/* Signalize to GUI Editor that it should render the overlay for each component */
			if (null != _selectorSignal)
				_selectorSignal.Emit(components);
		}

		private void FindComps(ICollection<Component> components, Component component, Selector selector)
		{
			/**
			 * Note: not equal, but containing
			 * We are looking for components having this selector as a part of theirs
			 * For instance, if selector is "Button", then "Button.miki" should also be processed (but only if not overriden with .miki declaration TODO)
			 * */
			if (MatchesSelector(component, selector))
				components.Add(component);

			foreach (DisplayListMember child in component.Children)
			{
				Component c = child as Component;
				FindComps(components, c, selector);
			}
		}

		///<summary>
		/// Returns true if the component matches the selector
		///</summary>
		///<param name="component"></param>
		///<param name="selector"></param>
		///<returns></returns>
		private bool MatchesSelector(IStyleClient component, Selector selector)
		{
			var declaration = StyleManager.Instance.GetStyleDeclaration(selector.ToString()); // s(component.GetType().FullName);
			//Debug.Log("declaration: " + declaration);

			//var declarations = StyleManager.Instance.GetStyleDeclarations(selector.ToString()); // s(component.GetType().FullName);
			//if (null != declarations)
			//    Debug.Log("Declarations for " + component.GetType().FullName  + ": " + declarations.Count);

		    if (null == declaration)
		        return false;

			if (declaration.MatchesStyleClient(component))
			{
				//Debug.Log("Match: " + component);
				return true;
			}

			return false;
		}
	}
}
