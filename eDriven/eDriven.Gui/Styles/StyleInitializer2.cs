/*
using System.Collections.Generic;
using eDriven.Gui.Styles.MediaQueries;
using UnityEngine;

namespace eDriven.Gui.Styles
{
	/// <summary>
	/// Loads styles from component metadata<br/>
	/// Uses reflection to read component metadata<br/>
	/// This should run only once - on application start<br/>
	/// It should not reload on scene change, because there's no change in assembly values<br/>
	/// It's currently being called by StageManager during its initialization 
	/// </summary>
	internal static class StyleInitializer2
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
		/// Style initialization should be run only once per level,
		/// so keeping the index of the last loaded level
		/// </summary>
		private static int _levelId = -1;

		private static bool _runOnce;

		/// <summary>
		/// Runs the style initialization
		/// </summary>
		internal static void Run()
		{
            var shouldRunMediaQueries = Settings.Instance.LiveMediaQueries &&
                                        MediaQueryManager.Instance.Queries.Count > 0;

            /**
             * Style initialization should be run once per level in play mode
             * However, if media queries present and enabled, it should be run on each application resize
             * Having a hard-lock here disabling the "non-media-query" app to initialize multiple times
             * #1#
            if (!shouldRunMediaQueries && !Application.isEditor && Application.loadedLevel == _levelId)
                return; // it should be run once per level, but only if no media queries present

			/*if (Application.loadedLevel == _levelId)
				return; // it should be run once per level#1#

			// this is a new level
			_levelId = Application.loadedLevel;

#if DEBUG
			if (DebugMode)
			{
				Debug.Log(string.Format("StyleInitializer2->Run"));
			}
#endif
			// 1. load styles from stylesheet
			var declarations = StyleSheetProcessor2.Load();

			// 2. apply styles to components
			if (null != declarations)
			{
				foreach (StyleDeclaration declaration in declarations)
				{
					var selector = CSSSelector.BuildSelector(declaration.Type, declaration.Class, declaration.Id);
					var dict = declaration.ToStyleTable();

					if (true /*!Application.isPlaying && moduleDescriptor.ProcessEditModeChanges ||
						Application.isPlaying && moduleDescriptor.ProcessPlayModeChanges#1#)
					{
						UnityComponentTraverser.Instance.InitStyles(selector, dict);
					}
				}
			}

			_runOnce = true;

#if DEBUG
			if (DebugMode)
			{
				Debug.Log("Styles initialized");
			}
#endif
		}
	}
}
*/
