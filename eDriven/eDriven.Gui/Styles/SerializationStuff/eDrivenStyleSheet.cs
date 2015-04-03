using System.Reflection;
using eDriven.Gui.Reflection;
using eDriven.Gui.Styles.Serialization;
using UnityEngine;

namespace eDriven.Gui.Styles
{
    /// <summary>
    /// eDrivenGui style sheet wrapper object
    /// </summary>
    [Obfuscation(Exclude = true)]
// ReSharper disable once InconsistentNaming
    public class eDrivenStyleSheet : MonoBehaviour
// ReSharper restore UnusedMember.Global
    {
        /// <summary>
        /// Stylesheet name
        /// </summary>
        [Saveable]
// ReSharper disable UnusedMember.Global
        public string Name = "Style sheet";
// ReSharper restore UnusedMember.Global

        /// <summary>
        /// Expanded
        /// </summary>
        [Saveable]
        // ReSharper disable UnusedMember.Global
        public bool Expanded = true; // expanded when created from scratch!
        // ReSharper restore UnusedMember.Global

        /*/// <summary>
        /// Editable
        /// </summary>
        [Saveable]
        // ReSharper disable UnusedMember.Global
        public bool Editable;
        // ReSharper restore UnusedMember.Global*/

        /// <summary>
        /// Editable
        /// </summary>
        [Saveable]
        // ReSharper disable UnusedMember.Global
        public bool Locked;
        // ReSharper restore UnusedMember.Global

        ///<summary>
        /// Style sheet
        ///</summary>
        [Saveable]
// ReSharper disable UnusedMember.Global
        public StyleSheet StyleSheet;
// ReSharper restore UnusedMember.Global

// ReSharper disable once UnusedMember.Local
        void OnEnable()
        {
            //LogUtil.PrintCurrentMethod();
            //StyleInitializer2.Run();
            StyleInitializer.Run();
        }
    }
}