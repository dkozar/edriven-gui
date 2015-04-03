using System;
using System.Reflection;
using UnityEngine;

namespace eDriven.Gui.Styles
{
    [Obfuscation(Exclude = true)]
    [Serializable]
    public class SkinStyleReference : ICloneable
    {
// ReSharper disable UnusedMember.Global
        ///<summary>
        ///</summary>
// ReSharper disable InconsistentNaming
        public static string SKIN = "Skin";
// ReSharper restore InconsistentNaming
// ReSharper restore UnusedMember.Global

        /// <summary>
        /// Skin to reference the style from
        /// </summary>
        public GUISkin Skin;

// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming
        ///<summary>
        ///</summary>
        public static string STYLE_NAME = "StyleName";
// ReSharper restore InconsistentNaming
// ReSharper restore UnusedMember.Global

        /// <summary>
        /// The name of the style to reference
        /// </summary>
        public string StyleName;

        #region Equals

        public bool Equals(SkinStyleReference other)
        {
            if (ReferenceEquals(null, other)) return false;
            return Equals(other.Skin, Skin) && Equals(other.StyleName, StyleName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof (SkinStyleReference)) return false;
            return Equals((SkinStyleReference) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Skin != null ? Skin.GetHashCode() : 0)*397) ^ (StyleName != null ? StyleName.GetHashCode() : 0);
            }
        }

        #endregion

        #region ICloneable

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public object Clone()
        {
            SkinStyleReference clone = new SkinStyleReference {Skin = Skin, StyleName = StyleName};
            return clone;
        }

        #endregion

    }
}