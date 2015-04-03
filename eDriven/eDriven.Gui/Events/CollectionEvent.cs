using System.Collections.Generic;
using eDriven.Core.Events;

namespace eDriven.Gui.Events
{
    /// <summary>
    /// Collection event
    /// </summary>
    public class CollectionEvent : Event
    {
// ReSharper disable UnusedMember.Global
        ///<summary>
        ///</summary>
// ReSharper disable InconsistentNaming
        public const string COLLECTION_CHANGE = "collectionChange";
// ReSharper restore InconsistentNaming
// ReSharper restore UnusedMember.Global

        #region Properties

        ///<summary>
        ///</summary>
        public CollectionEventKind Kind;

        ///<summary>
        ///</summary>
        public List<object> Items;

        ///<summary>
        ///</summary>
        public int Location;

        ///<summary>
        ///</summary>
        public int OldLocation;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public CollectionEvent(string type)
            : base(type)
        {
            Items = new List<object>();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public CollectionEvent(string type, List<object> items)
            : base(type)
        {
            Items = items ?? new List<object>();
        }

        #endregion

        public override string ToString()
        {
            return string.Format("{0}, Kind: {1}, Location: {2}, OldLocation: {3}, Items: {4}", base.ToString(), Kind, Location, OldLocation, Items);
        }

    }
}