using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour {
    public static GameManager Instance;

    public static int GridSize = 3;
    
    public static float CellOffset = 1.52f;

    public EGameState GameState = EGameState.Menu;

    public bool PlayerTurn = false;

    public int[,] GridData;

    public int LastPlayerX;
    public int LastPlayerY;

    public GameObject PlayerChessPrefab;
    public GameObject OtherChessPrefab;
    public DragChess DragChess = null;

    public List<GridCellComp> CellCompList = new List<GridCellComp>();
    public List<GameObject> StarList = new List<GameObject>();

    private float timer = -1;
    private float timerT = 1;
    private Action timerCallback = null;

    public GameObject Menu;
    public GameObject Wait2;
    public GameObject End;

	void Start () {
        Instance = this;

        Wait2.SetActive(false);
        End.SetActive(false);
	}

    public void Wait(float t, Action callback)
    {
        timer = 0;
        timerT = t;
        timerCallback = callback;
    }

    void Update()
    {
        if (timer != -1)
        {
            timer += Time.deltaTime;
            if (timer > timerT)
            {
                timer = -1;
                timerCallback();
            }
        }
    }


    public void Start5()
    {
        RestartLevel(5);
    }

    public void Start4()
    {
        RestartLevel(4);
    }

    public void Start3()
    {
        RestartLevel(3);
    }
	
    public void RestartLevel(int size)
    {
        GridSize = size;
        Menu.SetActive(false);

        GameState = EGameState.Game;

        foreach(GridCellComp cellComp in CellCompList)
        {
            GameObject.Destroy(cellComp.gameObject);
        }
        CellCompList.Clear();

        if (DragChess)
        {
            GameObject.Destroy(DragChess.gameObject);
        }

        GridData = new int[GridSize, GridSize];

        float beginX = -(GridSize - 1) / 2 * CellOffset;
        float beginY = -(GridSize - 1) / 2 * CellOffset;

        for (int i = 0; i < GridSize; i++)
        {
            for (int j = 0; j < GridSize; j++)
            {
                GameObject gridPrefab = Resources.Load<GameObject>("GridCell");
                GameObject newGrid = (GameObject)GameObject.Instantiate(gridPrefab);
                newGrid.transform.position = new Vector3(j * CellOffset + beginX, i * CellOffset + beginY);
                GridCellComp gridCellComp = newGrid.GetComponent<GridCellComp>();
                gridCellComp.gridX = j;
                gridCellComp.gridY = i;
                CellCompList.Add(gridCellComp);
            }
        }
        ShowDrawChess();
    }

    public void FillRandomGrid()
    {
        int gridX = 0;
        int gridY = 0;

        if(GridSize == 5)
        {
            List<int> freeAroundList = new List<int>();

            int x = 0;
            int y = 0;

            x = LastPlayerX + 1;
            y = LastPlayerY;
            if(x>=0 && x<GridSize && y>=0 && y<GridSize && GridData[y, x] == 0)
            {
                freeAroundList.Add(y*GridSize+x);
            }

            x = LastPlayerX;
            y = LastPlayerY + 1;
            if (x >= 0 && x < GridSize && y >= 0 && y < GridSize && GridData[y, x] == 0)
            {
                freeAroundList.Add(y * GridSize + x);
            }

            x = LastPlayerX;
            y = LastPlayerY - 1;
            if (x >= 0 && x < GridSize && y >= 0 && y < GridSize && GridData[y, x] == 0)
            {
                freeAroundList.Add(y * GridSize + x);
            }

            x = LastPlayerX - 1;
            y = LastPlayerY;
            if (x >= 0 && x < GridSize && y >= 0 && y < GridSize && GridData[y, x] == 0)
            {
                freeAroundList.Add(y * GridSize + x);
            }

            if(freeAroundList.Count > 0)
            {
                int index2 = freeAroundList[Random.Range(0, freeAroundList.Count - 1)];

                gridX = index2 % GridSize;
                gridY = index2 / GridSize;

                print("电脑填入： " + gridY + " " +  gridX);
                GridData[gridY, gridX] = 2;

                GridCellComp gridCellComp2 = CellCompList[index2];

                GameObject newChess2 = GameObject.Instantiate(OtherChessPrefab) as GameObject;
                newChess2.transform.parent = gridCellComp2.transform;
                newChess2.transform.localPosition = Vector3.zero;

                ShowDrawChess();

                if(CheckEnd())
                {
                    GameState = EGameState.End;
                    ShowWait();
                }
                return;
            }
        }


        List<int> freeIndexList = new List<int>();
        for (int i = 0; i < GridSize; i++)
        {
            for (int j=0; j<GridSize; j++)
            {
                if (GridData[i, j] == 0)
                {
                    freeIndexList.Add(i * GridSize + j); 
                }
            }
        }

        int index = freeIndexList[Random.Range(0, freeIndexList.Count - 1)];

        gridX = index % GridSize;
        gridY = index / GridSize;

        print("电脑填入： " + gridY + " " +  gridX);
        GridData[gridY, gridX] = 2;

        GridCellComp gridCellComp = CellCompList[index];

        GameObject newChess = GameObject.Instantiate(OtherChessPrefab) as GameObject;
        newChess.transform.parent = gridCellComp.transform;
        newChess.transform.localPosition = Vector3.zero;

        ShowDrawChess();

        if(CheckEnd())
        {
            GameState = EGameState.End;
            ShowWait();
        }
        
        
    }

    public void ShowDrawChess()
    {
        DragChess = ((GameObject)GameObject.Instantiate(Resources.Load<GameObject>("DragChess"))).GetComponent<DragChess>();
    }

    private int winner = 1;             // 0-tie 1-1 2-2
    private int finishState = -1;

    public bool CheckEnd()
    {
        // h 1
        for (int i = 0; i < GridSize; i++ )
        {
            bool win = true;
            for (int j = 0; j < GridSize; j++)
            { 
                if (GridData[i, j] != 1)
                {
                    win = false;
                    break;
                }
            }
            if(win)
            {
                finishState = i;
                winner = 1;
                return true;
            }
        }

        // h 2
        for (int i = 0; i < GridSize; i++ )
        {
            bool win = true;
            for (int j = 0; j < GridSize; j++)
            { 
                if (GridData[i, j] != 2)
                {
                    win = false;
                    break;
                }
            }
            if(win)
            {
                finishState = i;
                winner = 2;
                return true;
            }
        }

        // v 1
        for (int i = 0; i < GridSize; i++)
        {
            bool win = true;
            for (int j = 0; j < GridSize; j++)
            {
                if (GridData[j, i] != 1)
                {
                    win = false;
                    break;
                }
            }
            if (win)
            {
                finishState = i + GridSize;
                winner = 1;
                return true;
            }
        }

        // v 2
        for (int i = 0; i < GridSize; i++)
        {
            bool win = true;
            for (int j = 0; j < GridSize; j++)
            {
                if (GridData[j, i] != 2)
                {
                    win = false;
                    break;
                }
            }
            if (win)
            {
                finishState = i + GridSize;
                winner = 2;
                return true;
            }
        }

        // z 1
        {
            bool win = true;
            for(int i=0; i<GridSize; i++)
            {
                if(GridData[i, i] != 1)
                {
                    win = false;
                }
            }
            if (win)
            {
                finishState = GridSize * 2;
                winner = 1;
                return true;
            }
        }

        // z 2 
        {
            bool win = true;
            for (int i = 0; i < GridSize; i++)
            {
                if (GridData[i, i] != 2)
                {
                    win = false;
                }
            }
            if (win)
            {
                finishState = GridSize * 2;
                winner = 2;
                return true;
            }
        }

        // f 1
        {
            bool win = true;
            for (int i = 0; i < GridSize; i++)
            {
                if (GridData[i, GridSize - i - 1] != 1)
                {
                    win = false;
                }
            }
            if (win)
            {
                finishState = GridSize * 2 + 1;
                winner = 1;
                return true;
            }
        }


        // f 2
        {
            bool win = true;
            for (int i = 0; i < GridSize; i++)
            {
                if (GridData[i, GridSize - i - 1] != 2)
                {
                    win = false;
                }
            }
            if (win)
            {
                finishState = GridSize * 2 + 1;
                winner = 2;
                return true;
            }
        }

        // tie
        bool t = true;
        for (int i = 0; i < GridSize; i++)
        {
            for (int j = 0; j < GridSize; j++)
            {
                if (GridData[i, j] == 0)
                {
                    t = false;
                    break;
                }
            }
            if (!t)
                break;
        }

        if(t)
        {
            winner = 0;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void ShowWait()
    {
        if (winner == 0)
            starCount = 2;
        else if (winner == 1)
            starCount = 3;
        else if (winner == 2)
            starCount = 1;
        
        Wait2.SetActive(true);


        if (winner != 0)
        {
            for (int i = 0; i < GridSize; i++)
            {
                if (0 <= finishState && finishState < GridSize)
                {
                    GridCellComp sb = CellCompList[finishState * GridSize + i];
                    GameObject newLine = GameObject.Instantiate(Resources.Load<GameObject>("lineH")) as GameObject;
                    newLine.transform.parent = sb.transform;
                    newLine.transform.localPosition = new Vector3(0, 0, -1);
                    newLine.transform.localScale = Vector3.one;
                }
                else if (GridSize <= finishState && finishState < GridSize * 2)
                {
                    GridCellComp sb = CellCompList[i * GridSize + finishState - GridSize];
                    GameObject newLine = GameObject.Instantiate(Resources.Load<GameObject>("lineV")) as GameObject;
                    newLine.transform.parent = sb.transform;
                    newLine.transform.localPosition = new Vector3(0, 0, -1);
                    newLine.transform.localScale = Vector3.one;
                }
                else if (finishState == GridSize * 2)
                {
                    GridCellComp sb = CellCompList[i * GridSize + i];
                    GameObject newLine = GameObject.Instantiate(Resources.Load<GameObject>("lineF")) as GameObject;
                    newLine.transform.parent = sb.transform;
                    newLine.transform.localPosition = new Vector3(0, 0, -1);
                    newLine.transform.localScale = new Vector3(1.4f, 1,1);
                }
                else if (finishState == GridSize * 2 + 1)
                {
                    GridCellComp sb = CellCompList[i * GridSize + (GridSize - i - 1)];
                    GameObject newLine = GameObject.Instantiate(Resources.Load<GameObject>("lineZ")) as GameObject;
                    newLine.transform.parent = sb.transform;
                    newLine.transform.localPosition = new Vector3(0, 0, -1);
                    newLine.transform.localScale = new Vector3(1.4f, 1, 1);
                }
            }
        }
    }

    public void ShowEnd()
    {
        Wait2.SetActive(false);
        End.SetActive(true);
        StartCoroutine("showStar");
        Wait(3, () =>
        {
            Menu.SetActive(true);
            foreach (GameObject star in StarList)
            {
                GameObject.Destroy(star);
            }
            StarList.Clear();
            End.SetActive(false);
        });
    }

    float starCount = 0;
    public IEnumerator showStar()
    {
        float offset = -(starCount - 1) / 2 * 120;
        for (int i = 0; i < starCount; i++)
        {
            GameObject star = (GameObject)GameObject.Instantiate(Resources.Load<GameObject>("Star"));
            star.transform.parent = End.transform;
            Vector3 pos = new Vector3(offset + i * 120, 0, 0);
            star.transform.localPosition = pos;
            star.transform.localScale = Vector3.one;
            StarList.Add(star);
            yield return new WaitForSeconds(0.2f);
        }
        yield return null;
    }
}


public enum EGameState
{
    Menu,
    Game,
    Wait,
    End,
}

