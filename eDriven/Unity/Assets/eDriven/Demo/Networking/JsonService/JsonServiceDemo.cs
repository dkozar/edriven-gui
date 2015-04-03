using System;
using eDriven.Networking.Configuration;
using eDriven.Networking.Rpc.Json;
using eDriven.Tests.Networking.JsonService;
using UnityEngine;

public class JsonServiceDemo : MonoBehaviour
{
    // ReSharper disable UnusedMember.Local
    void Start()
    // ReSharper restore UnusedMember.Local
    {
        Debug.Log(new eDriven.Networking.Info());

        /**
         * Important:
         * Initialize serializer (wrapper) before the first service call
         * */
        JsonService.Serializer = new JsonFxSerializer();
                               
        #region Manual

        // instantiate service
        JsonService jsonService = new JsonService();

        // spacify service URL
        jsonService.Url = "http://alfa.trillenium.com/Razvoj/Server/TrilleniumWebService/JsonService.aspx";
        
        // add operation
        Operation operation = new Operation("DoIt",
            new Parameter("mojString", typeof (String)),
            new Parameter("mojInt", typeof (Int32))
        );
        operation.ReturnType = typeof (bool);

        // add destination
        jsonService.Destinations.Add(new Destination("Worker", operation));

        // select a default destination
        jsonService.Destination = "Worker";
        
        // send command
        jsonService.Send(new Command("DoIt", "bla", 1), delegate(object o)
        {
            //bool result = (bool) o;
            Debug.Log("Result1: " + o);
        });

        #endregion

        #region Using config

        ConfigLoader configLoader = gameObject.AddComponent<ConfigLoader>();
        configLoader.ConfigUrl = "";
        Configuration.Instance.AddEventListener(Configuration.INITIALIZED, delegate
                                                                               {
                                                                                   Debug.Log("Configuration initialized");

                                                                                   Configuration.Instance.Application.Services.Json["testService"].Send(new Command("DoIt", "bla", 1), 
                                                                                       delegate(object o)
                                                                                       {
                                                                                           //bool result = (bool) o;
                                                                                           Debug.Log("Result2: " + o);
                                                                                       }
                                                                                    );
                                                                               }
            );

        #endregion

    }
}