using eDriven.Core.Data.Collections;
using eDriven.Core.Util;
using UnityEngine;

public class ObservableCollectionDemo : MonoBehaviour
{
    private Timer _timer;

// ReSharper disable UnusedMember.Local
    void Start()
// ReSharper restore UnusedMember.Local
    {
        Debug.Log(new eDriven.Core.Info());
                               
        /**
         * ObservableCollection test
         * */
        FifoObservableCollection<string> list = new FifoObservableCollection<string>(5);
        list.CollectionChange += delegate
                                     {
                                         Debug.Log("CollectionChange. Count = " + list.Count);
                                     };
        
        list.ItemAdded += delegate
        {
            Debug.Log("ItemAdded. Count = " + list.Count);
        };

        list.ItemRemoved += delegate
        {
            Debug.Log("ItemRemoved. Count = " + list.Count);
        };

        /**
         * Timer
         * */
        _timer = new Timer(1);
        _timer.TickOnStart = true;
        _timer.Tick += delegate
                           {
                               //Debug.Log("Timer tick");
                               list.Add("dummy");
                           };
        _timer.Start();
    }
}