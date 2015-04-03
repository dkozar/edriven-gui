using System.Collections.Generic;

namespace eDriven.Gui.Styles
{
	internal class UnityComponentStyleApplier : IStyleApplyer
	{
// ReSharper disable once InconsistentNaming
		public const string MODULE_ID = "unity";

		readonly List<Selector> _selectors = new List<Selector>();

		public void Apply()
		{
			/**
			 * Style declaration cache should by now have all the declarations loaded
			 * We really don't need raw style declarations, because dealing with CSSStyleDeclarations here
			 * What we need is the selector for each declaration
			 * */
			List<Serialization.StyleDeclaration> declarations = StyleDeclarationCache.Instance.GetDeclarations(MODULE_ID);

		    if (null == declarations)
		        return;

			// we only need selectors, and don't want the duplication of the same selectors
			_selectors.Clear();

			foreach (Serialization.StyleDeclaration declaration in declarations)
			{

                //StyleProtoChain.GetClassStyleDeclarations(declaration)

				var selector = Selector.BuildSelector(declaration.Type, declaration.Class, declaration.Id);
				var exists = _selectors.Exists(delegate(Selector s)
				{
					return s.ToString() == selector.ToString();
				});

				if (!exists) // avoid duplication
					_selectors.Add(selector);

				/*if (true /*!Application.isPlaying && moduleDescriptor.ProcessEditModeChanges ||
						Application.isPlaying && moduleDescriptor.ProcessPlayModeChanges#1#)
				{
					UnityComponentTraverser.Instance.InitStyles(selector, dict);
				}*/
			}

/*			Debug.Log(@"Selectors:
" + ListUtil<Selector>.Format(_selectors));*/

			foreach (var selector in _selectors)
			{
				//var dict = declaration.ToStyleTable();
				var dict = StyleManager.Instance.GetStyleDeclaration(selector.ToString());
				if (null == dict)
					continue; // didn't pass the media query

				/*Debug.Log("    - pass");*/

				UnityComponentTraverser.Instance.InitStyles(selector, dict.CreateProtoChainRoot(false)); // do not register clones
			}
		}

		///<summary>
		///</summary>
		///<returns></returns>
		/*public List<StyleDeclaration> GetClassStyleDeclarations()
		{
			return StyleProtoChain.GetClassStyleDeclarations(this);
		}*/
	}
}
