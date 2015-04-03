using Assets.eDriven.Demo.Helpers;
using Assets.eDriven.Demo.Models;
using eDriven.Core.Caching;
using eDriven.Core.Geom;
using eDriven.Gui.Components;
using eDriven.Gui.Containers;
using eDriven.Gui.Data;
using eDriven.Gui.Form;
using eDriven.Gui.Managers;
using eDriven.Gui.Plugins;
using UnityEngine;
using Event=eDriven.Core.Events.Event;

namespace Assets.eDriven.Demo.Dialogs
{
    public class ResolutionDialog : Dialog
    {
        private Button _btnSubmit;
        private Button _btnCancel;
        private List _list;
        private CheckBox _chkFullScreen;

        public ResolutionDialog()
        {
            Title = "Resolution";
            //Padding = 10;
            Width = 400;

            CloseOnEsc = true;
            //Resizable = true;
            //ScrollContent = true;
            //Layout = new AbsoluteLayoutOld();
            //MarginLeft = 10; // TODO

            var btnClose = new Button
                               {
                                   Icon = (Texture)Resources.Load("Icons/close_16"), 
                                   Width = 20, 
                                   Height = 20
                               };
            btnClose.Click += delegate { PopupManager.Instance.RemovePopup(this); };

            //ToolGroup.AddChild(btnClose);
        }

        override protected void CreateChildren()
        {
            base.CreateChildren();

            Form form = new Form {PercentWidth = 100/*, Padding = 0*/};
            AddContentChild(form);

            // full screen checkbox
            _chkFullScreen = new CheckBox
                                 {
                                     Text = "Full screen",
                                     StyleName = "checkbox",
                                     Right = 10,
                                     Top = 10,
                                     //Padding = 0,
                                     ResizeWithContent = true,
                                     ToggleMode = true,
                                     FocusEnabled = false,
                                     Selected = OptionsModel.Instance.FullScreen
                                 };
            _chkFullScreen.Change += delegate
                                         {
                                             _list.Enabled = _chkFullScreen.Selected;
                                             HandleSubmitButton();
                                         };
            form.AddField("fullScreen", "Full screen:", _chkFullScreen);

            // resolution list
            _list = new List
                        {
                            //PercentWidth = 100,
                            RequireSelection = true,
                            SelectedItem = OptionsModel.Instance.Resolution,
                            Enabled = OptionsModel.Instance.FullScreen,
                            DataProvider = new ArrayList(Application.isEditor
                                               ? ResolutionHelper.GetDummyResolutionList()
                                               : ResolutionHelper.GetResolutionList())
                        };
            /*_list.SelectedIndexChanged += delegate
                                              {
                                                  HandleSubmitButton();
                                              };*/
            form.AddField("list", "Resolution:", _list);

            #region Buttons

            _btnSubmit = new Button
                             {
                                 Text = "Set resolution",
                                 Icon = ImageLoader.Instance.Load("Icons/accept"),
                                 Enabled = false
                             };
            _btnSubmit.Press += SetResolution;
            ControlBarGroup.AddChild(_btnSubmit);

            _btnCancel = new Button
                             {
                                 Text = "Cancel",
                                 Icon = ImageLoader.Instance.Load("Icons/cancel")
                             };
            _btnCancel.Press += delegate
                                    {
                                        ExecCallback(CANCEL);
                                    };
            ControlBarGroup.AddChild(_btnCancel);

            #endregion

            Plugins.Add(new TabManager());
        }

        public override System.Collections.Generic.List<DisplayListMember> GetTabChildren()
        {
            var list = new System.Collections.Generic.List<DisplayListMember> { _list, _btnSubmit, _btnCancel };
            return  list;
        }

        public override void SetFocus()
        {
            base.SetFocus();

            _list.SetFocus();
        }

        private void HandleSubmitButton()
        {
            //Debug.Log("OptionsModel.Instance.Resolution: " + OptionsModel.Instance.Resolution);
            //Debug.Log("_list.SelectedValue: " + _list.SelectedValue);
            // TODO: Bug with List not respecting the SelectedValue
            ResolutionDescriptor r = (ResolutionDescriptor)_list.SelectedItem;
            bool changed = !(r == OptionsModel.Instance.Resolution && 
                             _chkFullScreen.Selected == OptionsModel.Instance.FullScreen);
            //Debug.Log("changed: " + changed);
            _btnSubmit.Enabled = changed;
        }

        private void SetResolution(Event e)
        {
            bool fullScreen = _chkFullScreen.Selected;
            ResolutionDescriptor resolutionDescriptor = (ResolutionDescriptor)_list.SelectedItem;
            
            OptionsModel.Instance.FullScreen = fullScreen;
            OptionsModel.Instance.Resolution = resolutionDescriptor;

            Resolution resolution = resolutionDescriptor.Resolution;

            Debug.Log(string.Format("Setting resoulution to {0} [full screen: {1}]", new Point(resolution.width, resolution.height), fullScreen));
            Screen.SetResolution(resolution.width, resolution.height, fullScreen);

            ExecCallback(SUBMIT);
        }
    }
}