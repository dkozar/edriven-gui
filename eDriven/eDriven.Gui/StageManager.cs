using System;
using System.Collections.Generic;
using eDriven.Core;
using eDriven.Core.Managers;
using eDriven.Gui.Containers;
using eDriven.Gui.DragDrop;
using eDriven.Gui.Managers;
using eDriven.Core.Geom;
using eDriven.Gui.Reflection;
using eDriven.Gui.Styles;
using eDriven.Gui.Styles.MediaQueries;
using eDriven.Gui.Util;
using UnityEngine;

#if RELEASE
    using eDriven.Animation;
    using eDriven.Gui.Check;
    using eDriven.Animation.Check;
#endif

namespace eDriven.Gui
{
    /// <summary>
    /// Registers and runs multiple Stage instances
    /// </summary>
    /// <remarks>Author: Danko Kozar</remarks>
    public sealed class StageManager : IDisposable
    {
#if DEBUG
        // ReSharper disable UnassignedField.Global
        public static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif

        #region Static

// ReSharper disable InconsistentNaming
        public static int CURSOR_STAGE_DEPTH = -1000;
        public static int TOOLTIP_STAGE_DEPTH = -800;
        public static int GUI_INSPECTOR_STAGE_DEPTH = -600;
        public static int DRAG_DROP_STAGE_DEPTH = -400;
        public static int GLOBAL_LOADING_MASK_STAGE_DEPTH = -300;
        public static int POPUP_MANAGER_STAGE_DEPTH = -200;
// ReSharper restore InconsistentNaming

        #endregion

        #region Singleton

        private static StageManager _instance;
        public static StageManager Instance
        {
            get
            {
                if (_instance == null)
                {
#if DEBUG
                    if (DebugMode)
                        Debug.Log("Creating StageManager instance");
#endif
                    _instance = new StageManager();
                    _instance.Initialize();
                }
                return _instance;
            }
        }

        private readonly List<Stage> _stageListDesc = new List<Stage>();

        public List<Stage> StageList
        {
            get { return _stageListDesc; }
        }

        private readonly List<Stage> _stageListAsc = new List<Stage>();

        #endregion

        private void Initialize()
        {
            /**
             * Please don't remove this line
             * This is the only eDriven.Gui line I'd like to have written in Your app log :)
             * It's interesting to see if application uses the framework
             * */
            if (Framework.EnableInfoMessages)
                Debug.Log(string.Format(@"eDriven.Gui framework: instantiated: {0}", new Info()));

#if DEBUG
            Debug.Log(string.Format(@"***** Debug build *****"));
#endif

#if RELEASE
            Debug.Log(string.Format(@"***** eDriven.Gui Free Edition *****"));
#endif

#if !PRODUCTION
            Debug.Log(string.Format(@"***** Not a production build *****"));
#endif
            /**
             * 1) Connect to system manager clocks
             * */
            //SystemManager.Instance.UpdateSignal.Connect(_updateSlot, 0, false);
            
            //Debug.Log("*** SUBSCRIBING ***");
            SystemManager.Instance.RenderSignal.Connect(RenderSlot);
            SystemManager.Instance.ResizeSignal.Connect(ResizeSlot);
            SystemManager.Instance.SceneChangeSignal.Connect(SceneChangeSlot);
            //SystemManager.Instance.LevelInitSignal.Connect(LevelInitSlot);
            //SystemManager.Instance.LevelLoadedSignal.Connect(LevelInitSlot);
            //SystemManager.Instance.LevelLoadedSignal.Connect(LevelInitSlot);
            SystemManager.Instance.DisposingSignal.Connect(DisposingSlot, true); // auto disconnect

            /**
             * 3) let interested parties reference the stage list
             * */

            DragDropManager.StageList = _stageListDesc;

            CoordinateProcessor.StageListAsc = _stageListAsc;
            CoordinateProcessor.StageListDesc = _stageListDesc;

            // initialize styles
            //StyleInitializer.Run();

#pragma warning disable 168
            //var med = MouseEventDispatcher.Instance;

// ReSharper disable UnusedVariable
            var fm = FocusManager.Instance; // initialize listeners to mouse event manager
            var tm = TooltipManager.Instance; // initialize listeners to mouse event manager
            var km = KeyEventDispatcher.Instance;

            NamingUtil.Initialize();
            var cm = ComponentManager.Instance;
// ReSharper restore UnusedVariable

            /* Important! */
            // ReSharper disable once UnusedVariable
            var mq = MediaQueryManager.Instance;

// ReSharper disable once UnusedVariable
            var scm = StyleClientManager.Instance;

#pragma warning restore 168

#if DEBUG
            if (DebugMode)
                Debug.Log(string.Format("StageManager instantiated [width: {0}; height:{1}]", _size.X, _size.Y));
#endif

#if RELEASE
            // check piracy
            //GenuineChecker.Check();
#endif

#if RELEASE
            _acme = (Acme) Framework.GetComponent<Acme>(true);
            _lockStyle = LockButtonStyle.Instance;
            //if (!Application.isEditor) {
                _startTime = Time.time;
                System.Random random = new System.Random();
                _timeToWait = random.Next(10, 1200); // 10s - / 1200 = 20 min
                if (Application.isEditor)
                    _timeToWait += 300; // 5 minutes 
            //}
#endif
        }

        #region Properties

        //public bool ResetAspectRatioOnResize;

        #endregion

        #region Members

        private readonly Point _size = new Point();

#if RELEASE
        #region RELEASE

        private Acme _acme;

        /// <summary>
        /// GUIStyle for lock
        /// </summary>
        private GUIStyle _lockStyle;

        /// <summary>
        /// Dialog done?
        /// </summary>
        private bool _dialogDone;

        /// <summary>
        /// Application start time
        /// </summary>
        private float _startTime;

        /// <summary>
        /// Time to wait (in seconds) before the dialog is popped up
        /// </summary>
        private float _timeToWait;

        private readonly TweenFactory _alertAddedEffect = new TweenFactory(
            new Sequence(new LockAlertAddedAnimation())
        );

        private readonly TweenFactory _alertRemovedEffect = new TweenFactory(
            new Sequence(new LockAlertRemovedAnimation())
        );

        #endregion
#endif

        #endregion

        #region Stage registration

        /* Important: starting from -1, to initialize the 1st scene here! */
        private int _loadedLevel = -1;

        public void RegisterStage(Stage stage)
        {
#if DEBUG
            if (DebugMode)
                Debug.Log(string.Format("Registering stage [{0}] at depth {1}", stage, stage.ZIndex));
#endif      
            if (_stageListDesc.Contains(stage))
            {
                Debug.LogWarning(string.Format("Multiple registration of the same Stage [{0}]", stage));
                return;
            }

            if (_stageListDesc.Exists(delegate(Stage s) { return s.ZIndex == stage.ZIndex; }))
                Debug.LogWarning(string.Format("Note: Duplicated stage Z-index: {0} [{1}]", stage.ZIndex, stage));

            // add it to the collection (it has to be done prior to running the StyleInitializer, because the initializer uses this list
            _stageListDesc.Add(stage);

            // resize to screen size
            /*stage.Width = _size.X;
            stage.Height = _size.Y;*/

            stage.SetActualSize(_size.X, _size.Y);
            
            // NOTE: without the explicit sizing, loading mask doesn't center itself (?) (20131217)
            // (also should be done on screen resize)
            stage.Width = _size.X;
            stage.Height = _size.Y;

            StyleInitializer.Run();
            InitStageStyles(stage);

            // initialize (create children etc.)
            stage.Initialize();

            // sort stage list by depth on each addition
            Sort();
        }

        private static void InitStageStyles(Stage stage/*, bool fireStylesInitialized*/)
        {
            try
            {
                stage.RegenerateStyleCache(true);
                stage.StyleChanged(null);
                stage.NotifyStyleChangeInChildren(null, null, true);
                /*if (fireStylesInitialized)
                    */
                stage.StylesInitialized();
            }
            catch (Exception ex)
            {
                //Debug.LogError(ex.Message);
                throw new Exception("Couldn't init Stage styles", ex);
            }
        }

        public void UnregisterStage(Stage stage)
        {
#if DEBUG
            if (DebugMode) {
                Debug.Log("Unregistering stage: " + stage);
                Debug.Log("Contains: " + _stageListDesc.Contains(stage));
            }
#endif
            _stageListDesc.Remove(stage);
            _stageListAsc.Remove(stage);
        }

        /// <summary>
        /// Gets the stage at a supplied depth<br/>
        /// If not found, returns null
        /// </summary>
        /// <param name="depth"></param>
        /// <returns></returns>
        public Stage GetStageAtDepth(int depth)
        {
            return _stageListDesc.Find(delegate(Stage stage) { return stage.ZIndex == depth; });
        }

        /// <summary>
        /// Sorts the stage list (descending)
        /// </summary>
        public void Sort()
        {
            // sort stage list descending
            _stageListDesc.Sort(Stage.DescendingDepthComparison);

            // copy and sort the other list ascending
            _stageListAsc.Clear();
            _stageListAsc.AddRange(_stageListDesc);
            _stageListAsc.Reverse();

#if DEBUG
            if (DebugMode)
            {
                string s = "";
                int count = 0;
                int total = _stageListDesc.Count;
                _stageListDesc.ForEach(delegate (Stage stage)
                                        {
                                            s += stage;
                                            if (count < total - 1)
                                                s += ", ";
                                            count++;
                                        });
                if (DebugMode)
                    Debug.Log("Stage list sorted (bottom to top): " + s);
            }
#endif 
        }
        
        #endregion

        #region Resizing

        /**
         *  
         */ 
#pragma warning disable 649
        private bool _synchronousResize;
#pragma warning restore 649
        /// <summary>
        /// Update immediately when screen resizes so that we may appear in sync with it rather than visually "catching up" (experimental)
        /// </summary>
        public bool SynchronousResize
        {
            get { return _synchronousResize; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="size"></param>
        private void ResizeHandler(Point size)
        {
//#if DEBUG
//            if (DebugMode)
//                Debug.Log(string.Format("StageManager resized from[{0}, {1}] to: [{2}, {3}]", _size.X, _size.Y, size.X, size.Y));
//#endif

            _size.X = size.X;
            _size.Y = size.Y;

            // resize each stage
            _stageListDesc.ForEach(delegate(Stage stage)
                                    {
                                        /* NOTE: setting Width and Height doesn't trigger CommitProperties (because this is the top container ?) so no RESIZE event is dispatched */
                                        /*stage.Width = size.X;
                                        stage.Height = size.Y;*/

                                        /* However, SetActualSize does it internally (without invalidation) */
                                        stage.SetActualSize(size.X, size.Y);

                                        // NOTE: without the explicit sizing, loading mask doesn't center itself (?) (20131217)
                                        // (also should be done on stage registration)

                                        stage.Width = _size.X;
                                        stage.Height = _size.Y;

                                        // Update immediately when screen resizes so that we may appear
                                        // in sync with it rather than visually "catching up";
                                        if (_synchronousResize)
                                            stage.ValidateNow();
                                    });

//            if (ResetAspectRatioOnResize && Camera.current)
//            {
//#if DEBUG
//                if (DebugMode)
//                {
//                    Debug.Log("*** Reseting aspect ratio on camera ***");
//                    Camera.current.ResetAspect();
//                }
//#endif
//            }
        }

        #endregion

        #region Drawing

        #region Slots

        private void ResizeSlot(params object[] parameters)
        {
            ResizeHandler(parameters[0] as Point);
        }

        private void RenderSlot(params object[] parameters)
        {
            //Debug.Log("RenderSlot");
            Render();
        }

        public void Render()
        {

#if RELEASE

            /* HACK CHECK */
            if (null == _acme || !_acme.gameObject.activeInHierarchy || !_acme.enabled || _lockStyle == null)
                return;

            /* HACK CHECK (ANIMATION) */
            Acme2 acme = (Acme2) Framework.GetComponent<Acme2>(true);
            if (null == acme || !acme.gameObject.activeInHierarchy || !acme.enabled)
                return;

            if (!Application.isEditor && !_dialogDone && (Time.time - _startTime) > _timeToWait)
            {
                _dialogDone = true;
                const string text = @"Created using the free version of eDriven.Gui.
Please purchase the package and support further development!";
                //Debug.Log(text);

                Alert.Show(
                        new AlertOption(AlertOptionType.Title, "Info"),
                        new AlertOption(AlertOptionType.Message, text),
                        new AlertOption(AlertOptionType.Button, new AlertButtonDescriptor("ok", "OK", true)),
                        new AlertOption(AlertOptionType.AddedEffect, _alertAddedEffect),
                        new AlertOption(AlertOptionType.RemovedEffect, _alertRemovedEffect)
                    );
            }
#endif
            TextFieldFocusHelper.RenderDummyTextField();

            _stageListDesc.ForEach(delegate(Stage stage)
            {
                if (stage.QVisible)
                    stage.Draw();
            });

            if (FocusManager.AutoCorrectUnityFocus/* && Event.current.type == EventType.Repaint)*/)
                TextFieldFocusHelper.HandleFocus();
        }

        /// <summary>
        /// Subscribe to level init signal (Awake)
        /// We have to Flush styles on each scene change
        /// because we have to load new definitions in the next scene
        /// </summary>
        /// <param name="parameters"></param>
        /*private static void LevelInitSlot(params object[] parameters)
        {
            Debug.Log("##### LevelInitSlot #####");
            // initialize styles
            //StyleInitializer.Run();
        }*/

        /// <summary>
        /// Subscribe to scene change signal
        /// We have to Flush style mapper loader on each scene change
        /// because we have to load new definitions in the next scene
        /// </summary>
        /// <param name="parameters"></param>
        private static void SceneChangeSlot(params object[] parameters)
        {
            Debug.Log("##### SceneChangeSlot #####");
            // initialize styles
            //StyleInitializer.Run();
        }

        /*private static void LevelInitSlot(params object[] parameters) // 20131124 TODO
        {
            Debug.Log("##### LevelInitSlot #####");
            //StyleMapperLoader.Instance.Flush(); // TODO
            //StyleManager.Instance.ClearStyleDeclarations(); // TODO

            // initialize styles
            StyleInitializer.Run();
        }*/

        private void DisposingSlot(params object[] parameters)
        {
            Dispose();
        }

        #endregion

        #endregion

        #region Implementation of IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            //SystemManager.Instance.UpdateSignal.Disconnect(_updateSlot);
            SystemManager.Instance.RenderSignal.Disconnect(RenderSlot);
            SystemManager.Instance.ResizeSignal.Disconnect(ResizeSlot);
            //SystemManager.Instance.LevelInitSignal.Disconnect(LevelInitSlot);
            //SystemManager.Instance.LevelLoadedSignal.Disconnect(LevelInitSlot);
            SystemManager.Instance.SceneChangeSignal.Disconnect(SceneChangeSlot);
            //SystemManager.Instance.LevelLoadedSignal.Disconnect(LevelInitSlot);

            List<Stage> stages = new List<Stage>();
            _stageListDesc.ForEach(delegate (Stage s)
                                       {
                                           stages.Add(s);
                                       });
            stages.ForEach(delegate(Stage s) { s.Dispose(); });

            _stageListDesc.Clear();
            _stageListAsc.Clear();
            _instance = null;
        }

        #endregion

    }
}