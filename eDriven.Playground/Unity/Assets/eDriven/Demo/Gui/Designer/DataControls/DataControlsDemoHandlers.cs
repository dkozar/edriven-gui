using System;
using eDriven.Animation;
using eDriven.Audio;
using eDriven.Gui;
using eDriven.Gui.Components;
using eDriven.Gui.Designer;
using eDriven.Gui.Events;
using Assets.eDriven.Demo.Tweens;
using UnityEngine;
using Action = eDriven.Animation.Action;
using Event = eDriven.Core.Events.Event;

public class DataControlsDemoHandlers : MonoBehaviour
{

    public void ClickHandler(Event e)
    {
        //Debug.Log("ClickHandler: " + e.Target);
        Alert.Show("Event", string.Format(@"[{0}] received:

Type: {1}
Target: {2}
CurrentTarget: {3}", e.GetType(), e.Type, e.Target, e.CurrentTarget), AlertButtonFlag.Ok);
    }

    public void ChangeButtonColor(Event e)
    {
        // GUI lookup example
        Button button = GuiLookup.FindComponent<Button>(gameObject, "button1");
        if (null != button)
            button.Color = Color.green;
    }

    public void MouseOverHandler(Event e)
    {
        Debug.Log("MouseOverHandler: " + e.Target);
    }

    public void MouseOutHandler(Event e)
    {
        Debug.Log("MouseOutHandler: " + e.Target);
    }

    public void RightClickHandler(Event e)
    {
        Debug.Log("RightClickHandler: " + e.Target);
        Debug.Log("Loading level");
        Application.LoadLevel(1);
    }

    public void RightMouseUp(Event e)
    {
        Debug.Log("RightMouseUp: " + e.Target);
    }

    public void Test(Event e)
    {
        Debug.Log("Test: " + e.Target);
    }

    public void ChangeHandler(Event e)
    {
        Alert.Show("Event", string.Format(@"[{0}] received:

Type: {1}
Target: {2}
CurrentTarget: {3}", e.GetType(), e.Type, e.Target, e.CurrentTarget), AlertButtonFlag.Ok);
    }

    public void OpeningHandler(Event e)
    {
        Alert.Show("Combo box opening", string.Format(@"[{0}] received:

Type: {1}
Target: {2}
CurrentTarget: {3}", e.GetType(), e.Type, e.Target, e.CurrentTarget), AlertButtonFlag.Ok);
    }

    public void ClosingHandler(Event e)
    {
        Alert.Show("Combo box closing", string.Format(@"[{0}] received:

Type: {1}
Target: {2}
CurrentTarget: {3}", e.GetType(), e.Type, e.Target, e.CurrentTarget), AlertButtonFlag.Ok);
    }

    public void LoadLevel(Event e)
    {
        Debug.Log("Loading level 2");
        Application.LoadLevel(2);
    }

    public void Info(Event e)
    {
        Alert.Show(
            "Info",
            "This is the info message. The time is: " + DateTime.Now.ToLongTimeString(),
            AlertButtonFlag.Ok,
            new AlertOption(AlertOptionType.HeaderIcon, Resources.Load("Icons/information")),
            new AlertOption(AlertOptionType.AddedEffect, _alertEffect)
        );
    }

    private readonly TweenFactory _alertEffect = new TweenFactory(
        new Sequence(
            new Action(delegate { AudioPlayerMapper.GetDefault().PlaySound("dialog_open"); }), // dialog_open
            new Jumpy()
        )
    );

    public void TestEvent(Event e)
    {
        print(e); // prints event
        ComboBox combo = (ComboBox)e.Target;
        print("   SelectedIndex = " + combo.SelectedIndex);
    }

    public void SelectedIndexChangedTest(Event e)
    {
        IndexChangeEvent ice = (IndexChangeEvent)e;
        Debug.Log(string.Format("OldIndex: {0}, NewIndex: {1}", ice.OldIndex, ice.NewIndex));
    }

}