using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinChecker : MonoBehaviour
{
    [SerializeField] private CreateField createField;
    public void CheckMiniWin(int x, int y)
    {
        var curField = createField.BigField[y, x];
        curField.state = 3; //draw
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if(curField.field[i, j].state == 0)
                {
                    curField.state = 0; // not draw
                }
            }
        }
        for (int i = 0; i < 3; i++)
        {
            if ((curField.field[i, 0].state != 0 && curField.field[i, 0].state == curField.field[i, 1].state && curField.field[i, 0].state == curField.field[i, 2].state) || (curField.field[0, i].state != 0 && curField.field[0, i].state == curField.field[1, i].state && curField.field[0, i].state == curField.field[2, i].state))
            {
                curField.state = curField.field[i, 0].state;
            }
        }
        if ((curField.field[0, 0].state != 0 && curField.field[1, 1].state == curField.field[0, 0].state && curField.field[0, 0].state == curField.field[2, 2].state) || (curField.field[2, 0].state != 0 && curField.field[1, 1].state == curField.field[2, 0].state && curField.field[2, 0].state == curField.field[0, 2].state))
        {
            curField.state = curField.field[1, 1].state;
        }
        if(curField.state != 0)
        {
            for(int i = 0; i < 3; i++)
            {
                for(int j = 0; j < 3; j++)
                {
                    if (curField.field[i, j].state == 0) curField.field[i, j].state = 3;
                }
            } 
        }

    }
}
