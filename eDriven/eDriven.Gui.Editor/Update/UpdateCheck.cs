#region License

/*
 
Copyright (c) 2010-2014 Danko Kozar

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
 
*/

#endregion License

using System;
using System.Collections.Generic;
using System.Globalization;
using eDriven.Core.Serialization;
using eDriven.Gui.Editor.Dialogs;
using eDriven.Gui.Editor.Update;
using UnityEditor;
using UnityEngine;
using EditorSettings=eDriven.Gui.Editor.EditorSettings;
using Info=eDriven.Gui.Info;

internal class UpdateCheck
{
#if DEBUG
    // ReSharper disable UnassignedField.Global
        public static bool DebugMode;
    // ReSharper restore UnassignedField.Global
#endif

    #region Singleton

    private static UpdateCheck _instance;

    /// <summary>
    /// Singleton class for handling focus
    /// </summary>
    private UpdateCheck()
    {
        // Constructor is protected
    }

    /// <summary>
    /// Singleton instance
    /// </summary>
    public static UpdateCheck Instance
    {
        get
        {
            if (_instance == null)
            {
#if DEBUG
                    if (DebugMode)
                        Debug.Log(string.Format("Instantiating UpdateCheck instance"));
#endif
                _instance = new UpdateCheck();
                Initialize();
            }

            return _instance;
        }
    }

    #endregion

    /// <summary>
    /// Change this constant according to the version of eDriven.Gui 
    /// </summary>
    private const int MinMessageId = 3;

    /// <summary>
    /// Initializes the Singleton instance
    /// </summary>
    private static void Initialize()
    {
        //Debug.Log("### UpdateCheck: Initialize ###");
    }

    private WWW _request;
    private XmlLoadingState _xmlState;
    private List<InfoMessage> _messages = new List<InfoMessage>();
    
    internal bool DataReady { get; private set; }

    private bool _skipDialogIfIfNoNewUpdates;

    /// <summary>
    /// Runs the update check
    /// </summary>
    /// <param name="checkIfNeeded">Should the update be run only if really needed (which is being checked against settings)</param>
    /// <param name="skipDialogIfIfNoNewUpdates">Should the dialog be skipped if no updates available (the dialog normally shows there are no updates)</param>
    /// <param name="isAutomaticCheck">Is it the check by the system on startup</param>
    public void Run(bool checkIfNeeded, bool skipDialogIfIfNoNewUpdates, bool isAutomaticCheck)
    {
        if (null != _request)
            return;

        _skipDialogIfIfNoNewUpdates = skipDialogIfIfNoNewUpdates;

        bool isTimeToCheck = false;

        if (!checkIfNeeded)
        {
            isTimeToCheck = true;
        }

        if (!string.IsNullOrEmpty(EditorSettings.LastTimeChecked))
        {
            DateTime lastCheck = DateTime.Parse(EditorSettings.LastTimeChecked);

            //Debug.Log("EditorSettings.UpdateCheckPeriod: " + EditorSettings.UpdateCheckPeriod);
            var hours = CheckForUpdatePeriodTranslation.Instance[EditorSettings.UpdateCheckPeriod];
            //Debug.Log("hours: " + hours);

            TimeSpan ts = DateTime.Now.Subtract(lastCheck);
            //Debug.Log("ts.TotalHours: " + ts.TotalHours);
            if (ts.TotalHours > hours)
            {
                //Debug.Log("Should check");
                isTimeToCheck = true; // TODO: calculate if enought time passed
            }
            else if (ts.TotalHours < 0.0027) // don't alow to repeat the operation earlier than 10s
            {
                //Debug.Log("Too early");

                if (!isAutomaticCheck)
                {
                    EditorUtility.DisplayDialog("Update check message", @"The update check has been called multiple times in a row.

Please wait 10 seconds, and then run the update check again.", "OK");
                }
                return;
            }
//            else
//            {
//                Debug.Log("Still no time to check");
//            }
        }
        else
        {
            isTimeToCheck = true;
        }

        //Debug.Log("isTimeToCheck: " + isTimeToCheck);
        //isTimeToCheck = true; // TEMP (for test)

        if (isTimeToCheck)
        {
            Debug.Log("eDriven.Gui update check");
            string url = string.Format("http://edriven.dankokozar.com/gui/update/config?v={0}&m={1}&{2}", Info.AssemblyVersion, 
                Math.Max(EditorSettings.LastMessageId, MinMessageId), (DateTime.Now - new DateTime(1970, 1, 1)).Ticks);
            //Debug.Log(url);
            _request = new WWW(url);
            EditorApplication.update += Update;

            EditorSettings.LastTimeChecked = DateTime.Now.ToString(CultureInfo.InvariantCulture);
        }

        //EditorSettings.LastTimeChecked = null; // TEMP for test
    }

    private void Update()
    {
        //Debug.Log("OnTimerTick");
        if (_request.isDone)
        {
            EditorApplication.update -= Update;
            if (string.IsNullOrEmpty(_request.error))
            {
                ProcessXml(_request.text);
                _request = null;
            }
            else
            {
                Debug.LogWarning("Cannot get update");
            }
        }
    }

    private void ProcessXml(string xml)
    {
//        Debug.Log(@"UpdateCheck: Processing XML:
//" + xml);
        try
        {
            Configuration configuration = XmlSerializer<Configuration>.Deserialize(xml);

            if (null == configuration)
            {
                _xmlState = XmlLoadingState.Error;
                return;
            }

            _messages = configuration.Messages;
        }
        catch (Exception)
        {
            Debug.Log("Error loading data");
        }

        //Debug.Log("Processed: " + _messages.Count + " messages.");
        DataReady = true;
    }

    private enum XmlLoadingState
    {
        Loading, LoadingLogo, Finished, Error
    }

    /// <summary>
    /// Shows the update dialog
    /// It has to be called from OnGUI()
    /// If there is any message, the dialog will be popped up displaying those messages
    /// If there is no messages, the dialog will pop up if "showNoUpdatesDialog" has been set to true
    /// Else. no dialog well be shown
    /// This is done so that the ordinary check (on startup) could skip the dialog if no new messages (because it wants to be silent)
    /// </summary>
    public void ShowDialog()
    {
        //Debug.Log("ShowDialog");

        if (_skipDialogIfIfNoNewUpdates && _messages.Count == 0)
            return;

        // bring up the Add Event Handler dialog
        var dialog = UpdateCheckDialog.Instance;

        //Debug.Log("_messages: " + _messages.Count);
        dialog.Data = _messages;
        dialog.ShowUtility();

        DataReady = false;
    }
}