using eDriven.Core.Events;
using eDriven.Core.Managers.System;
using UnityEngine;
using Event=eDriven.Core.Events.Event;

public class GuiController : MonoBehaviour {

    public GUISkin Skin;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Local
    void OnGUI()
// ReSharper restore UnusedMember.Local
// ReSharper restore InconsistentNaming
    {
        GUI.skin = Skin;

        bool clicked = GUI.Button(new Rect(Screen.width - 110, 10, 100, 40), GameModel.Instance.Paused ? "Play" : "Pause");
        if (clicked)
        {
            GameModel.Instance.Paused = !GameModel.Instance.Paused;
        }
    }
}
