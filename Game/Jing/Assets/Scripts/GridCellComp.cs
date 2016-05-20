using UnityEngine;
using System.Collections;

public class GridCellComp : MonoBehaviour {

    public int gridX = 0;
    public int gridY = 0;

	void Start () {
	    
	}
	
	void Update () {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        
        if(Input.GetButtonUp("Fire1") && GameManager.Instance.GameState == EGameState.Game)
        {
            if (collider2D.bounds.Contains(mousePos))
            {
                GameManager.Instance.LastPlayerX = gridX;
                GameManager.Instance.LastPlayerY = gridY;

                if (GameManager.Instance.GridData[gridY, gridX] == 0)
                {
                    Debug.Log(string.Format("包含: {0}  {1}  ", gridX, gridY));
                    GameObject newChess = GameObject.Instantiate(GameManager.Instance.PlayerChessPrefab) as GameObject;
                    newChess.transform.parent = this.transform;
                    newChess.transform.localPosition = Vector3.zero;

                    GameManager.Instance.GridData[gridY, gridX] = 1;

                    GameObject.Destroy(GameManager.Instance.DragChess.gameObject);

                    if(GameManager.Instance.CheckEnd())
                    {
                        GameManager.Instance.GameState = EGameState.End;
                        GameManager.Instance.ShowWait(); 
                    }

                    if (GameManager.Instance.GameState == EGameState.Game)
                    {
                        GameManager.Instance.GameState = EGameState.Wait;
                        GameManager.Instance.Wait(1.0f, () =>
                        {
                            GameManager.Instance.GameState = EGameState.Game;
                            GameManager.Instance.FillRandomGrid();
                        });
                    }
                }
            }
            else
            {
            //    Debug.Log("鼠标位: " + mousePos.ToString());
            //    Debug.Log("触发区: " + collider2D.bounds.ToString());
            }
        }
	}

    public int Owner {
        get
        {
            return GameManager.Instance.GridData[gridY, gridX];
        }
    }
}
