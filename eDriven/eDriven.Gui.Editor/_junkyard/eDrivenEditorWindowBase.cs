//#region Copyright

///*
 
//Copyright (c) Danko Kozar 2012. All rights reserved.
 
//*/

//#endregion Copyright

//using System.Reflection;
//using UnityEditor;

//namespace eDriven.Gui.Editor
//{
//    [Obfuscation(Exclude = true)]
//    public abstract class eDrivenEditorWindowBase //: EditorWindow
//    {
//        public static SerializedObject SerializedObject;
//        //public static Object Target;

//        //protected static ComponentAdapter Adapter;
//        //protected static ContainerAdapter ContainerAdapter;

//        //// ReSharper disable UnusedMember.Local
//        //void OnSelectionChange()
//        //    // ReSharper restore UnusedMember.Local
//        //{
//        //    //Debug.Log("ProcessSelectionChange: " + Selection.activeObject);
//        //    if (null == Selection.activeGameObject)
//        //        return;

//        //    GameObject go = Selection.activeGameObject;

//        //    if (null == go)
//        //        throw new Exception("Couldn't get the selection");

//        //    Adapter = go.GetComponent(typeof(ComponentAdapter)) as ComponentAdapter;
//        //    if (null == Adapter)
//        //    {
//        //        HandleSelectionChange();
//        //        return; // not a GUI component
//        //        //throw new Exception("Adapter is null");
//        //    }
//        //    Target = Adapter;
//        //    ContainerAdapter = Target as ContainerAdapter;
            
//        //    HandleSelectionChange();
//        //}

//        //protected abstract void HandleSelectionChange();

//        //protected static bool CheckSelection(bool mustBeContainer)
//        //{
//        //    if (null == Selection.activeTransform)
//        //    {
//        //        GUILayout.Label(GuiContentCache.Instance.NoSelectionContent, StyleCache.Instance.WrongSelection, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
//        //        return false;
//        //    }

//        //    Adapter = GuiLookup.GetAdapter(Selection.activeTransform);
//        //    ContainerAdapter = Adapter as ContainerAdapter;

//        //    if (null == Adapter || null == SerializedObject)
//        //    {
//        //        GUILayout.Label(GuiContentCache.Instance.NotEDrivenComponentContent, StyleCache.Instance.WrongSelection, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
//        //        return false;
//        //    }

//        //    if (mustBeContainer && null == ContainerAdapter)
//        //    {
//        //        GUILayout.Label(GuiContentCache.Instance.NotAContainerContent, StyleCache.Instance.WrongSelection, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
//        //        return false;
//        //    }

//        //    return true;
//        //}
//    }
//}