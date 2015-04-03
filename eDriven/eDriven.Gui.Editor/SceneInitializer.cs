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
using eDriven.Audio;
using eDriven.Gui.Designer.Util;
using eDriven.Gui.Editor.Commands;
using eDriven.Gui.Editor.Persistence;
using eDriven.Gui.Mappers;
using UnityEditor;
using UnityEngine;
using Object=UnityEngine.Object;

namespace eDriven.Gui.Editor
{
    internal static class SceneInitializer
    {
#if DEBUG 
// ReSharper disable UnassignedField.Global
    /// <summary>
    /// Debug mode
    /// </summary>
    public static bool DebugMode = true;
// ReSharper restore UnassignedField.Global
#endif

        private const string GuiPath = "GUI";
        private const string AudioMapperPath = "GUI/Mappers/Audio";
        private const string FontMapperPath = "GUI/Mappers/Fonts";
#pragma warning disable 169
        private const string StyleMapperPath = "GUI/Mappers/Styles";
#pragma warning restore 169

        private const int NumberOfAudioSources = 16;
        private const int NumberOfAudioTokens = 16;

        private static bool _fontInitialized;
        private static bool _audioInitialized;

        public static FontMapper GetFontMapper(out MapperInfo info)
        {
            FontMapper mapper = null;
            info = MapperInfo.NotFound;
            Object[] fontMappers = Object.FindObjectsOfType(typeof(FontMapper));

            // traverse mappers
            foreach (Object o in fontMappers)
            {
                mapper = (FontMapper)o;
                //if (mapper.Id == CreateFontMapperCommand.FontMapperId)
                if (mapper.Default)
                {
                    if (!mapper.enabled) {
                        info = MapperInfo.MapperNotEnabled;
                        break;
                    }

                    if (null == mapper.Font)
                    {
                        info = MapperInfo.FontNotAttached;
                        break;
                    }

                    info = MapperInfo.Ok;
                    break;
                }
            }
            return mapper;
        }

        private static AudioPlayerMapper GetAudioMapper(out MapperInfo info)
        {
            AudioPlayerMapper mapper = null;
            info = MapperInfo.NotFound;
            Object[] audioMappers = Object.FindObjectsOfType(typeof(AudioPlayerMapper));

            // traverse mappers
            foreach (Object o in audioMappers)
            {
                mapper = (AudioPlayerMapper)o;
                if (mapper.Default)
                {
                    //if (!mapper.gameObject.activeInHierarchy)
                    //    info = AudioPlayerMapperInfo.MapperGameObjectNotEnabled;
                    info = !mapper.enabled ? MapperInfo.MapperNotEnabled : MapperInfo.Ok;
                    break;
                }
            }
            return mapper;
        }

        internal static bool Init()
        {
            InitFont();
            if (!_fontInitialized)
                return false;

            InitAudio();
            if (!_audioInitialized)
                return false;

            // select the "GUI" node
            var go = GameObjectUtil.CreateGameObjectAtPath(GuiPath);
            Selection.objects = new Object[] { go };

            return true;
        }

        private static void InitFont()
        {
            _fontInitialized = false;

            MapperInfo info;
            FontMapper mapper = GetFontMapper(out info);

            //Debug.Log("_fontInitialized: " + _fontInitialized);

            bool result;
            switch (info)
            {
                case MapperInfo.NotFound:
                    string text = @"eDriven.Gui needs a default font mapper for its components to work.

No default font mapper found in the scene.

Would you like to create it?";

                    if (EditorApplication.isPlaying)
                        text += @"

(Play mode will be stopped)";

                    result = EditorUtility.DisplayDialog("eDriven.Gui Info", text, "Yes", "No");
                    if (result)
                    {
#if DEBUG
                        if (DebugMode)
                        {
                            Debug.Log("Creating font mapper");
                        }
#endif

                        if (EditorApplication.isPlaying)
                        {
                            FontMapperAdditionProcessor.FontMapperPath = FontMapperPath;
                            Debug.Log("Stopping the application");
                            EditorApplication.isPlaying = false;
                            return;
                        }

                        CreateFontMapperCommand cmd = new CreateFontMapperCommand {FontMapperPath = FontMapperPath};
                        cmd.Run();
                    }
                    else
                    {
                        _fontInitialized = true;
                    }
                    break;
                case MapperInfo.MapperNotEnabled:
                    result = EditorUtility.DisplayDialog("Info", @"Default font mapper found but not enabled.

Would you like to enable it?", "Yes", "No");
                    if (result)
                    {
                        Debug.Log("Enabling default font mapper");
                        if (null == mapper)
                            throw new Exception("Error: default font mapper is null");
                        mapper.enabled = true;
                    }
                    _fontInitialized = true;
                    break;
                case MapperInfo.FontNotAttached:
                    EditorUtility.DisplayDialog("Info", @"Default font mapper found but no font attached.

Please attach the font.", "OK");
                    Selection.activeGameObject = mapper.gameObject;
                    EditorGUIUtility.PingObject(mapper);
                    break;
                default:
#if DEBUG
                    if (DebugMode)
                    {
                        Debug.Log("Font OK");
                    }
#endif
                    _fontInitialized = true;
                    break;
            }
        }

        private static void InitAudio()
        {
            _audioInitialized = false;

            MapperInfo info;
            AudioPlayerMapper mapper = GetAudioMapper(out info);

            bool result;
            switch (info)
            {
                case MapperInfo.NotFound:
                    string text = @"No default audio mapper found.

Would you like to create it?";

                    if (EditorApplication.isPlaying)
                        text += @"

(Play mode will be stopped)";

                    result = EditorUtility.DisplayDialog("Info", text, "Yes", "No");

                    if (result)
                    {
#if DEBUG
                        if (DebugMode)
                        {
                            Debug.Log("Creating audio mapper");
                        }
#endif

                        if (EditorApplication.isPlaying)
                        {
                            AudioMapperAdditionProcessor.AudioMapperPath = AudioMapperPath;
                            Debug.Log("Stopping the application");
                            EditorApplication.isPlaying = false;
                            return;
                        }

                        CreateAudioMapperCommand cmd = new CreateAudioMapperCommand
                                                           {
                                                               AudioMapperPath = AudioMapperPath,
                                                               NumberOfAudioSources = NumberOfAudioSources,
                                                               NumberOfAudioTokens = NumberOfAudioTokens
                                                           };
                        cmd.Run();
                    }
                    //else
                    //{
                    //    // never mind
                    //    _audioInitialized = true;
                    //}
                    break;
                case MapperInfo.MapperNotEnabled:
                    result = EditorUtility.DisplayDialog("Info", @"Audio mapper found but not enabled.

Would you like to enable it?", "Yes", "No");
                    if (result)
                    {
                        Debug.Log("Enabling default audio player mapper");
                        if (null == mapper)
                            throw new Exception("Error: mapper is null");
                        mapper.enabled = true;
                    }
                    _audioInitialized = true;
                    break;
                default:
#if DEBUG
                    if (DebugMode)
                    {
                        Debug.Log("Audio OK");
                    }
#endif
                    _audioInitialized = true;
                    break;
            }
        }

        ///// <summary>
        ///// TODO
        ///// </summary>
        //public static void InitStyleMapper()
        //{
        //    // TODO
        //    // ask to add the default style mapper for the particular component
        //    // use reflection
        //    // Component->StyleProxy->StyleName
        //}
    }

    internal enum MapperInfo
    {
        NotFound, MapperNotEnabled, FontNotAttached, Ok
    }
}
