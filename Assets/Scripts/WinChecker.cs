using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinChecker : MonoBehaviour
{
    [SerializeField] private CreateField createField;
    public int CheckMiniWin(int[,] curField)
    {

        int sumX = 0;
        int res = 1;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (curField[i, j] == 0)
                {
                    res = 0; // not draw
                }
                else if (curField[i, j] == 1)
                {
                    sumX++;
                }
            }
        }
        if (res == 1)
        {
            if (sumX >= 5)
            {
                res = 1;
            }
            else
            {
                res = 2;
            }
        }
        else
        {
            res = 0;
        }
        for (int i = 0; i < 3; i++)
        {
            if (curField[i, 0] != 0 && curField[i, 0] != 3 && curField[i, 0] == curField[i, 1] && curField[i, 0] == curField[i, 2])
            {
                res = curField[i, 0];
            }
            if (curField[0, i] != 0 && curField[0, i] != 3 && curField[0, i] == curField[1, i] && curField[0, i] == curField[2, i])
            {
                res = curField[0, i];
            }
        }
        if ((curField[0, 0] != 0 && curField[0, 0] != 3 && curField[1, 1] == curField[0, 0] && curField[0, 0] == curField[2, 2]) || (curField[2, 0] != 0 && curField[2, 0] != 3 && curField[1, 1] == curField[2, 0] && curField[2, 0] == curField[0, 2]))
        {
            res = curField[1, 1];
        }
        return res;

    }
    public int CheckBigWin(int[,] curField)
    {
        int result;
        result = 3; //draw
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (curField[i, j] == 0)
                {
                    result = 0; // not draw
                }
            }
        }

        for (int i = 0; i < 3; i++)
        {
            if (curField[i, 0] != 0 && curField[i, 0] == curField[i, 1] && curField[i, 0] == curField[i, 2])
            {
                result = curField[i, 0];
            }
            if (curField[0, i] != 0 && curField[0, i] == curField[1, i] && curField[0, i] == curField[2, i])
            {
                result = curField[0, i];
            }
        }
        if ((curField[0, 0] != 0 && curField[1, 1] == curField[0, 0] && curField[0, 0] == curField[2, 2]) || (curField[2, 0] != 0 && curField[1, 1] == curField[2, 0] && curField[2, 0] == curField[0, 2]))
        {
            result = curField[1, 1];
        }
        return result;
    }
}
