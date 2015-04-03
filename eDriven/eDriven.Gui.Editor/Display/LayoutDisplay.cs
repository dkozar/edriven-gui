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

using eDriven.Gui.Designer.Adapters;
using eDriven.Gui.Editor.Display.Layout;
using eDriven.Gui.Editor.Processing;
using eDriven.Gui.Editor.Rendering;
using UnityEditor;
using UnityEngine;

namespace eDriven.Gui.Editor.Display
{
	internal class LayoutDisplay : DisplayBase
	{
#if DEBUG
// ReSharper disable UnassignedField.Global
		/// <summary>
		/// Debug mode
		/// </summary>
		public static bool DebugMode;

// ReSharper restore UnassignedField.Global
#endif

		#region Singleton

		private static LayoutDisplay _instance;

		/// <summary>
		/// Singleton class for handling focus
		/// </summary>
		private LayoutDisplay()
		{
			// Constructor is protected
		}

		/// <summary>
		/// Singleton instance
		/// </summary>
		public static LayoutDisplay Instance
		{
			get
			{
				if (_instance == null)
				{
#if DEBUG
					if (DebugMode)
						Debug.Log(string.Format("Instantiating LayoutDisplay instance"));
#endif
					_instance = new LayoutDisplay();
					_instance.Initialize();
				}

				return _instance;
			}
		}

		#endregion

		/// <summary>
		/// Initializes the Singleton instance
		/// </summary>
		public override void Initialize() {}

		//private static SerializedProperty _useLayoutDescriptor;
		//private static SerializedProperty _layoutDescriptor;
		private static SerializedProperty _layout;

		//private bool _clicked;

		private readonly AbsoluteLayoutDisplay _absoluteLayoutDisplay = new AbsoluteLayoutDisplay();
		private readonly HorizontalLayoutDisplay _horizontalLayoutDisplay = new HorizontalLayoutDisplay();
		private readonly VerticalLayoutDisplay _verticalLayoutDisplay = new VerticalLayoutDisplay();
		private readonly TileLayoutDisplay _tileLayoutDisplay = new TileLayoutDisplay();
		
		private int _oldIndex = -1;
		private int _tabIndex;

		protected override void HandleSelectionChange()
		{
			//Debug.Log("HandleSelectionChange");

			/**
			 * If not a container, nothing to do here; so return immediatelly
			 * */
			if (null == GroupAdapter)
				return;

#if DEBUG
	if (DebugMode)
			{
				Debug.Log(string.Format(@"=========== Order ({0}) =========
{1}============================", GroupAdapter.LayoutOrder.Count, GroupAdapter.DescribeLayoutOrder()));
			}
#endif

			SerializedObject serializedObject = EditorState.SerializedObject;

			if (null == serializedObject)
				return;

			_layout = serializedObject.FindProperty("Layout");

			InitLayout(serializedObject);
		}

        /// <summary>
        /// Initializes previously saved layout from the serialized object
        /// </summary>
        /// <param name="serializedObject"></param>
		private void InitLayout(SerializedObject serializedObject)
		{
			// read serialized index
			_tabIndex = null == _layout ? 0 : _layout.enumValueIndex;
			LayoutTabBar.Instance.TabIndex = _tabIndex;

			switch (_tabIndex)
			{
				case 0:
					_absoluteLayoutDisplay.Initialize(serializedObject);
					break;
				case 1:
					_horizontalLayoutDisplay.Initialize(serializedObject);
					break;
				case 2:
					_verticalLayoutDisplay.Initialize(serializedObject);
					break;
				case 3:
					_tileLayoutDisplay.Initialize(serializedObject);
					break;
				default:
					Debug.LogWarning("No layout");
					break;
			}
		}

		internal override void OnLostFocus()
		{
			//throw new NotImplementedException();
		}

		public override void Render()
		{
			/* TODO TEST PropertyDrawers in EditorWindow: */
			//if (null != MainWindow.SerializedObject)
			//{
			//    var prop = MainWindow.SerializedObject.FindProperty("StyleSheet");
			//    EditorGUILayout.PropertyField(prop);
			//}

			// mode switch
			EditorGUILayout.BeginHorizontal(StyleCache.Instance.Toolbar);

			var oldEnabled = GUI.enabled;
			GUI.enabled = CheckSelection(true, false); // must be container, but don't render

			LayoutTabBar.Instance.Render();
			_tabIndex = LayoutTabBar.Instance.TabIndex;
			if (_tabIndex != _oldIndex)
			{
				_oldIndex = _tabIndex;
				LayoutTabBar.Instance.TabIndex = _oldIndex;
				//Debug.Log("index changed to " + _tabIndex);
				/*if (null != EditorState.SerializedObject)
					InitLayout(EditorState.SerializedObject);*/
				if (null != _layout && null != _layout.serializedObject)
				{
					_layout.enumValueIndex = _tabIndex;
                    InitLayout(EditorState.SerializedObject);
				}
			}

			GUI.enabled = oldEnabled;

			EditorGUILayout.EndHorizontal();

			if (!CheckSelection(true))
				return;

			if (null != EditorState.SerializedObject)
			{
				GUILayout.Space(3);

				try
				{
					if (null == _layout)
						return;

					switch (_layout.enumValueIndex)
					{
						case 0:
							_absoluteLayoutDisplay.Render();
							break;
						case 1:
							_horizontalLayoutDisplay.Render();
							break;
						case 2:
							_verticalLayoutDisplay.Render();
							break;
						case 3:
							_tileLayoutDisplay.Render();
							break;
						default:
							GUILayout.Label("Unknown layout");
							break;
					}
				}
// ReSharper disable EmptyGeneralCatchClause
				catch
// ReSharper restore EmptyGeneralCatchClause
				{/* Note: Silent fail for "NullReferenceException: SerializedObject of SerializedProperty has been Disposed." */}
			}

			if (null != EditorState.SerializedObject)
				EditorState.SerializedObject.ApplyModifiedProperties();

			if (GUI.changed)
			{
#if DEBUG
				if (DebugMode)
				{
					Debug.Log("GUI.changed: " + GUI.changed);
				}
#endif
				if (null != Target)
				{
					var t = (ComponentAdapter)Target;
					if (null != t.Component)
						t.Apply(t.Component);
				}
			}
		}

		public override void Update()
		{
			
		}
	}
}