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

using System;
using System.Collections.Generic;
using System.Text;
using eDriven.Gui.Designer;
using eDriven.Gui.Designer.Adapters;
using eDriven.Gui.Editor.Persistence;
using UnityEngine;

namespace eDriven.Gui.Editor.Hierarchy
{
    internal class ChildGroupPack //: ICloneable
    {
#if DEBUG
// ReSharper disable UnassignedField.Global
        /// <summary>
        /// Debug mode
        /// </summary>
        public static bool DebugMode;

// ReSharper restore UnassignedField.Global
#endif

        private readonly List<ChildGroup> _groups = new List<ChildGroup>();

        private readonly GroupAdapter _parentAdapter;
        public GroupAdapter ParentAdapter
        {
            get { return _parentAdapter; }
        }

        private ChildGroupPack(GroupAdapter parentAdapter)
        {
            _parentAdapter = parentAdapter;
        }

        public List<ChildGroup> Groups
        {
            get
            {
                return _groups;
            }
        }

        public void Add(ChildGroup group)
        {
            _groups.Add(group);
        }

        //public void Add(List<ComponentAdapter> adapters)
        //{
        //    _groups.Add(new ChildGroup(adapters)); // Beware: the bug was here - internally the changes were immediatelly applied to collections
        //    //_groups.Add(new ChildGroup(ListUtil<ComponentAdapter>.Clone(adapters))); //TODO??
        //}

        public int GetGroupHostingAdapterIndex(ComponentAdapter adapter)
        {
            for (int i = 0; i < Groups.Count; i++)
            {
                bool contains = Groups[i].Adapters.Contains(adapter);
                if (contains)
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// Removes records having null reference
        /// Null references appear after child component removed from th ehierarchy
        /// </summary>
        public void Consolidate()
        {
            for (int i = 0; i < Groups.Count; i++)
            {
                var group = Groups[i];
                for (int j = group.Adapters.Count - 1; j >= 0; j--)
                {
                    if (null == group.Adapters[j])
                    {
#if DEBUG
                        if (DebugMode)
                        {
                            Debug.Log("Removing null during the consolidation");
                        }
#endif
                        group.Adapters.RemoveAt(j);
                    }
                }
            }
        }

        public static ChildGroupPack Read(GroupAdapter groupAdapter)
        {
#if DEBUG
            if (DebugMode)
            {
                Debug.Log(string.Format(@"Read"));
            }
#endif
            ChildGroupPack childGroupPack = new ChildGroupPack(groupAdapter);

            var groupDescriptors = DesignerReflection.GetChildGroupsReferences(groupAdapter);

            foreach (ChildGroupDescriptor groupDescriptor in groupDescriptors)
            {
                List<ComponentAdapter> adapters = Core.Reflection.CoreReflector.GetMemberValue(groupDescriptor.CollectionMemberInfo, groupAdapter) as List<ComponentAdapter>;

                ChildGroup childGroup = new ChildGroup(adapters)
                {
                    GroupName = groupDescriptor.CollectionMemberInfo.Name
                };
                childGroupPack.Add(childGroup);
            }

#if DEBUG
            if (DebugMode)
            {
                Debug.Log("Read. childGroupPack: " + childGroupPack);
            }
#endif

            return childGroupPack;
        }

        public void Write()
        {
            ChildGroupPackWriter.Write(_parentAdapter, this);
        }

        public SaveablePack ToSaveablePack()
        {
            SaveablePack saveablePack = new SaveablePack(_parentAdapter.GetInstanceID());

            foreach (ChildGroup childGroup in _groups)
            {
                List<int> ids = new List<int>();
                foreach (ComponentAdapter adapter in childGroup.Adapters)
                {
                    if (null == adapter)
                    {
                        Debug.LogWarning("No adapter found");
                        continue;
                    }

                    ids.Add(adapter.GetInstanceID());
                }
                saveablePack.Add(ids);
            }

            return saveablePack;
        }

        /// <summary>
        /// Evaluates saved Adapter IDs to real adapters
        /// Note: DO NOT use useEditorUtility=true because it will grab the old adapter!!!
        /// </summary>
        /// <returns></returns>
        public static ChildGroupPack Apply(SaveablePack saveablePack)
        {
#if DEBUG
            if (DebugMode)
            {
                Debug.Log(@"*** WRITING PACK ***
" + saveablePack);
            }
#endif

            // Note: we have to call the override having the second argument, supplying "true"
            // This is because if this is the adapter existing prior to play mode start, the dictionary points to the "old" adapter (a stale one)
            // this way we are making sure that the system looks for the specified adapter in the scene first, and then uses the dictionary (for the "new" adapters)
            GroupAdapter parentAdapter = ComponentRegistry.Instance.Get(saveablePack.ParentInstanceId, true) as GroupAdapter;

            ChildGroupPack childGroupPack = Read(parentAdapter);

            int index = 0;
            foreach (List<int> childGroupIds in saveablePack.InstanceIds)
            {
                List<ComponentAdapter> adapters = new List<ComponentAdapter>();
                foreach (int adapterId in childGroupIds)
                {
                    ComponentAdapter childAdapter = (ComponentAdapter) ComponentRegistry.Instance.Get(adapterId, true); // FALSE!!!
                    /*if (null == childAdapter)
                        throw new Exception("Cannot find child adapter in dictionary: " + adapterId);*/
                    
                    adapters.Add(childAdapter);
                }
                
                var actualGroup = childGroupPack.Groups[index];
                actualGroup.Clear();
                actualGroup.AddRange(adapters);

                index++;
            }

#if DEBUG
            if (DebugMode)
            {
                Debug.Log("Apply. childGroupPack: " + childGroupPack);
            }
#endif

            return childGroupPack;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("ChildGroupPack [{0}][{1}] ({2} groups)", _parentAdapter, _parentAdapter.GetInstanceID(), _groups.Count));

            for (int i = 0; i < _groups.Count; i++)
            {
                var adapters = _groups[i].Adapters;
                sb.AppendLine(string.Format("   Group [{0}] ({1} adapters)", i, adapters.Count));
                foreach (ComponentAdapter adapter in adapters)
                {
                    sb.AppendLine(string.Format("       [{0}]{1}", 
                        null == adapter ? "-" : adapter.ToString(),
                        null == adapter ? string.Empty : string.Format("[{0}]", adapter.GetInstanceID())));
                }
            }

            return sb.ToString();
        }

        public void RegisterAdapters()
        {
            /**
             * 1. Register parent
             * */
            ComponentRegistry.Instance.Register(ParentAdapter.GetInstanceID(), ParentAdapter);

            /**
             * 2. Register children
             * */
            foreach (ChildGroup childGroup in Groups)
            {
                foreach (ComponentAdapter adapter in childGroup.Adapters)
                {
                    if (null == adapter)
                        throw new Exception("Child adapter is null");

                    ComponentRegistry.Instance.Register(adapter.GetInstanceID(), adapter);
                }
            }
        }
    }
}