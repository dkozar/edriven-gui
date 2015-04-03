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

using eDriven.Audio;
using eDriven.Gui.Designer.Util;
using eDriven.Gui.Editor.Persistence;
using UnityEditor;
using UnityEngine;

namespace eDriven.Gui.Editor.Commands
{
    internal class CreateAudioMapperCommand
    {
        public string AudioMapperPath;
        public int NumberOfAudioSources = 16;
        public int NumberOfAudioTokens = 16;

        public void Run()
        {
            GameObject go = GameObjectUtil.CreateGameObjectAtPath(AudioMapperPath);

            /**
             * 1. AudioPlayerMapper
             * */
            AudioPlayerMapper m = (AudioPlayerMapper)go.AddComponent(typeof(AudioPlayerMapper));
            m.Default = true; // set default

            /**
             * 2. N x AudioSource
             * */
            for (int i = 0; i < NumberOfAudioSources; i++)
            {
                AudioSource source = (AudioSource)go.AddComponent(typeof(AudioSource));
                source.playOnAwake = false; // do not play on awake
            }

            /**
             * 3. N x AudioToken
             * */
            for (int i = 0; i < NumberOfAudioTokens; i++)
            {
                go.AddComponent(typeof(AudioToken));
            }

            EditorUtility.DisplayDialog("Info", string.Format(@"Default audio mapper created at ""{0}""

You should drag this mapper to your camera.", AudioMapperPath), "OK");
            EditorGUIUtility.PingObject(m);
            Selection.objects = new Object[] { m };

            /**
             * 3. Re-scan the hierarchy
             * */
            HierarchyViewDecorator.Instance.ReScan(/*adapter.GetInstanceID()*/);
        }
    }
}