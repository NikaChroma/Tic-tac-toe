using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScript : MonoBehaviour
{
    [SerializeField] private Image currentPlayer;
    [SerializeField] private StepsController controller;
    [SerializeField] private Sprite[] sprites;

    private void Update()
    {
        CheckStep();
    }
    private void CheckStep()
    {
        currentPlayer.sprite = sprites[controller.Step % 2];
    }
}
