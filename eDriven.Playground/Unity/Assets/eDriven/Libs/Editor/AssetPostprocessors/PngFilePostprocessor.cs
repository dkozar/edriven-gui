using System;
using UnityEditor;
using UnityEngine;

namespace eDriven.Playground.Libs.Editor.AssetPostprocessors
{
// ReSharper disable UnusedMember.Global
    /// <summary>
    /// Changes the texture type to GUI and textureFormat to AutomaticTruecolor for each texture added to the specified folder (and subfolders)
    /// </summary>
    public class PngFilePostprocessor : AssetPostprocessor {
// ReSharper restore UnusedMember.Global

// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming
        private const string ICON_PATH = "/Resources/eDriven/Editor/";
// ReSharper restore InconsistentNaming
// ReSharper restore UnusedMember.Global

        //static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths) {
        //    foreach (string str in importedAssets) {
        //        Debug.Log("Reimported Asset: " + str);
        //        Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(assetPath);
        //    }
        //}

// ReSharper disable UnusedMember.Local
        void OnPostprocessTexture (Texture2D texture) {
// ReSharper restore UnusedMember.Local

            // Only post process textures if they are in a folder
			// "invert color" or a sub folder of it.

			//var lowerCaseAssetPath = assetPath.ToLower();
            if (assetPath.IndexOf(ICON_PATH) == -1)
				return;

            var importer = assetImporter as TextureImporter;
            if (null != importer)
            {
                //Debug.Log("Handling the importer");
                //importer.anisoLevel = 0;
                //importer.filterMode = FilterMode.Bilinear;
                importer.textureType = TextureImporterType.GUI;
                importer.textureFormat = TextureImporterFormat.AutomaticTruecolor;
                Debug.Log(string.Format(@"Changed texture importer settings:
{0}", assetPath));
            }

            // TODO:
            //Selection.activeObject = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Texture2D));
		}
    }
}