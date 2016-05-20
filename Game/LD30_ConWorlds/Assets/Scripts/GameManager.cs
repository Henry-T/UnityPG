using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class GameManager : MonoBehaviour {

	public static GameManager Instance;

    public static float GridDesignSize = 32;
    public static float GridWorldSize = GridDesignSize/100f;

    public static float PosStartX = -11.5f * GridWorldSize;
    public static float PosStartY =  -6.5f * GridWorldSize;

    public static float PosEndX = 11.5f * GridWorldSize;
    public static float PosEndY =  9.5f * GridWorldSize;


    public static float MinPosX = PosStartX - 0.5f * GridWorldSize;
    public static float MaxPosX = PosEndX + 0.5f * GridWorldSize;
    public static float MinPosY = PosStartY - 0.5f * GridWorldSize;
    public static float MaxPosY = PosEndY + 0.5f * GridWorldSize;

    public static int GridRowCount = 17;
    public static int GridColCount = 24;

    public EleComp SelEleComp;
    public Vector3 SelEleCompBeginMovePos;
    public Vector3 MouseDragBeginPos;
    public EleComp LastSelEleComp;

    public EleComp EleCompGreen;
    public EleComp EleCompRed;

    public GridOccupier[,] GridStats = new GridOccupier[GridRowCount, GridColCount];

    public List<EleComp> EleCompList = new List<EleComp>();

    public GameObject PanelMain;
    public GameObject PanelLevel;
    public GameObject PanelGame;
    public GameObject PanelWin;

    public List<GameObject> Guides = new List<GameObject>();
    public GameObject GuideGreen;
    public GameObject GuideRed;

    public bool[,] CheckWinGrids = new bool[GridRowCount, GridColCount];
    public Vector2 CheckGreenGrid;
    public Vector2 CheckRedGrid;

    // todo add level win situations here!

    // todo if scene is dirty and all elements stable, do a win check

	void Start () {
		Instance = this;

        PanelMain = GameObject.Find("PanelMain");
        PanelLevel = GameObject.Find("PanelLevel");
        PanelGame = GameObject.Find("PanelGame");
        PanelWin = GameObject.Find("PanelWin");

        PanelMain.SetActive(true);
        PanelLevel.SetActive(false);
        PanelGame.SetActive(false);
        PanelWin.SetActive(false);

        //ElementFactory.SpawnNewElement(0, 0, 0, 4, 2);
        //ElementFactory.SpawnNewElement(0, 1, 0, 4, 2).name = "L";
        //ElementFactory.SpawnNewElement(0, 2, 0, 4, 2);
        //ElementFactory.SpawnNewElement(0, 3, 0, 4, 2);
        //ElementFactory.SpawnNewElement(0, 4, 0, 4, 2);
        //ElementFactory.SpawnNewElement(0, 5, 0, 4, 2);
        //ElementFactory.SpawnNewElement(0, 6, 0, 4, 2);

        //ElementFactory.SpawnNewElement(1, 0, 0, 8, 2);
        //ElementFactory.SpawnNewElement(1, 1, 0, 8, 2);
        //ElementFactory.SpawnNewElement(1, 2, 0, 8, 2);
        //ElementFactory.SpawnNewElement(1, 3, 0, 8, 2);
        //ElementFactory.SpawnNewElement(1, 4, 0, 8, 2);
        //ElementFactory.SpawnNewElement(1, 5, 0, 8, 2);
        //ElementFactory.SpawnNewElement(1, 6, 0, 8, 2);

        ElementFactory.Initialize();

        for (int i = 0; i < GridRowCount; i++)
        {
            for(int j=0; j < GridColCount; j++)
            {
                GridStats[i, j] = new GridOccupier();
            }
        }

        //for (int i = 0; i < 100; i++)
        //    DoRandomSpawn();

        //GameState = EGameState.Gaming;
        //PanelMain.SetActive(false);
        //PlayLevel(4);
    }

    public void DoRandomSpawn()
    {
        ElementFactory.SpawnNewElement(Random.Range(0,2), Random.Range(0,7), 0, Random.Range(2, 22), Random.Range(1,16));
    }

    public static int GetGridWorldIndex(int gridX)
    {
        if (gridX >= 0 && gridX < 12)
            return 0;
        else
            return 1; 
    }

    public bool CheckGridInBoard(int gridX, int gridY)
    {
        if (gridX >= 0 && gridX < GridColCount && gridY >= 0 && gridY < GridRowCount)
            return true;
        else
            return false;
    }

    public static Vector3 GridToPos(Vector2 grid)
    {
        return new Vector3(GameManager.PosStartX + GameManager.GridWorldSize * grid.x, GameManager.PosStartY + GameManager.GridWorldSize * grid.y, 0);
    }

    public static Vector2 PosToGrid(Vector3 pos)
    {
        return new Vector2(Mathf.Round((pos.x - GameManager.PosStartX) * 100 / GameManager.GridDesignSize), Mathf.Round((pos.y - GameManager.PosStartY) * 100 / GameManager.GridDesignSize));
    }

	void Update () {
        if (GameState == EGameState.Gaming)
        {
            if(!SelEleComp)
                CheckWin();

            if (Input.GetMouseButtonDown(0) && !SelEleComp)
            {
                int layerMask = LayerMask.GetMask(new string[] { "Sensor", "ThingL", "ThingR" });
                Collider2D[] colliders = Physics2D.OverlapPointAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), layerMask);
                if (colliders.Length > 0)
                {
                    SelEleComp = colliders[0].transform.parent.gameObject.GetComponent<EleComp>();
                    LastSelEleComp = SelEleComp;
                    SelEleCompBeginMovePos = SelEleComp.transform.position;
                    MouseDragBeginPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                }
            }

            if (Input.GetMouseButtonUp(0))     // todo change back to !button after debugging
            {
                if (SelEleComp)
                {
                    // snap to grid
                    SelEleComp.SetGridPos(SelEleComp.GetNearestGridPos());

                    bool posConflict = false;

                    // you can insert element into others, so fix the y again
                    foreach (Vector2 relaPos in SelEleComp.ElementInfo.OccupyGridsL)
                    {
                        int checkX = (int)(SelEleComp.Grid.x + relaPos.x);
                        int checkY = (int)(SelEleComp.Grid.y + relaPos.y);
                        if (GetGridWorldIndex(checkX) == 0)
                        {
                            if (GridStats[checkY, checkX].State == EGridState.Occupied)
                            {
                                posConflict = true;
                                break;
                            }
                        }
                    }

                    foreach (Vector2 relaPos in SelEleComp.ElementInfo.OccupyGridsR)
                    {
                        int checkX = (int)(SelEleComp.Grid.x + relaPos.x);
                        int checkY = (int)(SelEleComp.Grid.y + relaPos.y);
                        if (GetGridWorldIndex(checkX) == 1)
                        {
                            if (GridStats[checkY, checkX].State == EGridState.Occupied)
                            {
                                posConflict = true;
                                break;
                            }
                        }
                    }

                    if (posConflict)
                    {
                        Vector3 realPos = SelEleComp.transform.position;
                        // note hack
                        Vector3 raisePos = new Vector3(realPos.x, GridWorldSize * 8, 0);
                        Vector2 stableGrid = SelEleComp.FindStableGrid(raisePos);
                        SelEleComp.SetGridPos(stableGrid);
                        SelEleComp.State = EEleCompState.Stable;
                    }
                    else
                    {
                        SelEleComp.State = EEleCompState.Falling;
                    }

                    SelEleComp = null;
                }
            }

            if (Input.GetMouseButton(0) && SelEleComp)
            {
                Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3 newPos = new Vector3(SelEleCompBeginMovePos.x + mouseWorldPos.x - MouseDragBeginPos.x, SelEleCompBeginMovePos.y + mouseWorldPos.y - MouseDragBeginPos.y, 0);
                // todo fixPos
                Vector3 fixedNewPos = SelEleComp.GetFixedLimitPos(newPos);

                SelEleComp.transform.position = fixedNewPos;
            }

            // todo check and update grid board states (accoirding to all EleComp Status)

            // cleanup
            for (int i = 0; i < GridRowCount; i++)
            {
                for (int j = 0; j < GridColCount; j++)
                {
                    GridStats[i, j].EleComp = null;
                    GridStats[i, j].PartIndex = 0;
                }
            }

            foreach (EleComp eleComp in EleCompList)
            {
                if (eleComp == SelEleComp || eleComp.State != EEleCompState.Stable)
                    continue;

                // left world
                for (int i = 0; i < eleComp.ElementInfo.OccupyGridsL.Length; i++)
                {
                    Vector2 relaGridPos = eleComp.ElementInfo.OccupyGridsL[i];
                    int gridX = (int)(eleComp.Grid.x + relaGridPos.x);
                    int gridY = (int)(eleComp.Grid.y + relaGridPos.y);

                    if (GetGridWorldIndex(gridX) == 0 && CheckGridInBoard(gridX, gridY))
                    {
                        GridStats[gridY, gridX].EleComp = eleComp;
                        GridStats[gridY, gridX].PartIndex = i;
                    }
                }

                // right world
                for (int i = 0; i < eleComp.ElementInfo.OccupyGridsR.Length; i++)
                {
                    Vector2 relaGridPos = eleComp.ElementInfo.OccupyGridsR[i];
                    int gridX = (int)(eleComp.Grid.x + relaGridPos.x);
                    int gridY = (int)(eleComp.Grid.y + relaGridPos.y);

                    if (GetGridWorldIndex(gridX) == 1 && CheckGridInBoard(gridX, gridY))
                    {
                        GridStats[gridY, gridX].EleComp = eleComp;
                        GridStats[gridY, gridX].PartIndex = i;
                    }
                }
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0) && !SelEleComp)
            {
                int layerMask = LayerMask.GetMask(new string[] { "GUIButton"});
                Collider2D[] colliders = Physics2D.OverlapPointAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), layerMask);
                if (colliders.Length > 0)
                {
                    GUIButton button = colliders[0].gameObject.GetComponent<GUIButton>();
                    if(button)
                    {
                        HandleButtonEvent(button.FuncName);
                    }
                }
            }
        }
	}

    void OnDrawGizmos()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Gizmos.DrawRay(ray);
    }

    public void ClearAll()
    {
        foreach (EleComp eleComp in GameManager.Instance.EleCompList)
        {
            Destroy(eleComp.gameObject);
        }
        EleCompList.Clear();

        foreach (GameObject guide in Guides)
        {
            Destroy(guide);
        }
        Guides.Clear();

        if (GuideGreen)
        {
            Destroy(GuideGreen);
            GuideGreen = null;
        }
        
        if (GuideRed)
        {
            Destroy(GuideRed);
            GuideRed = null;
        }
    }

    public int LevelId;
    public LevelData CurLevelData;
    public bool EnableWinCheck = false;

    public void LoadLevel(int levelId, LevelData levelData, bool loadAsEnd)
    {
        ClearAll();

        LevelId = levelId;
        CurLevelData = levelData;

        if(!loadAsEnd)
        {
            foreach(ElementData data in CurLevelData.LevelStart)
            {
                ElementFactory.SpawnNewElement(data.Catagory, data.Type, 0, data.GridX, data.GridY);
            }
        }
        else
        {
            foreach (ElementData data in CurLevelData.LevelEnd)
            {
                ElementFactory.SpawnNewElement(data.Catagory, data.Type, 0, data.GridX, data.GridY);
            }
        }
    } 

    public void PlayLevel(int levelId)
    {
        LevelData data = LevelDataBase.LoadLevelData(levelId);
        LoadLevel(levelId, data, false);
        EnableWinCheck = true;

        CheckWinGrids = new bool[GridRowCount, GridColCount];

        foreach(ElementData eleData in data.LevelEnd)
        {
            if(eleData.Catagory == 2)
            {
                CheckGreenGrid = new Vector2(eleData.GridX, eleData.GridY);
                continue;
            }

            if (eleData.Catagory == 3)
            {
                CheckRedGrid = new Vector2(eleData.GridX, eleData.GridY);
                continue;
            }

            ElementInfo eleInfo = ElementFactory.ElementInfos.First(ei => ei.Catagory == eleData.Catagory && ei.Type == eleData.Type);
            int eleGridX = eleData.GridX;
            int eleGridY = eleData.GridY;

            foreach(Vector2 pos in eleInfo.OccupyGridsL)
            {
                int eleSegX = (int)(eleGridX + pos.x);
                int eleSegY = (int)(eleGridY + pos.y);
                if(GetGridWorldIndex(eleSegX) == 0)
                {
                    CheckWinGrids[eleSegY, eleSegX] = true;
                }
            }

            foreach (Vector2 pos in eleInfo.OccupyGridsR)
            {
                int eleSegX = (int)(eleGridX + pos.x);
                int eleSegY = (int)(eleGridY + pos.y);
                if (GetGridWorldIndex(eleSegX) == 1)
                {
                    CheckWinGrids[eleSegY, eleSegX] = true;
                }
            }
        }

        // add guides
        for(int i=0; i<GridRowCount; i++)
        {
            for(int j=0; j<GridColCount; j++)
            {
                if(CheckWinGrids[i, j])
                {
                    GameObject guide = (GameObject)Instantiate(Resources.Load<GameObject>("guideElement"));
                    Guides.Add(guide);
                    guide.transform.position = new Vector3(PosStartX + GridWorldSize * j, PosStartY + GridWorldSize * i, -0.05f);
                }
            }
        }

        GuideGreen = (GameObject)Instantiate(Resources.Load<GameObject>("guideGreen"));
        GuideGreen.transform.position = new Vector3(PosStartX + GridWorldSize * (CheckGreenGrid.x + 0.5f), PosStartY + GridWorldSize * (CheckGreenGrid.y - 0.5f), -0.05f);

        GuideRed = (GameObject)Instantiate(Resources.Load<GameObject>("guideRed"));
        GuideRed.transform.position = new Vector3(PosStartX + GridWorldSize * (CheckRedGrid.x + 0.5f), PosStartY + GridWorldSize * (CheckRedGrid.y - 0.5f), -0.05f);


        // show intro
        Transform introTrans = PanelGame.transform.Find("Intro");
        if (introTrans)
            Destroy(introTrans.gameObject);

        GameObject intro = new GameObject("Intro");
        intro.AddComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Intros/intro_" + levelId.ToString("D2"));
        intro.transform.parent = PanelGame.transform;
    }

    public void CheckWin()
    {
        // check if all requested hole filled!

        if (!EleCompGreen.Grid.Equals(CheckGreenGrid))
            return;

        if (!EleCompRed.Grid.Equals(CheckRedGrid))
            return;

        bool notWin = false;
        for (int i = 0; i < GridRowCount; i++ )
        {
            for (int j = 0; j < GridColCount; j++)
            {
                if (CheckWinGrids[i, j] && GridStats[i, j].State != EGridState.Occupied)
                {
                    notWin = true;
                    break;
                }
            }
        }

        if(!notWin)
        {
            GameState = EGameState.Win;
            PanelGame.SetActive(false);
            PanelWin.SetActive(true);
        }
    }

    public EGameState GameState = EGameState.MainMenu;
    public void HandleButtonEvent(string funcName)
    {
        if(funcName == "startgame")
        {
            GameState = EGameState.Gaming;
            PlayLevel(1);
            PanelMain.SetActive(false);
            PanelGame.SetActive(true);
        }
        
        if(funcName == "showlevel")
        {
            GameState = EGameState.Level;
            PanelMain.SetActive(false);
            PanelLevel.SetActive(true);
        }

        if(funcName.StartsWith("beginlevel"))
        {
            string numStr = funcName.Replace("beginlevel", "");
            int lvl = int.Parse(numStr);
            GameState = EGameState.Gaming;
            PlayLevel(lvl);
            PanelLevel.SetActive(false);
            PanelGame.SetActive(true);
        }

        if(funcName == "nextlevel")
        {
            GameState = EGameState.Gaming;
            PanelGame.SetActive(true);
            PanelWin.SetActive(false);
            PlayLevel(LevelId + 1);
        }

        if(funcName == "back")
        {
            GameState = EGameState.MainMenu;
            PanelWin.SetActive(false);
            PanelMain.SetActive(true);
            ClearAll();
        }
    }
}

public class GridOccupier
{
    public EleComp EleComp;
    public int PartIndex;       // elementary block index

    // stat of this slot
    public EGridState State {
        get{
            if (EleComp == null)
                return EGridState.Empty;
            else if (EleComp.State == EEleCompState.Falling)
                return EGridState.Passing;
            else if (EleComp.State == EEleCompState.Stable)
                return EGridState.Occupied;
            else
            {
                Debug.LogError("state_impossible! for a GridOccupier");
                return EGridState.Empty;
            }
        }
    }
}


public enum EGridState
{
    Empty,
    Passing,    
    Occupied
}


public enum EGameState
{
    MainMenu,
    Level,
    Gaming,
    Win,
}