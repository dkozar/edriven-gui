using System;
using System.Collections.Generic;
using eDriven.Gui.Components;
using eDriven.Gui.Util;
using UnityEngine;

namespace eDriven.Gui.Managers.Invalidators
{
    internal sealed class DisplayListInvalidator : InvalidatorBase, IInvalidator
    {
#if DEBUG
        public static bool DebugMode;
#endif

        private DateTime _start;

        internal DisplayListInvalidator(PriorityQueue updateCompleteQueue)
            : base(updateCompleteQueue)
        {
        }

        #region IInvalidator

        /*public bool Invalid { get; private set; }*/

        public void Validate(ref InvalidationManagerClient currentObject)
        {
#if DEBUG
            {
                if (DebugMode)
                    Debug.Log("DisplayListInvalidator -> Validate");
            }
#endif
            //_currentObject = currentObject;

            // Keep traversing the invalidatePropertiesQueue until we've reached the end.
            // More elements may get added to the queue while we're in this loop, or a
            // a recursive call to this function may remove elements from the queue while
            // we're in this loop.
            InvalidationManagerClient obj = Queue.RemoveSmallest();

            while (null != obj)
            {
                //Debug.Log("InvalidationManager calling validateProperties() on " + obj);

                //if (obj is LoadingMaskAnimator)
                //    Debug.Log("Validating display list for: " + obj);

                //CONFIG::performanceInstrumentation
#if DEBUG
                {
                    if (DebugMode)
                        _start = DateTime.Now;
                }
#endif

                //if (-1 != obj.NestLevel)
                //{
                    currentObject = obj;
                    obj.ValidateDisplayList();
                    HandleUpdateCompletePendingFlag(obj);
                //}

                //CONFIG::performanceInstrumentation
#if DEBUG
                {
                    if (DebugMode)
                    {
                        var end = DateTime.Now.Subtract(_start);
                        InvalidationHelper.Log("ValidateDisplayList", obj);
                        Debug.Log(string.Format("DisplayListInvalidator -> Validated in {0} ms", end.Milliseconds));
                    }
                }
#endif

                // Once we start, don't stop.
                obj = Queue.RemoveSmallest();
            }

            if (Queue.IsEmpty())
            {
                // trace("Properties Queue is empty");

                Invalid = false;

                /*_systemManager.DispatchEvent(
                    new Event("validateDisplayListComplete"));*/
            }
        }

        private readonly List<AdditionalHandler> _additionalHandlers = new List<AdditionalHandler>();
        public List<AdditionalHandler> AdditionalHandlers
        {
            get { return _additionalHandlers; }
        }

        public void ValidateClient(InvalidationManagerClient target, ref InvalidationManagerClient currentObject)
        {
#if DEBUG
            {
                if (DebugMode)
                    Debug.Log("DisplayListInvalidator -> ValidateClient");
            }
#endif

            //IInvalidationManagerClient lastCurrentObject = _currentObject;

            //int i = 0;
            //bool done = false;
            //int oldTargetLevel = _targetLevel;

            //while (!done)
            //{
            //    done = true;


                // Keep traversing the invalidatePropertiesQueue until we've reached the end.
                // More elements may get added to the queue while we're in this loop, or a
                // a recursive call to this function may remove elements from the queue while
                // we're in this loop.
                InvalidationManagerClient obj = Queue.RemoveSmallestChild(target);

                while (null != obj)
                {
                    //Debug.Log("InvalidationManager calling validateProperties() on " + obj);

                    #region Debug

#if DEBUG
                    {
                        if (DebugMode)
                            _start = DateTime.Now;
                    }
#endif

                    #endregion

                    if (-1 != obj.NestLevel)
                    {
                        currentObject = obj;
                        obj.ValidateDisplayList();
                        HandleUpdateCompletePendingFlag(obj);
                    }

                    // process additional handlers
                    if (AdditionalHandlers.Count > 0)
                    {
                        foreach (AdditionalHandler handler in AdditionalHandlers)
                            handler(target);
                    }

                    #region Debug

#if DEBUG
                    {
                        if (DebugMode)
                        {
                            var end = DateTime.Now.Subtract(_start);
                            InvalidationHelper.Log("ValidateDisplayList", obj);
                            Debug.Log(string.Format("DisplayListInvalidator -> Validated in {0} ms", end.Milliseconds));
                        }
                    }
#endif

                    #endregion

                    // Once we start, don't stop.
                    obj = Queue.RemoveSmallestChild(target);
                }

                if (Queue.IsEmpty())
                {
                    // trace("Properties Queue is empty");

                    Invalid = false;
                    InvalidClient = false;
                    
                    /*_systemManager.DispatchEvent(
                        new Event("validatePropertiesComplete"));*/
                }

            //}

            //_currentObject = lastCurrentObject;
        }

        #endregion
    }
}
