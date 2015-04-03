using eDriven.Core.Managers.System;
using UnityEngine;

public class SystemManagerDemo1 : MonoBehaviour
{
// ReSharper disable UnusedMember.Local
    void Start()
// ReSharper restore UnusedMember.Local
    {
        Debug.Log(new eDriven.Core.Info());

        SystemManager.Instance.MouseDown += print;
        SystemManager.Instance.MouseUp += print;
        SystemManager.Instance.MouseMove += print;
        SystemManager.Instance.MouseDrag += print;
        SystemManager.Instance.KeyUp += print;
        SystemManager.Instance.KeyDown += print;
        SystemManager.Instance.Resize += print;
        
        //SystemManager.Instance.InputProcessed += print;
        //SystemManager.Instance.Update += print;

        //SystemManager.Instance.FixedUpdate += print;
        //SystemManager.DispatchFixedUpdate = true; // this flag is false by default

        //SystemManager.Instance.LateUpdate += print;
        //SystemManager.DispatchLateUpdate = true; // this flag is false by default

        //SystemManager.DispatchUpdate = false;
        //SystemManager.DispatchInputProcessed = false;
    }
}