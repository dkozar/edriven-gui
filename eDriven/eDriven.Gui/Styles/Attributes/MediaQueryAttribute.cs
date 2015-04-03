using System;

namespace eDriven.Gui.Styles
{
    /// <summary>
    /// Indicates that a method is used by the media query
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class MediaQueryAttribute : Attribute
    {
        /// <summary>
        /// Media query ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Not all the queries are running "equal" in the build and in the editor<br/>
        /// One of the examples is the Screen.width which returns the width of the current window, which doeesn't necesarilly have to be the game window<br/>
        /// Set to true if this is an editor override
        /// </summary>
        public bool EditorOverride { get; set; }

        /// <summary>
        /// The array of parameter descriptions
        /// </summary>
        public string[] Parameters { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="parameters"></param>
        public MediaQueryAttribute(string id, params string[] parameters)
        {
            Id = id;
            Parameters = parameters;
        }
    }
}