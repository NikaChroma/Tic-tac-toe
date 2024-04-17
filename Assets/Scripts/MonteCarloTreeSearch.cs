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
        TreeNode rootNode = new TreeNode(null, move, player, root);
        int n = 0;
        while (startTime - Time.time < 5 && n < 100)
        {
            n++;
            TreeNode node = Selection(rootNode);
            Expansion(node);
            int result = Simulation(node);
            Backpropagation(node, result);
        }

        return GetBestChild(rootNode).move;
    }
    private TreeNode Selection(TreeNode node) {

        while (node.children.Count > 0)
        {
            node = node.children[0];
            foreach (TreeNode child in node.children)
            {
                if (child.visits != 0)
                {
                    if (child.wins / child.visits > node.wins / node.visits)
                    {
                        node = child;
                    }
                }
                else
                {
                    if (node.visits != 0)
                    {
                        if (node.wins / node.visits < 0.5)
                        {
                            node = child;
                        }
                    }
                }
            }
        }
        return node;
    }
    private void Expansion(TreeNode node)
    {

        foreach (Move move in CheckCells(node))
        {
            TreeNode childNode = new TreeNode(node, move, (node.player + 1) % 2, node.miniFields);
            CopyField(node.miniFields);
            controller.SimStepProcessing(move.y1, move.x1, move.y2, move.x2, node.player);
            childNode.miniFields = field.SimulationField;
            node.children.Add(childNode);
        }

    }
    private int Simulation(TreeNode node)
    {
        CopyField(node.miniFields);
        int currentPlayer = node.player;
        field.SimResult = 0;
        controller.CheckCells(node.move.y2, node.move.x2, 1);
        while (field.SimResult == 0)
        {
            Move randomMove = controller.legalMovesCopy[UnityEngine.Random.Range(0, controller.legalMovesCopy.Count)];
            controller.SimStepProcessing(randomMove.y1, randomMove.x1, randomMove.y2, randomMove.x2, node.player);
            currentPlayer = (currentPlayer + 1) % 2;
        }
        return (field.SimResult - 1);
    }
    private void Backpropagation(TreeNode node, int result)
    {
        while (node != null)
        {
            node.visits++;
            node.wins += (node.player == (result + 1)) ? 1 : 0;
            node = node.parent;
        }
    }
    private TreeNode GetBestChild(TreeNode node)
    {
        double bestScore = double.MinValue;
        TreeNode bestChild = null;
        foreach (TreeNode child in node.children)
        {
            double score = child.wins / (double)child.visits;
            if (score > bestScore)
            {
                bestScore = score;
                bestChild = child;
            }
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
    private void CopyField(MiniField[,] miniFields)
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
    public class TreeNode
    {
        public TreeNode parent;
        public Move move;
        public int visits;
        public int wins;
        public List<TreeNode> children;
        public int player;
        public MiniField[,] miniFields;
        public TreeNode(TreeNode parent, Move move, int player, MiniField[,] miniFields)
        {
            this.parent = parent;
            this.move = move;
            this.visits = 0;
            this.wins = 0;
            this.children = new List<TreeNode>();
            this.player = player;
            this.miniFields = miniFields;
        }
    }
}

