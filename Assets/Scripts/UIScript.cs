using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScript : MonoBehaviour
{
    [SerializeField] private Image currentPlayer;
    [SerializeField] private StepsController controller;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private GameObject drawPanel;
    [SerializeField] private CreateField createField;
    private void Start()
    {
        resultPanel.SetActive(false);
        drawPanel.SetActive(false);
    }

    private void Update()
    {
        CheckStep();
        CheckWin();
    }
    private void CheckStep()
    {
        currentPlayer.sprite = sprites[controller.Step % 2];
    }
    private void CheckWin()
    {
        if (createField.Result == 0)
        {

        }
        if (createField.Result == 1)
        {
            resultPanel.SetActive(true);
            currentPlayer.sprite = sprites[0];
        }
        if(createField.Result == 2)
        {
            resultPanel.SetActive(true);
            currentPlayer.sprite = sprites[1];
        }
        if(createField.Result == 3)
        {
            drawPanel.SetActive(true);
            currentPlayer.sprite = sprites[2];
        }
    }
}
