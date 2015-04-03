using eDriven.Gui.Components;
using eDriven.Gui.Geom;
using eDriven.Gui.Graphics.Base;
using eDriven.Gui.Util;
using UnityEngine;
using Rect=eDriven.Gui.Graphics.Rect;

namespace eDriven.Gui.GUIStyles.Helper
{
    internal sealed class SliderHelper
    {
        public static void ApplyBackgroundStyle(ProgramaticStyle programaticStyle, int w, int weight)
        {
            programaticStyle.Border = new RectOffset(weight + 1, weight + 1, weight + 1, weight + 1);

            //programaticStyle.FontSize = 30;
            //programaticStyle.NormalTextColor = Color.grey;
            programaticStyle.NormalGraphics = new Rect(w, w,
                                                       new Fill(Color.white),
                                                       new Stroke(weight) { Color = Color.black }
                );
            //programaticStyle.HoverTextColor = Color.white;
            programaticStyle.HoverGraphics = new Rect(w, w,
                                                      new Fill(Color.white),
                                                      new Stroke(weight) { Color = Color.grey }
                );
            //programaticStyle.ActiveTextColor = Color.white;
            programaticStyle.ActiveGraphics = new Rect(w, w,
                                                       new Fill(Color.white), //new Color(0.2f, 0.9f, 0.2f, 1f)),
                                                       new Stroke(weight) { Color = Color.grey }
                );
        }

        public static void ApplyThumbStyle(ProgramaticStyle programaticStyle, int w, int weight)
        {
            programaticStyle.Border = new RectOffset(weight + 1, weight + 1, weight + 1, weight + 1);

            //programaticStyle.FontSize = 30;
            //programaticStyle.NormalTextColor = Color.grey;

            programaticStyle.Margin = new RectOffset(weight, weight, weight, weight);

            programaticStyle.NormalGraphics = new Rect(w, w,
                                                       new Fill(ColorMixer.FromHex(0x666666).ToColor())/*,
                                                       new Stroke(Color.white, weight)*/
                );
            //programaticStyle.HoverTextColor = Color.white;
            programaticStyle.HoverGraphics = new Rect(w, w,
                                                      new Fill(ColorMixer.FromHex(0x999999).ToColor())/*,
                                                      new Stroke(Color.white, weight)*/
                );
            //programaticStyle.ActiveTextColor = Color.white;
            programaticStyle.ActiveGraphics = new Rect(w, w,
                                                       new Fill(ColorMixer.FromHex(0xAAAAAA).ToColor())/*,
                                                       new Stroke(Color.white, weight)*/
                );
        }

        
    }
}