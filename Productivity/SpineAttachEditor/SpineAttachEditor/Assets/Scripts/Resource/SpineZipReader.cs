using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using Spine;
using System.Drawing;
using ICSharpCode.SharpZipLib.Zip;
using System.Text;
using ICSharpCode.SharpZipLib.Core;

// Unity3D运行时Spine加载工具类 (从Zip包加载)
public class SpineZipReader
{
    // NOTE
    // 1-指定一个Spine项目名
    // 2-载入同名的.atlas文件，取出材质数据
    // 3-载入同名的.jons文件，取出骨骼数据
    // 4-所有的资源已经到位，调用SpawnAnimatedSkeleton构建Spine动画

    public static float defaultScale = 0.01f;           // Spine没单位像素对应于Unity中的1m，需要缩小显示
    public static float defaultMix = 0.2f;
    public static string defaultShader = "Spine/Skeleton";

    public static SkeletonAnimation CreateSkeAnimFromZip(string fullZipPath)
    {
        // ======================
        // 解析zip
        // ======================
        FileInfo zipFInfo = new FileInfo(fullZipPath);
        DirectoryInfo zipDirInfo = zipFInfo.Directory;
        string spineName = Path.GetFileNameWithoutExtension(fullZipPath);

        ZipFile zf = new ZipFile(fullZipPath);
        ZipEntry jsonEntry = zf.GetEntry(spineName + ".json");
        ZipEntry atlasEntry = zf.GetEntry(spineName + ".atlas");

        Stream jsonStream = zf.GetInputStream(jsonEntry);
        Stream atlasStream = zf.GetInputStream(atlasEntry);

        String jsonContent = "";
        string atlasContent = "";

        using (StreamReader reader=  new StreamReader(jsonStream))
        {
            jsonContent = reader.ReadToEnd();
        }

        using (StreamReader reader = new StreamReader(atlasStream))
        {
            atlasContent = reader.ReadToEnd();
        }

        // ===================
        // Atlas 文件解析
        // ===================
        AtlasAsset atlasAsset = AtlasAsset.CreateInstance<AtlasAsset>();
        atlasAsset.atlasFileContent = atlasContent;

        string[] atlasLines = atlasContent.Split('\n');
        List<string> pageFiles = new List<string>();
        for (int i = 0; i < atlasLines.Length - 1; i++)
        {
            if (atlasLines[i].Length == 0)
                pageFiles.Add(atlasLines[i + 1]);
        }

        atlasAsset.materials = new Material[pageFiles.Count];

        for (int i = 0; i < pageFiles.Count; i++)
        {
            string textureKey = pageFiles[i];
            string nameOnly = Path.GetFileNameWithoutExtension(textureKey);
            Debug.Log("贴图Key " + textureKey + "   " + nameOnly);
            // 正确的textureKey应该是一个裸文件路径
            //if (textureKey.StartsWith("/"))
            //    textureKey = "." + textureKey;

            Material mat = new Material(Shader.Find("Spine/Skeleton"));
            mat.name = nameOnly;

            string fullPngPath = Path.Combine(zipDirInfo.FullName, textureKey.Replace(".png", ".jpg.mask"));

            if(!File.Exists(fullPngPath))
            {
                Debug.LogError("找不到引用的png: " + textureKey);
                return null;
            }

            string fullJpgPath = Path.Combine(zipDirInfo.FullName, textureKey.Replace(".png", ".jpg"));

            bool isSeperated = false;
            if(File.Exists(fullJpgPath))
            {
                isSeperated = true;
            }

            Texture2D tex2D = null;
            if (isSeperated)
            {
                Bitmap colorImg = new Bitmap(fullJpgPath);
                Bitmap alphaImg = new Bitmap(fullPngPath);

                tex2D = new Texture2D(colorImg.Width, colorImg.Height, TextureFormat.ARGB32, false);

                for (int row = 0; row < colorImg.Height; row++)
                {
                    for (int col = 0; col < colorImg.Width; col++)
                    {
                        System.Drawing.Color srcColor = colorImg.GetPixel(col, colorImg.Height - row - 1);
                        float alpha = alphaImg.GetPixel(col, row).A;
                        tex2D.SetPixel(col, row, new UnityEngine.Color(srcColor.R, srcColor.G, srcColor.B, alpha));
                    }
                }
            }
            else
            {
                Bitmap bitmap = new Bitmap(fullPngPath);
                MemoryStream bmpStream = new MemoryStream();
                bitmap.Save(bmpStream, System.Drawing.Imaging.ImageFormat.Png);
                bmpStream.Seek(0, SeekOrigin.Begin);

                tex2D = new Texture2D(bitmap.Width, bitmap.Height, TextureFormat.ARGB32, false);
                tex2D.LoadImage(bmpStream.ToArray());
            }

            tex2D.filterMode = FilterMode.Bilinear;
            tex2D.wrapMode = TextureWrapMode.Clamp;
            tex2D.Apply();

            // 调试模式 - 将atlas图片存为文件
            if (ToolManager.Instance.DebugMode)
            {
                File.WriteAllBytes(textureKey + ".png", tex2D.EncodeToPNG());
            }

            mat.mainTexture = tex2D;
            mat.mainTexture.name = nameOnly;
            atlasAsset.materials[i] = mat;
        }

        // ===================
        // Json 文件解析
        // ===================
        SkeletonDataAsset skelDataAsset = SkeletonDataAsset.CreateInstance<SkeletonDataAsset>();
        skelDataAsset.atlasAsset = atlasAsset;
        skelDataAsset.jsonFileName = spineName;
        skelDataAsset.skeletonJsonStr = jsonContent;
        skelDataAsset.fromAnimation = new string[0];
        skelDataAsset.toAnimation = new string[0];
        skelDataAsset.duration = new float[0];
        skelDataAsset.defaultMix = defaultMix;
        skelDataAsset.scale = defaultScale;

        // ===================
        // 实例化Spine对象
        // ===================
        return SpineFileReader.SpawnAnimatedSkeleton(skelDataAsset);
    }

    public static SkeletonAnimation SpawnAnimatedSkeleton(SkeletonDataAsset skeletonDataAsset, Skin skin = null)
    {
        GameObject go = new GameObject(skeletonDataAsset.name.Replace("_SkeletonData", ""), typeof(MeshFilter), typeof(MeshRenderer), typeof(SkeletonAnimation));
        SkeletonAnimation anim = go.GetComponent<SkeletonAnimation>();
        anim.skeletonDataAsset = skeletonDataAsset;

        bool requiresNormals = false;

        foreach (Material m in anim.skeletonDataAsset.atlasAsset.materials)
        {
            if (m.shader.name.Contains("Lit"))
            {
                requiresNormals = true;
                break;
            }
        }

        anim.calculateNormals = requiresNormals;

        if (skin == null)
            skin = skeletonDataAsset.GetSkeletonData(true).DefaultSkin;

        anim.Reset();

        anim.skeleton.SetSkin(skin);
        anim.initialSkinName = skin.Name;

        anim.skeleton.Update(1);
        anim.state.Update(1);
        anim.state.Apply(anim.skeleton);
        anim.skeleton.UpdateWorldTransform();

        return anim;
    }
}
