using System;
using System.Collections;
using eDriven.Core.Util;
using eDriven.Gui.Events;
using eDriven.Gui.GUIStyles;
using eDriven.Gui.Layout;
using eDriven.Gui.Managers;
using eDriven.Gui.Styles;
using UnityEngine;
using Event=eDriven.Core.Events.Event;

namespace eDriven.Gui.Components
{
    [Style(Name = "showBackground", Type = typeof(bool), Default = false)]
    [Style(Name = "buttonStyle", Type = typeof(GUIStyle), ProxyType = typeof(PagerButtonStyle))]
    [Style(Name = "decButtonStyle", Type = typeof(GUIStyle), ProxyType = typeof(PagerButtonStyle))]
    [Style(Name = "incButtonStyle", Type = typeof(GUIStyle), ProxyType = typeof(PagerButtonStyle))]
    [Style(Name = "disabledOverlayStyle", Type = typeof(GUIStyle), Default = null)]

    public class Pager : Group
    {
        public int MaxButtons = 30;

        private readonly ObjectPool<Button> _pool = new ObjectPool<Button>();

        private Button _btnDec;
        private Button _btnInc;
        //private TextField _txtPage;
        private Group _placeholder;

        readonly Hashtable _buttonStyles = new Hashtable
                                               {
                                                   {"cursor", "pointer"}
                                               };

        private bool _totalPagesChanged;
        private int _totalPages = 1;
        public int TotalPages
        {
            get { return _totalPages; }
            set
            {
                if (value < 1)
                    throw new Exception("TotalPages has to be 1 or greater");
                _totalPages = value;
                _totalPagesChanged = true;
                InvalidateProperties();
            }
        }

        private int _currentPageIndex;
        public int CurrentPageIndex
        {
            get { return _currentPageIndex; }
            set
            {
                if (value < 0)
                    return;
                if (value > _totalPages-1 )
                    return;
                if (value == _currentPageIndex)
                    return;

                _currentPageIndex = value;
                UpdateState();

                var e = new IndexChangeEvent(IndexChangeEvent.CHANGE) { NewIndex = CurrentPageIndex };
                DispatchEvent(e);
            }
        }

        public override void Initialize()
        {
            base.Initialize();

            Layout = new HorizontalLayout
            {
                VerticalAlign = VerticalAlign.Middle,
                Gap = 2
            };
        }

        public override void StyleChanged(string styleName)
        {
            base.StyleChanged(styleName);

            switch (styleName)
            {
                case "decButtonStyle":
                    _btnDec.SetStyle("buttonStyle", GetStyle(styleName));
                    break;
                case "incButtonStyle":
                    _btnInc.SetStyle("buttonStyle", GetStyle(styleName));
                    break;
                case "buttonStyle":
                    _placeholder.Children.ForEach(delegate(DisplayListMember child)
                    {
                        Button button = child as Button;
                        if (null != button)
                            button.SetStyle("buttonStyle", GetStyle(styleName));
                    });
                    break;

            }
        }

        protected override void CreateChildren()
        {
            base.CreateChildren();

            #region Dec

            _btnDec = new Button
                          {
                              Text = "<",
                              FocusEnabled = FocusEnabled,
                              ResizeWithContent = true,
                              ResizeWithStyleBackground = true,
                              Styles = _buttonStyles,
                              //Padding = 0
                          };

            _btnDec.SetStyle("paddingLeft", 0);
            _btnDec.SetStyle("paddingRight", 0);
            _btnDec.SetStyle("paddingTop", 0);
            _btnDec.SetStyle("paddingBottom", 0);

            _btnDec.SetStyle("buttonStyle", GetStyle("decButtonStyle"));
            _btnDec.Press += delegate { Browse(-1); };
            AddChild(_btnDec);

            #endregion

            _placeholder = new Group
                               {
                                   Layout = new HorizontalLayout { Gap = 2, VerticalAlign = VerticalAlign.Middle }
                               };
            _placeholder.AddEventListener(ButtonEvent.PRESS, delegate(Event e)
                                                            {
                                                                if (e.Target is Button)
                                                                {
                                                                    CurrentPageIndex = _placeholder.GetChildIndex((DisplayListMember) e.Target);
                                                                }
                                                            });
            AddChild(_placeholder);

            for (int i = 0; i < MaxButtons; i++)
            {
                _placeholder.AddChild(CreateButton(i));
            }

            //CreateButton(0);

            #region Text Field - old

//            _txtPage = new TextField
//                           {
//                               FocusEnabled = true,
//                               Width = 35,
//                               Text = (CurrentPage + 1).ToString(),
//                               //Styles = buttonStyles
//                           };
//            _txtPage.Change += delegate
//                                   {
//                                       Debug.Log("_txtPage.Text: " + _txtPage.Text);
//                                       int i = CurrentPage;
//                                       try {
//                                           i = Convert.ToInt32(_txtPage.Text);
//                                       }
//// ReSharper disable EmptyGeneralCatchClause
//                                       catch { /* silent fail */ }
//// ReSharper restore EmptyGeneralCatchClause

//                                       Debug.Log("i: " + i);
//                                       CurrentPage = Mathf.Clamp(i, 0, TotalPages);
//                                       Debug.Log("Change: " + CurrentPage);
//                                       _txtPage.Text = CurrentPage.ToString();
//                                   };
//            AddChild(_txtPage);

            #endregion

            #region Inc

            _btnInc = new Button
                          {
                              Text = ">",
                              FocusEnabled = FocusEnabled,
                              ResizeWithContent = true,
                              ResizeWithStyleBackground = true,
                              Styles = _buttonStyles,
                              //Padding = 0
                          };

            _btnInc.SetStyle("paddingLeft", 0);
            _btnInc.SetStyle("paddingRight", 0);
            _btnInc.SetStyle("paddingTop", 0);
            _btnInc.SetStyle("paddingBottom", 0);

            _btnInc.SetStyle("buttonStyle", GetStyle("incButtonStyle"));
            _btnInc.Press += delegate { Browse(1); };
            AddChild(_btnInc);

            #endregion
        }

        /// <summary>
        /// Changes the current page by i (-1, +1)
        /// </summary>
        /// <param name="i"></param>
        private void Browse(int i)
        {
            CurrentPageIndex += i;
        }

        private void UpdateState()
        {
            //_txtPage.Text = (CurrentPage + 1).ToString();

            //Debug.Log("_totalPages: " + _totalPages);

            var count = _placeholder.Children.Count;
            for (int i = 0; i < count; i++)
            {
                var b = _placeholder.Children[i] as Button;
                if (null != b)
                {
                    b.Selected = (i == CurrentPageIndex);
                    b.Enabled = !(i == CurrentPageIndex);
                    b.Visible = b.IncludeInLayout = i < _totalPages;
                    //b.ValidateNow();
                }
            }
            //_placeholder.InvalidateSize();
            _placeholder.ValidateNow();

            _btnDec.Enabled = (CurrentPageIndex > 0);
            _btnInc.Enabled = (CurrentPageIndex < _totalPages - 1);
        }

        protected override void CommitProperties()
        {
            base.CommitProperties();

            if (_totalPagesChanged)
            {
                _totalPagesChanged = false;
                UpdateState();
            }

            if (_focusEnabledChanged)
            {
                _focusEnabledChanged = false;

                var fe = base.FocusEnabled;
                _btnDec.FocusEnabled = _btnInc.FocusEnabled = fe;
                _placeholder.Children.ForEach(delegate(DisplayListMember child)
                                                  {
                                                      InteractiveComponent comp = child as InteractiveComponent;
                                                      if (null != comp)
                                                          comp.FocusEnabled = fe;
                                                  });
            }
        }

        private Button CreateButton(int i)
        {
            Button btn = _pool.Get();
            btn.Text = (i + 1).ToString();
            btn.FocusEnabled = FocusEnabled;
            //btn.MinWidth = 20;
            //btn.Width = 25;
            //btn.Height = 25;
            //btn.ResizeWithContent = true;
            btn.ResizeWithStyleBackground = true;
            btn.Styles = _buttonStyles;
            btn.Selected = (i == CurrentPageIndex);
            btn.Visible = i < _totalPages;
            btn.IncludeInLayout = i < _totalPages;
            //btn.Padding = 0; // paddings from style

            btn.SetStyle("paddingLeft", 0);
            btn.SetStyle("paddingRight", 0);
            btn.SetStyle("paddingTop", 0);
            btn.SetStyle("paddingBottom", 0);

            return btn;
        }

        public override System.Collections.Generic.List<DisplayListMember> GetTabChildren()
        {
            System.Collections.Generic.List<DisplayListMember> list = new System.Collections.Generic.List<DisplayListMember>();
            
            if (FocusManager.IsFocusCandidate(_btnDec))
                list.Add(_btnDec);

            list.AddRange(
                _placeholder.Children.FindAll(delegate(DisplayListMember child)
                                                  {
                                                      InteractiveComponent comp = child as InteractiveComponent;
                                                      return FocusManager.IsFocusCandidate(comp);
                                                  })
                );

            if (FocusManager.IsFocusCandidate(_btnInc))
                list.Add(_btnInc);

            return list;
        }

        private bool _focusEnabledChanged;
        public override bool FocusEnabled
        {
            get
            {
                return base.FocusEnabled;
            }
            set
            {
                base.FocusEnabled = value;
                _focusEnabledChanged = true;
                InvalidateProperties();
            }
        }

        public void Reset()
        {
            _currentPageIndex = 0;
            UpdateState();
        }

        public static int CalculatePageCount(int totalNodeCount, int pageSize)
        {
            if (totalNodeCount < 0)
                throw new Exception("totalNodeCount must be 0 or greater");

            if (pageSize <= 0)
                throw new Exception("pageSize must be 1 or greater");

            //Debug.Log("totalNodeCount: " + totalNodeCount);
            int pages = (int)(Mathf.Floor(totalNodeCount / (float)pageSize));
            int res = totalNodeCount % pageSize;
            if (res > 0)
                pages += 1;
            return Math.Max(pages, 1);
        }
    }
}