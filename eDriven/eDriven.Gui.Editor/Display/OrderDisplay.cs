#region License

/*
 
Copyright (c) 2010-2014 Danko Kozar

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
 
*/

#endregion License

using System.Collections.Generic;
using eDriven.Core.Signals;
using eDriven.Core.Util;
using eDriven.Gui.Components;
using eDriven.Gui.Containers;
using eDriven.Gui.Designer;
using eDriven.Gui.Designer.Adapters;
using eDriven.Gui.Editor.Dialogs;
using eDriven.Gui.Editor.Display.Order;
using eDriven.Gui.Editor.Hierarchy;
using eDriven.Gui.Editor.List;
using eDriven.Gui.Editor.Processing;
using eDriven.Gui.Editor.Rendering;
using UnityEngine;
using UnityEditor;
using Object=UnityEngine.Object;

namespace eDriven.Gui.Editor.Display
{
    internal class OrderDisplay : DisplayBase
    {
#if DEBUG
        // ReSharper disable UnassignedField.Global
        public static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif

        #region Singleton

        private static OrderDisplay _instance;

        /// <summary>
        /// Singleton class for handling focus
        /// </summary>
        private OrderDisplay()
        {
            // Constructor is protected
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static OrderDisplay Instance
        {
            get
            {
                if (_instance == null)
                {
#if DEBUG
                    if (DebugMode)
                        Debug.Log(string.Format("Instantiating OrderDisplay instance"));
#endif
                    _instance = new OrderDisplay();
                    _instance.Initialize();
                }

                return _instance;
            }
        }

        #endregion

        public readonly Signal CollectionChangedSignal = new Signal();

        public static float ElementHeight = 31;

        /// <summary>
        /// Initializes the Singleton instance
        /// </summary>
        public override void Initialize()
        {
            //Debug.Log("OrderDisplay Initialize");
            _list = new DraggableList { ScrollbarWidth = 15, ElementHeight = ElementHeight };
            _list.RemovedSignal.Connect(RemovedSlot);
            _list.PositionChangedSignal.Connect(PositionChangedSlot);

            EditorState.Instance.DepthChangeSignal.Connect(DepthChangeSlot);

            //if (null == ContainerAdapter)
            //    return;
        }

        #region Slots

        private void PositionChangedSlot(object[] parameters)
        {
            _positionChangedParms = parameters;
        }

        private void RemovedSlot(object[] parameters)
        {
            _removedParms = parameters;
        }

        private void DepthChangeSlot(object[] parameters)
        {
            _depthChangedParms = parameters;
        }

        #endregion

        #region Members

        private static DraggableList _list;
        //private ChildGroupList _data; // = new ChildGroupList();

        private bool _clicked;
        private static bool _depthMode;
        
        private Rect _lastRect;
        private Rect _displayRect;

        private bool _oldEnabled;

        private GroupManager _groupManager;

        Rect _rect;

        /// <summary>
        /// Set by the position changed slot
        /// Checked from the Update
        /// </summary>
        private object[] _positionChangedParms;

        /// <summary>
        /// Set by the removed slot
        /// Checked from the Update
        /// </summary>
        private object[] _removedParms;

        /// <summary>
        /// Set by the depth changed slot
        /// Checked from the Update
        /// </summary>
        private object[] _depthChangedParms;

        //private System.Collections.Generic.List<ChildGroupDescriptor> _groupDescriptors;

        #endregion

        #region Actions

        // we have to call it, because of deleting the currently selected container
        protected override void HandleHierarchyChange()
        {
            //LogUtil.PrintCurrentMethod();
            Refresh();
        }

        protected override void HandleSelectionChange()
        {
            //LogUtil.PrintCurrentMethod();
            Refresh();
        }

// ReSharper disable MemberCanBeMadeStatic.Global
        /// <summary>
        /// Used to clear the list before the hierarchy change
        /// If this is not done, we might get some exception regarding the reference to the non-existing adapter
        /// </summary>
        internal void Clear()
// ReSharper restore MemberCanBeMadeStatic.Global
        {
            //Debug.Log("*** CLEAR ***");
            _list.Data = new ChildGroupRenderingList();
        }

        public void Refresh()
        {
            //Debug.Log("*** REFRESH ***");
            if (null != GroupAdapter)
            {
                _groupManager = new GroupManager(GroupAdapter); // only if this is a container
            }
            
            DisplayData();
        }

        internal override void OnLostFocus()
        {
            
        }

        /// <summary>
        /// Changes the display of child descriptors
        /// </summary>
        //private void DisplayData(IEnumerable<ComponentAdapter> fieldInfoList)
        private void DisplayData()
        {
            if (null == _list)
                return;

            if (null == GroupAdapter)
            {
                _list.Data = null;
                return;
            }

            _list.Data = OrderDisplayBuilder.BuildContentGroups(GroupAdapter);
        }

        #endregion

        #region Methods

        public override void Render()
        {
            // mode switch
            EditorGUILayout.BeginHorizontal(StyleCache.Instance.Toolbar);

            _oldEnabled = GUI.enabled;
            GUI.enabled = null != Adapter;

            _clicked = GUILayout.Toggle(!_depthMode, new GUIContent("Order", TextureCache.Instance.OrderSelected), StyleCache.Instance.Toggle, GUILayout.Height(30), GUILayout.ExpandWidth(false));
            if (_clicked && _depthMode)
            {
                _depthMode = false;
            }

            GUI.enabled = _oldEnabled;

            _oldEnabled = GUI.enabled;
            GUI.enabled = false;

            _clicked = GUILayout.Toggle(_depthMode, new GUIContent("Depth", TextureCache.Instance.Depth), StyleCache.Instance.Toggle, GUILayout.Height(30), GUILayout.ExpandWidth(false));
            if (_clicked && !_depthMode)
            {
                _depthMode = true;
            }

            GUI.enabled = _oldEnabled;

            GUILayout.FlexibleSpace();

            // Note: we need to always allow the addition of a Stage
            if (GUILayout.Button(GuiContentCache.Instance.AddChildControlContent, StyleCache.Instance.Toggle, GUILayout.Height(30), GUILayout.ExpandWidth(false)))
            {
                ToolboxDialog.Instance.ShowUtility();
            }

            _oldEnabled = GUI.enabled;
            GUI.enabled = null != GroupAdapter;

            if (GUILayout.Button(GuiContentCache.Instance.RemoveAllChildrenButton, StyleCache.Instance.Button, GUILayout.Height(30)))
            {
                if (EditorUtility.DisplayDialog("Remove all?", "Are you sure you want to remove all children?", "OK", "Cancel"))
                {
                    if (null != GroupAdapter)
                    {
                        //Debug.Log("Removing all children...");
                        GroupAdapter.RemoveAllChildren();
                    }
                }
            }

            GUI.enabled = _oldEnabled;

            EditorGUILayout.EndHorizontal();

            if (null == _list.Data)
            {
                GUILayout.Label(GuiContentCache.Instance.NotAContainerContent, StyleCache.Instance.CenteredLabel, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                return;
            }

            int count = _list.Data.RenderingItemsCount;
            if (0 == count)
            {
                GUILayout.Label(GuiContentCache.Instance.NoComponentsInTheContainer, StyleCache.Instance.CenteredLabel, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                return;
            }

            _rect = GUILayoutUtility.GetLastRect();
// ReSharper disable once CompareOfFloatsByEqualityOperator
            if (_rect.width != 1 && _rect != _lastRect)
            {
                _lastRect = _rect;
                //_displayRect = new Rect(Padding, _lastRect.yMax + Padding, Bounds.width - Padding * 2, Bounds.height - _lastRect.yMax);
                _displayRect = new Rect(_lastRect.x + 1, _lastRect.yMax + 1, _lastRect.width - 2, Bounds.height - _lastRect.yMax);
                _list.UpdateLayout(_displayRect.width, _displayRect.height);
            }

            // wrap list inside area because it is not using GUILayout
            GUILayout.BeginArea(_displayRect);
            _list.Render();
            GUILayout.EndArea();
        }

        /// <summary>
        /// Checks the flags and executes the change received via the slots
        /// (the change that is not healthy to do inside the Render method)
        /// </summary>
        public override void Update()
        {
            if (null != _positionChangedParms)
            {
                //Debug.Log(string.Format("PositionChangedSlot: {0}, {1}", parameters[0], parameters[1]));
                DraggableItem draggableItem = (DraggableItem)_positionChangedParms[0];
                int newItemIndex = (int)_positionChangedParms[1];
                int newGroupIndex = (int)_positionChangedParms[2];

                _positionChangedParms = null;

                ComponentAdapter adapter = (ComponentAdapter)draggableItem.Data;

                // Note: this actually applies changes to collections
                var pack = _groupManager.Reorder(adapter, newGroupIndex, newItemIndex);

                ParentChildLinker.Instance.Update(GroupAdapter, pack);

                //Refresh();

                if (Application.isPlaying)
                {
                    //ParentChildLinker.Instance.Update(ContainerAdapter, pack); // TODO: calling ParentChildLinker from here messes the thing
                    RedrawComponentsInGameView(pack);
                }

                //HierarchyState.Instance.Rebuild();
                EditorState.Instance.HierarchyChange();

                /**
                 * Emit for debugging purposes
                 * */
                CollectionChangedSignal.Emit();
            }

            else if (null != _removedParms)
            {
                //Debug.Log(string.Format("RemovedSlot: {0}", parameters[0]));
                DraggableItem draggableItem = (DraggableItem)_removedParms[0];
                //int groupIndex = (int)_removedParms[1];
                //int itemIndex = (int)_removedParms[2];

                _removedParms = null;

                ComponentAdapter adapter = draggableItem.Data as ComponentAdapter;

                if (null == adapter)
                    return;

                //Debug.Log(string.Format("groupIndex: {0}, itemIndex: {1}", groupIndex, itemIndex));

                /**
                 * 1. Remove the adapter from the collection
                 * */
                var pack = _groupManager.Remove(adapter);

                /**
                 * NOTE: We should not call the linker directly
                 * That is because if we remove adapter from stage, the linker could not find it
                 * We call the EditorState.Instance.HierarchyChange() below, so the
                 * hierarchy change mechanism handles the removal stuff
                 * */
                //ParentChildLinker.Instance.Update(GroupAdapter, pack);

                if (Application.isPlaying)
                {
                    RedrawComponentsInGameView(pack);
                }

                if (adapter.transform.parent != GroupAdapter.transform)
                    return;

                //Refresh();
                
                /**
                 * 2. Check if the same adapter present in other collections
                 * If not, remove the transform
                 * */
                var group = _groupManager.GetGroupContainingAdapter(adapter);
                //Debug.Log("group: " + group);
                if (null == group)
                {
                    // this adapter is not present in any of the groups
                    // remove the transform now
                    OrderDisplayRow orderDisplayRow = (OrderDisplayRow)draggableItem;

                    Transform transform = orderDisplayRow.Adapter.transform;
                    GroupAdapter groupAdapter = transform.parent.GetComponent<GroupAdapter>();
                    groupAdapter.RemoveChild(orderDisplayRow.Adapter);

                    //int instanceId = transform.GetInstanceID();
                    Object.DestroyImmediate(transform.gameObject);
                }

                // apply the change, for not to get the "ArgumentException: Getting control 3's position in a group with only 3 controls when doing Repaint Aborting" error
                Refresh();

                //EditorState.Instance.HierarchyChange();

                /**
                 * Emit for debugging purposes
                 * */
                CollectionChangedSignal.Emit();
            }

            if (null != _depthChangedParms)
            {
                //Debug.Log("OrderDisplay: changing depth");
                // apply the change, for not to get the "ArgumentException: Getting control 3's position in a group with only 3 controls when doing Repaint Aborting" error
                //Refresh();

                _depthChangedParms = null;
            }
        }

        #endregion

        #region Helper

        private void RedrawComponentsInGameView(ChildGroupPack pack)
        {
            for (int i = 0; i < pack.Groups.Count; i++)
            {
                ChildGroupDescriptor groupDescriptor = _groupManager.GroupDescriptors[i];

                List<ComponentAdapter> adaptersCollection = groupDescriptor.GetChildAdaptersCollection(GroupAdapter);
                
                if (null == adaptersCollection)
                    return;

                // TODO: move to parent child linker
                var targetContainer = groupDescriptor.GetTargetContainer((IContentChildList)GroupAdapter.Component);
                if (null == targetContainer)
                    continue; // not instantiated

                targetContainer.RemoveAllContentChildren();

                foreach (ComponentAdapter adapter in adaptersCollection)
                {
                    if (null != adapter && adapter.gameObject.activeInHierarchy && adapter.enabled) // the adapter could be disabled in many ways
                        targetContainer.AddContentChild(adapter.Component);
                }
            }
        }

        #endregion
    }
}