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

using eDriven.Gui.Editor.Dialogs.Commands;
using eDriven.Gui.Editor.Styles;
using eDriven.Gui.Geom;
using eDriven.Gui.Styles;
using eDriven.Gui.Styles.MediaQueries;
using eDriven.Gui.Styles.Serialization;
using eDriven.Gui.Util;
using UnityEditor;
using UnityEngine;
using StyleDeclaration = eDriven.Gui.Styles.Serialization.StyleDeclaration;

namespace eDriven.Gui.Editor.Rendering
{
	[CustomPropertyDrawer(typeof(StyleDeclaration))]
// ReSharper disable UnusedMember.Global
	internal class StyleDeclarationPropertyDrawer : PropertyDrawer
// ReSharper restore UnusedMember.Global
	{
		private const float ButtonWidth = 20f, ButtonHeight = 15f, ButtonMargin = 1f, VerticalGap = 2f;

		private const int NumberOfButtons = 3; // 5

		public static float ButtonGroupWidth
		{
			get
			{
				return ButtonWidth * NumberOfButtons + ButtonMargin * (NumberOfButtons-1);
			}
		}

		private static readonly EdgeMetrics BorderMetrics = new EdgeMetrics(4, 4, 4, 4);

		public static int ToolbarHeight = 20;

		public override float GetPropertyHeight (SerializedProperty property, GUIContent label) {

			float height = StyleDeclarationInsetRenderer.HeaderHeight;
			if (!property.isExpanded)
			{
				return height;
			}

			SerializedProperty properties = property.FindPropertyRelative("Properties");
			int numberOfProperties = properties.arraySize;

			SerializedProperty mediaQueries = property.FindPropertyRelative("MediaQueries");
			int numberOfMediaQueries = mediaQueries.arraySize;

			if (numberOfProperties == 0 && numberOfMediaQueries == 0)
			{
				return height + StyleDeclarationInsetRenderer.HeaderHeight;
			}

			for (int i = 0; i < numberOfProperties; i++)
			{
				height += EditorGUI.GetPropertyHeight(properties.GetArrayElementAtIndex(i)) + VerticalGap;
			}

			for (int i = 0; i < numberOfMediaQueries; i++)
			{
				height += EditorGUI.GetPropertyHeight(mediaQueries.GetArrayElementAtIndex(i)) + VerticalGap;
			}

			height += BorderMetrics.Top + BorderMetrics.Bottom;

			if (!eDrivenStyleSheetEditor.EditorLocked)
				height += ToolbarHeight;

			return height;
		}

		private readonly StyleDeclarationInsetRenderer _insetRenderer = new StyleDeclarationInsetRenderer();

		public static string CurrentType;

		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
		{
            position.height = StyleDeclarationInsetRenderer.HeaderHeight;

			SerializedProperty module = property.FindPropertyRelative("Module");
			_insetRenderer.Error = string.IsNullOrEmpty(module.stringValue) ? "Style module not defined" : null;
            
			SerializedProperty type = property.FindPropertyRelative("Type");
			SerializedProperty classname = property.FindPropertyRelative("Class");
			SerializedProperty id = property.FindPropertyRelative("Id");

			var selectorString = StyleSelector.BuildString(type.stringValue, classname.stringValue, id.stringValue);

			var title = selectorString;

			SerializedProperty mediaQueries = property.FindPropertyRelative("MediaQueries");
			int size = mediaQueries.arraySize;
			if (size > 0)
			{
				/*var mkList = new System.Collections.Generic.List<string>();
				for (int i = 0; i < size; i++)
				{
					var query = mediaQueries.GetArrayElementAtIndex(i).FindPropertyRelative("Name").stringValue;
					mkList.Add(query);
				}*/
				//title += string.Format(" @media {0}", string.Join(", ", mkList.ToArray()));
                title += string.Format(" @media");
			}

            var isScanning = EditorSettings.LiveStyling && Application.isPlaying;

		    var passed = 0 == size || !isScanning;
            if (!passed)
            {
                // let's assume it will pass
                passed = true;

                // loop thgough each query
		        for (int i = 0; i < size; i++)
		        {
		            var query = mediaQueries.GetArrayElementAtIndex(i);
		            var name = query.FindPropertyRelative("Name").stringValue;
                    var value = SerializedPropertyHelper.Read(query); //Debug.Log("value: " + value);
                    passed = MediaQueryManager.Instance.EvaluateQuery(name, value);

                    // when a single query doesn't pass, break the loop
                    if (!passed)
		                break;
		        }
		        //GUI.backgroundColor = mediaQueryPasses ? Color.green : Color.red;
		    }

            _insetRenderer.MediaQueriesPassed = passed;
            
			CurrentType = type.stringValue;

			Rect pos2 = new Rect(position.x, position.y, position.width, property.isExpanded ? GetPropertyHeight(property, label) : position.height);

			/*label = */EditorGUI.BeginProperty(position, label, property);

			property.isExpanded = _insetRenderer.RenderStart(pos2, title, property.isExpanded);

			EditorGUI.EndProperty();
			
			if (!property.isExpanded)
			{
				return;
			}

			position.y += StyleDeclarationInsetRenderer.HeaderHeight;

			if (!eDrivenStyleSheetEditor.EditorLocked)
				position.y += ToolbarHeight;

			position.width -= BorderMetrics.Right;

			//EditorGUI.indentLevel += 1;
			position.x += BorderMetrics.Left;
				
			/**
			 * 1. Render media queries
			 * */
			var numberOfMediaQueries = RenderMediaQueries(ref position, property);

			/**
			 * 2. Render properties
			 * */
			var numberOfProperties = RenderProperties(position, property);

			/*if (Event.current.type == EventType.ValidateCommand)
			{
				Debug.Log(Event.current.type);
			}*/

			/*var isUndo = Event.current.type == EventType.ValidateCommand &&
						  Event.current.commandName == "UndoRedoPerformed";*/

			if (GUI.changed/* || isUndo*/)
			{
				eDrivenStyleSheet edss = (eDrivenStyleSheet)property.serializedObject.targetObject;
				StyleSheet ss = edss.StyleSheet;

				StyleDeclaration declaration = null;
				foreach (StyleDeclaration dec in ss.Declarations)
				{
					//Debug.Log(StyleSelector.BuildString(dec.Type, dec.Class, dec.Id));
					/* Note: this is buggy, think about how to reference it without using the selector */
					if (StyleSelector.BuildString(dec.Type, dec.Class, dec.Id) == selectorString)
					{
						//Debug.Log("Found declaration: " + dec);
						declaration = dec;
						break;
					}
				}

				if (null == declaration)
					return; // nothing found?

				/**
				 * 1. Get old properties
				 * */
				DictionaryDelta propertiesDelta = new DictionaryDelta();
				propertiesDelta.SnapshotBefore(declaration.Properties);

				/**
				 * 2. Apply changes
				 * */
				var propertiesChanged = property.serializedObject.ApplyModifiedProperties();

				/**
				 * 3. Get new properties
				 * */
				propertiesDelta.SnapshotAfter(declaration.Properties);

				/**
				 * 4. Process delta
				 * */
				propertiesDelta.Process();

				/**
				 * 1. Get old media queries
				 * */
				DictionaryDelta mediaQueriesDelta = new DictionaryDelta();
				mediaQueriesDelta.SnapshotBefore(declaration.MediaQueries);

				/**
				 * 2. Apply changes
				 * */
				var mediaQueriesChanged = property.serializedObject.ApplyModifiedProperties();

				/**
				 * 3. Get new properties
				 * */
				mediaQueriesDelta.SnapshotAfter(declaration.MediaQueries);

				/**
				 * 4. Process delta
				 * */
				propertiesDelta.Process();

				var selector = Selector.BuildSelector(type.stringValue, classname.stringValue, id.stringValue);

				var propertyCountChanged = _oldNumberOfProperties != numberOfProperties;
				var mediaQueryCountChanged = _oldNumberOfMediaQueries != numberOfMediaQueries;
				
				if (propertiesChanged || mediaQueriesChanged || propertyCountChanged || mediaQueryCountChanged)
				{
					var moduleId = property.FindPropertyRelative("Module").stringValue;
					if (string.IsNullOrEmpty(moduleId))
					{
						Debug.Log("Module not defined (unknown module ID)");
					}
					else
					{
						StyleModuleManager.Instance.GetModule(moduleId).UpdateStyles(selector, propertiesDelta);
					}

					if (propertyCountChanged)
					{
						_oldNumberOfProperties = numberOfProperties;
					}
					if (mediaQueryCountChanged)
					{
						_oldNumberOfMediaQueries = numberOfMediaQueries;
                    }

                    StyleSheetPropertyDrawer.ShouldProcessStyles = true;
				}

				GUI.changed = false;
			}
		}

		private int RenderMediaQueries(ref Rect position, SerializedProperty property)
		{
			SerializedProperty mediaQueries = property.FindPropertyRelative("MediaQueries");
			if (null == mediaQueries)
				return 0;

			int size = mediaQueries.arraySize;

			//GUI.changed = false; // do not propagate expand/collapse

			if (size > 0)
			{
				//position.width -= ButtonWidth * 3 + ButtonMargin * 2 + 4;
				position.y += BorderMetrics.Top;
				ShowMediaQueries(ref position, property);
			}
			return size;
		}

		private int RenderProperties(Rect position, SerializedProperty property)
		{
			SerializedProperty properties = property.FindPropertyRelative("Properties");

			int size = properties.arraySize;

			//GUI.changed = false; // do not propagate expand/collapse

			if (size > 0)
			{
				//position.width -= ButtonWidth * 3 + ButtonMargin * 2 + 4;
				position.y += BorderMetrics.Top;
				ShowProperties(position, property);
			}
			return size;
		}

		private static int _size;

		private int _oldNumberOfMediaQueries;
		private int _oldNumberOfProperties;
		
		private void ShowMediaQueries(ref Rect position, SerializedProperty property)
		{
			Rect buttonPosition = new Rect(position.xMax - ButtonGroupWidth, position.y, ButtonWidth, ButtonHeight);
			//buttonPosition.x +=  150 - ButtonGroupWidth - 4;
			//Debug.Log("buttonPosition: " + buttonPosition);

			SerializedProperty queries = property.FindPropertyRelative("MediaQueries");
			_size = queries.arraySize;
			//Debug.Log("properties.arraySize: " + size);

			int index = -1;

			for (int i = 0; i < _size; i++)
			{
				SerializedProperty element = queries.GetArrayElementAtIndex(i);

				position.height = EditorGUI.GetPropertyHeight(element, GUIContent.none); // todo: measure with icon?

				var elementPos = new Rect(position);
				if (!eDrivenStyleSheetEditor.EditorLocked)
					elementPos.width = position.width - ButtonGroupWidth;

				EditorGUI.PropertyField(elementPos, element, GUIContent.none, true);

				position.y += VerticalGap;

				//buttonPosition.x -= ButtonGroupWidth;
				if (!eDrivenStyleSheetEditor.EditorLocked)
				{
					if (ShowButtons(buttonPosition, queries, i))
					{
						index = i;
					}
				}

				buttonPosition.y = position.y += position.height;
			}

			if (index > -1)
				queries.DeleteArrayElementAtIndex(index);
		}

		private void ShowProperties(Rect position, SerializedProperty property) {

			Rect buttonPosition = new Rect(position.xMax - ButtonGroupWidth, position.y, ButtonWidth, ButtonHeight);
			//buttonPosition.x +=  150 - ButtonGroupWidth - 4;
			//Debug.Log("buttonPosition: " + buttonPosition);

			SerializedProperty properties = property.FindPropertyRelative("Properties");
			_size = properties.arraySize;
			//Debug.Log("properties.arraySize: " + size);

			int index = -1;

			for (int i = 0; i < _size; i++)
			{
				SerializedProperty element = properties.GetArrayElementAtIndex(i);

				position.height = EditorGUI.GetPropertyHeight(element, GUIContent.none);

				var elementPos = new Rect(position);
				if (!eDrivenStyleSheetEditor.EditorLocked)
					elementPos.width = position.width - ButtonGroupWidth;

				EditorGUI.PropertyField(elementPos, element, GUIContent.none, true);

				position.y += VerticalGap;

				//buttonPosition.x -= ButtonGroupWidth;
				if (!eDrivenStyleSheetEditor.EditorLocked)
				{
					if (ShowButtons(buttonPosition, properties, i))
					{
						index = i;
					}    
				}
				
				//Debug.Log("position: " + i + ": " + position);
				buttonPosition.y = position.y += position.height;
			}

			if (index > -1)
				properties.DeleteArrayElementAtIndex(index);
		}

		private static bool ShowButtons(Rect position, SerializedProperty property, int index)
		{
			var result = false;

			var pos = new Rect(position);
			pos.y -= 4;

			if (GUI.Button(pos, GuiContentCache.Instance.Delete, StyleCache.Instance.ImageOnlyNoFrameButton)) // EditorStyles.miniButtonRight
			{
				result = true; // removed //property.DeleteArrayElementAtIndex(index);
			}

			// move buttons

			if (index == 0)
				GUI.enabled = false;

			pos.x += ButtonWidth;
			if (GUI.Button(pos, GuiContentCache.Instance.MoveUp, StyleCache.Instance.ImageOnlyNoFrameButton)) // EditorStyles.miniButtonLeft
			{
				if (index > 0)
				{
					bool expanded1 = property.GetArrayElementAtIndex(index).isExpanded;
					bool expanded2 = property.GetArrayElementAtIndex(index - 1).isExpanded;
					property.MoveArrayElement(index, index - 1);
					property.GetArrayElementAtIndex(index).isExpanded = expanded1;
					property.GetArrayElementAtIndex(index - 1).isExpanded = expanded2;
				}
			}
			pos.x += ButtonWidth;

			if (index == 0)
				GUI.enabled = true;

			if (index == _size - 1)
				GUI.enabled = false;

			if (GUI.Button(pos, GuiContentCache.Instance.MoveDown, StyleCache.Instance.ImageOnlyNoFrameButton)) // EditorStyles.miniButtonLeft
			{
				bool expanded1 = property.GetArrayElementAtIndex(index).isExpanded;
				bool expanded2 = property.GetArrayElementAtIndex(index + 1).isExpanded;
				property.MoveArrayElement(index, index + 1);
				property.GetArrayElementAtIndex(index).isExpanded = expanded1;
				property.GetArrayElementAtIndex(index + 1).isExpanded = expanded2;
			}

			if (index == _size - 1)
				GUI.enabled = true;

			pos.x += ButtonWidth;

			//GUI.color = _oldColor;

			return result;
		}
	}
}