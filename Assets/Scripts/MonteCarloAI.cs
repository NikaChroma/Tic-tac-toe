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
    private int simulationsPerMove = 1000;
    public StepsController.Move GetBestMove(int player)
    {
        StepsController.Move bestMove = new();
        int bestSum = 0;
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
        //Debug.Log(bestSum);
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
            Move randomMove = controller.legalMovesCopy[Random.Range(0, controller.legalMovesCopy.Count)];
            //if(step == 0) Debug.Log(randomMove.y1 + " " + randomMove.x1 + " " + randomMove.y2 + " " + randomMove.x2);
            controller.SimStepProcessing(randomMove.y1, randomMove.x1, randomMove.y2, randomMove.x2, step);
            step++;
        }
        //Debug.Log(step);
        if (field.SimResult == player + 1)
        {
            return 1;
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
}
