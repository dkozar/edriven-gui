using System;
using System.Collections.Generic;
using eDriven.Core.Callback;
using UnityEngine;
using Object=UnityEngine.Object;

public class Main3CS : MonoBehaviour
{
    private readonly WwwQueue _wwwQueue = new WwwQueue();
    private readonly AssetQueue _assetQueue = new AssetQueue();

    private readonly List<AssetBundle> _loadedBundles = new List<AssetBundle>();
    private readonly List<Object> _objects = new List<Object>();

    // asset bundle URL | asset name (separator is "|")
    public string[] AssetUrls = new string[0];
    
    // ReSharper disable UnusedMember.Local
    // ReSharper disable InconsistentNaming
    void OnGUI()
    // ReSharper restore InconsistentNaming
    // ReSharper restore UnusedMember.Local
    { // classic OnGUI
        GUI.depth = 0;
        if (GUI.Button(new Rect(10, 10, 100, 50), "Load"))
        {
            // reset queues
            _wwwQueue.Reset();
            _assetQueue.Reset();

            _objects.ForEach(Destroy);
            _loadedBundles.ForEach(delegate (AssetBundle bundle){ bundle.Unload(true); });

            foreach (string s in AssetUrls)
            {
                string[] arr = s.Split('|');

                if (arr.Length != 2)
                    throw new Exception("Error in asset string");

                string bundleUrl = arr[0];
                string assetName = arr[1];

                _wwwQueue.Send(new WWW(bundleUrl),
                delegate(WWW request)
                {
                    //if (!string.IsNullOrEmpty(request.error)){
                    //    Debug.Log("Loading error: [" + bundleUrl + "]: " + request.error);
                    //    return;
                    //}
					
                    Debug.Log("Bundle loaded: " + request.url);
                    _loadedBundles.Add(request.assetBundle);

                    AssetBundleRequest assetBundleRequest = request.assetBundle.LoadAsync(assetName, typeof(GameObject));
                    _assetQueue.Send(assetBundleRequest,
                        delegate(AssetBundleRequest request2)
                        {
                            Debug.Log("Asset loaded: " + request2.asset.name);
                            GameObject go = (GameObject) Instantiate(request2.asset);
                            _objects.Add(go);

                            // add mouse orbit
                            GameObject cameraGo = GameObject.Find("Main Camera");
                            if (null != cameraGo)
                            {
                                MouseOrbitCs mouseOrbit = cameraGo.AddComponent<MouseOrbitCs>();
                                mouseOrbit.Target = go.transform;
                            }
                        }
                    );
                });
            }
        }
    }

// ReSharper disable UnusedMember.Local
    void Update()
// ReSharper restore UnusedMember.Local
    {
        _wwwQueue.Tick();
        _assetQueue.Tick();
    }
}