using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class StepsController : MonoBehaviour
{
    public int Step { get; private set; } = 0;
    [SerializeField] private CreateField createField;
    [SerializeField] private WinChecker winChecker;
    [SerializeField] private MonteCarloTreeSearch MCAI;
    private int gameState = 1;
    public struct Move
    {
        public int x1, y1, x2, y2;
        public Move(int x1, int y1, int x2, int y2)
        {
            this.x1 = x1;
            this.y1 = y1;
            this.x2 = x2;
            this.y2 = y2;
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            gameState = 0;
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            gameState = 1;
        }
    }
    public void StepProcessing(int y1, int x1, int y2, int x2)
    {
        int currentPlayer = Step % 2;

        UpdateField(y1, x1, y2, x2, currentPlayer);

        int[,] field = ExtractMiniFieldState(y1, x1);
        createField.BigField[y1, x1].state = winChecker.CheckMiniWin(field);

        if (createField.BigField[y1, x1].state != 0)
        {
            field = ExtractBigFieldState();
            createField.Result = winChecker.CheckBigWin(field);
        }

        CloseCells();
        CheckCells(y2, x2, 0);
        OpenCells();
        Step++;
        if (Step % 2 == 1 && createField.Result == 0 && gameState == 1) ComputerStep(new Move(x1, y1, x2, y2));
    }
    private void UpdateField(int y1, int x1, int y2, int x2, int currentPlayer)
    {
        var cellScript = createField.BigField[y1, x1].field[y2, x2].obj.GetComponent<CellScript>();
        cellScript.ChangeSprite(currentPlayer);
        createField.BigField[y1, x1].field[y2, x2].state = currentPlayer + 1;
        //Debug.Log(createField.BigField[y1, x1].field[y2, x2].state);
    }
    private int[,] ExtractMiniFieldState(int y, int x)
    {
        int[,] field = new int[3, 3];
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                field[i, j] = createField.BigField[y, x].field[i, j].state;
            }
        }
        return field;
    }

    private int[,] ExtractBigFieldState()
    {
        int[,] field = new int[3, 3];
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                field[i, j] = createField.BigField[i, j].state;
            }
        }
        return field;
    }
    private void ComputerStep(Move lastMove)
    {
        CloseCells();
        Move move = MCAI.GetBestMove(lastMove, 1);
        StepProcessing(move.y1, move.x1, move.y2, move.x2);

    } 
    public void SimStepProcessing(int y1, int x1, int y2, int x2, int simStep)
    {
        int currentPlayer = (Step + simStep) % 2;
        createField.SimulationField[y1, x1].field[y2, x2].state = currentPlayer + 1;
        
        int[,] field = new int[3, 3];
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                field[i, j] = createField.SimulationField[y1, x1].field[i, j].state;
            }
        }
        createField.SimulationField[y1, x1].state = winChecker.CheckMiniWin(field);
        if(createField.SimulationField[y1, x1].state != 0)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    field[i, j] = createField.SimulationField[i, j].state;
                }
            }
            createField.SimResult = winChecker.CheckBigWin(field);
        }
        
        CheckCells(y2, x2, 1);
    }
    public List<Move> legalMoves = new List<Move>();
    public List<Move> legalMovesCopy = new List<Move>();
    public void CheckCells(int a, int b, int state)
    {
        var field = createField.BigField;
        var list = legalMoves;
        if (state == 1)
        {
            list = legalMovesCopy;
            field = createField.SimulationField;
        }
        list.Clear();
        if (field[a, b].state == 0)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (field[a, b].field[i, j].state == 0)
                    {
                        Move move = new Move();
                        move.y1 = a;
                        move.x1 = b;
                        move.y2 = i;
                        move.x2 = j;
                        list.Add(move);
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if(field[i, j].state == 0) {
                        for (int k = 0; k < 3; k++)
                        {
                            for (int l = 0; l < 3; l++)
                            {
                                if (field[i, j].field[k, l].state == 0)
                                {
                                    Move move = new Move();
                                    move.y1 = i;
                                    move.x1 = j;
                                    move.y2 = k;
                                    move.x2 = l;
                                    list.Add(move);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    private void OpenCells()
    {
        foreach(Move move in legalMoves)
        {
            createField.BigField[move.y1, move.x1].field[move.y2, move.x2].obj.GetComponent<Button>().interactable = true;
        }
        if (createField.Result != 0)
        {
            CloseCells();
        }
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
}