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
    public struct Cell
    {
        public int state;
        public GameObject obj;
        public Cell(int state, GameObject obj)
        {
            this.state = state;
            this.obj = obj;
        }
    }
    public struct MiniField
    {
        public Cell[,] field;
        public int state;
        public MiniField(Cell[,] field, int state)
        {
            this .field = field;
            this.state = state;
        }
    }
    public MiniField[,] BigField = new MiniField[3, 3];
    public MiniField[,] SimulationField = new MiniField[3, 3];
    public int Result = 0, SimResult;

    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private Transform parentTransform;
    Canvas canvas;
    private void Create()
    {
        for (int i = 0; i < 3; i++)
        {
            for(int j = 0; j < 3; j++)
            {
                BigField[i, j] = new MiniField(new Cell[3,3], 0);
                SimulationField[i,j] = new MiniField(new Cell[3, 3], 0);
                for (int k = 0; k < 3; k++)
                {
                    for(int  l = 0; l < 3; l++)
                    {
                        SimulationField[i, j].field[k, l] = new Cell(0, null);
                        GameObject NewCell = Instantiate(cellPrefab, canvas.transform);
                        RectTransform rectTransform = NewCell.GetComponent<RectTransform>();
                        rectTransform.anchoredPosition = new Vector2(80 * l + 270 * j - 350, 80 * k + 270 * i - 350);
                        NewCell.transform.SetParent(parentTransform);
                        BigField[i, j].field[k, l] = new Cell(0, NewCell);
                    }
                }
            }
        }
    }
}
