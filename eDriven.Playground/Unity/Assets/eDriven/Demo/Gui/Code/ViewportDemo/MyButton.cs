using eDriven.Gui.Components;
using eDriven.Gui.Styles;

namespace Assets.eDriven.Demo.Gui.Code.ViewportDemo
{
    /// <summary>
    /// Just an override of the Button class
    /// </summary>
    [Style(Name = "someMyButtonStyle", Type = typeof(float), Default = 0.5f)] // , ProxyMemberName = "RepeatDelay"

    public class MyButton : Button
    {
    }
}
