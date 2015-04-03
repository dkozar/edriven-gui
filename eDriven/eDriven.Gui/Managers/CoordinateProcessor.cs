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
using eDriven.Gui.Components;
using eDriven.Gui.Containers;
using eDriven.Core.Geom;
using eDriven.Gui.Util;
using UnityEngine;
using Component=eDriven.Gui.Components.Component;

namespace eDriven.Gui.Managers
{
    /// <summary>
    /// Used for looking for components below the mouse pointer<br/>
    /// Operates with DisplayObjects and DisplayObjectContainers<br/>
    /// However, using the filter we can filter out components (because we are looking for components)
    /// </summary>
    public static class CoordinateProcessor
    {

#if DEBUG
        // ReSharper disable UnassignedField.Global
        /// <summary>
        /// Debug mode
        /// </summary>
        public static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif

        /// <summary>
        /// The filter testing the displayListMember to be included into the list
        /// </summary>
        /// <param name="displayListMember"></param>
        /// <returns></returns>
        public delegate bool Filter(DisplayListMember displayListMember);
        
        /// <summary>
        /// The ascending list of stages (front to back)
        /// </summary>
        internal static List<Stage> StageListAsc;

        /// <summary>
        /// The descending list of stages (back to front)
        /// </summary>
        internal static List<Stage> StageListDesc;

        private static bool _containsPoint;
        
        /// <summary>
        /// Stages excluded from processing
        /// </summary>
        private static readonly List<Stage> ExcludedStages = new List<Stage>();

        /// <summary>
        /// Add the stage to the exclude list
        /// </summary>
        /// <param name="stage"></param>
        public static void ExcludeStage(Stage stage)
        {
            if (!ExcludedStages.Contains(stage))
                ExcludedStages.Add(stage);
        }

        /// <summary>
        /// Looks for a front-most component inside of the parent component
        /// Goes top-down, from parent to children
        /// Returns the front-most rendered component on given coordinates (on a single stage)
        /// NOTE: RECURSIVE!!!
        /// </summary>
        /// <param name="dlm"></param>
        /// <param name="coords">Coordinates to test</param>
        /// <param name="filter">Filter</param>
        /// <param name="stopOnDisabled">Should we stop on disabled component</param>
        /// <param name="stopOnInvisible">Should we stop on invisible component</param>
        /// <returns></returns>
// ReSharper disable SuggestBaseTypeForParameter
        private static DisplayListMember GetComponentUnderCoordinates(DisplayListMember dlm, Point coords, Filter filter, bool stopOnDisabled, bool stopOnInvisible)
// ReSharper restore SuggestBaseTypeForParameter
        {
            //Debug.Log("GetComponentUnderCoordinates: " + dlm);

            InteractiveComponent component = dlm as InteractiveComponent;

            if (null != component)
            {
                if (stopOnInvisible && !component.Visible) // invisible
                    return null;
                if (stopOnDisabled && !component.Enabled) // disabled
                    return null;
            }

            DisplayListMember output = null;

            _containsPoint = dlm.ContainsPoint(coords, false);

            if (_containsPoint && PassesFilter(dlm, filter))
            {
                output = component;
            }

            GroupBase group = dlm as GroupBase;
            if (null != group)
            {
                if (!_containsPoint && group.ClipAndEnableScrolling)
                    return output;

                if (group.MouseChildren)
                {
                    foreach (DisplayListMember d in group.QDrawingList)
                    {
                        /* Recursive call! */
                        DisplayListMember c = GetComponentUnderCoordinates(d, coords, filter, stopOnDisabled, stopOnInvisible);
                        if (null != c)
                            output = c;
                    }
                }
            }
            else // simple component
            {
                var doc = dlm as DisplayObjectContainer;
                if (null != doc)
                {
                    if (doc.MouseChildren/* && doc.QNumberOfChildren > 0*/)
                    {
                        foreach (DisplayListMember d in doc.QDrawingList)
                        {
                            /* Recursive call! */
                            DisplayListMember c = GetComponentUnderCoordinates(d, coords, filter, stopOnDisabled, stopOnInvisible);
                            if (null != c)
                                output = c;
                        }
                    }
                }
            }

            return output;
        }

        /// <summary>
        /// Searchs throughout ALL STAGES for the top-most component
        /// Starts with the stage in front (with the lowest depth) and goes to higher depths
        /// Returns as soon as it finds any component
        /// </summary>
        /// <param name="coords"></param>
        /// <param name="filter"></param>
        /// <param name="stopOnDisabled"></param>
        /// <param name="stopOnInvisible">Should we stop on invisible component</param>
        /// <returns></returns>
        internal static DisplayListMember GetComponentUnderCoordinatesOnAllStages(Point coords, Filter filter, bool stopOnDisabled, bool stopOnInvisible)
        {
            //Debug.Log("GetComponentUnderCoordinatesOnAllStages: " + coords);

            if (null == StageListAsc)
                return null; // could be null on recompile // throw new Exception("StageListAsc is null"); //return null;

            DisplayListMember comp = null;

            int stageCount = StageListAsc.Count; // using the ascending stage list
            int count = 0;

            // search from the front stage to the back
            while (null == comp && count < stageCount)
            {
                Stage stage = StageListAsc[count];
                count++;
                
                if (null != ExcludedStages && ExcludedStages.Contains(stage)) // skip excluded stages
                    continue;

                // search the component on a stage
                DisplayListMember dlm = GetComponentUnderCoordinates(stage, coords, filter, stopOnDisabled, stopOnInvisible);
                if (null != dlm)
                {
                    comp = dlm; // component found, this breaks the while loop
                }
            }

#if DEBUG
            if (DebugMode)
            {
                Debug.Log(string.Format("ClickManager: GetComponentUnderCoordinatesOnAllStages returns [{0}]", (null != comp) ? comp.ToString() : "*NULL*"));
            }
#endif

            return comp;
        }

        /// <summary>
        /// Looks for a stack of components under the specified coordinates inside of the parent component
        /// Does the top-down processing: from parent to children
        /// Returns the top-most rendered component on given coordinates (on a single stage)
        /// Notes:
        /// 1. The returned list must be refersed in order to get the top component at the beginning of the list!
        /// 2. This is a recursive operation
        /// </summary>
        /// <param name="dlm">Parent component</param>
        /// <param name="coords">Coordinates</param>
        /// <param name="filter">Filter</param>
        /// <param name="stopOnDisabled">Should the process stop if component disabled</param>
        /// <param name="stopOnInvisible">Should the process stop if component not visible></param>
        /// <param name="list">The list passed by reference</param>
        /// <returns></returns>
        private static void GetComponentStackUnderCoordinates(DisplayListMember dlm, Point coords, Filter filter, bool stopOnDisabled, bool stopOnInvisible, ref List<DisplayListMember> list)
        {
            var component = dlm as InteractiveComponent;

            //Debug.Log("GetComponentStackUnderCoordinates: " + dlm);

            if (null != component)
            {
                if (stopOnInvisible && !dlm.Visible) // invisible
                    return;

                if (stopOnDisabled && !component.Enabled) // disabled
                    return;
            }

            _containsPoint = dlm.ContainsPoint(coords, false);

            //Debug.Log("    _containsPoint: " + dlm);

            if (_containsPoint && PassesFilter(dlm, filter))
            {
                //Debug.Log("    PassesFilter: " + dlm);
                list.Add(dlm);
            }

            Group @group = dlm as Group;

            // in the case of container check the children 
            if (null != @group)
            {
                if (!_containsPoint /*&& container.QClipContent*/)
                    return;

                // the click was inside the container bounds, or container isn't clipping
                // check the children for clicks
                if (@group.MouseChildren)
                {
                    foreach(DisplayListMember d in @group.QDrawingList)
                    {
                        //Component child = d as Component;
                        //if (null != child)
                        //{
                            /* Recursive call! */
                            //DisplayListMember c = GetComponentUnderCoordinates(d, coords, filter, stopOnDisabled);
                            GetComponentStackUnderCoordinates(d, coords, filter, stopOnDisabled, stopOnInvisible, ref list);
                            //Debug.Log("   -> " + c);
                            //if (null != c)
                            //    list.Add(c);
                        //}
                    }
                }
            }
            else // simple component
            {
                DisplayObjectContainer doc = dlm as DisplayObjectContainer;
                // the click was inside the container bounds, or container isn't clipping
                // check the children for clicks

                if (null != doc && doc.QNumberOfChildren > 0 && doc.MouseChildren)
                {
                    foreach (DisplayListMember d in doc.QDrawingList)
                    {
                        GetComponentStackUnderCoordinates(d, coords, filter, stopOnDisabled, stopOnInvisible, ref list);
                        //DisplayListMember c = GetComponentUnderCoordinates(d, coords, filter, stopOnDisabled);
                        //if (null != c)
                        //    list.Add(c);
                    }
                }
            }
        }

        /// <summary>
        /// Searches throughout ALL the stages BACK TO FRONT
        /// Starts with the stage at back and goes to the front
        /// Returns the list of filtered components under the mouse
        /// This method is used when testing for components under the mouse for mouse wheel
        /// For a mouse wheel, a single component isn't enough, since it could be already scrolled to its maximum
        /// In that case the mouse wheel gesture is passed to the component below, and so on
        /// </summary>
        /// <param name="coords"></param>
        /// <param name="filter"></param>
        /// <param name="stopOnDisabled"></param>
        /// <param name="stopOnInvisible"></param>
        /// <returns></returns>
        internal static List<DisplayListMember> GetComponentStackUnderCoordinatesOnAllStages(Point coords, Filter filter, bool stopOnDisabled, bool stopOnInvisible)
        {
            if (null == StageListDesc)
                return null; // could be null on recompile //throw new Exception("StageListDesc is null"); //return null;

            List<DisplayListMember> components = new List<DisplayListMember>();

            /* Note: Using ascending order (negative values in ZIndex have priority)
             * However, we are reversing the order at the end of this call, so we'll use the descending list here (back to front) 
             */
            int stageCount = StageListDesc.Count;
            int count = 0;

            // search from back to front
            while (count < stageCount)
            {
                Stage stage = StageListDesc[count];
                count++;

                if (null != ExcludedStages && ExcludedStages.Contains(stage)) // skip excluded stages
                    continue;

                // search the component on a stage
                GetComponentStackUnderCoordinates(stage, coords, filter, stopOnDisabled, stopOnInvisible, ref components);
            }

#if DEBUG
            if (DebugMode)
            {
                Debug.Log(string.Format("ClickManager: GetComponentUnderCoordinatesOnAllStages returns [{0}] components", components.Count));
            }
#endif

            components.Reverse();

            return components;
        }

        private static bool PassesFilter(DisplayListMember component, Filter filter)
        {
            return null == filter || filter(component);
        }
    }
}