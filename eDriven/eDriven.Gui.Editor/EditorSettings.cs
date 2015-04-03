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

using eDriven.Gui.Editor.IO;
using UnityEditor;
using UnityEngine;

namespace eDriven.Gui.Editor
{
	internal static class EditorSettings
	{
		private const string WatchChangesKey = "eDriven_c77f7aa2-d77c-4dd8-9b8b-e60a46ab0400";
		private const string AutoSaveKey = "eDriven_1bfd337e-97f3-4a55-b6fc-b40b709070c5";
		private const string InspectorEnabledKey = "eDriven_3e84370d-ce1b-4aea-9d8b-25bdde674cc4";
		private const string LiveStylingKey = "eDriven_f804b56d-7c54-48b5-98ca-ad74b9c4f97e";

		private const string ShowControlsKey = "eDriven_5afcbf58-3b21-4c28-9daf-1e18493f834e";
		private const string TabIndexKey = "eDriven_08a7fc94-d015-4677-8d85-c766a07691b5";
		private const string ShowEventPhasesKey = "eDriven_33e20d01-a651-4296-99a6-2cf9c8f1097b";
		private const string ShowAddHandlerPanelKey = "eDriven_0ce67063-e86f-4f64-8729-7f44dc512623";
		private const string ShowDebugOptionsInInspectorKey = "eDriven_c5c75707-4d0e-417a-9ce3-5061e99ecb73";
		private const string ShowGuidInInspectorKey = "eDriven_db23af2e-72e0-433f-a514-72fe4502c199";
		private const string ShowUniqueIdInInspectorKey = "eDriven_fc5969c9-9729-4f24-8909-5c22377c33b8";
		private const string ShowEventTypePopupKey = "eDriven_c41d96c2-6966-4d89-a694-f2ea9399e621";
		private const string ShowAddChildOptionsKey = "eDriven_862a2aaf-46e7-48af-a885-9091081d403a";
		private const string SelectUponCreationKey = "eDriven_76fc8bd3-8e57-4e04-8b9b-798670a7630e";
		private const string ExpandWidthUponCreationKey = "eDriven_5aa939c1-6a71-4f67-82b5-3ba7d664fb93";
		private const string ExpandHeightUponCreationKey = "eDriven_ac8aae88-9bdf-4d07-9578-5f420f1c8a3f";
		private const string FactoryModeUponCreationKey = "eDriven_0e67c3c7-e025-43d6-b1e0-9ace8228ac9f";
		private const string ApplyLastUsedSkinUponCreationKey = "eDriven_ad535962-c5ce-45d7-94af-c87f60d789d1";
		private const string ComponentDescriptorMainExpandedKey = "eDriven_d45655fc-1439-4c6b-88b1-6eb98ea69914";
		private const string ComponentDescriptorSizingExpandedKey = "eDriven_2cf65bf1-fa62-4784-b29c-93812676252b";
		private const string ComponentDescriptorConstrainsExpandedKey = "eDriven_66500f62-1e36-41e9-b036-10d72ba2c19a";
		private const string ComponentDescriptorPaddingExpandedKey = "eDriven_05545aee-d698-4abd-b922-fc56b40d8851";
		//private const string ShowSkinClassPopupKey = "eDriven_83bdda57-b31f-4c7c-9774-3face491a1ff";
		private const string ShowStyleMapperPopupKey = "eDriven_b32c3921-68bf-408a-aa1e-2454d46f33e0";
		private const string ScriptExtensionKey = "eDriven_9337b811-b5b2-41d7-a2fc-05f03ea2b617";
		private const string AddEventHandlerInputModeKey = "eDriven_622537bd-a56c-49b4-8f7f-6b55b8571644";
		private const string CreationSettingsCapturePhaseKey = "eDriven_31cbe994-2f7c-4c35-8e07-8f5624482cd4";
		private const string CreationSettingsTargetPhaseKey = "eDriven_21e0c8d5-7256-4b91-9da0-05e40214bea4";
		private const string CreationSettingsBubblingPhaseKey = "eDriven_5d09148d-3b22-4fee-9006-89dd4c5859fa";
		private const string CreationSettingsCastKey = "eDriven_1c3c768f-18a0-451d-aef3-7e42e2f2058a";
		private const string CreationSettingsAddComponentInstantiatedHandlerKey = "eDriven_f0f593ea-3e8f-4c71-a3d9-7f525127fe87";
		private const string CreationSettingsAddInitializeComponentHandlerKey = "eDriven_fce462ff-c9c7-40e9-aa20-88777b9d1876";
		private const string CreationSettingsOpenScriptKey = "eDriven_5d09148d-3b22-4fee-9006-89dd4c5859fa";
		private const string MouseDoubleClickEnabledKey = "eDriven_2921e653-7805-4ea8-b5aa-f2ce6873e00c";
		private const string UseDarkSkinKey = "eDriven_b70faf87-f557-4d0b-8712-189d7df6ff61";
		private const string CheckForUpdatesKey = "eDriven_5f061c5d-13a3-4447-93ad-2e0addd227b4";
		private const string ReadyToProcessHierarchyChangesKey = "eDriven_be778486-ef7e-4b26-833f-a583311a3fca";
		private const string LastMessageIdKey = "eDriven_237c42fe-d0f7-4357-a528-c26f03a18163";
		private const string LastTimeCheckedKey = "eDriven_3a062eec-c046-4e98-8dc0-d88ea6bfb2a6";
		private const string UpdateCheckPeriodKey = "eDriven_956543fc-ed48-4899-822f-7e4652457c83";

		private const string HierarchyWindowWriteToLogKey = "eDriven_70412112-a3c5-497f-9d5c-e913b3d917de";
		private const string PersistenceWindowWriteToLogKey = "eDriven_572abdf2-e4e3-4acb-b632-a68ecc19194d";

		private const string ReferenceTabIndexKey = "eDriven_1d717f94-661e-4fb5-ae33-a588148dc349";
		private const string ReferenceSelectedTypeKey = "eDriven_83291279-7ed0-4dc8-9490-f3433d2f6ca0";
		
		private const string ReferenceShowComponentsKey = "eDriven_7e5ff227-2578-41ee-9394-ea3854a2bcda";
		private const string ReferenceShowSkinnableComponentsKey = "eDriven_dbe87433-0def-47ac-8a2a-bacbdf146e5a";
		private const string ReferenceShowSkinsKey = "eDriven_7580f85f-6b63-4e4c-96e6-e52ad797b335";

		private const string ReferencePrintEventsKey = "eDriven_b91e59fa-f209-4eb1-86ac-93548a2e406a";
		private const string ReferencePrintMulticastDelegatesKey = "eDriven_2e9d0574-9ac7-4386-a5f1-f4ddac6ba968";
		private const string ReferencePrintStylesKey = "eDriven_e0f9195e-0c03-4cb1-a828-2f9941c590bb";
		private const string ReferencePrintSkinsKey = "eDriven_2e6e48df-1bd5-47f7-8b81-6879d6e10a1f";
		private const string ReferencePrintSkinPartsKey = "eDriven_9520c3fd-1e29-4ef8-88d6-386197ffe16e";
		private const string ReferencePrintSkinStatesKey = "eDriven_b89480fc-a31f-40ee-b1dd-b8ef6b2bcce4";
		private const string ReferencePrintSignalsKey = "eDriven_a4142352-09c4-45a8-9a7b-487b30af42ac";

		private const string PaddingExpandedKey = "eDriven_a88c4922-580e-41a3-b65d-83192fc44832";
		private const string HierarchyWindowAutoUpdateKey = "eDriven_7bb3ae3f-8b31-4002-8957-096cd930ff1f";
		private const string PersistenceWindowAutoUpdateKey = "eDriven_5bb3e2af-1768-4f18-92a4-0acdffc15110";

		//private const string SceneNameKey = "27ed896f-3a99-4fbd-aa1b-8847ccdfd392";

#if DEBUG
// ReSharper disable UnassignedField.Global
		/// <summary>
		/// Debug mode
		/// </summary>
		public static bool DebugMode;

// ReSharper restore UnassignedField.Global
#endif

		static EditorSettings()
		{
			//Debug.Log("-----> Getting EditorSettings");

			// defaults
			if (!EditorPrefs.HasKey(ShowEventTypePopupKey))
				EditorPrefs.SetBool(ShowEventTypePopupKey, true);
			if (!EditorPrefs.HasKey(ShowStyleMapperPopupKey))
				EditorPrefs.SetBool(ShowStyleMapperPopupKey, true);
			if (!EditorPrefs.HasKey(WatchChangesKey))
				EditorPrefs.SetBool(WatchChangesKey, true);
			if (!EditorPrefs.HasKey(ScriptExtensionKey))
				EditorPrefs.SetString(ScriptExtensionKey, ScriptExtensions.CSHARP);
			if (!EditorPrefs.HasKey(CreationSettingsTargetPhaseKey))
				EditorPrefs.SetBool(CreationSettingsTargetPhaseKey, true);
			if (!EditorPrefs.HasKey(CreationSettingsBubblingPhaseKey))
				EditorPrefs.SetBool(CreationSettingsBubblingPhaseKey, true);
			if (!EditorPrefs.HasKey(CreationSettingsCastKey))
				EditorPrefs.SetBool(CreationSettingsCastKey, true);
			if (!EditorPrefs.HasKey(CreationSettingsOpenScriptKey))
				EditorPrefs.SetBool(CreationSettingsOpenScriptKey, true);
			if (!EditorPrefs.HasKey(MouseDoubleClickEnabledKey))
				EditorPrefs.SetBool(MouseDoubleClickEnabledKey, true);
			if (!EditorPrefs.HasKey(UseDarkSkinKey))
				EditorPrefs.SetBool(UseDarkSkinKey, EditorGUIUtility.isProSkin);//  UnityEditorInternal.InternalEditorUtility.HasPro());
			if (!EditorPrefs.HasKey(ReadyToProcessHierarchyChangesKey))
				EditorPrefs.SetBool(ReadyToProcessHierarchyChangesKey, true);
			if (!EditorPrefs.HasKey(CheckForUpdatesKey))
				EditorPrefs.SetBool(CheckForUpdatesKey, true);
			if (!EditorPrefs.HasKey(LastMessageIdKey))
				EditorPrefs.SetInt(LastMessageIdKey, 0);
			if (!EditorPrefs.HasKey(UpdateCheckPeriodKey))
				EditorPrefs.SetInt(UpdateCheckPeriodKey, 2);

			if (!EditorPrefs.HasKey(ReferenceShowComponentsKey))
				EditorPrefs.SetBool(ReferenceShowComponentsKey, true);
			if (!EditorPrefs.HasKey(ReferenceShowSkinnableComponentsKey))
				EditorPrefs.SetBool(ReferenceShowSkinnableComponentsKey, true);
			if (!EditorPrefs.HasKey(ReferenceShowSkinsKey))
				EditorPrefs.SetBool(ReferenceShowSkinsKey, true);
			if (!EditorPrefs.HasKey(ApplyLastUsedSkinUponCreationKey))
				EditorPrefs.SetBool(ApplyLastUsedSkinUponCreationKey, true);
			if (!EditorPrefs.HasKey(ReferencePrintEventsKey))
				EditorPrefs.SetBool(ReferencePrintEventsKey, true);
			if (!EditorPrefs.HasKey(ReferencePrintMulticastDelegatesKey))
				EditorPrefs.SetBool(ReferencePrintMulticastDelegatesKey, true);
			if (!EditorPrefs.HasKey(ReferencePrintStylesKey))
				EditorPrefs.SetBool(ReferencePrintStylesKey, true);
			if (!EditorPrefs.HasKey(ReferencePrintSkinsKey))
				EditorPrefs.SetBool(ReferencePrintSkinsKey, true);
			if (!EditorPrefs.HasKey(ReferencePrintSkinPartsKey))
				EditorPrefs.SetBool(ReferencePrintSkinPartsKey, true);
			if (!EditorPrefs.HasKey(ReferencePrintSkinStatesKey))
				EditorPrefs.SetBool(ReferencePrintSkinStatesKey, true);
			if (!EditorPrefs.HasKey(ReferencePrintSignalsKey))
				EditorPrefs.SetBool(ReferencePrintSignalsKey, true);
			if (!EditorPrefs.HasKey(PaddingExpandedKey))
				EditorPrefs.SetBool(PaddingExpandedKey, true);
			if (!EditorPrefs.HasKey(PersistenceWindowAutoUpdateKey))
				EditorPrefs.SetBool(PersistenceWindowAutoUpdateKey, true);
		}

		public static bool WatchChanges
		{
			get { return EditorPrefs.GetBool(WatchChangesKey); }
			set
			{
				if (value == EditorPrefs.GetBool(WatchChangesKey))
					return;

				EditorPrefs.SetBool(WatchChangesKey, value);
#if DEBUG
				if (DebugMode)
				{
					Debug.Log(string.Format(@"WatchChanges changed to " + value));
				}
#endif
			}
		}

		public static bool AutoSave
		{
			get { return EditorPrefs.GetBool(AutoSaveKey); }
			set
			{
				if (value == EditorPrefs.GetBool(AutoSaveKey))
					return;

				EditorPrefs.SetBool(AutoSaveKey, value);
#if DEBUG
				if (DebugMode)
				{
					Debug.Log(string.Format(@"AutoSave changed to " + value));
				}
#endif
			}
		}

		public static bool InspectorEnabled
		{
			get { return EditorPrefs.GetBool(InspectorEnabledKey); }
			set
			{
				if (value == EditorPrefs.GetBool(InspectorEnabledKey))
					return;

				EditorPrefs.SetBool(InspectorEnabledKey, value);
#if DEBUG
	if (DebugMode)
	{
		Debug.Log(string.Format(@"InspectorEnabled changed to " + value));
	}
#endif
			}
		}
		
		/// <summary>
		/// Live styling is turned off upon the first-time load (for a better user experience)
		/// </summary>
		public static bool LiveStyling
		{
			get { return EditorPrefs.GetBool(LiveStylingKey); }
			set
			{
				if (value == EditorPrefs.GetBool(LiveStylingKey))
					return;

				EditorPrefs.SetBool(LiveStylingKey, value);
#if DEBUG
				if (DebugMode)
				{
					Debug.Log(string.Format(@"LiveStyling changed to " + value));
				}
#endif
			}
		}
		
		public static bool ShowControls
		{
			get { return EditorPrefs.GetBool(ShowControlsKey); }
			set
			{
				if (value == EditorPrefs.GetBool(ShowControlsKey))
					return;

				EditorPrefs.SetBool(ShowControlsKey, value);
#if DEBUG
				if (DebugMode)
				{
					Debug.Log(string.Format(@"ShowControls changed to " + value));
				}
#endif
			}
		}

		public static int TabIndex
		{
			get { return EditorPrefs.GetInt(TabIndexKey); }
			set
			{
				if (value == EditorPrefs.GetInt(TabIndexKey))
					return;

				EditorPrefs.SetInt(TabIndexKey, value);
#if DEBUG
				if (DebugMode)
				{
					Debug.Log(string.Format(@"TabIndex changed to " + value));
				}
#endif
			}
		}

		public static bool ShowEventPhases
		{
			get { return EditorPrefs.GetBool(ShowEventPhasesKey); }
			set
			{
				if (value == EditorPrefs.GetBool(ShowEventPhasesKey))
					return;

				EditorPrefs.SetBool(ShowEventPhasesKey, value);
#if DEBUG
				if (DebugMode)
				{
					Debug.Log(string.Format(@"ShowEventPhases changed to " + value));
				}
#endif
			}
		}

		public static bool ShowAddHandlerPanel
		{
			get { return EditorPrefs.GetBool(ShowAddHandlerPanelKey); }
			set
			{
				if (value == EditorPrefs.GetBool(ShowAddHandlerPanelKey))
					return;

				EditorPrefs.SetBool(ShowAddHandlerPanelKey, value);
#if DEBUG
				if (DebugMode)
				{
					Debug.Log(string.Format(@"ShowAddHandlerPanel changed to " + value));
				}
#endif
			}
		}

		public static bool ShowDebugOptionsInInspector
		{
			get { return EditorPrefs.GetBool(ShowDebugOptionsInInspectorKey); }
			set
			{
				if (value == EditorPrefs.GetBool(ShowDebugOptionsInInspectorKey))
					return;

				EditorPrefs.SetBool(ShowDebugOptionsInInspectorKey, value);
#if DEBUG
				if (DebugMode)
				{
					Debug.Log(string.Format(@"ShowDebugOptionsInInspector changed to " + value));
				}
#endif
			}
		}

		public static bool ShowGuidInInspector
		{
			get { return EditorPrefs.GetBool(ShowGuidInInspectorKey); }
			set
			{
				if (value == EditorPrefs.GetBool(ShowGuidInInspectorKey))
					return;

				EditorPrefs.SetBool(ShowGuidInInspectorKey, value);
#if DEBUG
				if (DebugMode)
				{
					Debug.Log(string.Format(@"ShowGuidInInspector changed to " + value));
				}
#endif
			}
		}

		public static bool ShowUniqueIdInInspector
		{
			get { return EditorPrefs.GetBool(ShowUniqueIdInInspectorKey); }
			set
			{
				if (value == EditorPrefs.GetBool(ShowUniqueIdInInspectorKey))
					return;

				EditorPrefs.SetBool(ShowUniqueIdInInspectorKey, value);
#if DEBUG
				if (DebugMode)
				{
					Debug.Log(string.Format(@"ShowUniqueIdInInspector changed to " + value));
				}
#endif
			}
		}

		public static bool ShowEventTypePopup
		{
			get { return EditorPrefs.GetBool(ShowEventTypePopupKey); }
			set
			{
				if (value == EditorPrefs.GetBool(ShowEventTypePopupKey))
					return;

				EditorPrefs.SetBool(ShowEventTypePopupKey, value);
#if DEBUG
				if (DebugMode)
				{
					Debug.Log(string.Format(@"ShowEventTypePopup changed to " + value));
				}
#endif
			}
		}

		public static bool ShowAddChildOptions
		{
			get { return EditorPrefs.GetBool(ShowAddChildOptionsKey); }
			set
			{
				if (value == EditorPrefs.GetBool(ShowAddChildOptionsKey))
					return;

				EditorPrefs.SetBool(ShowAddChildOptionsKey, value);
#if DEBUG
				if (DebugMode)
				{
					Debug.Log(string.Format(@"ShowAddChildOptions changed to " + value));
				}
#endif
			}
		}

		public static bool SelectUponCreation
		{
			get { return EditorPrefs.GetBool(SelectUponCreationKey); }
			set
			{
				if (value == EditorPrefs.GetBool(SelectUponCreationKey))
					return;

				EditorPrefs.SetBool(SelectUponCreationKey, value);
#if DEBUG
				if (DebugMode)
				{
					Debug.Log(string.Format(@"SelectUponCreation changed to " + value));
				}
#endif
			}
		}

		public static bool ExpandWidthUponCreation
		{
			get { return EditorPrefs.GetBool(ExpandWidthUponCreationKey); }
			set
			{
				if (value == EditorPrefs.GetBool(ExpandWidthUponCreationKey))
					return;

				EditorPrefs.SetBool(ExpandWidthUponCreationKey, value);
#if DEBUG
				if (DebugMode)
				{
					Debug.Log(string.Format(@"ExpandWidthUponCreation changed to " + value));
				}
#endif
			}
		}

		public static bool ExpandHeightUponCreation
		{
			get { return EditorPrefs.GetBool(ExpandHeightUponCreationKey); }
			set
			{
				if (value == EditorPrefs.GetBool(ExpandHeightUponCreationKey))
					return;

				EditorPrefs.SetBool(ExpandHeightUponCreationKey, value);
#if DEBUG
				if (DebugMode)
				{
					Debug.Log(string.Format(@"ExpandHeightUponCreation changed to " + value));
				}
#endif
			}
		}

		public static bool FactoryModeUponCreation
		{
			get { return EditorPrefs.GetBool(FactoryModeUponCreationKey); }
			set
			{
				if (value == EditorPrefs.GetBool(FactoryModeUponCreationKey))
					return;

				EditorPrefs.SetBool(FactoryModeUponCreationKey, value);
#if DEBUG
				if (DebugMode)
				{
					Debug.Log(string.Format(@"FactoryModeUponCreation changed to " + value));
				}
#endif
			}
		}

		public static bool ApplyLastUsedSkinUponCreation
		{
			get { return EditorPrefs.GetBool(ApplyLastUsedSkinUponCreationKey); }
			set
			{
				if (value == EditorPrefs.GetBool(ApplyLastUsedSkinUponCreationKey))
					return;

				EditorPrefs.SetBool(ApplyLastUsedSkinUponCreationKey, value);
#if DEBUG
				if (DebugMode)
				{
					Debug.Log(string.Format(@"ApplyLastUsedSkinUponCreation changed to " + value));
				}
#endif
			}
		}

		public static bool ComponentDescriptorMainExpanded
		{
			get { return EditorPrefs.GetBool(ComponentDescriptorMainExpandedKey); }
			set
			{
				if (value == EditorPrefs.GetBool(ComponentDescriptorMainExpandedKey))
					return;

				EditorPrefs.SetBool(ComponentDescriptorMainExpandedKey, value);
#if DEBUG
				if (DebugMode)
				{
					Debug.Log(string.Format(@"ComponentDescriptorMainExpanded changed to " + value));
				}
#endif
			}
		}

		public static bool ComponentDescriptorSizingExpanded
		{
			get { return EditorPrefs.GetBool(ComponentDescriptorSizingExpandedKey); }
			set
			{
				if (value == EditorPrefs.GetBool(ComponentDescriptorSizingExpandedKey))
					return;

				EditorPrefs.SetBool(ComponentDescriptorSizingExpandedKey, value);
#if DEBUG
				if (DebugMode)
				{
					Debug.Log(string.Format(@"ComponentDescriptorSizingExpanded changed to " + value));
				}
#endif
			}
		}

		public static bool ComponentDescriptorConstrainsExpanded
		{
			get { return EditorPrefs.GetBool(ComponentDescriptorConstrainsExpandedKey); }
			set
			{
				if (value == EditorPrefs.GetBool(ComponentDescriptorConstrainsExpandedKey))
					return;

				EditorPrefs.SetBool(ComponentDescriptorConstrainsExpandedKey, value);
#if DEBUG
				if (DebugMode)
				{
					Debug.Log(string.Format(@"ComponentDescriptorConstrainsExpanded changed to " + value));
				}
#endif
			}
		}

		public static bool ComponentDescriptorPaddingExpanded
		{
			get { return EditorPrefs.GetBool(ComponentDescriptorPaddingExpandedKey); }
			set
			{
				if (value == EditorPrefs.GetBool(ComponentDescriptorPaddingExpandedKey))
					return;

				EditorPrefs.SetBool(ComponentDescriptorPaddingExpandedKey, value);
#if DEBUG
				if (DebugMode)
				{
					Debug.Log(string.Format(@"ComponentDescriptorPaddingExpanded changed to " + value));
				}
#endif
			}
		}

//        public static bool ShowSkinClassPopup
//        {
//            get { return EditorPrefs.GetBool(ShowSkinClassPopupKey); }
//            set
//            {
//                if (value == EditorPrefs.GetBool(ShowSkinClassPopupKey))
//                    return;

//                EditorPrefs.SetBool(ShowSkinClassPopupKey, value);
//#if DEBUG
//                if (DebugMode)
//                {
//                    Debug.Log(string.Format(@"ShowSkinClassPopup changed to " + value));
//                }
//#endif
//            }
//        }

		public static bool ShowStyleMapperPopup
		{
			get { return EditorPrefs.GetBool(ShowStyleMapperPopupKey); }
			set
			{
				if (value == EditorPrefs.GetBool(ShowStyleMapperPopupKey))
					return;

				EditorPrefs.SetBool(ShowStyleMapperPopupKey, value);
#if DEBUG
				if (DebugMode)
				{
					Debug.Log(string.Format(@"ShowStyleMapperPopup changed to " + value));
				}
#endif
			}
		}

		public static bool AddEventHandlerInputMode
		{
			get { return EditorPrefs.GetBool(AddEventHandlerInputModeKey); }
			set
			{
				if (value == EditorPrefs.GetBool(AddEventHandlerInputModeKey))
					return;

				EditorPrefs.SetBool(AddEventHandlerInputModeKey, value);
#if DEBUG
				if (DebugMode)
				{
					Debug.Log(string.Format(@"AddEventHandlerInputMode changed to " + value));
				}
#endif
			}
		}

		public static string ScriptExtension
		{
			get { return EditorPrefs.GetString(ScriptExtensionKey); }
			set
			{
				if (value == EditorPrefs.GetString(ScriptExtensionKey))
					return;

				EditorPrefs.SetString(ScriptExtensionKey, value);
#if DEBUG
				if (DebugMode)
				{
					Debug.Log(string.Format(@"ScriptExtension changed to " + value));
				}
#endif
			}
		}

		public static bool CreationSettingsCapturePhase
		{
			get { return EditorPrefs.GetBool(CreationSettingsCapturePhaseKey); }
			set
			{
				if (value == EditorPrefs.GetBool(CreationSettingsCapturePhaseKey))
					return;

				EditorPrefs.SetBool(CreationSettingsCapturePhaseKey, value);
#if DEBUG
				if (DebugMode)
				{
					Debug.Log(string.Format(@"CreationSettingsCapturePhase changed to " + value));
				}
#endif
			}
		}

		public static bool CreationSettingsTargetPhase
		{
			get { return EditorPrefs.GetBool(CreationSettingsTargetPhaseKey); }
			set
			{
				if (value == EditorPrefs.GetBool(CreationSettingsTargetPhaseKey))
					return;

				EditorPrefs.SetBool(CreationSettingsTargetPhaseKey, value);
#if DEBUG
				if (DebugMode)
				{
					Debug.Log(string.Format(@"CreationSettingsTargetPhase changed to " + value));
				}
#endif
			}
		}

		public static bool CreationSettingsBubblingPhase
		{
			get { return EditorPrefs.GetBool(CreationSettingsBubblingPhaseKey); }
			set
			{
				if (value == EditorPrefs.GetBool(CreationSettingsBubblingPhaseKey))
					return;

				EditorPrefs.SetBool(CreationSettingsBubblingPhaseKey, value);
#if DEBUG
				if (DebugMode)
				{
					Debug.Log(string.Format(@"CreationSettingsBubblingPhase changed to " + value));
				}
#endif
			}
		}

		public static bool CreationSettingsCast
		{
			get { return EditorPrefs.GetBool(CreationSettingsCastKey); }
			set
			{
				if (value == EditorPrefs.GetBool(CreationSettingsCastKey))
					return;

				EditorPrefs.SetBool(CreationSettingsCastKey, value);
#if DEBUG
				if (DebugMode)
				{
					Debug.Log(string.Format(@"CreationSettingsCast changed to " + value));
				}
#endif
			}
		}

		public static bool CreationSettingsAddComponentInstantiatedHandler
		{
			get { return EditorPrefs.GetBool(CreationSettingsAddComponentInstantiatedHandlerKey); }
			set
			{
				if (value == EditorPrefs.GetBool(CreationSettingsAddComponentInstantiatedHandlerKey))
					return;

				EditorPrefs.SetBool(CreationSettingsAddComponentInstantiatedHandlerKey, value);
#if DEBUG
				if (DebugMode)
				{
					Debug.Log(string.Format(@"CreationSettingsAddComponentInstantiatedHandler changed to " + value));
				}
#endif
			}
		}

		public static bool CreationSettingsAddInitializeComponentHandler
		{
			get { return EditorPrefs.GetBool(CreationSettingsAddInitializeComponentHandlerKey); }
			set
			{
				if (value == EditorPrefs.GetBool(CreationSettingsAddInitializeComponentHandlerKey))
					return;

				EditorPrefs.SetBool(CreationSettingsAddInitializeComponentHandlerKey, value);
#if DEBUG
				if (DebugMode)
				{
					Debug.Log(string.Format(@"CreationSettingsAddInitializeComponentHandler changed to " + value));
				}
#endif
			}
		}

		public static bool CreationSettingsOpenScript
		{
			get { return EditorPrefs.GetBool(CreationSettingsOpenScriptKey); }
			set
			{
				if (value == EditorPrefs.GetBool(CreationSettingsOpenScriptKey))
					return;

				EditorPrefs.SetBool(CreationSettingsOpenScriptKey, value);
#if DEBUG
				if (DebugMode)
				{
					Debug.Log(string.Format(@"CreationSettingsOpenScript changed to " + value));
				}
#endif
			}
		}

		public static bool MouseDoubleClickEnabled
		{
			get { return EditorPrefs.GetBool(MouseDoubleClickEnabledKey); }
			set
			{
				if (value == EditorPrefs.GetBool(MouseDoubleClickEnabledKey))
					return;

				EditorPrefs.SetBool(MouseDoubleClickEnabledKey, value);
#if DEBUG
				if (DebugMode)
				{
					Debug.Log(string.Format(@"MouseDoubleClickEnabled changed to " + value));
				}
#endif
			}
		}

		public static bool UseDarkSkin
		{
			get { return EditorPrefs.GetBool(UseDarkSkinKey); }
			set
			{
				if (value == EditorPrefs.GetBool(UseDarkSkinKey))
					return;

				EditorPrefs.SetBool(UseDarkSkinKey, value);
#if DEBUG
				if (DebugMode)
				{
					Debug.Log(string.Format(@"UseDarkSkin changed to " + value));
				}
#endif
			}
		}

		public static bool CheckForUpdates
		{
			get { return EditorPrefs.GetBool(CheckForUpdatesKey); }
			set
			{
				if (value == EditorPrefs.GetBool(CheckForUpdatesKey))
					return;

				EditorPrefs.SetBool(CheckForUpdatesKey, value);
#if DEBUG
				if (DebugMode)
				{
					Debug.Log(string.Format(@"CheckForUpdates changed to " + value));
				}
#endif
			}
		}

		public static int UpdateCheckPeriod
		{
			get { return EditorPrefs.GetInt(UpdateCheckPeriodKey); }
			set
			{
				if (value == EditorPrefs.GetInt(UpdateCheckPeriodKey))
					return;

				EditorPrefs.SetInt(UpdateCheckPeriodKey, value);
#if DEBUG
				if (DebugMode)
				{
					Debug.Log(string.Format(@"UpdateCheckPeriod changed to " + value));
				}
#endif
			}
		}

		public static int LastMessageId
		{
			get { return EditorPrefs.GetInt(LastMessageIdKey); }
			set
			{
				if (value == EditorPrefs.GetInt(LastMessageIdKey))
					return;

				EditorPrefs.SetInt(LastMessageIdKey, value);
#if DEBUG
				if (DebugMode)
				{
					Debug.Log(string.Format(@"LastMessageId changed to " + value));
				}
#endif
			}
		}

		public static string LastTimeChecked
		{
			get { return EditorPrefs.GetString(LastTimeCheckedKey); }
			set
			{
				if (value == EditorPrefs.GetString(LastTimeCheckedKey))
					return;

				EditorPrefs.SetString(LastTimeCheckedKey, value);
#if DEBUG
				if (DebugMode)
				{
					Debug.Log(string.Format(@"LastMessageId changed to " + value));
				}
#endif
			}
		}

		/// <summary>
		/// Variable containing the number of OnHierarchyChange frames to be skipped
		/// This number is being set on Play mode stop to 1 or 2, depending if there are changes or not:
		/// 1 = only assembly reload (no changes)
		/// 2 = assembly reload + applying changes
		/// This is important because we don't want to react to the hierarchy changed caused by the persistence mechanism
		/// (we have to ignore this change)
		/// Immediatelly after this OnHierarchyChange call we have to set this to 0 so the changes in Edit mode could then be processed
		/// </summary>
		public static int ReadyToProcessHierarchyChanges
		{
			get { return EditorPrefs.GetInt(ReadyToProcessHierarchyChangesKey); }
			set
			{
				if (value == EditorPrefs.GetInt(ReadyToProcessHierarchyChangesKey))
					return;

				EditorPrefs.SetInt(ReadyToProcessHierarchyChangesKey, value);
#if DEBUG
				if (DebugMode)
				{
					Debug.Log(string.Format(@"ReadyToProcessHierarchyChanges changed to " + value));
				}
				//Debug.Log(string.Format(@"*** ReadyToProcessHierarchyChanges changed to " + value));
#endif
			}
		}

		public static bool HierarchyWindowWriteToLog
		{
			get { return EditorPrefs.GetBool(HierarchyWindowWriteToLogKey); }
			set
			{
				if (value == EditorPrefs.GetBool(HierarchyWindowWriteToLogKey))
					return;

				EditorPrefs.SetBool(HierarchyWindowWriteToLogKey, value);
#if DEBUG
				if (DebugMode)
				{
					Debug.Log(string.Format(@"HierarchyWindowWriteToLog changed to " + value));
				}
#endif
			}
		}

		public static bool PersistenceWindowWriteToLog
		{
			get { return EditorPrefs.GetBool(PersistenceWindowWriteToLogKey); }
			set
			{
				if (value == EditorPrefs.GetBool(PersistenceWindowWriteToLogKey))
					return;

				EditorPrefs.SetBool(PersistenceWindowWriteToLogKey, value);
#if DEBUG
				if (DebugMode)
				{
					Debug.Log(string.Format(@"PersistenceWindowWriteToLog changed to " + value));
				}
#endif
			}
		}

		public static bool ReferencePrintEvents
		{
			get { return EditorPrefs.GetBool(ReferencePrintEventsKey); }
			set
			{
				if (value == EditorPrefs.GetBool(ReferencePrintEventsKey))
					return;

				EditorPrefs.SetBool(ReferencePrintEventsKey, value);
#if DEBUG
				if (DebugMode)
				{
					Debug.Log(string.Format(@"ReferencePrintEvents changed to " + value));
				}
#endif
			}
		}

		public static bool ReferencePrintMulticastDelegates
		{
			get { return EditorPrefs.GetBool(ReferencePrintMulticastDelegatesKey); }
			set
			{
				if (value == EditorPrefs.GetBool(ReferencePrintMulticastDelegatesKey))
					return;

				EditorPrefs.SetBool(ReferencePrintMulticastDelegatesKey, value);
#if DEBUG
				if (DebugMode)
				{
					Debug.Log(string.Format(@"ReferencePrintMulticastDelegates changed to " + value));
				}
#endif
			}
		}

		public static bool ReferencePrintStyles
		{
			get { return EditorPrefs.GetBool(ReferencePrintStylesKey); }
			set
			{
				if (value == EditorPrefs.GetBool(ReferencePrintStylesKey))
					return;

				EditorPrefs.SetBool(ReferencePrintStylesKey, value);
#if DEBUG
				if (DebugMode)
				{
					Debug.Log(string.Format(@"ReferencePrintStyles changed to " + value));
				}
#endif
			}
		}

		public static bool ReferencePrintSkins
		{
			get { return EditorPrefs.GetBool(ReferencePrintSkinsKey); }
			set
			{
				if (value == EditorPrefs.GetBool(ReferencePrintSkinsKey))
					return;

				EditorPrefs.SetBool(ReferencePrintSkinsKey, value);
#if DEBUG
				if (DebugMode)
				{
					Debug.Log(string.Format(@"ReferencePrintSkins changed to " + value));
				}
#endif
			}
		}

		public static bool ReferencePrintSkinParts
		{
			get { return EditorPrefs.GetBool(ReferencePrintSkinPartsKey); }
			set
			{
				if (value == EditorPrefs.GetBool(ReferencePrintSkinPartsKey))
					return;

				EditorPrefs.SetBool(ReferencePrintSkinPartsKey, value);
#if DEBUG
				if (DebugMode)
				{
					Debug.Log(string.Format(@"ReferencePrintSkinParts changed to " + value));
				}
#endif
			}
		}

		public static bool ReferencePrintSkinStates
		{
			get { return EditorPrefs.GetBool(ReferencePrintSkinStatesKey); }
			set
			{
				if (value == EditorPrefs.GetBool(ReferencePrintSkinStatesKey))
					return;

				EditorPrefs.SetBool(ReferencePrintSkinStatesKey, value);
#if DEBUG
				if (DebugMode)
				{
					Debug.Log(string.Format(@"ReferencePrintSkinStates changed to " + value));
				}
#endif
			}
		}

		public static bool ReferencePrintSignals
		{
			get { return EditorPrefs.GetBool(ReferencePrintSignalsKey); }
			set
			{
				if (value == EditorPrefs.GetBool(ReferencePrintSignalsKey))
					return;

				EditorPrefs.SetBool(ReferencePrintSignalsKey, value);
#if DEBUG
				if (DebugMode)
				{
					Debug.Log(string.Format(@"ReferencePrintSignals changed to " + value));
				}
#endif
			}
		}

		public static bool ReferenceShowComponents
		{
			get { return EditorPrefs.GetBool(ReferenceShowComponentsKey); }
			set
			{
				if (value == EditorPrefs.GetBool(ReferenceShowComponentsKey))
					return;

				EditorPrefs.SetBool(ReferenceShowComponentsKey, value);
#if DEBUG
				if (DebugMode)
				{
					Debug.Log(string.Format(@"ReferenceShowComponents changed to " + value));
				}
#endif
			}
		}

		public static bool ReferenceShowSkinnableComponents
		{
			get { return EditorPrefs.GetBool(ReferenceShowSkinnableComponentsKey); }
			set
			{
				if (value == EditorPrefs.GetBool(ReferenceShowSkinnableComponentsKey))
					return;

				EditorPrefs.SetBool(ReferenceShowSkinnableComponentsKey, value);
#if DEBUG
				if (DebugMode)
				{
					Debug.Log(string.Format(@"ReferenceShowSkinnableComponents changed to " + value));
				}
#endif
			}
		}

		public static bool ReferenceShowSkins
		{
			get { return EditorPrefs.GetBool(ReferenceShowSkinsKey); }
			set
			{
				if (value == EditorPrefs.GetBool(ReferenceShowSkinsKey))
					return;

				EditorPrefs.SetBool(ReferenceShowSkinsKey, value);
#if DEBUG
				if (DebugMode)
				{
					Debug.Log(string.Format(@"ReferenceShowSkins changed to " + value));
				}
#endif
			}
		}

		public static string ReferenceSelectedType
		{
			get { return EditorPrefs.GetString(ReferenceSelectedTypeKey); }
			set
			{
				if (value == EditorPrefs.GetString(ReferenceSelectedTypeKey))
					return;

				EditorPrefs.SetString(ReferenceSelectedTypeKey, value);
#if DEBUG
				if (DebugMode)
				{
					Debug.Log(string.Format(@"ReferenceSelectedType changed to " + value));
				}
#endif
			}
		}

		public static int ReferenceTabIndex
		{
			get { return EditorPrefs.GetInt(ReferenceTabIndexKey); }
			set
			{
				if (value == EditorPrefs.GetInt(ReferenceTabIndexKey))
					return;

				EditorPrefs.SetInt(ReferenceTabIndexKey, value);
#if DEBUG
				if (DebugMode)
				{
					Debug.Log(string.Format(@"ReferenceTabIndex changed to " + value));
				}
#endif
			}
		}

		public static bool PaddingExpanded
		{
			get { return EditorPrefs.GetBool(PaddingExpandedKey); }
			set
			{
				if (value == EditorPrefs.GetBool(PaddingExpandedKey))
					return;

				EditorPrefs.SetBool(PaddingExpandedKey, value);
#if DEBUG
				if (DebugMode)
				{
					Debug.Log(string.Format(@"PaddingExpanded changed to " + value));
				}
#endif
			}
		}

		public static bool HierarchyWindowAutoUpdate
		{
			get { return EditorPrefs.GetBool(HierarchyWindowAutoUpdateKey); }
			set
			{
				if (value == EditorPrefs.GetBool(HierarchyWindowAutoUpdateKey))
					return;

				EditorPrefs.SetBool(HierarchyWindowAutoUpdateKey, value);
#if DEBUG
				if (DebugMode)
				{
					Debug.Log(string.Format(@"HierarchyWindowAutoUpdate changed to " + value));
				}
#endif
			}
		}

		public static bool PersistenceWindowAutoUpdate
		{
			get { return EditorPrefs.GetBool(PersistenceWindowAutoUpdateKey); }
			set
			{
				if (value == EditorPrefs.GetBool(PersistenceWindowAutoUpdateKey))
					return;

				EditorPrefs.SetBool(PersistenceWindowAutoUpdateKey, value);
#if DEBUG
				if (DebugMode)
				{
					Debug.Log(string.Format(@"PersistenceWindowAutoUpdate changed to " + value));
				}
#endif
			}
		}
	}
}