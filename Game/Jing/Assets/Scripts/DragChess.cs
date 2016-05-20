using UnityEngine;
using System.Collections;

public class DragChess : MonoBehaviour {

    private bool dragging = false;
    private Vector3 vel = new Vector3();
    void Update()
    {
        if (!dragging && Input.GetButtonDown("Fire1") && GameManager.Instance.GameState == EGameState.Game)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            if (collider2D.bounds.Contains(mousePos))
            {
                dragging = true;
            }
        }

        if (dragging)
        {
            Vector3 p = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            p.z = 0;
            transform.position = p;
        }
        else
        {
            transform.position = Vector3.SmoothDamp(transform.position, new Vector3(-6, 2, 0), ref vel, 0.5f);
        }

        if (dragging && Input.GetButtonUp("Fire1"))
        {
            dragging = false;
        }
    }
}
