using eDriven.Networking.Configuration;
using eDriven.Networking.Rpc;
using eDriven.Networking.Rpc.Core;
using UnityEngine;

public class HttpServiceDemo : MonoBehaviour
{
// ReSharper disable UnusedMember.Local
    void Start()
// ReSharper restore UnusedMember.Local
    {
        Debug.Log(new eDriven.Networking.Info());

        #region Manual

        HttpService httpService = new HttpService();
        httpService.ResponseMode = ResponseMode.WWW;
        /*AsyncToken token = */
        httpService.Send("http://dankokozar.com/images/Kiklop.gif", delegate/*(object data)*/
                                                                        {
                                                                            //WWW www = (WWW) data;
                                                                            Debug.Log("Loaded1");
                                                                        });

        #endregion

        #region Using config

        ConfigLoader configLoader = gameObject.AddComponent<ConfigLoader>();
        configLoader.ConfigUrl = "";
        Configuration.Instance.AddEventListener(Configuration.DESERIALIZED, delegate // works with Configuration.INITIALIZED, but Configuration.DESERIALIZED is enough for HttpService
            {
                Debug.Log("Configuration initialized");

                Configuration.Instance.Application.Services.Http["dankokozar"].Send(
                    new Responder(delegate/*(object o)*/
                    {
                        //WWW www = (WWW) data;
                        Debug.Log("Loaded2");
                    })
                    );
            }
        );

        #endregion
    }
}