using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class EleComp : MonoBehaviour {
    public ElementInfo ElementInfo;

    public static float MaxSpeed = 10;
    public static float Gravity = 10;

    public float CurSpeed = 0;

    // shared through both world
    public EEleCompState State;   // Stable / Falling 
    public Vector3 StableGrid;

    private Vector2 fallTargetGrid;
    private Vector3 fallTargetPos { get { return GameManager.GridToPos(fallTargetGrid); } }

    public Vector2 Grid
    {
        get
        {
            return GetFloorGrid();
        }
    }

    public bool Dragging {
        get { return GameManager.Instance.SelEleComp == this; }
    }
    
    void Start () {
        fallTargetGrid = new Vector2(Grid.x, 0);
	}
	
	void Update () {

        if (Dragging)
        {
            CurSpeed = 0;
        }
        else
        {
            // -- check stability every update
            // we have to keeps checking stable grid in case someelse took it, even during falling !

            Vector2 oldStableGrid = fallTargetGrid;
            Vector2 stableGrid = FindStableGrid();

            //if(!oldStableGrid.Equals(stableGrid))
            //{
            //    Debug.LogWarning("更新稳定位置 " + stableGrid.ToString());
            //}

            if (State == EEleCompState.Stable)
            {
                // basically switch state from stable to falling
                if (!Grid.Equals(stableGrid))
                {
                    // Debug.LogWarning("因底部抽取再次下落");
                    State = EEleCompState.Falling;
                    fallTargetGrid = stableGrid;
                }
                //else if(name == "L")
                //{
                //    print("why " + Grid.ToString() + " -- " + stableGrid.ToString());
                //}
            }

            fallTargetGrid = stableGrid;

            if (State == EEleCompState.Falling)
            {
                Vector3 oldPos = transform.position;
                if (transform.position.y > fallTargetPos.y)
                {
                    CurSpeed += Time.deltaTime * Gravity;
                    CurSpeed = Mathf.Clamp(CurSpeed, 0, MaxSpeed);
                    transform.position = new Vector3(fallTargetPos.x, oldPos.y - Time.deltaTime * CurSpeed, 0);
                }

                if (transform.position.y < fallTargetPos.y)
                {
                    State = EEleCompState.Stable;
                    transform.position = fallTargetPos; 
                }
            }
        }
	}

    public Vector2 FindStableGrid(Vector3? overridePos = null )
    {
        // stable grid is basically the grid current element can stand stable

        // note when search for stable base, don't take itself into consideration
        Vector2 curGrid = overridePos == null ? GetFloorGrid() : GameManager.PosToGrid(overridePos.Value);

        // find bottom lines first 
        List<Vector2> bottomsL = new List<Vector2>();
        foreach (Vector2 segPos in ElementInfo.OccupyGridsL)
        {
            // check this as bottom one 
            if (!bottomsL.Exists(pos => pos.x == segPos.x && pos.y < segPos.y))
            {
                // remove all upper ones
                bottomsL.RemoveAll(pos => pos.x == segPos.x && pos.y >= segPos.y);
                bottomsL.Add(segPos);
            }
        }

        List<Vector2> bottomsR = new List<Vector2>();
        foreach (Vector2 segPos in ElementInfo.OccupyGridsR)
        {
            // check this as bottom one 
            if (!bottomsR.Exists(pos => pos.x == segPos.x && pos.y < segPos.y))
            {
                // remove all upper ones
                bottomsR.RemoveAll(pos => pos.x == segPos.x && pos.y >= segPos.y);
                bottomsR.Add(segPos);
            }
        }

        Vector2 supportGrid = new Vector2(curGrid.x, -1);

        // check left world
        foreach (Vector2 segPos in bottomsL)
        {
            int checkSegX = (int)(curGrid.x + segPos.x);
            if (GameManager.GetGridWorldIndex(checkSegX) != 0)
                continue;

            int step = 1;
            while(true)
            {
                int checkGridY = (int)(curGrid.y - step);

                int checkSegY = (int)(curGrid.y + segPos.y - step);
                if (checkSegY == -1)
                {
                    if (checkGridY >= supportGrid.y)
                    {
                        supportGrid = new Vector2(curGrid.x, curGrid.y - step);
                    }
                    break;
                }

                if (GameManager.Instance.GridStats[checkSegY, checkSegX].State == EGridState.Occupied)
                {
                    if (checkGridY >= supportGrid.y)
                    {
                        supportGrid = new Vector2(curGrid.x, curGrid.y - step);
                        break; 
                    }
                }
                step++;
            }
        }

        // check right world
        foreach (Vector2 segPos in bottomsR)
        {
            int checkSegX = (int)(curGrid.x + segPos.x);
            if (GameManager.GetGridWorldIndex(checkSegX) != 1)
                continue;

            int step = 1;
            while (true)
            {
                int checkGridY = (int)(curGrid.y - step);

                int checkSegY = (int)(curGrid.y + segPos.y - step);
                if (checkSegY == -1)
                {
                    if (checkGridY >= supportGrid.y)
                    {
                        supportGrid = new Vector2(curGrid.x, curGrid.y - step);
                    }
                    break;
                }

                if (GameManager.Instance.GridStats[checkSegY, checkSegX].State == EGridState.Occupied)
                {
                    if (checkGridY >= supportGrid.y)
                    {
                        supportGrid = new Vector2(curGrid.x, curGrid.y - step);
                        break;
                    }
                }
                step++;
            }
        }

        Vector2 stableGrid = new Vector2(curGrid.x, supportGrid.y + 1);     // never mind, will get overwrited any way
        // Vector2 stableGrid = new Vector2(curGrid.x, 0);

        return stableGrid;
    }

    public Vector2 GetNearestGridPos()
    {
        return new Vector2(Mathf.Round((transform.position.x-GameManager.PosStartX)*100/GameManager.GridDesignSize), Mathf.Round((transform.position.y-GameManager.PosStartY)*100/GameManager.GridDesignSize));
    }
    
    public Vector3 GetFixedLimitPos(Vector3 pos)
    {
        // the pos is intented world position
        
        // do a simple fix first to ensure element on board
        //pos.x = Mathf.Clamp(pos.x, GameManager.PosStartX, GameManager.PosEndX);
        //pos.y = Mathf.Clamp(pos.y, GameManager.PosStartY, GameManager.PosEndY);

        Rect fixBound = Rect.MinMaxRect(
            GameManager.PosStartX - ElementInfo.LeftEdgeAll * GameManager.GridWorldSize,
            GameManager.PosStartY - ElementInfo.BottomEdgeAll * GameManager.GridWorldSize,
            GameManager.PosEndX - ElementInfo.RightEdgeAll * GameManager.GridWorldSize, 
            GameManager.PosEndY - ElementInfo.TopEdgeAll * GameManager.GridWorldSize);

        // note: i don't know if the "better" solution really better, but certenly it takes much more time

        // ****** the "better" solution (not completed)
        //Vector2 roughGrid = GetNearestGridPos();
        //// seems not need to fix grid at all!
        //// roughGrid.x = Mathf.Clamp(roughGrid.x, 0, GameManager.GridColCount - 1);
        //// roughGrid.y = Mathf.Clamp(roughGrid.x, 0, GameManager.GridRowCount - 1);

        //// fix x accuratly
        //roughGrid.x = Mathf.Clamp(roughGrid.x, roughGrid.x - ElementInfo.LeftEdgeL, roughGrid.x - ElementInfo.RightEdgeR);

        //// fix y accuratly
        //// we need take both world element into account for the Y grid
        //int runtimeBottomEdge = 999;
        //int runtimeTopEdge = -999;

        //foreach (Vector2 relaGrid in ElementInfo.OccupyGridsL)
        //{
        //    if (GameManager.CheckGridWorldIndex((int)(roughGrid.x + relaGrid.x)) == 0)
        //    {
        //        runtimeBottomEdge = (int)relaGrid.y < runtimeBottomEdge ? (int)relaGrid.y : runtimeBottomEdge;
        //        runtimeTopEdge = (int)relaGrid.y > runtimeTopEdge ? (int)(relaGrid.y) : runtimeTopEdge;
        //    }
        //}

        //foreach (Vector2 relaGrid in ElementInfo.OccupyGridsR)
        //{
        //    if (GameManager.CheckGridWorldIndex((int)(roughGrid.x + relaGrid.x)) == 1)
        //    {
        //        runtimeBottomEdge = (int)relaGrid.y < runtimeBottomEdge ? (int)relaGrid.y : runtimeBottomEdge;
        //        runtimeTopEdge = (int)relaGrid.y > runtimeTopEdge ? (int)(relaGrid.y) : runtimeTopEdge;
        //    }
        //}

        //roughGrid.y = Mathf.Clamp(roughGrid.y, roughGrid.y - runtimeBottomEdge, roughGrid.y - runtimeTopEdge);

        //return GameManager.GridToPos(roughGrid);
        // ******

        pos.x = Mathf.Clamp(pos.x, fixBound.xMin, fixBound.xMax);
        pos.y = Mathf.Clamp(pos.y, fixBound.yMin, fixBound.yMax);

        return pos;
    }

    public Vector2 GetFloorGrid()
    {
        return GameManager.PosToGrid(transform.position);
    }

    public void SetGridPos(Vector2 gridPos)
    {
        transform.position = GameManager.GridToPos(gridPos);
    }
}

public enum EEleCompState  //(state is valid only not dragging)
{
    Stable,
    Falling,
}