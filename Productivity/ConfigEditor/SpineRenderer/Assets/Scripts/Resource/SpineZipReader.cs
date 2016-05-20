using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using Spine;
using System.Drawing;
using ICSharpCode.SharpZipLib.Zip;
using System.Text;
using ICSharpCode.SharpZipLib.Core;
using System.Drawing.Imaging;

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
                Debug.LogError(String.Format("找不到引用的png: {0} {1}", textureKey, fullPngPath));
                return null;
            }

            string fullJpgPath = Path.Combine(zipDirInfo.FullName, textureKey.Replace(".png", ".jpg"));

            bool isSeperated = false;
            if(File.Exists(fullJpgPath))
            {
                isSeperated = true;
            }

            Texture2D tex2D = null;

            //int alphaCnt = 0;
            //int rCnt = 0;
            //int gCnt = 0;
            //int bCnt = 0;
            // WWW www = new WWW();

            if (isSeperated)
            {
                Bitmap colorImg = new Bitmap(fullJpgPath);
                Bitmap alphaImg = new Bitmap(fullPngPath);

                //int imageWidth = colorImg.Width;
                //int imageHeight = colorImg.Height;

                //Bitmap combineImg = new Bitmap(imageWidth, imageHeight, colorImg.PixelFormat);

                //Rectangle rect = new Rectangle(0, 0, imageWidth, imageHeight);

                //BitmapData colorData = colorImg.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                //BitmapData alphaData = alphaImg.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                //BitmapData combineData = combineImg.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

                //Debug.Log(colorData.Stride + " / " + imageWidth);
                //int byteCnt = Math.Abs(colorData.Stride) * imageHeight;
                //byte[] colorBytes = new byte[byteCnt];
                //byte[] alphaBytes = new byte[byteCnt];

                //System.Runtime.InteropServices.Marshal.Copy(colorData.Scan0, colorBytes, 0, byteCnt);
                //System.Runtime.InteropServices.Marshal.Copy(alphaData.Scan0, alphaBytes, 0, byteCnt);

                //for (int counter = 0; counter < colorBytes.Length; counter += 4)
                //{
                //    colorBytes[counter+3] = 0;
                //    if (counter < 300)
                //        Debug.Log(alphaBytes[counter]);
                //}

                //System.Runtime.InteropServices.Marshal.Copy(colorBytes, 0, combineData.Scan0, byteCnt);

                //colorImg.UnlockBits(colorData);
                //alphaImg.UnlockBits(alphaData);
                //combineImg.UnlockBits(combineData);

                tex2D = new Texture2D(colorImg.Width, colorImg.Height, TextureFormat.ARGB32, false);

                System.Drawing.Color srcColor;
                System.Drawing.Color alpColor;
                int alpha;
                UnityEngine.Color finalColor;

                // 最大瓶颈在这个 for 循环
                for (int row = 0; row < colorImg.Height; row++)
                {
                    for (int col = 0; col < colorImg.Width; col++)
                    {
                        srcColor = colorImg.GetPixel(col, colorImg.Height - row - 1);
                        alpColor = alphaImg.GetPixel(col, colorImg.Height - row - 1);
                        alpha = alpColor.R;

                        // System.Drawing.Color color = combineImg.GetPixel(col, colorImg.Height - row - 1);                        

                        // NOTE Bitmap是0-255，Texture是0-1
                        finalColor.r = srcColor.R/255f;
                        finalColor.g = srcColor.G/255f;
                        finalColor.b = srcColor.B/255f;
                        finalColor.a = alpha/255f;
                        tex2D.SetPixel(col, row, finalColor);

                        // tex2D.SetPixel(col, row, new UnityEngine.Color(color.R/255f, color.G/255f, color.B/255f, color.A/255f));
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
