using UnityEngine;
using System.Collections;

/// <summary>
/// The example of scrollview bug when using GUI.BeginScrollView with alwaysShowHorizontal and alwaysShowVertical parameters
/// 
/// If you have your content height (the 3rd parameter) smaller than a view height (1st parameter), and you set alwaysShowHorizontal to true, the ScrollView goes wild. Same with width and alwaysShowVertical.
/// 
/// What this was intended to do is to display scrollbars although they are not needed
/// What this actually does is making the Scrollview go wild
/// What this should do is to always render scrollbars. If content is smaller than the view itself, scrollbars are at 100% and should not be movable, and scrollPosition should always be (0, 0)
/// 
/// Note to UT: Since this bug exists from the beginning of time, probably nobody uses these parameters, so your possible fix wouldn't break the existing apps!
/// </summary>
/// <remarks>Reported by: Danko Kozar</remarks>
public class ScrollViewTest : MonoBehaviour {
	
	private Vector2 scrollPosition = Vector2.zero;
	
	public float ViewHeight = 300;
	public float ContentHeight = 200; // works fine with 400, but when content height less then the view height, the ScrollView goes wild!

    void OnGUI() {
        
		scrollPosition = GUI.BeginScrollView(
            new Rect(0, 0, 210, ViewHeight),
            scrollPosition,
            new Rect(0, 0, 210, ContentHeight),
			true,
			true
		);       

        GUI.Button(new Rect(0, 0, 100, 20), "Top-left");
        GUI.Button(new Rect(110, 0, 100, 20), "Top-right");
        GUI.Button(new Rect(0, 180, 100, 20), "Bottom-left");
        GUI.Button(new Rect(110, 180, 100, 20), "Bottom-right");

        GUI.EndScrollView();
    }
}