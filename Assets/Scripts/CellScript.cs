using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class CellScript : MonoBehaviour
{
    [SerializeField] private Sprite[] sprites;
    private Image image;
    private void Start()
    {
        image = GetComponent<Image>();
    }

    public void ChangeSprite(int n)
    {
        image.sprite = sprites[n];
    }
}
