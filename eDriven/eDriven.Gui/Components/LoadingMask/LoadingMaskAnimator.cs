using eDriven.Animation;
using eDriven.Animation.Animation;
using eDriven.Gui.Reflection;
using eDriven.Gui.Styles;
using UnityEngine;

namespace eDriven.Gui.Components
{
    [Style(Name = "skinClass", Default = typeof(LoadingMaskSkin))]
    [Style(Name = "animationPackage", Type = typeof(string), Default = "Progress/default/package")]
    [Style(Name = "animationId", Type = typeof(string), Default = "progress")]

    public class LoadingMaskAnimator : ProgressIndicatorBase
    {
        public LoadingMaskAnimator()
        {
            MouseEnabled = true;
            MouseChildren = true;
            StopMouseWheelPropagation = true;
        }

        #region Skin parts

        // ReSharper disable UnassignedField.Global
        // ReSharper disable MemberCanBePrivate.Global

        ///<summary>
        /// Title label
        ///</summary>
        [SkinPart(Required = false)]
        public Label LabelDisplay;

        ///<summary>
        /// Title label
        ///</summary>
        [SkinPart(Required = false)]
        public Image IconDisplay;

        // ReSharper restore MemberCanBePrivate.Global
        // ReSharper restore UnassignedField.Global

        #endregion

        protected override void PartAdded(string partName, object instance)
        {
            base.PartAdded(partName, instance);

            if (instance == LabelDisplay)
                LabelDisplay.Text = Message;

            /*if (instance == IconDisplay)
                IconDisplay.Initialize();*/
        }

        private bool _animationIdChanged;
        private string _animationId;
        /// <summary>
        /// Animation ID
        /// </summary>
        public string AnimationId
        {
            get { 
                return _animationId;
            }
            set
            {
                if (value == _animationId)
                    return;
                {
                    _animationId = value;
                    _animationIdChanged = true;
                    InvalidateProperties();
                }
            }
        }

        //public static AnimationPackage Package;

        private bool _packageChanged;
        private AnimationPackage _package;
        /// <summary>
        /// Animation package
        /// </summary>
        public AnimationPackage Package
        {
            get { 
                return _package;
            }
            set
            {
                if (value == _package)
                    return;
                {
                    _package = value;
                    _packageChanged = true;
                    InvalidateProperties();
                }
            }
        }

        private readonly AnimationPackageLoader _loader = new AnimationPackageLoader();

        #region Styles

        public override void StylesInitialized()
        {
            base.StylesInitialized();

            Package = _loader.Load((string)GetStyle("animationPackage"));
            AnimationId = (string)GetStyle("animationId");
        }

        public override void StyleChanged(string styleName)
        {
            base.StyleChanged(styleName);
            switch (styleName)
            {
                case "animationPackage":
                    Package = _loader.Load((string)GetStyle(styleName));
                    //Debug.Log("Package changed to: " + Package);
                    break;

                case "animationId":
                    AnimationId = (string)GetStyle(styleName);
                    //Debug.Log("AnimationId changed to: " + AnimationId);
                    break;
            }
        }

        #endregion

        private void OnFrameChange(object[] parameters)
        {
            Frame f = (Frame)parameters[0];
            IconDisplay.Texture = f.Texture;
        }

        private string _message;
        public override string Message
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
                if (null != LabelDisplay)
                {
                    LabelDisplay.Text = _message;
                    //LabelDisplay.Visible = LabelDisplay.IncludeInLayout = !string.IsNullOrEmpty(_message);
                }
            }
        }

        public override void Play()
        {
            //Debug.Log("Package: " + Package);
            if (null == _animator)
                _animator = new FrameAnimator(); //AnimationPackageLoader.Load("Progress/default/package") 

            //Animation animation = (Animation) Package.Get("progress"); //.Clone();
            //_animator.SetAnimation("progress", animation);
            _animator.FrameChangeSignal.Connect(OnFrameChange);
            
            _animator.Package = Package;
            _animator.Play(AnimationId); // "progress"
        }

        public override void Stop()
        {
            if (null != _animator)
            {
                _animator.FrameChangeSignal.Disconnect(OnFrameChange);
                _animator.Stop();
            }
        }

        protected override void CreationComplete()
        {
            base.CreationComplete();

            if (!string.IsNullOrEmpty(_message))
                Play();
        }

        private FrameAnimator _animator;

        //public void Reset()
        //{
        //    _animator.Reset();
        //}

        protected override void CommitProperties()
        {
            base.CommitProperties();

            if (_packageChanged || _animationIdChanged)
            {
                _packageChanged = false;
                _animationIdChanged = false;

                //Debug.Log("*****");

                if (null != _animator && _animator.IsPlaying)
                {
                    Stop();
                    Play();
                }
            }
        }
    }
}