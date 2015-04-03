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

using UnityEditor;
using UnityEngine;

namespace eDriven.Gui.Editor.Rendering
{
    internal static class SynchronizedBlock
    {
        internal static void SyncVars(SerializedProperty left, SerializedProperty right, SerializedProperty top, SerializedProperty bottom, int value)
        {
            left.intValue = value;
            right.intValue = value;
            top.intValue = value;
            bottom.intValue = value;
        }

        internal static void Render(
            SerializedProperty syncSwitch, 
            SerializedProperty left, SerializedProperty right, SerializedProperty top, SerializedProperty bottom,
            ref int prevLeft, ref int prevRight, ref int prevTop, ref int prevBottom/*, int maximum = 1000*/)
        {
            //GUILayout.BeginHorizontal();
            
            //GUILayout.BeginVertical();
            syncSwitch.boolValue = EditorGUILayout.Toggle("Sync", syncSwitch.boolValue);

            left.intValue = EditorGUILayout.IntField("Left", left.intValue, GUILayout.ExpandWidth(false));
            right.intValue = EditorGUILayout.IntField("Right", right.intValue, GUILayout.ExpandWidth(false));
            top.intValue = EditorGUILayout.IntField("Top", top.intValue, GUILayout.ExpandWidth(false));
            bottom.intValue = EditorGUILayout.IntField("Bottom", bottom.intValue, GUILayout.ExpandWidth(false));
            //GUILayout.EndVertical();

            #region _advanced

            /*var last = GUILayoutUtility.GetLastRect();
            Debug.Log(last);
            GUILayout.BeginArea(new Rect(last.xMin, last.y, 200, 200));

            GUI.Box(new Rect(0, 0, 200, 200), string.Empty);
            
            top.intValue = EditorGUI.IntField(new Rect(80, 0, 40, 20), string.Empty, top.intValue);

            GUILayout.BeginHorizontal();
            left.intValue = EditorGUILayout.IntField(string.Empty, left.intValue, GUILayout.Width(30), GUILayout.ExpandWidth(false));

            GUILayout.Box(string.Empty);

            right.intValue = EditorGUILayout.IntField(string.Empty, right.intValue, GUILayout.Width(30), GUILayout.ExpandWidth(false));
            GUILayout.EndHorizontal();

            bottom.intValue = EditorGUILayout.IntField(string.Empty, bottom.intValue, GUILayout.Width(30), GUILayout.ExpandWidth(false));
            
            GUILayout.EndArea();*/

            #endregion

            //GUILayout.EndHorizontal();

            /*left.intValue = (int)EditorGUILayout.Slider("Left", left.intValue, 0, maximum);
            right.intValue = (int)EditorGUILayout.Slider("Right", right.intValue, 0, maximum);
            top.intValue = (int)EditorGUILayout.Slider("Top", top.intValue, 0, maximum);
            bottom.intValue = (int)EditorGUILayout.Slider("Bottom", bottom.intValue, 0, maximum);*/

            if (syncSwitch.boolValue)
            {
                if (left.intValue != prevLeft)
                {
                    prevLeft = left.intValue;
                    SyncVars(left, right, top, bottom, left.intValue);
                }
                if (right.intValue != prevRight)
                {
                    prevRight = right.intValue;
                    SyncVars(left, right, top, bottom, right.intValue);
                }
                if (top.intValue != prevTop)
                {
                    prevTop = top.intValue;
                    SyncVars(left, right, top, bottom, top.intValue);
                }
                if (bottom.intValue != prevBottom)
                {
                    prevBottom = bottom.intValue;
                    SyncVars(left, right, top, bottom, bottom.intValue);
                }
            }
        }
    }
}