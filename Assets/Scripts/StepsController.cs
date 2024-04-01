using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class StepsController : MonoBehaviour
{
    public int Step = 0;
    private int x1, y1, x2, y2;
    [SerializeField] private CreateField createField;

    void Start()
    {

    }

    void Update()
    {

    }
    public void StepProcessing(int a, int b, int c, int d)
    {
        y1 = a;
        x1 = b;
        y2 = c;
        x2 = d;
        if (Step % 2 == 0)
        {
            ChangeSprite(0);
            createField.BigField[y1, x1].field[y2, x2].state = 1;
        }
        else
        {
            ChangeSprite(1);
            createField.BigField[y1, x1].field[y2, x2].state = 2;
        }
        CloseCells();
        OpenCells();
        Step++;
    }
    private void CloseCells()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                for (int k = 0; k < 3; k++)
                {
                    for (int l = 0; l < 3; l++)
                    {
                        createField.BigField[i, j].field[k, l].obj.GetComponent<Button>().interactable = false;
                    }
                }
            }
        }
    }
    private void OpenCells()
    {
        if(createField.BigField[y2, x2].state == 0)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (createField.BigField[y2, x2].field[i, j].state == 0)
                    {
                        createField.BigField[y2, x2].field[i, j].obj.GetComponent<Button>().interactable = true;
                    }
                }
            }
        }
    }
    private void ChangeSprite(int n)
    {
        createField.BigField[y1, x1].field[y2, x2].obj.GetComponent<CellScript>().ChangeSprite(n);

    }
}
 