using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static StepsController;
using static CreateField;
using UnityEditor.Experimental.GraphView;

public class MonteCarloTreeSearch : MonoBehaviour
{
    [SerializeField] private CreateField field;
    [SerializeField] private StepsController controller;
    [SerializeField] private WinChecker winChecker;


    public Move GetBestMove(Move move, int player)
    {
        //Debug.Log("Начат поиск лучшего");
        TreeNode rootNode = new TreeNode(null, move, player, field.BigField);
        int n = 0;
        int wins = 0;
        while (n < 100000)
        {
            n++;
            TreeNode node = SelectNode(rootNode);
            ExpandNode(node);
            int result = SimulateGame(node);
            Backpropagate(node, result);
            if (result == 1) wins++;
        }
        Debug.Log("Вероятность победы " + wins/n + "%");
        return GetBestChild(rootNode).move;
    }

    private TreeNode SelectNode(TreeNode node)
    {
        //Debug.Log("Выбрана нода");
        TreeNode selectedNode = node;
        while (selectedNode.children.Count > 0)
        {
            //Debug.Log("Дети - " + selectedNode.children.Count);
            selectedNode = UCBSelect(selectedNode);
        }
        return selectedNode;
    }

    private TreeNode UCBSelect(TreeNode node)
    {
        double bestUCBValue = 0;
        TreeNode bestChild = node.children[0];
        foreach (TreeNode child in node.children)
        {
            if(child.visits == 0) child.visits = 1;
            //Debug.Log(child.move.y1 + " " + child.move.x1 + " " + child.move.y2 + " " + child.move.x2);
            double UCBValue = (child.wins / (double)child.visits) + Math.Sqrt(2 * Math.Log(node.visits) / child.visits);
            if (UCBValue > bestUCBValue)
            {
                bestUCBValue = UCBValue;
                bestChild = child;
            }
            //Debug.Log(UCBValue);
        }
        //Debug.Log(bestChild.move.y1 + " " + bestChild.move.x1 + " " + bestChild.move.y2 + " " + bestChild.move.x2);
        //Debug.Log("  ");

        return bestChild;
    }

    private void ExpandNode(TreeNode node)
    {
        //Debug.Log(node.move.y2 + " " + node.move.x2);
        foreach (Move move in CheckCells(node))
        {
            TreeNode childNode = new TreeNode(node, move, node.player, node.miniFields);
            node.children.Add(childNode);
        }
        //Debug.Log("Число доступных ходов - " + CheckCells(node).Count);
    }
    private List<Move> CheckCells(TreeNode node)
    {
        var field = node.miniFields;
        List<Move> moves = new List<Move>();
        if (field[node.move.y2, node.move.x2].state == 0)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (field[node.move.y2, node.move.x2].field[i, j].state == 0)
                    {
                        Move move = new Move(node.move.x2, node.move.y2, j, i);
                        moves.Add(move);
                        //Debug.Log(move.y1 + " " + move.x1 + " " + move.y2 + " " + move.x2);
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
                    if (field[i, j].state == 0)
                    {
                        for (int k = 0; k < 3; k++)
                        {
                            for (int l = 0; l < 3; l++)
                            {
                                if (field[i, j].field[k, l].state == 0)
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

    private int SimulateGame(TreeNode node)
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
        //Debug.Log("Отыграно!");
        return (field.SimResult - 1);
    }

    private void Backpropagate(TreeNode node, int result) 
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
        //Debug.Log(bestScore);
        //Debug.Log(bestChild.move.y2 + " " + bestChild.move.x2);
        return bestChild;
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


}

