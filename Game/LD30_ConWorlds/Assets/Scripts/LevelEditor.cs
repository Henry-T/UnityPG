using UnityEngine;
using System.Collections;

public class LevelEditor : MonoBehaviour {

    public int LvlMode = 0;     // 0-Start 1-End

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SwitchLevelStart(int lvlId)
    {
        LvlMode = 0;

        LevelData data = LevelDataBase.LoadLevelData(lvlId);
        GameManager.Instance.LoadLevel(lvlId, data, false);
        GameManager.Instance.EnableWinCheck = false;
    }

    public void SwitchLevelEnd(int lvlId)
    {
        LvlMode = 1;

        LevelData data = LevelDataBase.LoadLevelData(lvlId);
        GameManager.Instance.LoadLevel(lvlId, data, true);
        GameManager.Instance.EnableWinCheck = false;
    }

    void OnGUI()
    {
//#if UNITY_EDITOR

        // Level bar
        GUILayout.Label("Level ID: " + GameManager.Instance.LevelId);
        GUILayout.Space(5);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Load_Start: ");
        GUILayout.Space(4);
        if (GUILayout.Button("1")) { SwitchLevelStart(1); }
        if (GUILayout.Button("2")) { SwitchLevelStart(2); }
        if (GUILayout.Button("3")) { SwitchLevelStart(3); }
        if (GUILayout.Button("4")) { SwitchLevelStart(4); }
        if (GUILayout.Button("5")) { SwitchLevelStart(5); }
        if (GUILayout.Button("6")) { SwitchLevelStart(6); }
        if (GUILayout.Button("7")) { SwitchLevelStart(7); }
        if (GUILayout.Button("8")) { SwitchLevelStart(8); }
        if (GUILayout.Button("9")) { SwitchLevelStart(9); }
        if (GUILayout.Button("10")) { SwitchLevelStart(10); }
        GUILayout.EndHorizontal();

        GUILayout.Space(5);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Load_End  : ");
        GUILayout.Space(4);
        if (GUILayout.Button("1")) { SwitchLevelEnd(1); }
        if (GUILayout.Button("2")) { SwitchLevelEnd(2); }
        if (GUILayout.Button("3")) { SwitchLevelEnd(3); }
        if (GUILayout.Button("4")) { SwitchLevelEnd(4); }
        if (GUILayout.Button("5")) { SwitchLevelEnd(5); }
        if (GUILayout.Button("6")) { SwitchLevelEnd(6); }
        if (GUILayout.Button("7")) { SwitchLevelEnd(7); }
        if (GUILayout.Button("8")) { SwitchLevelEnd(8); }
        if (GUILayout.Button("9")) { SwitchLevelEnd(9); }
        if (GUILayout.Button("10")) { SwitchLevelEnd(10); }
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        GUILayout.BeginArea(new Rect(0, 80, 90, Screen.height - 80));
        if(GUILayout.Button("save_start"))
        {
            LevelData data = GameManager.Instance.CurLevelData;
            data.LevelStart.Clear();

            foreach(EleComp eleComp in GameManager.Instance.EleCompList)
            {
                ElementData eleData = new ElementData();
                eleData.Catagory = eleComp.ElementInfo.Catagory;
                eleData.Type = eleComp.ElementInfo.Type;
                eleData.GridX = (int)eleComp.Grid.x;
                eleData.GridY = (int)eleComp.Grid.y;
                data.LevelStart.Add(eleData);
            }

            LevelDataBase.SaveLevelData(GameManager.Instance.LevelId, data);
        }

        if (GUILayout.Button("save_end"))
        {
            LevelData data = GameManager.Instance.CurLevelData;
            data.LevelEnd.Clear();

            foreach (EleComp eleComp in GameManager.Instance.EleCompList)
            {
                ElementData eleData = new ElementData();
                eleData.Catagory = eleComp.ElementInfo.Catagory;
                eleData.Type = eleComp.ElementInfo.Type;
                eleData.GridX = (int)eleComp.Grid.x;
                eleData.GridY = (int)eleComp.Grid.y;
                data.LevelEnd.Add(eleData);
            }

            LevelDataBase.SaveLevelData(GameManager.Instance.LevelId, data);
        }


        if (GUILayout.Button("c_green"))
        {
            ElementFactory.SpawnNewElement(2, 0, 0, 6, 10);
        }
        if (GUILayout.Button("c_red"))
        {
            ElementFactory.SpawnNewElement(3, 0, 0, 6, 10);
        }

        if(GUILayout.Button("c_gray_0"))
        {
            ElementFactory.SpawnNewElement(0, 0, 0, 6, 10);
        }
        if (GUILayout.Button("c_gray_1"))
        {
            ElementFactory.SpawnNewElement(0, 1, 0, 6, 10);
        }
        if (GUILayout.Button("c_gray_2"))
        {
            ElementFactory.SpawnNewElement(0, 2, 0, 6, 10);
        }
        if (GUILayout.Button("c_gray_3"))
        {
            ElementFactory.SpawnNewElement(0, 3, 0, 6, 10);
        }
        if (GUILayout.Button("c_gray_4"))
        {
            ElementFactory.SpawnNewElement(0, 4, 0, 6, 10);
        }
        if (GUILayout.Button("c_gray_5"))
        {
            ElementFactory.SpawnNewElement(0, 5, 0, 6, 10);
        }
        if (GUILayout.Button("c_gray_6"))
        {
            ElementFactory.SpawnNewElement(0, 6, 0, 6, 10);
        }

        if (GUILayout.Button("c_color_0"))
        {
            ElementFactory.SpawnNewElement(1, 0, 0, 6, 10);
        }
        if (GUILayout.Button("c_color_1"))
        {
            ElementFactory.SpawnNewElement(1, 1, 0, 6, 10);
        }
        if (GUILayout.Button("c_color_2"))
        {
            ElementFactory.SpawnNewElement(1, 2, 0, 6, 10);
        }
        if (GUILayout.Button("c_color_3"))
        {
            ElementFactory.SpawnNewElement(1, 3, 0, 6, 10);
        }
        if (GUILayout.Button("c_color_4"))
        {
            ElementFactory.SpawnNewElement(1, 4, 0, 6, 10);
        }
        if (GUILayout.Button("c_color_5"))
        {
            ElementFactory.SpawnNewElement(1, 5, 0, 6, 10);
        }
        if (GUILayout.Button("c_color_6"))
        {
            ElementFactory.SpawnNewElement(1, 6, 0, 6, 10);
        }
        GUILayout.EndArea();

        if(GUI.Button(new Rect(Screen.width-80, 0, 80, 20), "delete"))
        {
            if(GameManager.Instance.LastSelEleComp)
            {
                GameManager.Instance.EleCompList.Remove(GameManager.Instance.LastSelEleComp);
                GameObject.Destroy(GameManager.Instance.LastSelEleComp.gameObject);
                GameManager.Instance.LastSelEleComp = null;
            }
        }
//#endif
    }
}
