/*
using System;
using eDriven.Core.Events;
using eDriven.Gui.Events;

namespace eDriven.Gui.Util
{
    public class SelectionModel : EventDispatcher
    {
        // ReSharper disable InconsistentNaming
        public const string CARET_INDEX_CHANGED = "caretIndexChanged";
        // ReSharper restore InconsistentNaming

        /// <summary>
        /// The flag indicating that in this model something has to be selected (index has to be greater then -1 if there are any items)
        /// </summary>
        public bool RequireSelection;

        private int _itemCount;
        /// <summary>
        /// Item count
        /// </summary>
        public int ItemCount
        {
            get
            {
                return _itemCount;
            }
            set
            {
                if (value < 0)
                    throw new Exception("ItemCount couldn't be less than 0");
                _itemCount = value;
            }
        }

        private int _selectedIndex = -1;
        /// <summary>
        /// Selected index
        /// </summary>
        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                if (value == _selectedIndex)
                    return;

                if (value < -1)
                    _selectedIndex = -1;
                else
                    _selectedIndex = value;

                _caretIndex = value;
            }
        }

        private int _caretIndex = 0; //-1;
        public int CaretIndex
        {
            get { return _caretIndex; }
            set
            {
                _caretIndex = value;
            }
        }

        private int _previousSelectedIndex = -1;
        private int _previousCaretIndex = -1;

        private bool _selectedIndexChanged;
        private bool _caretIndexChanged;

        /// <summary>
        /// Changes the selected index and returns true if it actually changed
        /// </summary>
        /// <returns></returns>
        public bool Apply()
        {
            //Debug.Log(string.Format("Applying. Item count: {0}, SelectedIndex: {1}", ItemCount, SelectedIndex));

            if (-1 == SelectedIndex)
            {
                if (RequireSelection && ItemCount > 0)
                    SelectedIndex = 0;
            }
            else if (SelectedIndex > ItemCount - 1)
            {
                if (RequireSelection && ItemCount > 0)
                    SelectedIndex = ItemCount - 1;
                else
                    SelectedIndex = -1;
            }
            else if (SelectedIndex < -1)
            {
                if (RequireSelection && ItemCount > 0)
                    SelectedIndex = 0;
                else
                    SelectedIndex = -1;
            }

            if (CaretIndex < 0)
            {
                CaretIndex = 0;
            }
            else if (CaretIndex > ItemCount - 1)
            {
                CaretIndex = ItemCount == 0 ? 0 : ItemCount - 1;
            }

            _selectedIndexChanged = _previousSelectedIndex != SelectedIndex;
            _caretIndexChanged = _previousCaretIndex != CaretIndex;

            //if (_selectedIndexChanged && null != SelectedIndexChangeCallback)
            //    SelectedIndexChangeCallback(SelectedIndex);

            if (_selectedIndexChanged && HasEventListener(IndexChangeEvent.CHANGE))
            {
                //Debug.Log("Dispatching CHANGE: " + SelectedIndex);
                IndexChangeEvent ice = new IndexChangeEvent(IndexChangeEvent.CHANGE)
                {
                    NewIndex = SelectedIndex,
                    OldIndex = _previousSelectedIndex
                };
                DispatchEvent(ice);
            }

            if (_caretIndexChanged && HasEventListener(CARET_INDEX_CHANGED))
            {
                //Debug.Log("Dispatching CARET_INDEX_CHANGED: " + CaretIndex);
                IndexChangeEvent ice = new IndexChangeEvent(CARET_INDEX_CHANGED)
                {
                    NewIndex = CaretIndex,
                    OldIndex = _previousCaretIndex
                };
                DispatchEvent(ice);
            }

            _previousSelectedIndex = SelectedIndex;
            _previousCaretIndex = CaretIndex;

            return _selectedIndexChanged;
        }

        public void SelectCaretPosition()
        {
            //Debug.Log("SelectCaretPosition: " + _caretIndex);
            SelectedIndex = _caretIndex;
            Apply();
        }
    }
}
*/
