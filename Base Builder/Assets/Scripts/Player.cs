using UnityEngine;
using System.Collections.Generic;

public class Player
{
    bool botClicked;
    /*
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            controllaClick();
        }
    }
    private void controllaClick()
    {
        Vector2 pos=Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero);
        if (hit.collider != null)
        {
            MineraleNode mine = hit.collider.GetComponent<MineraleNode>();
            if (mine != null)
            {
                botClicked = true;
                mine.colpisci();
            }
            else botClicked = false;
        }
        else botClicked = false;
    }*/

}
