using UnityEngine;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

public class LevelDataBase
{
    public static LevelData LoadLevelData(int lvlId)
    {
#if UNITY_WEBPLAYER
        TextAsset asset = Resources.Load<TextAsset>("Levels/Level_" + lvlId.ToString("D2"));

        XmlSerializer xmlSerializer = new XmlSerializer(typeof(LevelData));
        StringReader reader = new StringReader(asset.text);
        return (LevelData)xmlSerializer.Deserialize(reader);
        reader.Close();
#else
        FileInfo fInfo = new FileInfo("Levels/Level_" + lvlId.ToString("D2") + ".xml");
        if (fInfo.Exists)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(LevelData));
            StreamReader reader = File.OpenText(fInfo.FullName);
            return (LevelData)xmlSerializer.Deserialize(reader);
            reader.Close();
        }
        else
        {
            return new LevelData();
        }
#endif
    }

    public static void SaveLevelData(int lvlId, LevelData data)
    {
        FileInfo fInfo = new FileInfo("Levels/Level_" + lvlId.ToString("D2") + ".xml");
        if (fInfo.Exists)
            File.Delete(fInfo.FullName);

        StreamWriter writer = fInfo.CreateText();
        XmlSerializer xmlSerializer = new XmlSerializer(typeof(LevelData));
        xmlSerializer.Serialize(writer, data);
        writer.Close();
    }
}

public class LevelData
{
    public List<ElementData> LevelStart = new List<ElementData>();
    public List<ElementData> LevelEnd = new List<ElementData>();
}

public class ElementData
{
    public int Catagory;
    public int Type;
    public int GridX;
    public int GridY;
}