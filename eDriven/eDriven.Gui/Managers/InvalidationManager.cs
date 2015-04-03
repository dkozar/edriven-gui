using eDriven.Core.Managers;
using eDriven.Core.Signals;
using eDriven.Gui.Components;
using eDriven.Gui.Managers.Invalidators;
using eDriven.Gui.Util;
using UnityEngine;

namespace eDriven.Gui.Managers
{
	internal delegate void AdditionalHandler(InvalidationManagerClient obj);

	/// <summary>
	/// His majesty... Invalidation Manager!!!
	/// By far the most important piece in the framework. :)
	/// </summary>
	public sealed class InvalidationManager
	{

#if DEBUG
		internal static bool DebugMode;
#endif

		#region Singleton

		private static InvalidationManager _instance;

		private InvalidationManager()
		{
			// Constructor is protected!
		}

		/// <summary>
		/// Singleton instance
		/// </summary>
		public static InvalidationManager Instance
		{
			get
			{
#if DEBUG
				//if (DebugMode)
				//    Debug.Log(string.Format("Getting layout manager instance"));
#endif

				if (_instance == null)
				{
					_instance = new InvalidationManager();
					_instance.Initialize();
				}

				return _instance;
			}
		}

		#endregion

		internal readonly Signal UpdateCompleteSignal = new Signal();

		private const int MaxHDepth = 1000000;

		bool _done;
		InvalidationManagerClient _obj;

		private void Initialize()
		{
			_systemManager.LevelLoadedSignal.Connect(LevelLoadedSlot);
			_systemManager.DisposingSignal.Connect(DisposingSlot);

			_propertyInvalidator = new PropertyInvalidator(_updateCompleteQueue);
			_sizeInvalidator = new SizeInvalidator(_updateCompleteQueue);
			_displayListInvalidator = new DisplayListInvalidator(_updateCompleteQueue);
			_transformInvalidator = new TransformInvalidator(_updateCompleteQueue);
			_eventInvalidator = new EventInvalidator(_updateCompleteQueue);
			
			// additional handlers
			_sizeInvalidator.AdditionalHandlers.Add(AdditionalPropertyHandler);

			// additional handlers
			_displayListInvalidator.AdditionalHandlers.Add(AdditionalPropertyHandler);
			_displayListInvalidator.AdditionalHandlers.Add(AdditionalSizeHandler);
		}

		private void AdditionalPropertyHandler(InvalidationManagerClient target)
		{
			/*if (!_done)
				return;*/

			//Debug.Log("Executing _invalidateClientPropertiesFlag handler for: " + target);
			if (_propertyInvalidator.InvalidClient)
			{
				// did any properties get invalidated while validating size?
				_obj = _propertyInvalidator.Queue.RemoveSmallestChild(target);
				if (null != _obj)
				{
					// re-queue it. we'll pull it at the beginning of the loop
					_propertyInvalidator.Queue.AddObject(target, target.NestLevel);
					_done = false;
					//break;
				}
			}
		}

		private void AdditionalSizeHandler(InvalidationManagerClient target)
		{
			/*if (!_done)
				return;*/

			//Debug.Log("Executing _invalidateClientSizeFlag handler for: " + target);
			if (_sizeInvalidator.InvalidClient)
			{
				// did any properties get invalidated while validating size?
				_obj = _sizeInvalidator.Queue.RemoveLargestChild(target);
				if (null != _obj)
				{
					// re-queue it. we'll pull it at the beginning of the loop
					_sizeInvalidator.Queue.AddObject(target, target.NestLevel);
					_done = false;
					//break;
				}
			}
		}

		#region Members

		private readonly SystemManager _systemManager = SystemManager.Instance;

		private readonly PriorityQueue _updateCompleteQueue = new PriorityQueue();

		private PropertyInvalidator _propertyInvalidator;
		private SizeInvalidator _sizeInvalidator;
		private DisplayListInvalidator _displayListInvalidator;
		private TransformInvalidator _transformInvalidator;
		private EventInvalidator _eventInvalidator;
		
		#endregion

		#region Invalidation methods

		internal void InvalidateProperties(InvalidationManagerClient obj)
		{
#if DEBUG
			{
				if (DebugMode)
					InvalidationHelper.Log("InvalidateProperties", obj);
			}
#endif
			if (!_propertyInvalidator.Invalid)
				CheckSignal();

			_invalidateClientPropertiesFlag = _targetLevel <= obj.NestLevel;

			_propertyInvalidator.Invalidate(obj, _invalidateClientPropertiesFlag);
		}

		internal void InvalidateTransform(InvalidationManagerClient obj)
		{
			//InvalidationHelper.Log("InvalidateTransform", obj);
#if DEBUG
			{
				if (DebugMode)
					InvalidationHelper.Log("InvalidateTransform", obj);
			}
#endif
			if (!_transformInvalidator.Invalid)
				CheckSignal();

			_invalidateClientTransformFlag = _targetLevel <= obj.NestLevel;

			//if (((InvalidationManagerClient)obj).Id == "btn")
			//    InvalidationHelper.Log("InvalidateTransform", obj);

			_transformInvalidator.Invalidate(obj, _invalidateClientTransformFlag);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
        internal void InvalidateSize(InvalidationManagerClient obj)
		{
#if DEBUG
			{
				if (DebugMode)
					InvalidationHelper.Log("InvalidateSize", obj);
			}
#endif
			if (!_sizeInvalidator.Invalid)
				CheckSignal();

			_invalidateClientSizeFlag = _targetLevel <= obj.NestLevel;

			_sizeInvalidator.Invalidate(obj, _invalidateClientSizeFlag);
		}

		internal void InvalidateDisplayList(InvalidationManagerClient obj)
		{
#if DEBUG
			{
				if (DebugMode)
					InvalidationHelper.Log("InvalidateDisplayList", obj);
			}
#endif
			if (!_displayListInvalidator.Invalid)
				CheckSignal();

			_invalidateClientDisplayListFlag = _targetLevel <= obj.NestLevel;

			_displayListInvalidator.Invalidate(obj, _invalidateClientDisplayListFlag);
		}

		internal void InvalidateEventQueue(InvalidationManagerClient obj)
		{
#if DEBUG
			{
				if (DebugMode)
					InvalidationHelper.Log("InvalidateEventQueue", obj);
			}
#endif
			if (!_eventInvalidator.Invalid)
				CheckSignal();

			_invalidateClientEventsFlag = _targetLevel <= obj.NestLevel;

			_eventInvalidator.Invalidate(obj, _invalidateClientEventsFlag);
		}

		#endregion

		#region Validation methods

		/// <summary>
		/// 
		/// </summary>
		public void ValidateNow()
		{
			DoValidate();
		}

		private InvalidationManagerClient _currentObject;

		private int _targetLevel = MaxHDepth;

		// flag when in validateClient to check the properties queue again
		private bool _invalidateClientPropertiesFlag;
		private bool _invalidateClientSizeFlag;
		private bool _invalidateClientDisplayListFlag;
		private bool _invalidateClientTransformFlag;
		private bool _invalidateClientEventsFlag;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="target"></param>
		/// <param name="skipDisplayList"></param>
		public void ValidateClient(InvalidationManagerClient target, bool skipDisplayList = false)
		{
			InvalidationManagerClient lastCurrentObject = _currentObject;
			
			int oldTargetLevel = _targetLevel;

			if (MaxHDepth == _targetLevel)
				_targetLevel = target.NestLevel;

			_done = false;

			while (!_done)
			{
				// assume we won't find anything
				_done = true;

				/**
				 * 1) Validate properties
				 * */
				_propertyInvalidator.ValidateClient(target, ref _currentObject);

				/**
				 * 2) Validate size
				 * */
				//InvalidationHelper.Log("## Validate size", target);
				_sizeInvalidator.ValidateClient(target, ref _currentObject);

				/**
				 * 3) Validate display list
				 * */
				if (!skipDisplayList)
					_displayListInvalidator.ValidateClient(target, ref _currentObject);

				/**
				 * 4) Validate transforms
				 * */
				_transformInvalidator.ValidateClient(target, ref _currentObject); // moved 20130807

				/**
				 * 5) Validate events
				 * */
				_eventInvalidator.ValidateClient(target, ref _currentObject);
			}

			if (MaxHDepth == oldTargetLevel)
			{
				_targetLevel = MaxHDepth;
				if (!skipDisplayList)
				{
					_obj = _updateCompleteQueue.RemoveLargestChild(target);
					while (null != _obj)
					{
						if (!_obj.Initialized)
							_obj.Initialized = true;

						if (_obj.HasEventListener(FrameworkEvent.UPDATE_COMPLETE))
							_obj.DispatchEvent(new FrameworkEvent(FrameworkEvent.UPDATE_COMPLETE));
						_obj.UpdateFlag = false;
						_obj = _updateCompleteQueue.RemoveLargestChild(target);
					}
				}
			}

			_currentObject = lastCurrentObject;
		}

		#endregion

		#region IDisposable

		private void Dispose()
		{
			_systemManager.PreRenderSignal.Disconnect(PreRenderSlot);

			//_styleInvalidator = null;
			//_propertyInvalidator = null;
			//_transformInvalidator = null;
			//_sizeInvalidator = null;
			//_displayListInvalidator = null;
			//_eventInvalidator = null;

			_updateCompleteQueue.RemoveAll();
		}

		#endregion

		#region Private methods

		private void CheckSignal()
		{
			if (!_systemManager.PreRenderSignal.HasSlot(PreRenderSlot))
			{
#if DEBUG
				{
					if (DebugMode)
						Debug.Log("* Signal connect *");
				}
#endif
				_systemManager.PreRenderSignal.Connect(PreRenderSlot, 0/*, true*/); // one time!

				// NOTE: Due to screen flickering with popups, I changed UpdateSignal subscription to InputSignal (20120208)
				// NOTE: I changed InputSignal subscription to RenderSignal (20120228)
				// it seems it works OK now
			}
		}

		//private int _measureCount;
		private void DoValidate()
		{
			if (_propertyInvalidator.Invalid)
			{
				_propertyInvalidator.Validate(ref _currentObject);
			}

			if (_sizeInvalidator.Invalid)
			{
				//Debug.Log("_styleInvalidator.Invalid: " + _styleInvalidator.Invalid);
				//Debug.Log("*** Measuring: " + _measureCount++);
				_sizeInvalidator.Validate(ref _currentObject);
			}

			if (_displayListInvalidator.Invalid)
			{
				_displayListInvalidator.Validate(ref _currentObject);
			}

			if (_transformInvalidator.Invalid)
			{
				_transformInvalidator.Validate(ref _currentObject);
			}

			if (_eventInvalidator.Invalid)
			{
				_eventInvalidator.Validate(ref _currentObject);
			}
			
			if (_propertyInvalidator.Invalid ||
				_transformInvalidator.Invalid ||
				_sizeInvalidator.Invalid ||
				_displayListInvalidator.Invalid ||
				_eventInvalidator.Invalid)
			{
				// do nothing (attachListeners(systemManager);)
			}
			else
			{
				InvalidationManagerClient obj = _updateCompleteQueue.RemoveLargest();
				while (null != obj)
				{
					if (!obj.Initialized/* && obj.InternalStructureBuilt*/)
					{
#if DEBUG
						{
							if (DebugMode)
								InvalidationHelper.Log("Creation complete", obj);
						}
#endif
						// Most important: setting the initialized property here
						// after all the initial VALIDATION cycles
						obj.Initialized = true;
					}
					if (obj.HasEventListener(FrameworkEvent.UPDATE_COMPLETE))
						obj.DispatchEvent(new FrameworkEvent(FrameworkEvent.UPDATE_COMPLETE));
					obj.UpdateFlag = false;
					obj = _updateCompleteQueue.RemoveLargest();
				}

				//Debug.Log("updateComplete");

				/**
				 * We are emmiting the update complete signal here
				 * The listeners of this signal are connected for one shot only
				 * They internally reset their ForceLayout flags etc.
				 * */
				UpdateCompleteSignal.Emit();
			}
		}

		#endregion

		#region Implementation of ISlot

		internal void PreRenderSlot(params object[] parameters)
		{
#if DEBUG
			{
				if (DebugMode)
					Debug.Log("* Signal receive *");
			}
#endif

			DoValidate();

			// if all validated, disconnect
			if (!_propertyInvalidator.Invalid
				&& !_transformInvalidator.Invalid
				&& !_sizeInvalidator.Invalid
				&& !_displayListInvalidator.Invalid
				&& !_eventInvalidator.Invalid)
			{
#if DEBUG
				{
					if (DebugMode)
						Debug.Log("* Signal disconnect *");
				}
#endif
				_systemManager.PreRenderSignal.Disconnect(PreRenderSlot);
			}
		}

		internal void LevelLoadedSlot(params object[] parameters)
		{
			//Dispose();
		}

		internal void DisposingSlot(params object[] parameters)
		{
#if DEBUG
	if (DebugMode)
	{
		Debug.Log("Disposing invalidation manager ###");
	}
#endif
			Dispose();
		}

		#endregion
	}
}