using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using Spine;
using System.Drawing;
using ICSharpCode.SharpZipLib.Zip;

// Unity3D运行时Spine加载工具类 (从文件加载) 
// 因为Zip处理方式与文件处理方式差别较大，因此与SpineZipReader分开
public class SpineFileReader
{
    // NOTE
    // 1-指定一个项目名
    // 2-载入同名的.atlas文件，调用IngestSpineAtlas取出材质数据
    // 3-载入同名的.jons文件，调用IngestSpineProject取出骨骼数据
    // 4-所有的资源已经到位，调用SpawnAnimatedSkeleton构建Spine动画

    public class ImageSearchInfo
    {
        public string ImageKey;      // 搜索Key
        public bool IsSeparated;     // 是否为分割图片
        public string PngPath;       // 正常PNG或PNG-8
        public string JpgPath;       // RGB图片
    }

    private static List<ImageSearchInfo> imgSearchTable;

    public static void SetImageSearchTable(List<ImageSearchInfo> imgInfos) {
        imgSearchTable = imgInfos;
    }

    // Create SkeAnim from File
    public static SkeletonAnimation CreateSkeAnimFromFile()
    {
        AtlasAsset atlasAsset = SpineFileReader.IngestSpineAtlas("man_ok.atlas");
        SkeletonDataAsset skeletonDataAsset = SpineFileReader.IngestSpineProject("man_ok.json", atlasAsset);
        return SpineFileReader.SpawnAnimatedSkeleton(skeletonDataAsset);
    }

    public static SkeletonAnimation CreateSkeAnimFromZip(string fullZipPath)
    {
        FileInfo zipFInfo = new FileInfo(fullZipPath);
        string spineName = Path.GetFileNameWithoutExtension(fullZipPath);

        ZipFile zf = new ZipFile(fullZipPath);
        ZipEntry jsonEntry = zf.GetEntry(spineName + ".json");
        ZipEntry atlasEntry = zf.GetEntry(spineName + ".atlas");

        List<ZipEntry> pngEntries = new List<ZipEntry>();
        List<ZipEntry> jpgEntries = new List<ZipEntry>();

        // 将图片加入查找表
        foreach (ZipEntry entry in zf)
        {
            string ext = Path.GetExtension(entry.Name);
            if (ext == ".png")
                pngEntries.Add(entry);
            else if (ext == ".jpg")
                jpgEntries.Add(entry);
        }

        AtlasAsset atlasAsset = SpineFileReader.IngestSpineAtlas("man_ok.atlas");
        SkeletonDataAsset skeletonDataAsset = SpineFileReader.IngestSpineProject("man_ok.json", atlasAsset);
        return SpineFileReader.SpawnAnimatedSkeleton(skeletonDataAsset);
    }

	// 读取Atlas文件
    public static AtlasAsset IngestSpineAtlas(string atlasFilePath){

        string primaryName = Path.GetFileNameWithoutExtension(atlasFilePath);
		string assetPath = Path.GetDirectoryName(atlasFilePath);
		
		AtlasAsset atlasAsset = AtlasAsset.CreateInstance<AtlasAsset>();
		// atlasAsset.atlasFile = atlasText;
        atlasAsset.atlasFileContent = File.ReadAllText(atlasFilePath);

        string atlasTextContent = File.ReadAllText(atlasFilePath);

        string[] atlasLines = atlasTextContent.Split('\n');
		List<string> pageFiles = new List<string>();
		for(int i = 0; i < atlasLines.Length-1; i++){
			if(atlasLines[i].Length == 0)
				pageFiles.Add(atlasLines[i+1]);
		}
		
		atlasAsset.materials = new Material[pageFiles.Count];

		for(int i = 0; i < pageFiles.Count; i++)
		{
			string texturePath = assetPath + "/" + pageFiles[i];

            Material mat = new Material(Shader.Find("Spine/Skeleton"));
            // TODO 异步加载Texture
            // Texture2D texture = null;
            if (texturePath.StartsWith("/"))
                texturePath = "." + texturePath;

            Debug.Log("贴图路径 " + texturePath);
            Bitmap bitmap = new Bitmap(texturePath);

            Texture2D tex2D = new Texture2D(bitmap.Width, bitmap.Height, TextureFormat.ARGB32, false);
            tex2D.name = Path.GetFileNameWithoutExtension(texturePath);

            int pixelCnt = 0;
            for(int row = 0; row<bitmap.Height ; row++)
            {
                for(int col= 0; col<bitmap.Width; col++)
                {
                    System.Drawing.Color srcColor = bitmap.GetPixel(col, bitmap.Height - row -1);
                    tex2D.SetPixel(col, row, new UnityEngine.Color(srcColor.R, srcColor.G, srcColor.B, srcColor.A));
                    pixelCnt++;
                }
            }

            tex2D.filterMode = FilterMode.Bilinear;
            tex2D.wrapMode = TextureWrapMode.Clamp;

            tex2D.Apply();

            Debug.Log("拷贝像素 "  + pixelCnt);
            mat.mainTexture = tex2D;
            atlasAsset.materials[i] = mat;
        }
        return atlasAsset;
	}

    public static float defaultScale = 0.01f;
    //public static float defaultScale = 1f;
    public static float defaultMix = 0.2f;
    public static string defaultShader = "Spine/Skeleton";

    // 读取Json文件
    public static SkeletonDataAsset IngestSpineProject(string spineJsonPath, AtlasAsset atlasAsset = null)
    {
        string jsonContentStr = File.ReadAllText(spineJsonPath);

        string primaryName = Path.GetFileNameWithoutExtension(spineJsonPath);
        FileInfo fInfo = new FileInfo(spineJsonPath + ".json");
        string assetPath = Path.GetDirectoryName(fInfo.Directory.FullName);

        if (spineJsonPath != null && atlasAsset != null)
        {
            SkeletonDataAsset skelDataAsset = SkeletonDataAsset.CreateInstance<SkeletonDataAsset>();
            skelDataAsset.atlasAsset = atlasAsset;
            // skelDataAsset.skeletonJSON = spineJsonPath;
            skelDataAsset.jsonFileName = primaryName;
            skelDataAsset.skeletonJsonStr = jsonContentStr;
            skelDataAsset.fromAnimation = new string[0];
            skelDataAsset.toAnimation = new string[0];
            skelDataAsset.duration = new float[0];
            skelDataAsset.defaultMix = defaultMix;
            skelDataAsset.scale = defaultScale;

            return skelDataAsset;
        }
        else
        {
            Debug.LogError("json 或 atlas 为 null !");
            return null;
        }
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
