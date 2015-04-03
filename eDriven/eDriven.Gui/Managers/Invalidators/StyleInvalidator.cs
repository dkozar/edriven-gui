//using System;
//using System.Collections.Generic;
//using eDriven.Gui.Components;
//using eDriven.Gui.Layout;
//using UnityEngine;

//namespace eDriven.Gui.Managers.Invalidators
//{
//    internal sealed class StyleInvalidator : InvalidatorBase, IInvalidator
//    {
//#if DEBUG
//        public static bool DebugMode;
//#endif

//        private readonly PriorityQueue _queue = new PriorityQueue();
//        internal PriorityQueue Queue
//        {
//            get { return _queue; }
//        }

//        private DateTime _start;

//        internal StyleInvalidator(PriorityQueue updateCompleteQueue)
//            : base(updateCompleteQueue)
//        {
//        }

//        #region IInvalidator

//        public bool Invalid { get; private set; }

//        public void Invalidate(IInvalidationManagerClient obj, ref bool invalidateClientFlag)
//        {
//            _queue.AddObject(obj, obj.NestLevel);
//            //Debug.Log("*** StyleInvalidator -> Invalidate " + obj);
//            Invalid = true;
//        }

//        public void Validate(ref IInvalidationManagerClient currentObject)
//        {
//#if DEBUG
//            {
//                if (DebugMode)
//                    Debug.Log("StyleInvalidator -> Validate");
//            }
//#endif
//            //_currentObject = currentObject;

//            // Keep traversing the invalidateStylesQueue until we've reached the end.
//            // More elements may get added to the queue while we're in this loop, or a
//            // a recursive call to this function may remove elements from the queue while
//            // we're in this loop.
//            IInvalidationManagerClient obj = (IInvalidationManagerClient)_queue.RemoveSmallest();

//            while (null != obj)
//            {
//                // trace("InvalidationManager calling validateProperties() on " + Object(obj) + " " + DisplayObject(obj).width + " " + DisplayObject(obj).height);

//                //CONFIG::performanceInstrumentation
//#if DEBUG
//                {
//                    if (DebugMode)
//                        _start = DateTime.Now;
//                }
//#endif

//                if (-1 != obj.NestLevel)
//                {
//                    currentObject = obj;
//                    obj.ValidateStyles();
//                    HandleUpdateCompletePendingFlag(obj);
//                }

//                //CONFIG::performanceInstrumentation
//#if DEBUG
//                {
//                    if (DebugMode)
//                    {
//                        //var end = DateTime.Now.Subtract(_start);
//                        InvalidationHelper.Log("ValidateStyles", obj);
//                        //Debug.Log(string.Format("ValidateTransform() took {0} ms", end.Milliseconds));
//                    }
//                }
//#endif
//                // Once we start, don't stop.
//                obj = (IInvalidationManagerClient)_queue.RemoveSmallest();
//            }

//            //Debug.Log("Checking if Styles Queue is empty");
//            if (_queue.IsEmpty())
//            {
//                //Debug.Log("Styles Queue is empty");

//                Invalid = false;

//                /*_systemManager.DispatchEvent(
//                    new Event("validateStylesComplete"));*/
//            }
//        }

//        private readonly List<AdditionalHandler> _additionalHandlers = new List<AdditionalHandler>();
//        public List<AdditionalHandler> AdditionalHandlers
//        {
//            get { return _additionalHandlers; }
//        }

//        public void ValidateClient(IInvalidationManagerClient target, ref IInvalidationManagerClient currentObject, ref bool invalidateClientFlag)
//        {
//            //Debug.Log("************** StyleInvalidator -> ValidateClient " + target);
//#if DEBUG
//            {
//                if (DebugMode)
//                    Debug.Log("StyleInvalidator -> ValidateClient");
//            }
//#endif

//            //IInvalidationManagerClient lastCurrentObject = _currentObject;

//            //int i = 0;
//            //bool done = false;
//            //int oldTargetLevel = _targetLevel;

//            //while (!done)
//            //{
//            //    done = true;


//                // Keep traversing the invalidateStylesQueue until we've reached the end.
//                // More elements may get added to the queue while we're in this loop, or a
//                // a recursive call to this function may remove elements from the queue while
//                // we're in this loop.
//                IInvalidationManagerClient obj = (IInvalidationManagerClient)_queue.RemoveSmallestChild(target);

//                while (null != obj)
//                {
//                    //Debug.Log("InvalidationManager calling validateProperties() on " + obj);

//                    //CONFIG::performanceInstrumentation
//#if DEBUG
//                    {
//                        if (DebugMode)
//                            _start = DateTime.Now;
//                    }
//#endif
//                    //if (-1 != obj.NestLevel)
//                    //{
//                        currentObject = obj;
//                        obj.ValidateStyles();
//                        HandleUpdateCompletePendingFlag(obj);
//                    //}

//                    // process additional handlers
//                    if (AdditionalHandlers.Count > 0)
//                    {
//                        foreach (AdditionalHandler handler in AdditionalHandlers)
//                            handler(target);
//                    }

//                    //CONFIG::performanceInstrumentation
//#if DEBUG
//                    {
//                        if (DebugMode)
//                        {
//                            var end = DateTime.Now.Subtract(_start);
//                            InvalidationHelper.Log("ValidateStyles", obj);
//                            Debug.Log(string.Format("StyleInvalidator -> Validated in {0} ms", end.Milliseconds));
//                        }
//                    }
//#endif

//                    // Once we start, don't stop.
//                    obj = (IInvalidationManagerClient)_queue.RemoveSmallestChild(target);
//                }

//                if (_queue.IsEmpty())
//                {
//                    // trace("Properties Queue is empty");

//                    Invalid = false;
//                    invalidateClientFlag = false;

//                    /*_systemManager.DispatchEvent(
//                        new Event("validatePropertiesComplete"));*/
//                }

//            //}

//            //_currentObject = lastCurrentObject;
//        }

//        #endregion
//    }
//}
