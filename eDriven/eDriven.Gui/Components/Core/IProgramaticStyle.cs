using UnityEngine;

namespace eDriven.Gui.Components
{
    public interface IProgramaticStyle
    {
        /// <summary>
        /// NormalColor background
        /// </summary>
        //Texture2D NormalBackground { get; set; }

        /// <summary>
        /// NormalColor background
        /// </summary>
        GUIStyleState Normal { get; set; }

        /// <summary>
        /// NormalColor background
        /// </summary>
        GUIStyleState Hover { get; set; }

        /// <summary>
        /// NormalColor background
        /// </summary>
        GUIStyleState Active { get; set; }

        /// <summary>
        /// NormalColor background
        /// </summary>
        GUIStyleState Focused { get; set; }

        /// <summary>
        /// Border
        /// </summary>
        RectOffset Border { get; set; }

        GUIStyle Style { get; set; }
        
        void Invalidate();
        
        void Commit();
    }
}