using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static StepsController;
using static CreateField;

public class MonteCarloTreeSearch : MonoBehaviour
{
    [SerializeField] private CreateField field;
    [SerializeField] private StepsController controller;
    [SerializeField] private WinChecker winChecker;


    public Move GetBestMove(Move move, int player)
    {
        float startTime = Time.time;
        MiniField[,] root = field.BigField; 
        TreeNode rootNode = new TreeNode(null, move, player, NewField(root), 0);
        int n = 0;
        int wins = 0;
        while (startTime - Time.time < 5 && n < 10000)
        {
            n++;
            TreeNode node = Selection(rootNode);
            Expansion(node);
            int result = Simulation(node);
            if (result == 1) wins++;
            Backpropagation(node, result);
        }
        Debug.Log(wins);

        return GetBestChild(rootNode).move;
    }
    private TreeNode Selection(TreeNode node) {
        TreeNode bestNode = node;
        while (node.children.Count > 0)
        {
            double C = 0.4; // Параметр исследования
            double bestUCB1 = 0;
            bestNode = node.children[0];
            foreach (TreeNode child in node.children)
            {
                double UCB1 = (double)child.wins / child.visits + C * Math.Sqrt(Math.Log(node.visits) / child.visits);
                if (UCB1 > bestUCB1)
                {
                    bestUCB1 = UCB1;
                    bestNode = child;
                }
            }
            node = bestNode;
        }
        return bestNode;
    }
    private void Expansion(TreeNode node)
    {

        foreach (Move move in CheckCells(node))
        {
            TreeNode childNode = new TreeNode(node, move, (node.player + 1) % 2, NewField(node.miniFields), 0);
            SimToMini(node.miniFields);
            controller.SimStepProcessing(move.y1, move.x1, move.y2, move.x2, node.player);
            if(field.SimResult != 0) childNode.result = field.SimResult;
            MiniToSim(childNode.miniFields);
            node.children.Add(childNode);
        }

    }
    private int Simulation(TreeNode node)
    {
        SimToMini(node.miniFields);
        int currentPlayer = node.player;
        field.SimResult = 0;
        if(node.result != 0) field.SimResult = node.result;
        controller.CheckCells(node.move.y2, node.move.x2, 1);
        while (field.SimResult == 0)
        {
            Move randomMove = controller.legalMovesCopy[UnityEngine.Random.Range(0, controller.legalMovesCopy.Count)];
            controller.SimStepProcessing(randomMove.y1, randomMove.x1, randomMove.y2, randomMove.x2, node.player);
            currentPlayer = (currentPlayer + 1) % 2;
        }
        if (field.SimResult != 3)
        {
            return (field.SimResult - 1);
        }
        else
        {
            return (1);
        }
    }
    private void Backpropagation(TreeNode node, int result)
    {
        while (node != null)
        {
            node.visits++;
            node.wins += result;
            node = node.parent;
        }
    }
    private TreeNode GetBestChild(TreeNode node)
    {
        double bestScore = double.MinValue;
        TreeNode bestChild = null;
        foreach (TreeNode child in node.children)
        {
            if (child.visits == 0) child.visits = 1;
            double score = child.wins / (double)child.visits;
            if (score > bestScore)
            {
                bestScore = score;
                bestChild = child;
            }
            Debug.Log(child.visits + " " + child.wins);
        }
        return bestChild;
    }

    private List<Move> CheckCells(TreeNode node)
    {
        if (node == null) Debug.Log("NUL");
        var fields = node.miniFields;
        List<Move> moves = new List<Move>();
        if (fields[node.move.y2, node.move.x2].state == 0)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (fields[node.move.y2, node.move.x2].field[i, j].state == 0)
                    {
                        Move move = new Move(node.move.x2, node.move.y2, j, i);
                        moves.Add(move);
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
                    if (fields[i, j].state == 0)
                    {
                        for (int k = 0; k < 3; k++)
                        {
                            for (int l = 0; l < 3; l++)
                            {
                                if (fields[i, j].field[k, l].state == 0)
                                {
                                    Move move = new Move(j, i, l, k);
                                    moves.Add(move);
                                }
                            }
                        }
                    }
                }
            }
        }
        return moves;
    }
    public class TreeNode
    {
        public TreeNode parent;
        public Move move;
        public int visits;
        public int wins;
        public List<TreeNode> children;
        public int player;
        public MiniField[,] miniFields;
        public int result;
        public TreeNode(TreeNode parent, Move move, int player, MiniField[,] miniFields, int result)
        {
            this.parent = parent;
            this.move = move;
            this.visits = 0;
            this.wins = 0;
            this.children = new List<TreeNode>();
            this.player = player;
            this.miniFields = miniFields;
            this.result = result;
        }
    }
    private void Draw(MiniField[,] miniFields)
    {
        for(int i = 0; i < 3; i++)
        {
            for(int j = 0; j < 3; j++)
            {
                for(int  k = 0; k < 3; k++)
                {
                    Debug.Log(miniFields[i, j].field[k, 0].state + " " + miniFields[i, j].field[k, 1].state + " " + miniFields[i, j].field[k, 2].state);
                }
                Debug.Log("");
            }
        }
    }
    private void MiniToSim(MiniField[,] miniFields)
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                miniFields[i, j].state = field.SimulationField[i, j].state;
                for (int k = 0; k < 3; k++)
                {
                    for (int l = 0; l < 3; l++)
                    {
                        miniFields[i, j].field[k, l].state = field.SimulationField[i, j].field[k, l].state;
                    }
                }
            }
        }
    }
    private void SimToMini(MiniField[,] miniFields)
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                field.SimulationField[i, j].state = miniFields[i, j].state;
                for (int k = 0; k < 3; k++)
                {
                    for (int l = 0; l < 3; l++)
                    {
                        field.SimulationField[i, j].field[k, l].state = miniFields[i, j].field[k, l].state;
                    }
                }
            }
        }
    }
    private MiniField[,] NewField(MiniField[,] miniFields)
    {
        MiniField[,] newField = new MiniField[3,3];
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                newField[i, j] = new MiniField(new Cell[3, 3], miniFields[i, j].state);
                for (int k = 0; k < 3; k++)
                {
                    for (int l = 0; l < 3; l++)
                    {
                        newField[i, j].field[k, l] = new Cell(miniFields[i, j].field[k, l].state, null);
                    }
                }
            }
        }
        return newField;
    }

}

