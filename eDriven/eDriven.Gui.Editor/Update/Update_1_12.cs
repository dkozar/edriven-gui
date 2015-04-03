/*using System;
using System.Collections.Generic;
using eDriven.Gui.Designer;
using eDriven.Gui.Designer.Adapters;
using eDriven.Gui.Editor.Persistence;
using UnityEngine;
using Object=UnityEngine.Object;

namespace eDriven.Gui.Editor.Update
{
// ReSharper disable InconsistentNaming
    internal class Update_1_12 : IUpdate
// ReSharper restore InconsistentNaming
    {
#if DEBUG
        // ReSharper disable UnassignedField.Global
        public static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif

        public void Process()
        {
            Debug.Log(string.Format("Processing " + this));
            Object[] objects = Object.FindObjectsOfType(typeof(StageAdapter));

            List<GroupAdapter> stageAdapters = new List<GroupAdapter>();
            foreach (var o in objects)
            {
                stageAdapters.Add((GroupAdapter)o);
            }

//#if DEBUG
//            PersistenceManager.DebugMode = true;
//#endif

            Iterate(stageAdapters);

//#if DEBUG
//            PersistenceManager.DebugMode = false;
//#endif
        }

        private static void Iterate(IEnumerable<GroupAdapter> componentAdapterList)
        {
            //Debug.Log(string.Format("Iterating {0} Stage objects", componentAdapterList.Count));
            bool foundChanges = false;
            foreach (GroupAdapter adapter in componentAdapterList)
            {
                //Debug.Log("adapter.LayoutOrder: " + adapter.LayoutOrder.Count);
                //if (adapter.LayoutOrder.Count > 0 && adapter.ChildAdapters.Count == 0)
                if (adapter.transform.childCount > 0) // && adapter.ChildAdapters.Count == 0)
                {
                    if (ConvertAdapterData(adapter))
                        foundChanges = true;
                }
            }

            if (foundChanges)
            {
                Debug.LogWarning("Serialization update pending. Please stop the play mode, save the scene and restart.");
            }
        }

        private static bool ConvertAdapterData(GroupAdapter groupAdapter)
        {
            var transform = groupAdapter.transform;
            var childCount = transform.childCount;
            bool foundChanges = false;

            FixBlankListMembers(groupAdapter);

            if (HasMappingErrors(groupAdapter))
            {
                //Debug.Log("Converting adapter data: " + Report(containerAdapter), containerAdapter);

                foundChanges = true;

                // monitor before convert
                //Selection.activeTransform = transform;
                PersistenceManager.Instance.Watch(groupAdapter);

                List<Transform> allChildTransforms = new List<Transform>();
                for (int i = 0; i < groupAdapter.transform.childCount; i++)
                {
                    allChildTransforms.Add(groupAdapter.transform.GetChild(i));
                }

                groupAdapter.ContentChildren.Clear();

                // put transforms from LayoutOrder
                foreach (string s in groupAdapter.LayoutOrder.Items)
                {
                    if (!groupAdapter.GuidToTransformDict.ContainsKey(s))
                        continue;

                    Transform childTransform = groupAdapter.GuidToTransformDict[s];
                    if (!childTransform)
                        continue;
                }

                // put transforms not found in LayoutOrder
                foreach (Transform childTransform in allChildTransforms)
                {
                    ComponentAdapter adapter = GuiLookup.GetAdapter(childTransform);
                    groupAdapter.ContentChildren.Add(adapter);
                }
                // Note: Important! The List collection needs to be reapplied to be able to participate in the persistance
                groupAdapter.ContentChildren = new List<ComponentAdapter>(groupAdapter.ContentChildren);

                //Debug.Log("    -> done: " + Report(containerAdapter), containerAdapter);
            }

            // children
            for (int i = 0; i < childCount; i++)
            {
                Transform childTransform = transform.GetChild(i);
                ComponentAdapter adapter = GuiLookup.GetAdapter(childTransform);
                GroupAdapter ca = adapter as GroupAdapter;
                if (null != ca)
                    ConvertAdapterData(ca);
            }

            //FixDuplicatedMembers(containerAdapter);

            return foundChanges;
        }

        private static bool HasMappingErrors(GroupAdapter adapter)
        {
            if (adapter is PanelAdapter)
            {
                var panelAdapter = (PanelAdapter)adapter;
                return adapter.transform.childCount != panelAdapter.ContentChildren.Count + panelAdapter.ToolGroupChildren.Count;
            }

            if (adapter is DialogAdapter)
            {
                var dialogAdapter = (DialogAdapter)adapter;
                return adapter.transform.childCount != dialogAdapter.ContentChildren.Count + dialogAdapter.ToolGroupChildren.Count + dialogAdapter.ControlBarGroupChildren.Count;
            }

            return false;
        }

        private static void FixBlankListMembers(GroupAdapter adapter)
        {
            for (int i = adapter.ContentChildren.Count - 1; i >= 0; i--)
            {
                if (adapter.ContentChildren[i] == null)
                    adapter.ContentChildren.RemoveAt(i);
            }

            adapter.ContentChildren = new List<ComponentAdapter>(adapter.ContentChildren);

            if (adapter is PanelAdapter)
            {
                var panelAdapter = (PanelAdapter)adapter;

                for (int i = panelAdapter.ToolGroupChildren.Count - 1; i >= 0; i--)
                {
                    if (panelAdapter.ToolGroupChildren[i] == null)
                        panelAdapter.ToolGroupChildren.RemoveAt(i);
                }

                panelAdapter.ToolGroupChildren = new List<ComponentAdapter>(panelAdapter.ToolGroupChildren);
            }

            if (adapter is DialogAdapter)
            {
                var dialogAdapter = (DialogAdapter)adapter;

                for (int i = dialogAdapter.ControlBarGroupChildren.Count - 1; i >= 0; i--)
                {
                    if (dialogAdapter.ControlBarGroupChildren[i] == null)
                        dialogAdapter.ControlBarGroupChildren.RemoveAt(i);
                }

                dialogAdapter.ControlBarGroupChildren = new List<ComponentAdapter>(dialogAdapter.ControlBarGroupChildren);
            }
        }

        private static void FixDuplicatedMembers(GroupAdapter adapter)
        {
            if (adapter is PanelAdapter)
            {
                var panelAdapter = (PanelAdapter)adapter;

                for (int i = panelAdapter.ToolGroupChildren.Count - 1; i >= 0; i--)
                {
                    ComponentAdapter child = panelAdapter.ToolGroupChildren[i];
                    if (adapter.ContentChildren.Contains(child))
                        adapter.ContentChildren.Remove(child);
                }
            }

            if (adapter is DialogAdapter)
            {
                var dialogAdapter = (DialogAdapter)adapter;

                for (int i = dialogAdapter.ControlBarGroupChildren.Count - 1; i >= 0; i--)
                {
                    ComponentAdapter child = dialogAdapter.ToolGroupChildren[i];
                    if (adapter.ContentChildren.Contains(child))
                        adapter.ContentChildren.Remove(child);
                }
            }
        }

        private static string Report(GroupAdapter adapter)
        {
            return string.Format(@"adapter: {0} {4}
    transform.childCount: {1}; ContentChildren.Count: {2}; LayoutOrder.Count: {3}", 
                adapter, adapter.transform.childCount, adapter.ContentChildren.Count, adapter.LayoutOrder.Count,
                HasMappingErrors(adapter) ? "***" : string.Empty);
        }
    }
}*/