using System.Collections.Generic;
using eDriven.Gui.Styles;

namespace eDriven.Gui.Components
{
    /// <summary>
    /// 
    /// </summary>
    public interface IStyleClient : ISimpleStyleClient
    {
        /// <summary>
        /// Style mapper being used for styling this component and/or its children
        /// </summary>
        //object StyleName { get; set; }

        /// <summary>
        /// Inheriting styles
        /// </summary>
        StyleTable InheritingStyles { get; set; }
        
        /// <summary>
        /// Non-inheriting styles
        /// </summary>
        StyleTable NonInheritingStyles { get; set; }

        /// <summary>
        /// Local style declaration
        /// </summary>
        StyleDeclaration StyleDeclaration { get; set; }

        /// <summary>
        /// Gets the style specified with a supplied style name
        /// </summary>
        /// <param name="styleName"></param>
        /// <returns></returns>
        object GetStyle(string styleName);

        /// <summary>
        /// Sets the style
        /// </summary>
        /// <param name="styleName"></param>
        /// <param name="style"></param>
        void SetStyle(string styleName, object style);

        /// <summary>
        /// Clears the style
        /// </summary>
        /// <param name="styleName"></param>
        void ClearStyle(string styleName);

        /// <summary>
        /// Returns true if the component has the style defined
        /// </summary>
        /// <param name="styleName"></param>
        /// <returns></returns>
        bool HasStyle(string styleName);

        /// <summary>
        /// Regenerates style cache
        /// </summary>
        /// <param name="recursive"></param>
        void RegenerateStyleCache(bool recursive);

        /// <summary>
        /// Propagates style change to children
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="value"></param>
        /// <param name="recursive"></param>
        void NotifyStyleChangeInChildren(string prop, object value, bool recursive);

        ///<summary>
        ///</summary>
        ///<returns></returns>
        List<StyleDeclaration> GetClassStyleDeclarations();

        ///<summary>
        ///</summary>
        IStyleClient StyleParent { get; }

        ///<summary>
        ///</summary>
        ///<param name="cssState"></param>
        ///<returns></returns>
        //bool MatchesCSSState(string cssState); // TEMP commented

        ///<summary>
        ///</summary>
        ///<param name="cssState"></param>
        ///<returns></returns>
        bool MatchesCSSType(string cssState);
    }
}