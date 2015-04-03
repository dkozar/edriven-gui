using System;
using eDriven.Animation;
using eDriven.Audio;
using eDriven.Gui;
using eDriven.Gui.Components;
using eDriven.Gui.Designer;
using Assets.eDriven.Demo.Tweens;
using UnityEngine;
using Action=eDriven.Animation.Action;
using Event=eDriven.Core.Events.Event;

public class ButtonDemo : MonoBehaviour {

	public void ClickHandler(Event e)
	{
		//Debug.Log("ClickHandler: " + e.Target);
		Alert.Show("Event",
				   string.Format(@"[{0}] received:

Type: {1}
Target: {2}
CurrentTarget: {3}", e.GetType(), e.Type,
								 e.Target, e.CurrentTarget), AlertButtonFlag.Ok,
				   new AlertOption(AlertOptionType.HeaderIcon, Resources.Load("Icons/information")),
				   new AlertOption(AlertOptionType.AddedEffect, _alertEffect)
		);
	}

	public void PaintButtonGreen(Event e)
	{
		// GUI lookup example
		Button button = GuiLookup.GetComponent("button1") as Button;
		if (null != button)
			button.Color = Color.green;
	}

	public void RemoveButtonColor(Event e)
	{
		// GUI lookup example
		Button button = GuiLookup.GetComponent("button1") as Button;
		if (null != button)
			button.Color = Color.white;
	}

	public void MouseOverHandler(Event e)
	{
		Debug.Log("MouseOverHandler: " + e.Target);
	}

	public void MouseOutHandler(Event e)
	{
		Debug.Log("MouseOutHandler: " + e.Target);
	}

	public void LoadLevel(Event e)
	{
		Debug.Log("Loading level 1");
		Application.LoadLevel(1);
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
}