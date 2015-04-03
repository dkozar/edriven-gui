using UnityEngine;

/// <summary>
/// The example of event-processing order bug when 2 components overlapping
/// 
/// This happens in the case when any UnityGUI controls overlap
/// It also happens with buttons (the bottom one handles the mouse click), but I don't mind it because I do my own "get-component-under-the-mouse" lookup
/// Also I don't mind it with mouse wheel over the scrollview (this is just the example for introducing the subject, a proof of concept, but the bug is general),
/// since I do my own "mousewheel-to-scrolling" thing (but other users could mind it)
/// 
/// Where I didn't find a way to handle it is when using scrollbars that by chance have overlapping scrollers 
/// (see: http://forum.unity3d.com/threads/136709-ScrollView-and-or-TextArea-overlapping-and-focus-GUI.depth...)
/// When attempting to scroll the upper scrollbar by dragging a thumb, the bottom one processes mouse events, so I'm stuck
/// 
/// To reproduce this bug, please attach the script to any game object and run
/// Then, use the mouse wheel over the overlapped area: the bottom scrollview scrolls
/// 
/// - What this should do is is to alow the top component to react to mouse events
/// - What this actually does that the bottom component actualy reacts
/// 
/// Note to UT: When i previously asked about this issue (via newsgroup) I got the answer that I should use GUI.Window for the upper scrollbar.
/// But, one might choose not to use one - and that's because moving the window introduces a few serious problems
/// In my case, I have a draggable window class, which doesn't extend UnityGUI window,
/// In fact, I can pop-up any component, not windows only (this is important for comboboxes etc.)
/// So, this is a very common scenario in GUI dev. and should work without using the window
/// </summary>
/// <remarks>Reported by: Danko Kozar</remarks>
public class GuiEventProcessingOrderBug : MonoBehaviour {
 
    private Vector2 _scrollPosition1 = new Vector2();
    private Vector2 _scrollPosition2 = new Vector2();
 
    void OnGUI()
    {
        /**
         * Bottom scrollview
         * */
        _scrollPosition1 = GUI.BeginScrollView(new Rect(100, 100, 200, 200), _scrollPosition1, new Rect(0, 0, 300, 300));
        GUI.Box(new Rect(0, 0, 300, 300), @"BOTTOM");
        GUI.EndScrollView();
 
        /**
         * Top scrollview
         * */
        _scrollPosition2 = GUI.BeginScrollView(new Rect(200, 200, 200, 200), _scrollPosition2, new Rect(0, 0, 300, 300));
        GUI.Box(new Rect(0, 0, 300, 300), @"TOP");
        GUI.EndScrollView();
		
//		/**
//         * Bottom scrollview
//         * */
//        _scrollPosition1 = GUI.BeginScrollView(new Rect(100, 100, 200, 200), _scrollPosition1, new Rect(0, 0, 180, 300));
//        GUI.Box(new Rect(0, 0, 200, 300), @"BOTTOM");
//        GUI.EndScrollView();
// 
//        /**
//         * Top scrollview
//         * */
//        _scrollPosition2 = GUI.BeginScrollView(new Rect(97, 200, 200, 200), _scrollPosition2, new Rect(0, 0, 180, 300));
//        GUI.Box(new Rect(0, 0, 200, 300), @"TOP");
//        GUI.EndScrollView();
    }
}