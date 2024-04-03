using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepsOrder : MonoBehaviour
{
    [SerializeField] private StepsController controller;
    void Start()
    {
        
    }
    void Update()
    {
        
    }

    private void CheckPlayer()
    {
        if(controller.Step % 2 == 1)
        {
            
        }
    }
}
