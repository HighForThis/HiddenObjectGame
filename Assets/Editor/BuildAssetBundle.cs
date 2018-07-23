using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

public class BuildAssetBundle{

	[MenuItem("build/1.create All Scene AssetBundleName")]
	static void clearAssetBundleName()
	{
		string[] files=Directory.GetFiles (Application.dataPath, "*.unity", SearchOption.AllDirectories);	
		for (int i = 0; i < files.Length; i++) 
		{
			string currentFile=files [i].Replace ("\\", "/");
			int startIndex = currentFile.IndexOf ("Assets");
			string assetFile = currentFile.Substring (startIndex,currentFile.Length-startIndex);
			string sceneName = Path.GetFileNameWithoutExtension (assetFile);
			AssetImporter ai=AssetImporter.GetAtPath (assetFile);
			ai.assetBundleName = sceneName;
			ai.assetBundleVariant = "assetbundle";
		}
	}

	[MenuItem("build/2.Build All AssetBundle")]
	static void buildAssetBundle()
	{
		string outPath=Application.streamingAssetsPath+"/AssetBundle";
		if (!Directory.Exists (outPath)) 
		{
			Directory.CreateDirectory (outPath);
		}
        BuildPipeline.BuildAssetBundles(outPath, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
	}

	[MenuItem("build/3.clear All AssetBundle")]
	static void clearAssetBundle()
	{
		string outPath=Application.streamingAssetsPath+"/AssetBundle";
		if (!Directory.Exists (outPath)) 
		{
			return;
		}

		deleteFileOrFolder (outPath);
		AssetDatabase.Refresh ();
	}

	static void deleteFileOrFolder(string fileOrFolder)
	{
		if (Directory.Exists (fileOrFolder)) 
		{
			string[] allFiles=Directory.GetFiles (fileOrFolder);
			if (allFiles != null) 
			{
				for (int i = 0; i < allFiles.Length; i++) 
				{
					deleteFileOrFolder (allFiles [i]);
				}				
			}

			string[] allFolders=Directory.GetDirectories (fileOrFolder);
			if (allFolders != null) 
			{
				for (int i = 0; i < allFolders.Length; i++) 
				{
					deleteFileOrFolder (allFolders [i]);
				}				
			}

			Directory.Delete (fileOrFolder);
		}
		else 
		{
			if (File.Exists (fileOrFolder)) 
			{
				File.Delete (fileOrFolder);
			}
		}
	}
}
