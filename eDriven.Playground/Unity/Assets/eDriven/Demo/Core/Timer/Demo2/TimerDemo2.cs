using eDriven.Audio;
using eDriven.Core.Util;
using Assets.eDriven.Demo.Core.Timer.Demo2;
using UnityEngine;
using Event=eDriven.Core.Events.Event;


public class TimerDemo2 : MonoBehaviour
{
    public GameObject Prefab;
    private Timer _timer;

    void Start()
    {
        _timer = new Timer(1) { TickOnStart = true, RepeatCount = 10 - 1 }; // 10 cubes (with tick on start)
        _timer.Tick += OnTimerTick;
        _timer.Start();
    }


    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 100, 30), "Start timer"))
        {
            AudioPlayerMapper.GetDefault().PlaySound("button_click1");
            _timer.Start();
        }

        if (GUI.Button(new Rect(120, 10, 100, 30), "Pause timer")) {
            AudioPlayerMapper.GetDefault().PlaySound("button_click2");
            _timer.Pause();
        }

        if (GUI.Button(new Rect(230, 10, 100, 30), "Reset"))
        {
            AudioPlayerMapper.GetDefault().PlaySound("button_click2");
            _timer.Stop();
            Cleanup();
        }
    }

    private void OnTimerTick(Event e)
    {
        Debug.Log("OnTimerTick");
        SpawnObject();
    }

    private static void Cleanup()
    {
        Debug.Log("OnTimerStop");
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Cube");

        // destroy all Cubes
        foreach (GameObject o in gameObjects)
        {
            Destroy(o);
        }
    }

    private void SpawnObject()
    {
        // position
        float dx = Randomizer.RandomizeAround(0, 0.5f);
        float dy = Randomizer.RandomizeAround(0, 0.5f);
        float dz = Randomizer.RandomizeAround(0, 0.5f);
        Vector3 position = transform.position + new Vector3(dx, dy, dz);

        // instantiation
        GameObject go = (GameObject)Instantiate(Prefab, position, Quaternion.identity);

        // re-parenting
        go.transform.parent = transform;

        AudioPlayerMapper.GetDefault().PlaySound("spawn");
    }
}
