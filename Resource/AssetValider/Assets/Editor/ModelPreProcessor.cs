using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AnimEntry
{
	public string Name;
	public int StartFrame;
	public int EndFrame;
}

public class ModelPreProcesser : AssetPostprocessor
{
	public string GetShaderMap(string materialName)
	{
        return "Transparent/Cutout/Soft Edge Unlit";

        if (materialName == "Unlit")
            return "Unlit/Texture";
        else if(materialName == "Unlit_Transparent")
            return "Unlit/Transparent";
        else if (materialName == "Diffuse")
            return "Diffuse";
        else if (materialName == "Transparent")
            return "Transparent/Diffuse";
        else if (materialName == "Material #42")    // TEST
            return "Diffuse";
        else if (materialName.StartsWith("toutie"))
            return "Transparent/Cutout/Diffuse";
        else if (materialName == "wutoutie")
            return "Diffuse";
        return "";
    }

	void OnPreprocessModel()
	{
		ModelImporter modelImporter = assetImporter as ModelImporter;

        modelImporter.materialName = ModelImporterMaterialName.BasedOnMaterialName;
        modelImporter.globalScale = 1;
		
		FileInfo fileInfo = new FileInfo (assetPath);
		if (fileInfo.Extension.ToLower() != ".fbx")
		{
			return;
		}
		
		string animConfigPath = Path.Combine (fileInfo.Directory.FullName, 
		                                      fileInfo.Name.Substring(0, fileInfo.Name.Length-4) + ".txt");
		
		if (File.Exists(animConfigPath))
		{
			StreamReader fileReader = new StreamReader(animConfigPath);
			List<string> fileLines = new List<string>();
			string line;
			int count = 0;
			while((line=fileReader.ReadLine()) != null)
			{
				fileLines.Add(line);
				count ++;
				if(count > 10)
					break;
			}
			
			List<AnimEntry> animEntryList = new List<AnimEntry>();
			foreach(string l in fileLines)
			{
				string[] contentAry = l.Split(new char[]{':', '-'});
				if(contentAry.Length == 3)
				{
					AnimEntry animEntry = new AnimEntry();
					animEntry.Name = contentAry[0];
					if(int.TryParse(contentAry[1], out animEntry.StartFrame) &&
					   int.TryParse(contentAry[2], out animEntry.EndFrame))
					{
						animEntryList.Add(animEntry);
					}
				}
			}
			
			if(animEntryList.Count > 0)
			{
				modelImporter.animationType = ModelImporterAnimationType.Legacy;
				modelImporter.importAnimation = true;

				ModelImporterClipAnimation[] clips = new ModelImporterClipAnimation[animEntryList.Count];
				
				int index = 0;
				foreach(AnimEntry animEntry in animEntryList)
				{
					ModelImporterClipAnimation clipAnim = new ModelImporterClipAnimation();
                    if(animEntry.Name == "run" || animEntry.Name == "ready" || animEntry.Name == "idle" || animEntry.Name == "stand" || animEntry.Name == "airStay")
                        clipAnim.wrapMode = WrapMode.Loop;

					clipAnim.name = animEntry.Name;
					clipAnim.firstFrame = animEntry.StartFrame;
					clipAnim.lastFrame = animEntry.EndFrame;

					clips[index] = clipAnim;
					index ++;
				}
				modelImporter.clipAnimations = clips;
			}
		}
	}
    
    void OnPostprocessModel(GameObject gameObject)
    {
        Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
        
        foreach (Renderer renderer in renderers)
        {
            Material sharedMaterial = renderer.sharedMaterial;
            string shaderName = GetShaderMap(sharedMaterial.name);
            if(!string.IsNullOrEmpty(shaderName))
                renderer.sharedMaterial.shader = Shader.Find(shaderName);
        }
    }
}

