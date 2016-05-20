//
//  LoadBundlesScene.cs
//
//  Created by Niklas Borglund
//  Copyright (c) 2012 Cry Wolf Studios. All rights reserved.
//  crywolfstudios.net
//
// This is an example script on how you could load assets from a bundle
// created with the Bundle Creator. Be sure to build the asset bundles before
// you use this script.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class LoadBundlesScene : MonoBehaviour 
{
    //Lists of all the scripts that is downloading from asset bundle
    private List<LoadAssetFromBundle> assetsToLoad = new List<LoadAssetFromBundle>();
    
    //Since I might want to asset from the same bundle, I'll queue the downloads
    //i.e not download simultaneously so that i don't download the same bundle twice.
    private bool isDownloaded = true;

    //The url to the AssetBundles folder 
    private string baseURL;
    private string filePrefix = "file://";

    void Start()
    {
        //be sure to use the same location you specified as Export Location in the Bundle Creator
        // or if this is in the web player, direct the path to your online folder where the bundles are uploaded
        baseURL = Application.dataPath + "/../AssetBundles/";

        //Load the bundles
        if (File.Exists(baseURL + "logobundle_01.unity3d")) //Check if the asset bundle is built
        {
            //Load the two logo files stored in the bundle
            LoadAssetFromBundle cryWolfLogo = this.gameObject.AddComponent<LoadAssetFromBundle>();
            cryWolfLogo.QueueBundleDownload("pre_cryWolfLogo", "logobundle_01.unity3d", 1);
            cryWolfLogo.baseURL = filePrefix + baseURL;

            LoadAssetFromBundle cryWolfLogoURL = this.gameObject.AddComponent<LoadAssetFromBundle>();
            cryWolfLogoURL.QueueBundleDownload("pre_cryWolfLogo_url", "logobundle_01.unity3d", 1);
            cryWolfLogoURL.baseURL = filePrefix + baseURL;

            //Add them to the download list
            assetsToLoad.Add(cryWolfLogo);
            assetsToLoad.Add(cryWolfLogoURL);
        }
        else
        {
            //The file does not exist, you need to build the bundle first
            Debug.LogError("Bundles are not built! Open the Bundle Creator in Assets->BundleCreator>Asset Bundle Creator to build your bundles.");
        }
    }
	

	void Update () 
    {
        if (assetsToLoad.Count > 0)
        {			
			for(int i = (assetsToLoad.Count -1); i >= 0; i--)
			{
				LoadAssetFromBundle asset = assetsToLoad[i];
				if(asset.IsDownloadDone)
				{
					//The download is done, instantiate the asset from the bundle
					asset.InstantiateAsset();
					//Remove the asset from the loading list
					assetsToLoad.RemoveAt(i);
					//Destroy the LoadAssetFromBundle Script
                	Destroy(asset);
					//This means an asset is downloaded, which means you can start on the next one
					isDownloaded = true;
				}
			}

            if (isDownloaded) //The download is complete
            {
                //Start the next download
                foreach (LoadAssetFromBundle asset in assetsToLoad)
                {
                    if (!asset.HasDownloadStarted)
                    {
                        //Start the download
                        asset.DownloadAsset();
						
						//set the isDownloaded to false again
						isDownloaded = false;
						
                        //break the loop
                        break;
                    }
                }
            }
        }
		else //If there is nothing left to load, then destroy this game object
		{
			Destroy(this.gameObject);
		}
	}
}
