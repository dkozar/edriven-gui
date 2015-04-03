namespace eDriven.Gui.Data
{
    public interface ICollectionView
    {
        /// <summary>
        /// Filter function
        /// </summary>
        FilterFunction FilterFunction { get; set; }

        /// <summary>
        /// Sort
        /// </summary>
        Sort Sort { get; set; }

        /// <summary>
        /// Returns true if the collection contains the item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool Contains(object item);

        /// <summary>
        /// Notify the view that an item has been updated
        /// </summary>
        /// <param name="item"></param>
        /// <param name="property"></param>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        void ItemUpdated(object item, object property = null, object oldValue = null, object newValue = null);

        /// <summary>
        /// Applies the sort and filter to the view (refreshes the view)
        /// </summary>
        /// <returns></returns>
        bool Refresh();
    }
}