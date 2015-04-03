using eDriven.Core.Events;

namespace eDriven.Gui.Data
{
    /// <summary>
    /// Observable collection of items organized in an ordinal fashion
    /// </summary>
    public interface IList : IEventDispatcher
    {
        /// <summary>
        /// The number of items in this collection. 0 means no items while -1 means the length is unknown. 
        /// </summary>
        int Length { get; }

        /// <summary>
        /// Adds the specified item to the end of the list
        /// </summary>
        /// <param name="item"></param>
        void AddItem(object item);

        /// <summary>
        /// Adds the item at the specified index
        /// </summary>
        /// <param name="item"></param>
        /// <param name="index"></param>
        void AddItemAt(object item, int index);

        ///<summary>
        /// Gets the item at the specified index
        ///</summary>
        ///<param name="index"></param>
        ///<returns></returns>
        object GetItemAt(int index);

        ///<summary>
        /// Returns the index of the item if it is in the list such that GetItemAt(index) == item
        ///</summary>
        ///<param name="item"></param>
        ///<returns></returns>
        int GetItemIndex(object item);

        ///<summary>
        /// Notifies the view that an item has been updated
        ///</summary>
        ///<param name="item">The item being updated</param>
        ///<param name="property">The property being updated</param>
        ///<param name="oldValue">Old value</param>
        ///<param name="newValue">New value</param>
        void ItemUpdated(object item, object property /* = null*/,
                         object oldValue /* = null*/,
                         object newValue /* = null*/);

        ///<summary>
        /// Removes all items from the list
        ///</summary>
        void RemoveAll();

        ///<summary>
        /// Removes the item at the specified index and returns it.<br/>
        /// Any items that were after this index are now one index earlier.
        ///</summary>
        ///<param name="index"></param>
        ///<returns></returns>
        object RemoveItemAt(int index);

        ///<summary>
        /// Places the item at the specified index.<br/>
        /// If an item was already at that index the new item will replace it and it will be returned.
        ///</summary>
        ///<param name="item"></param>
        ///<param name="index"></param>
        ///<returns></returns>
        object SetItemAt(object item, int index);

        ///<summary>
        /// Returns an array of items that is populated in the same order
        ///</summary>
        ///<returns></returns>
        object[] ToArray();
    }
}
