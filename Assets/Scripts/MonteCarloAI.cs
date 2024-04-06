using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static CreateField;
using static StepsController;


public class MonteCarloAI : MonoBehaviour
{
    [SerializeField] private CreateField field;
    [SerializeField] private StepsController controller;
    [SerializeField] private WinChecker winChecker;
    private int simulationsPerMove = 2000;
    public StepsController.Move GetBestMove(int player)
    {
        StepsController.Move bestMove = new();
        int bestSum = 0;
        if (GetBetterFieldsOnField().Count > 0 && controller.Step > 20)
        {
            List<Move> betterFields = GetBetterFieldsOnField();
            foreach (Move move in betterFields)
            {
                CopyField();
                int sum = 0;
                for (int i = 0; i < simulationsPerMove; i++)
                {
                    sum += SimulateGame(player);
                }
                if (sum > bestSum)
                {
                    bestSum = sum;
                    bestMove = move;
                }
            }
        }
        else if (GetBestStepsOnField().Count > 0)
        {
            List<Move> betterFields = GetBestStepsOnField();
            foreach (Move move in betterFields)
            {
                CopyField();
                int sum = 0;
                for (int i = 0; i < simulationsPerMove; i++)
                {
                    sum += SimulateGame(player);
                }
                if (sum > bestSum)
                {
                    bestSum = sum;
                    bestMove = move;
                }
            }
        }
        else
        {
            foreach (Move move in controller.legalMoves)
            {
                CopyField();
                int sum = 0;
                for (int i = 0; i < simulationsPerMove; i++)
                {
                    sum += SimulateGame(player);
                }
                if (sum > bestSum)
                {
                    bestSum = sum;
                    bestMove = move;
                }
            }
        }
        Debug.Log(bestMove.y1 + " " + bestMove.x1 + " " + bestMove.y2 + " " + bestMove.x2);
        return bestMove;
    }
    private int SimulateGame(int player)
    {
        CopyField();
        int step = 0;
        field.SimResult = 0;
        controller.legalMovesCopy.Clear();
        foreach (Move move in controller.legalMoves)
        {
            controller.legalMovesCopy.Add(move);
        }
        while (field.SimResult == 0)
        {
            Move randomMove;
            if (HasAvailableCornerMove() && step < 10)
            {
                List<Move> cornerMoves = GetCornerMoves();
                randomMove = cornerMoves[Random.Range(0, cornerMoves.Count)];
            }
            else if (GetBestSteps().Count > 0)
            {
                List<Move> betterFields = GetBestSteps();
                randomMove = betterFields[Random.Range(0, betterFields.Count)];
            }
            else if(GetBetterFields().Count > 0)
            {
                List<Move> betterFields = GetBetterFields();
                randomMove = betterFields[Random.Range(0, betterFields.Count)];
            }
            else
            {
                randomMove = controller.legalMovesCopy[Random.Range(0, controller.legalMovesCopy.Count)];
            }
            controller.SimStepProcessing(randomMove.y1, randomMove.x1, randomMove.y2, randomMove.x2, step);
            step++;
        }
        if (field.SimResult == player + 1)
        {
            return 100 - step;
        }
        else
        {
            return 0;
        }

    }
    private void CopyField()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                field.SimulationField[i, j].state = field.BigField[i, j].state;
                for (int k = 0; k < 3; k++)
                {
                    for (int l = 0; l < 3; l++)
                    {
                        field.SimulationField[i, j].field[k, l].state = field.BigField[i, j].field[k, l].state;
                    }
                }
            }
        }
    }
    private List<Move> GetBestStepsOnField()
    {
        List<Move> bestSteps = new List<Move>();
        foreach (Move move in controller.legalMoves)
        {
            int[,] newField = new int[3, 3];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    newField[i, j] = field.BigField[move.y1, move.x1].field[i, j].state;
                }
            }
            newField[move.y2, move.x2] = 1;
            if (winChecker.CheckMiniWin(newField) != 0)
            {
                bestSteps.Add(move);
            }
            else
            {
                newField[move.y2, move.x2] = 2;
                if (winChecker.CheckMiniWin(newField) != 0)
                {
                    bestSteps.Add(move);
                }
            }
        }

        return bestSteps;
    }
    private List<Move> GetBetterFieldsOnField()
    {
        List<Move> betterFields = new List<Move>();
        foreach (Move move in controller.legalMoves)
        {
            if (field.BigField[move.y2, move.x2].state == 0)
            {
                int[,] newField = new int[3, 3];
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        newField[i, j] = field.BigField[i, j].state;
                    }
                }
                newField[move.y2, move.x2] = 1;
                if (winChecker.CheckMiniWin(newField) == 0)
                {
                    betterFields.Add(move);
                }
                else
                {
                    newField[move.y2, move.x2] = 2;
                    if (winChecker.CheckMiniWin(newField) == 0)
                    {
                        betterFields.Add(move);
                    }
                }
            }
        }

        return betterFields;
    }
    private List<Move> GetBestSteps()
    {
        List<Move> bestSteps = new List<Move>();
        foreach (Move move in controller.legalMovesCopy)
        {
            int[,] newField = new int[3, 3];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    newField[i, j] = field.SimulationField[move.y1, move.x1].field[i, j].state;
                }
            }
            newField[move.y2, move.x2] = 1;
            if (winChecker.CheckMiniWin(newField) != 0)
            {
                bestSteps.Add(move);
            }
            else
            {
                newField[move.y2, move.x2] = 2;
                if (winChecker.CheckMiniWin(newField) != 0)
                {
                    bestSteps.Add(move);
                }
            }
        }

        return bestSteps;
    }
    private List<Move> GetBetterFields()
    {
        List<Move> betterFields = new List<Move>();
        foreach(Move move in controller.legalMovesCopy)
        {
            if(field.SimulationField[move.y2, move.x2].state == 0)
            {
                int[,] newField = new int[3, 3];
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        newField[i, j] = field.SimulationField[i, j].state;
                    }
                }
                newField[move.y2, move.x2] = 1;
                if (winChecker.CheckMiniWin(newField) == 0)
                {
                    betterFields.Add(move);
                }
                else
                {
                    newField[move.y2, move.x2] = 2;
                    if (winChecker.CheckMiniWin(newField) == 0)
                    {
                        betterFields.Add(move);
                    }
                }
            }
        }

        return betterFields;
    }
    private bool HasAvailableCornerMove()
    {
        // ѕровер€ем, есть ли доступные угловые ходы
        foreach (Move move in controller.legalMovesCopy)
        {
            if ((move.x2 == 0 || move.x2 == 2) && (move.y2 == 0 || move.y2 == 2))
            {
                return true;
            }
        }
        return false;
    }
    private List<Move> GetCornerMoves()
    {
        // ѕолучаем список всех доступных угловых ходов
        List<Move> cornerMoves = new List<Move>();
        foreach (Move move in controller.legalMovesCopy)
        {
            if ((move.x2 == 0 || move.x2 == 2) && (move.y2 == 0 || move.y2 == 2))
            {
                cornerMoves.Add(move);
            }
        }
        return cornerMoves;
    }
}
