using eDriven.Core.Geom;
using eDriven.Gui.Components;
using eDriven.Gui.Geom;
using eDriven.Gui.Graphics;
using eDriven.Gui.Graphics.Base;
using eDriven.Gui.Util;
using UnityEngine;
using Rect=eDriven.Gui.Graphics.Rect;

namespace eDriven.Gui.GUIStyles.Helper
{
    internal static class ScrollbarHelper
    {
        public static Color NormalColor = ColorMixer.FromHex(0xd0d0d0).ToColor();
        public static Color HoverColor = ColorMixer.FromHex(0xe0e0e0).ToColor();
        public static Color ActiveColor = ColorMixer.FromHex(0xeeeeee).ToColor();
        public static Color BackgroundColor = ColorMixer.FromHex(0x999999).ToColor();
        public static Color StrokeColor = Color.black;

        public static void ApplyBackgroundStyle(ProgramaticStyle programaticStyle, int w, int weight)
        {
            programaticStyle.Border = new RectOffset(weight + 1, weight + 1, weight + 1, weight + 1);

            //programaticStyle.FontSize = 30;
            //programaticStyle.NormalTextColor = Color.grey;
            programaticStyle.NormalGraphics = new Rect(w, w,
                                                       new Fill(BackgroundColor),
                                                       new Stroke(weight) { Color = StrokeColor }
                );
            //programaticStyle.HoverTextColor = Color.white;
            programaticStyle.HoverGraphics = new Rect(w, w,
                                                      new Fill(BackgroundColor),
                                                      new Stroke(weight) { Color = StrokeColor }
                );
            //programaticStyle.ActiveTextColor = Color.white;
            programaticStyle.ActiveGraphics = new Rect(w, w,
                                                       new Fill(BackgroundColor), //new Color(0.2f, 0.9f, 0.2f, 1f)),
                                                       new Stroke(weight) { Color = StrokeColor }
                );
        }

        public static void ApplyThumbStyle(ProgramaticStyle programaticStyle, int w, int weight)
        {
            programaticStyle.Border = new RectOffset(weight + 1, weight + 1, weight + 1, weight + 1);

            //programaticStyle.FontSize = 30;
            //programaticStyle.NormalTextColor = Color.grey;
            programaticStyle.NormalGraphics = new Rect(w, w,
                                                       new Fill(NormalColor),
                                                       new Stroke(weight) { Color = StrokeColor }
                );
            //programaticStyle.HoverTextColor = Color.white;
            programaticStyle.HoverGraphics = new Rect(w, w,
                                                      new Fill(HoverColor),
                                                      new Stroke(weight) { Color = StrokeColor }
                );
            //programaticStyle.ActiveTextColor = Color.white;
            programaticStyle.ActiveGraphics = new Rect(w, w,
                                                       new Fill(ActiveColor),
                                                       new Stroke(weight) { Color = StrokeColor }
                );
        }

        public static void ApplyButtonStyle(ProgramaticStyle programaticStyle, int w, int weight, TriangleDirection direction)
        {
            programaticStyle.Border = new RectOffset(weight + 1, weight + 1, weight + 1, weight + 1);

            var size = new Point(w, w);
            int pixels = w <= 20 ? 4 : 5; // the "height" of the triangle in pixels

            //programaticStyle.FontSize = 30;
            //programaticStyle.NormalTextColor = Color.grey;
            programaticStyle.NormalGraphics = new GraphicGroup(w, w,
                                                               new Rect(
                                                                   new Fill(NormalColor),
                                                                   new Stroke(weight) { Color = StrokeColor }
                                                               ),
                                                               new Triangle(new Fill(ColorMixer.FromHex(0x666666).ToColor())) { Direction = direction, Pixels = pixels }
                );
            //programaticStyle.HoverTextColor = Color.white;
            programaticStyle.HoverGraphics = new GraphicGroup(w, w,
                                                              new Rect(
                                                                  new Fill(HoverColor),
                                                                  new Stroke(weight) { Color = StrokeColor }
                                                               ),
                                                              new Triangle(new Fill(Color.black)) { Direction = direction, Pixels = pixels }
                );
            //programaticStyle.ActiveTextColor = Color.white;
            programaticStyle.ActiveGraphics = new GraphicGroup(w, w,
                                                               new Rect(
                                                                   new Fill(ActiveColor),
                                                                   new Stroke(weight) { Color = StrokeColor }
                                                               ),
                                                               new Triangle(new Fill(ColorMixer.FromHex(0x333333).ToColor())) { Direction = direction, Pixels = pixels }
                );
        }

        
    }
}