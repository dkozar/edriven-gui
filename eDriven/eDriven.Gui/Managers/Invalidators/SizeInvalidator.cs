using System;
using System.Collections.Generic;
using eDriven.Gui.Components;
using eDriven.Gui.Util;
using UnityEngine;

namespace eDriven.Gui.Managers.Invalidators
{
    internal sealed class SizeInvalidator : InvalidatorBase, IInvalidator
    {
#if DEBUG
        public static bool DebugMode;
#endif
        private DateTime _start;

        internal SizeInvalidator(PriorityQueue updateCompleteQueue)
            : base(updateCompleteQueue)
        {
        }

        #region IInvalidator

        public void Validate(ref InvalidationManagerClient currentObject)
        {
#if DEBUG
            {
                if (DebugMode)
                    Debug.Log("SizeInvalidator -> Validate");
            }
#endif
            //_currentObject = currentObject;

            // Keep traversing the invalidatePropertiesQueue until we've reached the end.
            // More elements may get added to the queue while we're in this loop, or a
            // a recursive call to this function may remove elements from the queue while
            // we're in this loop.
            InvalidationManagerClient obj = Queue.RemoveLargest();

            while (null != obj)
            {
                //Debug.Log("InvalidationManager calling validateProperties() on " + obj);

                //CONFIG::performanceInstrumentation

                #region Debug

#if DEBUG
                {
                    if (DebugMode)
                        _start = DateTime.Now;
                }
#endif

                #endregion


                //if (obj is ComboBox)
                //    Debug.Log("validating combo: " + obj);

                //if (-1 != obj.NestLevel)
                //{
                    currentObject = obj;
                    obj.ValidateSize(false);
                    //obj.ValidateSize(!(currentObject is Container)); // added 20121212. ComboBox didn't measure when in designer application (its children were never measured, because it is not a container)
                    //obj.ValidateSize(true);
                    HandleUpdateCompletePendingFlag(obj);
                //}

                #region Debug

#if DEBUG
                {
                    if (DebugMode)
                    {
                        var end = DateTime.Now.Subtract(_start);
                        InvalidationHelper.Log("ValidateSize", obj);
                        Debug.Log(string.Format("SizeInvalidator -> Validated in {0} ms", end.Milliseconds));
                    }
                }
#endif

                #endregion


                obj = Queue.RemoveLargest();
            }

            if (Queue.IsEmpty())
            {
                // trace("Properties Queue is empty");

                Invalid = false;

                /*_systemManager.DispatchEvent(
                    new Event("validateSizeComplete"));*/
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
                    Debug.Log("SizeInvalidator -> ValidateClient");
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
            InvalidationManagerClient obj = Queue.RemoveLargestChild(target);

                while (null != obj)
                {
                    //Debug.Log("   ->Validating size: " + obj);

                    #region Debug

#if DEBUG
                    {
                        if (DebugMode)
                            _start = DateTime.Now;
                    }
#endif

                    #endregion

                    /*if (-1 != obj.NestLevel)
                    {*/
                        currentObject = obj;
                        obj.ValidateSize(false);
                        //Debug.Log("       -> Width: " + ((DisplayObject)obj).Width + "; Height: " + ((DisplayObject)obj).Height);
                        HandleUpdateCompletePendingFlag(obj);
                    /*}*/

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
                            InvalidationHelper.Log("ValidateSize", obj);
                            Debug.Log(string.Format("SizeInvalidator -> Validated in {0} ms", end.Milliseconds));
                        }
                    }
#endif

                    #endregion

                    // Once we start, don't stop.
                    obj = Queue.RemoveLargestChild(target);
                }

                if (Queue.IsEmpty())
                {
                    //Debug.Log("Size Queue is empty");

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
