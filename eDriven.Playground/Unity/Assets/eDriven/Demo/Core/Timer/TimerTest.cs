using eDriven.Core.Util;
using UnityEngine;
using Event=eDriven.Core.Events.Event;

/// <summary>
/// Timer test
/// Attach this script anywhere in the scene, and then 
/// 1) load another level
/// 2) then load this scene again
/// </summary>
/// <remarks>
/// Example by Danko Kozar
/// </remarks>
public class TimerTest : MonoBehaviour {

    private Timer _timer;
    
	void Start()
    {
        Debug.Log("Start");
        _timer = new Timer(1); // tick each second
	    _timer.Tick += TickHandler; // subscribe
        _timer.Start();
    }

    void TickHandler(Event e)
    {
        Debug.Log("Tick!");
    }

    void OnDestroy()
    {
        Debug.Log("OnDestroy");
        _timer.Tick -= TickHandler; // unsubscribe
        _timer.Stop(); // stop the timer
        _timer.Dispose(); // dispose?
    }
}
