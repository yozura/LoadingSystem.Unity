using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingUITester : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            LoadingUIController.Instance.LoadScene("Scene2");
        }
    }
}
