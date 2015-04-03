//using System;
//using System.Collections.Generic;
//using System.Reflection;
//using System.Text;
//using eDriven.Gui.Editor.Reflection;
//using eDriven.Gui.Styles;
//using UnityEditor;
//using UnityEngine;

//[CustomEditor(typeof(SkinStyleMapper))]
//[CanEditMultipleObjects]
//[Obfuscation(Exclude = true)]
//public class SkinStyleMapperEditor : Editor
//{
//    #region Available skins

//    private List<string> _availableSkinClasses = new List<string>();
//    private int _skinIndex;
//    private int _tempIndex;
//    private bool _cannotFind;
//    private string _chosenSkinClassName;

//    void Awake()
//    {
//        InitSkinClasses();
//    }

//    public override void OnInspectorGUI()
//    {
//        RenderSkinClassBlock();

//        base.OnInspectorGUI();
//    }

//    private void InitSkinClasses()
//    {
//        Dictionary<string, Type> skins = new Dictionary<string, Type>();

//        //Debug.Log("FindSelectedStyleMapperIndex: " + value);
//        //var skin = adapter.SkinClass;
//        eDriven.Gui.Reflection.ReflectionUtil.GetAllStyleableClasses(ref skins);
//#if DEBUG
//        if (true)
//        {
//            StringBuilder sb = new StringBuilder();
//            if (skins.Count == 0)
//            {
//                sb.AppendLine("No available skins.");
//            }
//            else
//            {
//                foreach (KeyValuePair<string, Type> pair in skins)
//                {
//                    sb.AppendLine(string.Format("    {0} -> {1}", pair.Key, pair.Value));
//                }
//            }

////            Debug.Log(string.Format(@"====== Skins ======
////{0}", sb));
//        }
//#endif
//        _availableSkinClasses = new List<string>();
//        //if (_couldNotLocateMapper)
//        //    list.Add("=== Not found ===");
//        //list.Add("= Default =");
//        foreach (string skinClass in skins.Keys)
//        {
//            _availableSkinClasses.Add(skinClass);
//        }

//        //Debug.Log("_availableSkinClasses: " + _availableSkinClasses.Count);
//    }

//    private void RenderSkinClassBlock()
//    {
//        //Debug.Log(string.Format("value1: {0}", value));

//        EditorGUILayout.BeginHorizontal();

//        string[] skins = _availableSkinClasses.ToArray();

//        var oldEnabled = GUI.enabled;

//        _tempIndex = EditorGUILayout.Popup(
//            "Skin class",
//            _skinIndex,
//            skins
//        );

//        GUI.enabled = oldEnabled;

//        bool changed = false;
//        if (_tempIndex != _skinIndex)
//        {
//            _cannotFind = false;
//            _skinIndex = _tempIndex;
//            changed = true;
//            _chosenSkinClassName = _availableSkinClasses[_skinIndex];
//            Debug.Log(string.Format("Change: [index: {0}, value: {1}]", _skinIndex, _chosenSkinClassName));
//            ProcessSelectedSkin(_chosenSkinClassName);
//        }

//        GUILayout.Space(4);

//        //GUILayout.Label(string.Empty, GUILayout.ExpandWidth(false), GUILayout.Width(40));

//        EditorGUILayout.EndHorizontal();

//        if (_cannotFind)
//        {
//            EditorGUILayout.HelpBox("Couldn't locate skin class. Using the default class instead.", MessageType.Warning, true);
//            //value = string.Empty;
//        }

//        if (changed)
//        {
//            //Debug.Log("_chosenSkinClassName: " + _chosenSkinClassName);
//            //SkinClass.stringValue = _chosenSkinClassName;    
//        }
//    }

//    private void ProcessSelectedSkin(string s)
//    {
//        Type type = ReflectionUtil.GetTypeByFullName(s);
//        //Debug.Log("type: " + type);

//        var attributes = ReflectionUtil.GetStyleAttributes(s, true);

//        StringBuilder sb = new StringBuilder();
//        foreach (KeyValuePair<string, StyleAttribute> pair in attributes)
//        {
//            sb.AppendLine(pair.Key);
//        }

//        Debug.Log(string.Format(@"--- Mappable attributes for {0} [{1}]: 
//{2}", s, attributes.Count, sb));
//    }

//    #endregion
//}