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

using eDriven.Gui.Editor.Dialogs;
using eDriven.Gui.Editor.Processing;
using UnityEditor;
using UnityEngine;

namespace eDriven.Gui.Editor.Rendering
{
	internal class OptionsBox
	{
#if DEBUG
		// ReSharper disable UnassignedField.Global
		public static bool DebugMode;
		// ReSharper restore UnassignedField.Global
#endif

		#region Singleton

		private static OptionsBox _instance;

		/// <summary>
		/// Singleton class for handling focus
		/// </summary>
		private OptionsBox()
		{
			// Constructor is protected
		}

		/// <summary>
		/// Singleton instance
		/// </summary>
		public static OptionsBox Instance
		{
			get
			{
				if (_instance == null)
				{
#if DEBUG
					if (DebugMode)
						Debug.Log(string.Format("Instantiating OptionsBox instance"));
#endif
					_instance = new OptionsBox();
					Initialize();
				}

				return _instance;
			}
		}

		#endregion

		/// <summary>
		/// Initializes the Singleton instance
		/// </summary>
		private static void Initialize()
		{

		}

		private Vector2 _scrollPosition;

		//private bool _useDarkSkin;
		//private bool _oldUseDarkSkin;

		private readonly TabBarRenderer _skinTabBarRenderer = new TabBarRenderer();
		private readonly TabBarRenderer _checkForUpdatesRenderer = new TabBarRenderer();
		private readonly TabBarRenderer _updateCheckPeriodRenderer = new TabBarRenderer();

		private bool _showCheckUpdatesHelp;

		private bool _skinExpanded = true;
		private bool _updatesExpanded = true;
        private bool _cleanupExpanded = true;

		internal void Render()
		{
			_scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.ExpandWidth(true));

			_skinExpanded = EditorGUILayout.Foldout(_skinExpanded, "Skin");

			if (_skinExpanded)
			{
				GUILayout.BeginVertical(StyleCache.Instance.Fieldset);

				//GUILayout.Label("Skin", EditorStyles.largeLabel, GUILayout.ExpandWidth(true));

				// 1. skin
				RenderSkinOption();

				GUILayout.EndVertical();
			}

			GUILayout.Space(4);

			_updatesExpanded = EditorGUILayout.Foldout(_updatesExpanded, "Updates");

			if (_updatesExpanded)
			{
				GUILayout.BeginVertical(StyleCache.Instance.Fieldset);

				//GUILayout.Label("Updates", EditorStyles.largeLabel, GUILayout.ExpandWidth(true));

				// 2. check for updates
				RenderCheckForUpdatesOption();

				// 3. update period
				if (EditorSettings.CheckForUpdates)
				{
					// label
					RenderUpdateCheckPeriod();
				}

				// 4. check for update now
				RenderCheckForUpdateNow();

				GUILayout.EndVertical();
			}

            _cleanupExpanded = EditorGUILayout.Foldout(_cleanupExpanded, "Cleanup");

            if (_cleanupExpanded)
            {
                GUILayout.BeginVertical(StyleCache.Instance.Fieldset);

                //GUILayout.Label("Updates", EditorStyles.largeLabel, GUILayout.ExpandWidth(true));

                // 2. check for updates
                RenderRemoveGhostObjectsButton();

                GUILayout.EndVertical();
            }

			GUILayout.FlexibleSpace();

			GUILayout.EndScrollView();

			GUILayout.FlexibleSpace();

			GUILayout.Space(1);

			if (UpdateCheck.Instance.DataReady)
				UpdateCheck.Instance.ShowDialog();
		}

		private void RenderSkinOption()
		{
			GUILayout.BeginHorizontal(GUILayout.ExpandHeight(false));

			// label
			EditorGUILayout.BeginVertical(GUILayout.Height(39));
			GUILayout.FlexibleSpace();

			GUILayout.Label("Skin:", StyleCache.Instance.Label);

			GUILayout.FlexibleSpace();
			EditorGUILayout.EndVertical();

			// tabs
			if (_skinTabBarRenderer.Tabs.Count == 0)
			{
				// tab style
				_skinTabBarRenderer.TabStyle = StyleCache.Instance.Toggle; // GreenToggle;

				// tabs
				_skinTabBarRenderer.Tabs.Add(
					new TabDescriptor(
						new GUIContent("Light", TextureCache.Instance.LightBulbOn),
						new GUIContent("Light", TextureCache.Instance.LightBulbOn)
						)
					);
				_skinTabBarRenderer.Tabs.Add(
					new TabDescriptor(
						new GUIContent("Dark", TextureCache.Instance.LightBulbOff),
						new GUIContent("Dark", TextureCache.Instance.LightBulbOff)
						)
					);
			}

			_skinTabBarRenderer.TabIndex = EditorSettings.UseDarkSkin ? 1 : 0;

			_skinTabBarRenderer.Render();

			if (_skinTabBarRenderer.Changed)
			{
				EditorSettings.UseDarkSkin = _skinTabBarRenderer.TabIndex == 1;
				StyleCache.Reset();
				TextureCache.Reset();
				TabBarRenderer.ResetStyles();
				UpdateTabStyles();
				EditorState.Instance.ThemeChanged();
			}

			GUILayout.EndHorizontal();

			//GUILayout.Space(4);
		}
		
		private void RenderCheckForUpdatesOption()
		{
			GUILayout.BeginHorizontal(GUILayout.ExpandHeight(false));

			// label
			EditorGUILayout.BeginVertical(GUILayout.Height(39));
			GUILayout.FlexibleSpace();

			GUILayout.Label("Automatically check for updates:", StyleCache.Instance.Label);

			GUILayout.FlexibleSpace();
			EditorGUILayout.EndVertical();

			// tabs
			if (_checkForUpdatesRenderer.Tabs.Count == 0)
			{
				// tab style
				_checkForUpdatesRenderer.TabStyle = StyleCache.Instance.Toggle;

				// tabs
				_checkForUpdatesRenderer.Tabs.Add(
					new TabDescriptor(
						new GUIContent("Off", TextureCache.Instance.Off),
						new GUIContent("Off", TextureCache.Instance.Off)
						)
					);
				_checkForUpdatesRenderer.Tabs.Add(
					new TabDescriptor(
						new GUIContent("On", TextureCache.Instance.On),
						new GUIContent("On", TextureCache.Instance.On)
						)
					);
			}

#if RELEASE
			EditorSettings.CheckForUpdates = true;
			GUI.enabled = false;
#endif
			_checkForUpdatesRenderer.TabIndex = EditorSettings.CheckForUpdates ? 1 : 0;
			_checkForUpdatesRenderer.Render();

#if RELEASE
			GUI.enabled = true;
#endif

			if (_checkForUpdatesRenderer.Changed)
			{
				EditorSettings.CheckForUpdates = _checkForUpdatesRenderer.TabIndex == 1;
			}

			// help button
			EditorGUILayout.BeginVertical(GUILayout.Height(39));
			GUILayout.FlexibleSpace();

			_showCheckUpdatesHelp = GUILayout.Toggle(_showCheckUpdatesHelp, TextureCache.Instance.Help, StyleCache.Instance.ImageOnlyNoFrameButton, GUILayout.ExpandWidth(false));
			
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndVertical();

			GUILayout.EndHorizontal();

			if (_showCheckUpdatesHelp)
			{
				EditorGUILayout.HelpBox(Help.CheckForUpdates, MessageType.Info, true);
			}

			//GUILayout.Space(4);
		}

		private void RenderUpdateCheckPeriod()
		{
			EditorGUILayout.BeginVertical(GUILayout.Height(30));
			GUILayout.FlexibleSpace();

			GUILayout.Label("Check for update every:", StyleCache.Instance.Label);

			GUILayout.FlexibleSpace();
			EditorGUILayout.EndVertical();

			GUILayout.BeginHorizontal(GUILayout.ExpandHeight(false));

			// tabs
			if (_updateCheckPeriodRenderer.Tabs.Count == 0)
			{
				// tab style
				_updateCheckPeriodRenderer.TabStyle = StyleCache.Instance.Toggle;

				// tabs
				_updateCheckPeriodRenderer.Tabs.Add(
					new TabDescriptor(
						new GUIContent("1h"),
						new GUIContent("1h")
					)
				);
				_updateCheckPeriodRenderer.Tabs.Add(
					new TabDescriptor(
						new GUIContent("3h"),
						new GUIContent("3h")
						)
					);
				_updateCheckPeriodRenderer.Tabs.Add(
					new TabDescriptor(
						new GUIContent("6h"),
						new GUIContent("6h")
					)
				);
				_updateCheckPeriodRenderer.Tabs.Add(
					new TabDescriptor(
						new GUIContent("12h"),
						new GUIContent("12h")
						)
					);
				_updateCheckPeriodRenderer.Tabs.Add(
					new TabDescriptor(
						new GUIContent("1d"),
						new GUIContent("1d")
						)
					);
				_updateCheckPeriodRenderer.Tabs.Add(
					new TabDescriptor(
						new GUIContent("3d"),
						new GUIContent("3d")
						)
					);
				_updateCheckPeriodRenderer.Tabs.Add(
					new TabDescriptor(
						new GUIContent("1w"),
						new GUIContent("1w")
						)
					);
				_updateCheckPeriodRenderer.Tabs.Add(
					new TabDescriptor(
						new GUIContent("2w"),
						new GUIContent("2w")
						)
					);
				_updateCheckPeriodRenderer.Tabs.Add(
					new TabDescriptor(
						new GUIContent("1m"),
						new GUIContent("1m")
						)
					);
				_updateCheckPeriodRenderer.Tabs.Add(
					new TabDescriptor(
						new GUIContent("3m"),
						new GUIContent("3m")
						)
					);
			}

#if RELEASE
				EditorSettings.CheckForUpdates = true;
				GUI.enabled = false;
#endif
			_updateCheckPeriodRenderer.TabIndex = EditorSettings.UpdateCheckPeriod;
			_updateCheckPeriodRenderer.Render();

#if RELEASE
				GUI.enabled = true;
#endif

			if (_updateCheckPeriodRenderer.Changed)
			{
				EditorSettings.UpdateCheckPeriod = _updateCheckPeriodRenderer.TabIndex;
			}

			GUILayout.EndHorizontal();

			//GUILayout.Space(4);
		}

		private static void RenderCheckForUpdateNow()
		{
			GUILayout.BeginHorizontal(GUILayout.ExpandHeight(false));

			// button
			EditorGUILayout.BeginVertical(GUILayout.Height(39));
			GUILayout.FlexibleSpace();

			bool clicked = GUILayout.Button(GuiContentCache.Instance.CheckForUpdatesButton, StyleCache.Instance.ControlButton, GUILayout.ExpandWidth(false));

			GUILayout.FlexibleSpace();
			EditorGUILayout.EndVertical();

			GUILayout.EndHorizontal();

			if (clicked)
				UpdateCheck.Instance.Run(false, false, false); // 1) do not run the "needed" check, 2) show dialog even if there are no updates
		}

        private static void RenderRemoveGhostObjectsButton()
        {
            GUILayout.BeginHorizontal(GUILayout.ExpandHeight(false));

            // button
            EditorGUILayout.BeginVertical(GUILayout.Height(39));
            GUILayout.FlexibleSpace();

            bool clicked = GUILayout.Button(GuiContentCache.Instance.RemoveGhostObjectsButton, StyleCache.Instance.ControlButton, GUILayout.ExpandWidth(false));

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();

            GUILayout.EndHorizontal();

            if (clicked)
                OptionsDialog.RemoveFrameworkObjects();
        }

		private void UpdateTabStyles()
		{
			_skinTabBarRenderer.TabStyle = StyleCache.Instance.Toggle; // GreenToggle; // reset style
			_checkForUpdatesRenderer.TabStyle = StyleCache.Instance.Toggle; // reset style
			_updateCheckPeriodRenderer.TabStyle = StyleCache.Instance.Toggle; // reset style
		}
	}
}