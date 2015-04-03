using System;
using UnityEngine;

namespace eDriven.Gui.Components
{
    public class TextBase : SimpleComponent
    {
        private bool _textChanged;
        private string _text = string.Empty;
        private bool _multilineChanged;
        private bool _multiline;
        private bool _textureChanged;
        private Texture _texture;

        /// <summary>
        /// Button Label
        /// </summary>
        public virtual string Text
        {
            get { return _text; }
            set
            {
                if (value != _text)
                {
                    _text = value;
                    _textChanged = true;
                    InvalidateProperties();
                    //InvalidateSize();
                }
            }
        }

        public bool Multiline
        {
            get { 
                return _multiline;
            }
            set
            {
                if (value == _multiline)
                    return;
                {
                    _multiline = value;
                    _multilineChanged = true;
                    InvalidateProperties();
                }
            }
        }

        
        protected override void InitializeContent()
        {
            //Debug.Log("Label -> InitializeContent: " + _text);

            base.InitializeContent();

            //Debug.Log("Multiline: " + _multiline);

            /**
             * Initialize GUIContent
             * */
            if (!_multiline)
                ClipFirstLine();

            //Content = new GUIContent();
            if (!string.IsNullOrEmpty(_text))
                Content.text = _text;

            // Tooltip
            //if (!string.IsNullOrEmpty(Tooltip))
            //    Content.tooltip = Tooltip;
        }

        private bool _rendered;
        /// <summary>
        /// This is a flag indicated that a text field has been rendered at least once
        /// Focus management is relying on this property
        /// </summary>
        public bool Rendered
        {
            get { return _rendered; }
            set
            {
                if (_rendered || !value)
                    return;

                _rendered = true;
                DispatchEvent(new FrameworkEvent(FrameworkEvent.FIRST_SHOW));
            }
        }

        #region INVALIDATION

        protected override void CommitProperties()
        {
            base.CommitProperties();

            //Debug.Log("Label:CommitProperties");

            if (_textChanged || _textureChanged)
            {
                //Debug.Log("Text: " + _text);

                //if (_textureChanged && null != Content)
                //    UnityEngine.Object.Destroy(Content.image);

                _textChanged = false;
                _textureChanged = false;

                InitializeContent();
                InvalidateSize();
                //InvalidateParentSizeAndDisplayList(); // --> we don't need this actually
            }

            if (_multilineChanged)
            {
                _multilineChanged = false;
                //InitializeContent();
                InvalidateSize();
                if (!_multiline) // bug solved 20130324 (there was no negation present)
                    ClipFirstLine();
            }
        }

        #endregion

        #region Helper

        protected void ClipFirstLine()
        {
            if (null == _text)
                return;
            string[] arr = _text.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            _text = arr.Length > 0 ? arr[0] : string.Empty;
        }

        #endregion
    }
}