using UnityEngine;

namespace eDriven.Gui.Styles
{
    ///<summary>
    ///</summary>
    [Styleable(LoadMethod = "RegenerateStyleCache", GetMethod = "GetStyle", SetMethod = "SetStyle")]
    public class TestComponent : MonoBehaviour, IStyleable
    {
        public string MyProperty = "Foo";

        [Style(Default = "Default Foo")]
        public string MyStyledProperty = "Foo";

        [Style(Default = "Default Foo Named")]
        public string MyStyledProperty2 = "Foo";

// ReSharper disable once UnusedMember.Local
        void Awake()
        {
            //Debug.Log("before");
            MyProperty = "Bar";
            //Debug.Log("after");
        }

        #region Implementation of IStyleable

        /// <summary>
        /// Sets the style on the component
        /// </summary>
        /// <param name="styleName"></param>
        /// <param name="value"></param>
        public void SetStyle(string styleName, object value)
        {
            //StyleManager.Instance.SetStyle(this, styleName, value); // -> this has to go out of style declaration (separate dictionary for manually set properties)
        }

        /// <summary>
        /// Returns the style specified with style name
        /// </summary>
        /// <param name="styleName"></param>
        /// <returns></returns>
        public object GetStyle(string styleName)
        {
            //return StyleManager.Instance.GetStyle(this, styleName);
            return "foo";
        }

        /// <summary>
        /// Notifies the style change
        /// </summary>
        /// <param name="styleName"></param>
        /// <param name="value"></param>
        public void StyleChanged(string styleName, object value)
        {
            Debug.Log("StyleChanged: " + styleName + ": " + value);
        }

        #endregion

// ReSharper disable UnusedMember.Local
// ReSharper disable InconsistentNaming
        void OnGUI()
// ReSharper restore InconsistentNaming
// ReSharper restore UnusedMember.Local
        {
            //Debug.Log("! " + GetStyle("foo"));
            GUI.Button(new Rect(4, 4, 200, 40), MyProperty);
            GUI.Button(new Rect(4, 54, 200, 40), MyStyledProperty);
            GUI.Button(new Rect(4, 104, 200, 40), MyStyledProperty2);
        }
    }
}