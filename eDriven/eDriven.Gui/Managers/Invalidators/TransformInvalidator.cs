using System;
using eDriven.Gui.Components;
using eDriven.Gui.Util;
using UnityEngine;

namespace eDriven.Gui.Managers.Invalidators
{
    internal sealed class TransformInvalidator : InvalidatorBase, IInvalidator
    {
#if DEBUG
        public static bool DebugMode;
#endif

        private DateTime _start;

        internal TransformInvalidator(PriorityQueue updateCompleteQueue)
            : base(updateCompleteQueue)
        {
        }

        #region IInvalidator

        public void Validate(ref InvalidationManagerClient currentObject)
        {
#if DEBUG
            {
                if (DebugMode)
                    Debug.Log("TransformInvalidator -> Validate");
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

                //CONFIG::performanceInstrumentation
#if DEBUG
                {
                    if (DebugMode)
                        _start = DateTime.Now;
                }
#endif

                //if (-1 != obj.NestLevel)
                //{
                    //_currentObject = obj;
                    obj.ValidateTransform();
                    HandleUpdateCompletePendingFlag(obj);
                //}

                //CONFIG::performanceInstrumentation
#if DEBUG
                {
                    if (DebugMode)
                    {
                        var end = DateTime.Now.Subtract(_start);
                        InvalidationHelper.Log("ValidateTransform", obj);
                        Debug.Log(string.Format("TransformInvalidator -> Validated in {0} ms", end.Milliseconds));
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
                    new Event("validatePositionComplete"));*/
            }
        }

        public void ValidateClient(InvalidationManagerClient target, ref InvalidationManagerClient currentObject)
        {
#if DEBUG
            {
                if (DebugMode)
                    Debug.Log("TransformInvalidator -> ValidateClient");
            }
#endif

            //InvalidationManagerClient lastCurrentObject = _currentObject;

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
#if DEBUG
                    {
                        if (DebugMode)
                            _start = DateTime.Now;
                    }
#endif
                    if (-1 != obj.NestLevel)
                    {
                        currentObject = obj;
                        obj.ValidateTransform();
                        HandleUpdateCompletePendingFlag(obj);
                    }


#if DEBUG
                    {
                        //InvalidationHelper.Log("ValidateTransform", obj);
                        if (DebugMode)
                        {
                            var end = DateTime.Now.Subtract(_start);
                            InvalidationHelper.Log("ValidateTransform", obj);
                            Debug.Log(string.Format("TransformInvalidator -> Validated in {0} ms", end.Milliseconds));
                        }
                    }
#endif

                    // Once we start, don't stop.
                    obj = Queue.RemoveSmallestChild(target);
                }

                if (Queue.IsEmpty())
                {
                    Invalid = false;
                    InvalidClient = false;
                }

            //}

            //_currentObject = lastCurrentObject;
        }

        #endregion
    }
}
