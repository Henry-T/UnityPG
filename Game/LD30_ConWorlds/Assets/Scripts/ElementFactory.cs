using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ElementFactory 
{

    public static List<ElementInfo> ElementInfos = new List<ElementInfo>() { 
        // cat 0
        new ElementInfo(){Catagory=0, Type=0,
            AssetL = "gray_0",
            AssetR = "gray_0",
            OffsetL = new Vector3(0.5f, 0, 0), 
            OffsetR = new Vector3(0.5f, 0, 0), 
            CollidPointsL=new Vector2[]{new Vector2(-2,0.5f), new Vector2(2,0.5f), new Vector2(2,-0.5f), new Vector2(-2, -0.5f)},
            CollidPointsR=new Vector2[]{new Vector2(-2,0.5f), new Vector2(2,0.5f), new Vector2(2,-0.5f), new Vector2(-2, -0.5f)},
            OccupyGridsL = new Vector2[]{new Vector2(-1,0), new Vector2(0,0), new Vector2(1,0), new Vector2(2,0)},
            OccupyGridsR = new Vector2[]{new Vector2(-1,0), new Vector2(0,0), new Vector2(1,0), new Vector2(2,0)}
        },
        new ElementInfo(){Catagory=0, Type=1,  
            AssetL = "gray_1",
            AssetR = "gray_1",
            OffsetL = new Vector3(0f, 0.5f, 0), 
            OffsetR = new Vector3(0f, 0.5f, 0), 
            CollidPointsL=new Vector2[]{new Vector2(-1.5f, 1), new Vector2(-0.5f, 1), new Vector2(-0.5f, 0), new Vector2(1.5f,0), new Vector2(1.5f, -1f), new Vector2(-1.5f, -1f)},
            CollidPointsR=new Vector2[]{new Vector2(-1.5f, 1), new Vector2(-0.5f, 1), new Vector2(-0.5f, 0), new Vector2(1.5f,0), new Vector2(1.5f, -1f), new Vector2(-1.5f, -1f)},
            OccupyGridsL = new Vector2[]{new Vector2(-1,1), new Vector2(-1,0), new Vector2(0,0), new Vector2(1,0)},
            OccupyGridsR = new Vector2[]{new Vector2(-1,1), new Vector2(-1,0), new Vector2(0,0), new Vector2(1,0)}
        },
        new ElementInfo(){Catagory=0, Type=2,  
            AssetL = "gray_2",
            AssetR = "gray_2",
            OffsetL = new Vector3( 0, 0.5f, 0),  
            OffsetR = new Vector3( 0, 0.5f, 0), 
            CollidPointsL=new Vector2[]{new Vector2(-1.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 0), new Vector2(1.5f,0), new Vector2(1.5f, -1), new Vector2(-0.5f, -1), new Vector2(-0.5f, 0), new Vector2(-1.5f, 0)},
            CollidPointsR=new Vector2[]{new Vector2(-1.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 0), new Vector2(1.5f,0), new Vector2(1.5f, -1), new Vector2(-0.5f, -1), new Vector2(-0.5f, 0), new Vector2(-1.5f, 0)},
            OccupyGridsL = new Vector2[]{new Vector2(-1,1), new Vector2(0,1), new Vector2(0,0), new Vector2(1,0)},
            OccupyGridsR = new Vector2[]{new Vector2(-1,1), new Vector2(0,1), new Vector2(0,0), new Vector2(1,0)}
        },
        new ElementInfo(){Catagory=0, Type=3, 
            AssetL = "gray_3",
            AssetR = "gray_3", 
            OffsetL = new Vector3( 0, -0.5f, 0), 
            OffsetR = new Vector3( 0, -0.5f, 0), 
            CollidPointsL=new Vector2[]{new Vector2(-0.5f, 1), new Vector2(1.5f, 1), new Vector2(1.5f, 0), new Vector2(0.5f, 0), new Vector2(0.5f, -1), new Vector2(-1.5f, -1), new Vector2(-1.5f, 0), new Vector2(-0.5f, 0)},
            CollidPointsR=new Vector2[]{new Vector2(-0.5f, 1), new Vector2(1.5f, 1), new Vector2(1.5f, 0), new Vector2(0.5f, 0), new Vector2(0.5f, -1), new Vector2(-1.5f, -1), new Vector2(-1.5f, 0), new Vector2(-0.5f, 0)},
            OccupyGridsL = new Vector2[]{new Vector2(-1,-1), new Vector2(0,-1), new Vector2(0,0), new Vector2(1,0)},
            OccupyGridsR = new Vector2[]{new Vector2(-1,-1), new Vector2(0,-1), new Vector2(0,0), new Vector2(1,0)}
        },
        new ElementInfo(){Catagory=0, Type=4,  
            AssetL = "gray_4",
            AssetR = "gray_4",
            OffsetL = new Vector3( 0, -0.5f, 0), 
            OffsetR = new Vector3( 0, -0.5f, 0), 
            CollidPointsL=new Vector2[]{new Vector2(-1.5f, 1), new Vector2(1.5f, 1), new Vector2(1.5f, 0), new Vector2(-0.5f, 0), new Vector2(-0.5f, -1), new Vector2(-1.5f, -1)},
            CollidPointsR=new Vector2[]{new Vector2(-1.5f, 1), new Vector2(1.5f, 1), new Vector2(1.5f, 0), new Vector2(-0.5f, 0), new Vector2(-0.5f, -1), new Vector2(-1.5f, -1)},
            OccupyGridsL = new Vector2[]{new Vector2(-1,-1), new Vector2(-1,0), new Vector2(0,0), new Vector2(1,0)},
            OccupyGridsR = new Vector2[]{new Vector2(-1,-1), new Vector2(-1,0), new Vector2(0,0), new Vector2(1,0)}
        },
        new ElementInfo(){Catagory=0, Type=5,  
            AssetL = "gray_5",
            AssetR = "gray_5",
            OffsetL = new Vector3( 0, 0.5f, 0), 
            OffsetR = new Vector3( 0, 0.5f, 0), 
            CollidPointsL=new Vector2[]{new Vector2(-0.5f,1), new Vector2(0.5f, 1), new Vector2(0.5f, 0), new Vector2(1.5f, 0), new Vector2(1.5f, -1), new Vector2(-1.5f, -1), new Vector2(-1.5f, 0), new Vector2(-0.5f, 0)},
            CollidPointsR=new Vector2[]{new Vector2(-0.5f,1), new Vector2(0.5f, 1), new Vector2(0.5f, 0), new Vector2(1.5f, 0), new Vector2(1.5f, -1), new Vector2(-1.5f, -1), new Vector2(-1.5f, 0), new Vector2(-0.5f, 0)},
            OccupyGridsL = new Vector2[]{new Vector2(-1,0), new Vector2(0,0), new Vector2(0,1), new Vector2(1,0)},
            OccupyGridsR = new Vector2[]{new Vector2(-1,0), new Vector2(0,0), new Vector2(0,1), new Vector2(1,0)}
        },
        new ElementInfo(){Catagory=0, Type=6,  
            AssetL = "gray_6",
            AssetR = "gray_6",
            OffsetL = new Vector3(0.5f, -0.5f, 0), 
            OffsetR = new Vector3(0.5f, -0.5f, 0), 
            CollidPointsL = new Vector2[]{new Vector2(-1,1), new Vector2(1,1), new Vector2(1,-1), new Vector2(-1,-1)},
            CollidPointsR = new Vector2[]{new Vector2(-1,1), new Vector2(1,1), new Vector2(1,-1), new Vector2(-1,-1)},
            OccupyGridsL = new Vector2[]{new Vector2(0,0), new Vector2(1,0), new Vector2(0,-1), new Vector2(1,-1)},
            OccupyGridsR = new Vector2[]{new Vector2(0,0), new Vector2(1,0), new Vector2(0,-1), new Vector2(1,-1)}
        },
        // cat 1
        new ElementInfo(){Catagory=1, Type=0,
            AssetL = "green_0",
            AssetR = "red_6",  
            OffsetL = new Vector3(0.5f, 0, 0), 
            OffsetR = new Vector3(0.5f, -0.5f, 0), 
            CollidPointsL=new Vector2[]{new Vector2(-2,0.5f), new Vector2(2,0.5f), new Vector2(2,-0.5f), new Vector2(-2, -0.5f)},
            CollidPointsR = new Vector2[]{new Vector2(-1,1), new Vector2(1,1), new Vector2(1,-1), new Vector2(-1,-1)},
            OccupyGridsL = new Vector2[]{new Vector2(-1,0), new Vector2(0,0), new Vector2(1,0), new Vector2(2,0)},
            OccupyGridsR = new Vector2[]{new Vector2(0,0), new Vector2(1,0), new Vector2(0,-1), new Vector2(1,-1)}
        },
        new ElementInfo(){Catagory=1, Type=1,  
            AssetL = "green_1",
            AssetR = "red_2",  
            OffsetL = new Vector3( 0, 0.5f, 0), 
            OffsetR = new Vector3( 0, 0.5f, 0), 
            CollidPointsL=new Vector2[]{new Vector2(-1.5f, 1), new Vector2(-0.5f, 1), new Vector2(-0.5f, 0), new Vector2(1.5f,0), new Vector2(1.5f, -0.5f), new Vector2(-1.5f, -0.5f)},
            CollidPointsR=new Vector2[]{new Vector2(-1.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 0), new Vector2(1.5f,0), new Vector2(1.5f, -1), new Vector2(-0.5f, -1), new Vector2(-0.5f, 0), new Vector2(-1.5f, 0)},
            OccupyGridsL = new Vector2[]{new Vector2(-1,1), new Vector2(-1,0), new Vector2(0,0), new Vector2(1,0)},
            OccupyGridsR = new Vector2[]{new Vector2(-1,1), new Vector2(0,1), new Vector2(0,0), new Vector2(1,0)}
        },
        new ElementInfo(){Catagory=1, Type=2,  
            AssetL = "green_2",
            AssetR = "red_1",  
            OffsetL = new Vector3( 0, 0.5f, 0), 
            OffsetR = new Vector3( 0, 0.5f, 0), 
            CollidPointsL=new Vector2[]{new Vector2(-1.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 0), new Vector2(1.5f,0), new Vector2(1.5f, -1), new Vector2(-0.5f, -1), new Vector2(-0.5f, 0), new Vector2(-1.5f, 0)},
            CollidPointsR=new Vector2[]{new Vector2(-1.5f, 1), new Vector2(-0.5f, 1), new Vector2(-0.5f, 0), new Vector2(1.5f,0), new Vector2(1.5f, -0.5f), new Vector2(-1.5f, -0.5f)},
            OccupyGridsL = new Vector2[]{new Vector2(-1,1), new Vector2(0,1), new Vector2(0,0), new Vector2(1,0)},
            OccupyGridsR = new Vector2[]{new Vector2(-1,1), new Vector2(-1,0), new Vector2(0,0), new Vector2(1,0)}
        },
        new ElementInfo(){Catagory=1, Type=3,  
            AssetL = "green_3",
            AssetR = "red_4",  
            OffsetL = new Vector3( 0, -0.5f, 0), 
            OffsetR = new Vector3( 0, -0.5f, 0), 
            CollidPointsL=new Vector2[]{new Vector2(-0.5f, 1), new Vector2(1.5f, 1), new Vector2(1.5f, 0), new Vector2(0.5f, 0), new Vector2(0.5f, -1), new Vector2(-1.5f, -1), new Vector2(-1.5f, 0), new Vector2(-0.5f, 0)},
            CollidPointsR=new Vector2[]{new Vector2(-1.5f, 1), new Vector2(1.5f, 1), new Vector2(1.5f, 0), new Vector2(-0.5f, 0), new Vector2(-0.5f, -1), new Vector2(-1.5f, -1)},
            OccupyGridsL = new Vector2[]{new Vector2(-1,-1), new Vector2(0,-1), new Vector2(0,0), new Vector2(1,0)},
            OccupyGridsR = new Vector2[]{new Vector2(-1,-1), new Vector2(-1,0), new Vector2(0,0), new Vector2(1,0)}
        },
        new ElementInfo(){Catagory=1, Type=4,  
            AssetL = "green_4",
            AssetR = "red_3",  
            OffsetL = new Vector3( 0, -0.5f, 0),
            OffsetR = new Vector3( 0, -0.5f, 0),  
            CollidPointsL=new Vector2[]{new Vector2(-1.5f, 1), new Vector2(1.5f, 1), new Vector2(1.5f, 0), new Vector2(-0.5f, 0), new Vector2(-0.5f, -1), new Vector2(-1.5f, -1)},
            CollidPointsR=new Vector2[]{new Vector2(-0.5f, 1), new Vector2(1.5f, 1), new Vector2(1.5f, 0), new Vector2(0.5f, 0), new Vector2(0.5f, -1), new Vector2(-1.5f, -1), new Vector2(-1.5f, 0), new Vector2(-0.5f, 0)},
            OccupyGridsL = new Vector2[]{new Vector2(-1,-1), new Vector2(-1,0), new Vector2(0,0), new Vector2(1,0)},
            OccupyGridsR = new Vector2[]{new Vector2(-1,-1), new Vector2(0,-1), new Vector2(0,0), new Vector2(1,0)}
        },
        new ElementInfo(){Catagory=1, Type=5,  
            AssetL = "green_5",
            AssetR = "red_5",  
            OffsetL = new Vector3( 0, 0.5f, 0), 
            OffsetR = new Vector3( 0, 0.5f, 0), 
            CollidPointsL=new Vector2[]{new Vector2(-0.5f,1), new Vector2(0.5f, 1), new Vector2(0.5f, 0), new Vector2(1.5f, 0), new Vector2(1.5f, -1), new Vector2(-1.5f, -1), new Vector2(-1.5f, 0), new Vector2(-0.5f, 0)},
            CollidPointsR=new Vector2[]{new Vector2(-0.5f,1), new Vector2(0.5f, 1), new Vector2(0.5f, 0), new Vector2(1.5f, 0), new Vector2(1.5f, -1), new Vector2(-1.5f, -1), new Vector2(-1.5f, 0), new Vector2(-0.5f, 0)},
            OccupyGridsL = new Vector2[]{new Vector2(-1,0), new Vector2(0,0), new Vector2(0,1), new Vector2(1,0)},
            OccupyGridsR = new Vector2[]{new Vector2(-1,0), new Vector2(0,0), new Vector2(0,1), new Vector2(1,0)}
        },
        new ElementInfo(){Catagory=1, Type=6,  
            AssetL = "green_6",
            AssetR = "red_0",  
            OffsetL = new Vector3(0.5f, -0.5f, 0), 
            OffsetR = new Vector3(0.5f, 0, 0),  
            CollidPointsL = new Vector2[]{new Vector2(-1,1), new Vector2(1,1), new Vector2(1,-1), new Vector2(-1,-1)},
            CollidPointsR=new Vector2[]{new Vector2(-2,0.5f), new Vector2(2,0.5f), new Vector2(2,-0.5f), new Vector2(-2, -0.5f)},
            OccupyGridsL = new Vector2[]{new Vector2(0,0), new Vector2(1,0), new Vector2(0,-1), new Vector2(1,-1)},
            OccupyGridsR = new Vector2[]{new Vector2(-1,0), new Vector2(0,0), new Vector2(1,0), new Vector2(2,0)}
        },
        // cat2 guy_green
        new ElementInfo(){Catagory=2, Type=0,
            AssetL = "guy_green",
            AssetR = "guy_green",
            OffsetL = new Vector3(0.5f, -0.5f, 0), 
            OffsetR = new Vector3(0.5f, -0.5f, 0), 
            CollidPointsL = new Vector2[]{new Vector2(-1,1), new Vector2(1,1), new Vector2(1,-1), new Vector2(-1,-1)},
            CollidPointsR = new Vector2[]{new Vector2(-1,1), new Vector2(1,1), new Vector2(1,-1), new Vector2(-1,-1)},
            OccupyGridsL = new Vector2[]{new Vector2(0,0), new Vector2(1,0), new Vector2(0,-1), new Vector2(1,-1)},
            OccupyGridsR = new Vector2[]{new Vector2(0,0), new Vector2(1,0), new Vector2(0,-1), new Vector2(1,-1)}
        },

        // cat3 guy_red
        new ElementInfo(){Catagory=3, Type=0,
            AssetL = "guy_red",
            AssetR = "guy_red",
            OffsetL = new Vector3(0.5f, -0.5f, 0), 
            OffsetR = new Vector3(0.5f, -0.5f, 0), 
            CollidPointsL = new Vector2[]{new Vector2(-1,1), new Vector2(1,1), new Vector2(1,-1), new Vector2(-1,-1)}, 
            CollidPointsR = new Vector2[]{new Vector2(-1,1), new Vector2(1,1), new Vector2(1,-1), new Vector2(-1,-1)},
            OccupyGridsL = new Vector2[]{new Vector2(0,0), new Vector2(1,0), new Vector2(0,-1), new Vector2(1,-1)},
            OccupyGridsR = new Vector2[]{new Vector2(0,0), new Vector2(1,0), new Vector2(0,-1), new Vector2(1,-1)}
        },
    }; 

    public static void Initialize()
    {
        foreach (ElementInfo eleInfo in ElementInfos)
            eleInfo.Initialize();
    }


	public static GameObject SpawnNewElement(int catagory, int type, int facing, int gridX, int gridY )
   	{
        // create the holder    
        GameObject newEle = new GameObject("new element");
        ElementInfo eleInfo = ElementInfos.First(ei=>ei.Catagory==catagory && ei.Type == type);
        EleComp eleComp = newEle.AddComponent<EleComp>();
        eleComp.ElementInfo = eleInfo;
        eleComp.SetGridPos(new Vector2(gridX, gridY));
        newEle.transform.rotation = Quaternion.Euler(0, 0, facing * Mathf.PI / 2.0f);

        // create sprite in left world
        GameObject spriteL = new GameObject("sprite");
        spriteL.transform.parent = newEle.transform;
        spriteL.layer = LayerMask.NameToLayer("ThingL");
        spriteL.AddComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Elements/"+eleInfo.AssetL);
        Vector3 offsetL = eleInfo.OffsetL;
        spriteL.transform.localPosition = new Vector3(offsetL.x * GameManager.GridWorldSize, offsetL.y * GameManager.GridWorldSize, 0);

        PolygonCollider2D polyColl2DL = spriteL.AddComponent<PolygonCollider2D>();
        List<Vector2> collPointsL = new List<Vector2>();
        foreach(Vector2 p in eleInfo.CollidPointsL)
        {
            collPointsL.Add(new Vector2(p.x * GameManager.GridWorldSize, p.y * GameManager.GridWorldSize));
        }
        polyColl2DL.SetPath(0, collPointsL.ToArray());

        // create sprite in right world
        GameObject spriteR = new GameObject("sprite");
        spriteR.transform.parent = newEle.transform;
        spriteR.layer = LayerMask.NameToLayer("ThingR");
        spriteR.AddComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Elements/" + eleInfo.AssetR);
        Vector3 offsetR = eleInfo.OffsetR;
        spriteR.transform.localPosition = new Vector3(offsetR.x * GameManager.GridWorldSize, offsetR.y * GameManager.GridWorldSize, 0);

        PolygonCollider2D polyColl2DR = spriteR.AddComponent<PolygonCollider2D>();
        List<Vector2> collPointsR = new List<Vector2>();
        foreach (Vector2 p in eleInfo.CollidPointsR)
        {
            collPointsR.Add(new Vector2(p.x * GameManager.GridWorldSize, p.y * GameManager.GridWorldSize));
        }
        polyColl2DR.SetPath(0, collPointsR.ToArray());

        GameManager.Instance.EleCompList.Add(eleComp);

        if (catagory == 2)
            GameManager.Instance.EleCompGreen = eleComp;

        if (catagory == 3)
            GameManager.Instance.EleCompRed = eleComp;



        return newEle;
	}
}

public class ElementInfo
{
    public int Catagory;
    public int Type;
    public Vector3 OffsetL;
    public Vector3 OffsetR;
    public string AssetL;
    public string AssetR;
    public Vector2[] CollidPointsL;
    public Vector2[] CollidPointsR;
    public Vector2[] OccupyGridsL;
    public Vector2[] OccupyGridsR;
    public int LeftEdgeL = 999;
    public int RightEdgeL = -999;
    public int BottomEdgeL = 999;
    public int TopEdgeL = -999;
    public int LeftEdgeR = 999;
    public int RightEdgeR = -999;
    public int BottomEdgeR = 999;
    public int TopEdgeR = -999;
    public int LeftEdgeAll = 0;
    public int RightEdgeAll = 0;
    public int BottomEdgeAll = 0;
    public int TopEdgeAll = 0;

    public void Initialize()
    {
        // calc all edges
        foreach(Vector2 grid in OccupyGridsL)
        {
            LeftEdgeL = (int)grid.x < LeftEdgeL ? (int)grid.x : LeftEdgeL;
            RightEdgeL = (int)grid.x > RightEdgeL ? (int)grid.x : RightEdgeL;
            BottomEdgeL = (int)grid.y < BottomEdgeL ? (int)grid.y : BottomEdgeL;
            TopEdgeL = (int)grid.y > TopEdgeL ? (int)grid.y : TopEdgeL;
        }

        foreach (Vector2 grid in OccupyGridsR)
        {
            LeftEdgeR = (int)grid.x < LeftEdgeR ? (int)grid.x : LeftEdgeR;
            RightEdgeR = (int)grid.x > RightEdgeR ? (int)grid.x : RightEdgeR;
            BottomEdgeR = (int)grid.y < BottomEdgeR ? (int)grid.y : BottomEdgeR;
            TopEdgeR = (int)grid.y > TopEdgeR ? (int)grid.y : TopEdgeR;
        }

        LeftEdgeAll = Mathf.Min(LeftEdgeL, LeftEdgeR);
        RightEdgeAll = Mathf.Max(RightEdgeL, RightEdgeR);
        BottomEdgeAll = Mathf.Min(BottomEdgeL, BottomEdgeR);
        TopEdgeAll = Mathf.Max(TopEdgeL, TopEdgeR);
    }
}