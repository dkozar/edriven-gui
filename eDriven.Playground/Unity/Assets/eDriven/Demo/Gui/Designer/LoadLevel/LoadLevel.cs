using UnityEngine;
using Event = eDriven.Core.Events.Event;

namespace Assets.eDriven.Demo.Gui.Designer.LoadLevel
{
    public class LoadLevel : MonoBehaviour {
        
        public void Load(Event e)
        {
            Debug.Log("Loading level 1");
            Application.LoadLevel(1);
        }
    }
}