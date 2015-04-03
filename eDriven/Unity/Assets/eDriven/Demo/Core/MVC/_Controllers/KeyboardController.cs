using eDriven.Core.Events;
using eDriven.Core.Managers.System;
using UnityEngine;
using Event=eDriven.Core.Events.Event;

public class KeyboardController : MonoBehaviour {

    public GUISkin Skin;

    // Use this for initialization
// ReSharper disable UnusedMember.Local
    void Start()
// ReSharper restore UnusedMember.Local
    {
        SystemManager.Instance.KeyUp += OnKeyUp;
    }

// ReSharper disable MemberCanBeMadeStatic.Local
    private void OnKeyUp(Event e)
// ReSharper restore MemberCanBeMadeStatic.Local
    {
        KeyboardEvent ke = (KeyboardEvent)e;
        Debug.Log("OnKeyUp: " + ke.KeyCode);

        switch (ke.KeyCode)
        {
            case KeyCode.Return:
            case KeyCode.P:
                GameModel.Instance.Paused = !GameModel.Instance.Paused;
                break;
            case KeyCode.R:
                GameModel.Instance.Reset();
                break;
        }
    }
}
