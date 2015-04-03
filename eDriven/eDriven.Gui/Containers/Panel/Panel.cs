using eDriven.Gui.Components;
using eDriven.Gui.Reflection;
using eDriven.Gui.Styles;
using UnityEngine;

namespace eDriven.Gui.Containers
{
    [Style(Name = "skinClass", Default = typeof(PanelSkin))]
    
    public class Panel : SkinnableContainer
    {
        public Panel()
        {
            MouseEnabled = true;
            MinWidth = 131;
            MinHeight = 127;
        }

        private bool _iconChanged;
        private Texture _icon;
        public Texture Icon
        {
            get { 
                return _icon;
            }
            set
            {
                if (value == _icon)
                    return;

                _icon = value;
                _iconChanged = true;
                InvalidateProperties();
            }
        }

        private bool _titleChanged;
        private string _title;
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                _titleChanged = true;
                InvalidateProperties();
            }
        }

        #region Skin parts

        // ReSharper disable MemberCanBeProtected.Global
        // ReSharper disable FieldCanBeMadeReadOnly.Global

        private readonly DoubleGroup _headerDoubleGroup = new DoubleGroup();

        ///<summary>Header group
        ///</summary>
        [SkinPart(Required = false)]
        public Group HeaderGroup
        {
            get { return _headerDoubleGroup.Part; }
            internal set { _headerDoubleGroup.Part = value; }
        }

        ///<summary>
        /// Icon display
        ///</summary>
        [SkinPart(Required = false)]
        public Image HeaderIconDisplay;

        ///<summary>
        /// Label display
        ///</summary>
        [SkinPart(Required = false)]
        public Label TitleDisplay;

        private readonly DoubleGroup _toolDoubleGroup = new DoubleGroup();

        ///<summary>
        /// Tool group 
        ///</summary>
        [SkinPart(Required = false)]
        public Group ToolGroup
        {
            get { return _toolDoubleGroup.Part; }
            internal set { _toolDoubleGroup.Part = value; }
        }

        // ReSharper enable MemberCanBeProtected.Global
        // ReSharper enable FieldCanBeMadeReadOnly.Global

        private readonly DoubleGroup _controlBarDoubleGroup = new DoubleGroup();

        ///<summary>
        /// Control bar group 
        ///</summary>
        [SkinPart(Required = false)]
        public Group ControlBarGroup
        {
            get
            {
                return _controlBarDoubleGroup.Part;
            }
            internal set
            {
                _controlBarDoubleGroup.Part = value;
            }
        }

        #endregion

        #region Lifecycle methods

        protected override void PartAdded(string partName, object instance)
        {
            base.PartAdded(partName, instance);

            if (instance == TitleDisplay)
            {
                //Debug.Log("TitleLabel added: " + TitleLabel);
                TitleDisplay.Text = _title;
            }

            else if (instance == ToolGroup)
            {
                if (_toolDoubleGroup.Modified)
                {
                    _toolDoubleGroup.MoveToReal();
                }
            }

            else if (instance == ControlBarGroup)
            {
                if (_controlBarDoubleGroup.Modified) {
                    _controlBarDoubleGroup.MoveToReal();
                }
            }
        }

        protected override void PartRemoved(string partName, object instance)
        {
            base.PartRemoved(partName, instance);
            
            if (instance == ToolGroup)
            {
                if (_toolDoubleGroup.Modified)
                {
                    _toolDoubleGroup.MoveToPlaceholder();
                }
            }

            else if (instance == ControlBarGroup)
            {
                if (_controlBarDoubleGroup.Modified) {
                    _controlBarDoubleGroup.MoveToPlaceholder();
                }
            }
        }

        protected override void CommitProperties()
        {
            base.CommitProperties();

            if (_iconChanged)
            {
                _iconChanged = false;
                if (null != HeaderIconDisplay)
                    HeaderIconDisplay.Texture = _icon;
            }

            if (_titleChanged)
            {
                _titleChanged = false;
                if (null != TitleDisplay)
                    TitleDisplay.Text = _title;
            }
        }

        #endregion
    }
}