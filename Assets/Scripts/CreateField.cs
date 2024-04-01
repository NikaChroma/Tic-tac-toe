using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CreateField : MonoBehaviour
{
    void Awake()
    {
        canvas = FindObjectOfType<Canvas>();
        Create();
    }
    public class cell
    {
        public int state = 0;
        public GameObject obj;
    }
    public class MiniField
    {
        public cell[,] field = new cell[3,3];
        public int state = 0;
    }
    public MiniField[,] BigField = new MiniField[3, 3]; 

    [SerializeField] private GameObject CellPrefab;
    [SerializeField] private Transform parentTransform;
    Canvas canvas;
    private void Create()
    {
        for (int i = 0; i < 3; i++)
        {
            for(int j = 0; j < 3; j++)
            {
                BigField[i, j] = new MiniField();
                for (int k = 0; k < 3; k++)
                {
                    for(int  l = 0; l < 3; l++)
                    {
                        BigField[i, j].field[k, l] = new cell();
                        GameObject NewCell = Instantiate(CellPrefab, canvas.transform);
                        RectTransform rectTransform = NewCell.GetComponent<RectTransform>();
                        rectTransform.anchoredPosition = new Vector2(80 * l + 270 * j - 350, 80 * k + 270 * i - 350);
                        NewCell.transform.SetParent(parentTransform);
                        BigField[i, j].field[k, l].obj = NewCell;
                    }
                }
            }
        }
    }
}
