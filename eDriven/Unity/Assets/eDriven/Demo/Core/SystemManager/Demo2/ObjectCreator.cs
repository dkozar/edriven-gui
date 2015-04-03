using eDriven.Core.Events;
using eDriven.Core.Managers.System;
using eDriven.Core.Util;
using UnityEngine;
using Event=eDriven.Core.Events.Event;
using Random=System.Random;

public class ObjectCreator : MonoBehaviour 
{
    public GameObject Prefab;

	// Use this for initialization
// ReSharper disable UnusedMember.Local
	void Start () {
// ReSharper restore UnusedMember.Local

        SystemManager.Instance.AddEventListener(KeyboardEvent.KEY_UP, OnKeyUp);
        //SystemManager.Instance.KeyUp += OnKeyUp;
	}

// ReSharper disable MemberCanBeMadeStatic.Local
    private void OnKeyUp(Event e)
// ReSharper restore MemberCanBeMadeStatic.Local
    {
        if (!enabled) return;

        KeyboardEvent ke = (KeyboardEvent)e;
        Debug.Log("OnKeyUp: " + ke.KeyCode);
        if (ke.KeyCode == KeyCode.Return)
            SpawnObject();
        else if (ke.KeyCode == KeyCode.R)
            Reset();
    }

    private void SpawnObject()
    {
        // position
        float dx = Randomizer.RandomizeAround(0, 0.5f);
        float dy = Randomizer.RandomizeAround(0, 0.5f);
        float dz = Randomizer.RandomizeAround(0, 0.5f);
        Vector3 position = transform.position + new Vector3(dx, dy, dz);

        // instantiation
        GameObject go = (GameObject) Instantiate(Prefab, position, Quaternion.identity);

        // re-parenting
        go.transform.parent = transform;
    }

    private static void Reset()
    {
        Debug.Log("OnReset");
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Cube");

        // destroy all Cubes
        foreach (GameObject o in gameObjects)
        {
            Destroy(o);
        }
    }
}