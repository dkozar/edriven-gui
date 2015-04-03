using UnityEngine;

/// <summary>
/// </summary>
/// <remarks>Reported by: Danko Kozar</remarks>
public class ClippedTest : MonoBehaviour
{
	
	//private Vector2 scrollPosition = Vector2.zero;
	
	public float ViewHeight = 300;
	public float ContentHeight = 200; // works fine with 400, but when content height less then the view height, the ScrollView goes wild!

    float _w = -1;
    private float _amount1;
    private float _amount2;
    private float _amount3;
    private float _amount4;
    private float _amount5;

    void OnGUI() {

        if (_w < 0)
            _w = GUI.skin.GetStyle("verticalscrollbar").fixedWidth;

        //GUI.BeginGroup(new Rect(200, 200, 210, ViewHeight), GUI.skin.GetStyle("box"));

        //GUI.BeginGroup(new Rect(0, -100, 210, ViewHeight), GUI.skin.GetStyle("box"));       

        //GUI.Button(new Rect(0, 0, 100, 50), "Top-left");
        //GUI.Button(new Rect(110, 0, 100, 50), "Top-right");
        //GUI.Button(new Rect(0, 180, 100, 50), "Bottom-left");
        //GUI.Button(new Rect(110, 180, 100, 50), "Bottom-right");

        //GUI.EndGroup();

        //GUI.EndGroup();

        _amount1 = GUI.VerticalScrollbar(new Rect(50, 50, _w, 400), _amount1, 100, 500, 0);
        _amount2 = GUI.VerticalScrollbar(new Rect(50 + 50, 50, _w, 400), _amount2, 200, 500, 0);
        _amount3 = GUI.VerticalScrollbar(new Rect(50 + 50 * 2, 50, _w, 400), _amount3, 500, 500, 0);
        _amount4 = GUI.VerticalScrollbar(new Rect(50 + 50 * 3, 50, _w, 400), _amount4, 100, 1000, 0);
        _amount5 = GUI.VerticalScrollbar(new Rect(50 + 50 * 4, 50, _w, 400), _amount5, 200, 1000, 0);
    }
}