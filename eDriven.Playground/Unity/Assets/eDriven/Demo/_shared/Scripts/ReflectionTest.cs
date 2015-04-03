using System;
using System.Reflection;
using Assets.eDriven.Demo.Scripts;
using UnityEngine;

/// <summary>
/// Reflection test
/// Author: Danko Kozar
/// </summary>
public class ReflectionTest : MonoBehaviour
{
    private string _text = string.Empty;

    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 150, 30), "Start Reflection Test"))
            StartTest();

        GUI.TextArea(new Rect(10, 50, 600, 600), _text);
    }

    private void StartTest()
    {
        Type type = null;
        PropertyInfo propertyInfo = null;
        FieldInfo fieldInfo = null;
        object value;
        PropertyInfo[] properties;
        MethodInfo methodInfo;

        int started = 0;
        int ended = 0;

        TestClass instance = new TestClass();

        _text = "##### Test started #####\n\n";

        _text += "GetType() ";
        try {
            started++;
            type = instance.GetType();
            _text += "-> Type is:" + type + "\n\n";
            ended++;
        }
        catch (Exception ex)
        {
            _text += "-> Error:" + ex.Message + "\n\n";
        }

        _text += "Getting FullName ";
        try
        {
            started++;
            string fullName = type.FullName;
            _text += "-> type.FullName is:" + fullName + "\n\n";
            ended++;
        }
        catch (Exception ex)
        {
            _text += "-> Error:" + ex.Message + "\n\n";
        }

        _text += "GetProperty() ";
        try {
            started++;
            propertyInfo = type.GetProperty("Property");
            _text += "-> PropertyInfo: " + propertyInfo + "\n\n";
            ended++;
        }
        catch (Exception ex)
        {
            _text += "-> Error:" + ex.Message + "\n\n";
        }

        _text += "pi.GetValue() ";
        try {
            started++;
            value = propertyInfo.GetValue(instance, null);
            _text += "-> value:" + value + "\n\n";
            ended++;
        }
        catch (Exception ex)
        {
            _text += "-> Error:" + ex.Message + "\n\n";
        }

        _text += "pi.SetValue() ";
        try {
            started++;
            propertyInfo.SetValue(instance, "foo 2", null);
            _text += "-> value:" + propertyInfo.GetValue(instance, null) +"\n\n";
            ended++;
        }
        catch (Exception ex)
        {
            _text += "-> Error:" + ex.Message + "\n\n";
        }

        _text += "pi.Name ";
        try {
            started++;
            value = propertyInfo.Name;
            _text += "-> pi.Name:" + value + "\n\n";
            ended++;
        }
        catch (Exception ex)
        {
            _text += "-> Error:" + ex.Message + "\n\n";
        }

        _text += "GetField() ";
        try {
            started++;
            fieldInfo = type.GetField("Field");
            _text += "-> FieldInfo: " + fieldInfo + "\n\n";
            ended++;
        }
        catch (Exception ex)
        {
            _text += "-> Error:" + ex.Message + "\n\n";
        }

        _text += "fi.GetValue() ";
        try {
            started++;
            value = fieldInfo.GetValue(instance);
            _text += "-> value:" + value + "\n\n";
            ended++;
        }
        catch (Exception ex)
        {
            _text += "-> Error:" + ex.Message + "\n\n";
        }

        _text += "fi.SetValue() ";
        try {
            started++;
            fieldInfo.SetValue(instance, "bar 2");
            _text += "-> value:" + fieldInfo.GetValue(instance) + "\n\n";
            ended++;
        }
        catch (Exception ex)
        {
            _text += "-> Error:" + ex.Message + "\n\n";
        }

        _text += "fi.Name ";
        try {
            started++;
            value = fieldInfo.Name;
            _text += "-> fis.Name:" + value + "\n\n";
            ended++;
        }
        catch (Exception ex)
        {
            _text += "-> Error:" + ex.Message + "\n\n";
        }

        _text += "type.GetProperties() ";
        try {
            started++;
            properties = type.GetProperties();
            _text += "-> props.Length: " + properties.Length + "\n\n";
            ended++;
        }
        catch (Exception ex)
        {
            _text += "-> Error:" + ex.Message + "\n\n";
        }

        _text += "type.GetMethod() ";
        try
        {
            started++;
            methodInfo = type.GetMethod("Method");
            _text += "-> methodInfo: " + methodInfo + "\n\n";
            ended++;
        }
        catch (Exception ex)
        {
            _text += "-> Error:" + ex.Message + "\n\n";
        }

        _text += "##### " +ended + "/" + started + " tests passed #####";
    }
}