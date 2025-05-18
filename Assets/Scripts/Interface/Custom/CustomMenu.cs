using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomMenu : MonoBehaviour
{
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gameObject.SetActive(false);
        }
    }
}
