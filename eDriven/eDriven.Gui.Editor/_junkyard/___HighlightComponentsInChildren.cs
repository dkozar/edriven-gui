//#region Copyright

///*
 
//Copyright (c) Danko Kozar 2012. All rights reserved.
 
//*/

//#endregion Copyright

//using System;
//using System.Collections.Generic;
//using UnityEditor;
//using UnityEngine;

///// <summary>
///// Highlights components of type YourType in children of a gameObject this script is attached to 
///// </summary>
///// <remarks>Author: Danko Kozar</remarks>
//public class HighlightComponentsInChildren : MonoBehaviour
//{
//    public Type ComponentType = typeof(ContainerAdapter);

//// ReSharper disable UnusedMember.Local
//    void Awake()
//// ReSharper restore UnusedMember.Local
//    {
//        Component[] components = gameObject.GetComponentsInChildren(ComponentType);
        
//        List<GameObject> list = new List<GameObject>();
//        foreach (Component component in components)
//        {
//            EditorGUIUtility.PingObject(component);
//            list.Add(component.gameObject);
//        }

//        Selection.objects = list.ToArray();
//    }
//}