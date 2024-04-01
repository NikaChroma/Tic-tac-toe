using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static CreateField;

public class ClickController : MonoBehaviour
{
    [SerializeField] private CreateField createField;
    [SerializeField] private StepsController stepsController;
    void Start()
    {
        AddOnClick();
    }

    private void AddOnClick()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                for (int k = 0; k < 3; k++)
                {
                    for (int l = 0; l < 3; l++)
                    {
                        int tempI = i;
                        int tempJ = j;
                        int tempK = k;
                        int tempL = l;
                        createField.BigField[i, j].field[k, l].obj.GetComponent<Button>().onClick.AddListener(() => ClickOnCell(tempI, tempJ, tempK, tempL));
                    }
                }
            }
        }
    }
    private void ClickOnCell(int a, int b, int c, int d)
    {
        stepsController.StepProcessing(a, b, c, d);
    }
}
