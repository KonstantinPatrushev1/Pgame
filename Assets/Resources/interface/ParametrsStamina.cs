using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParametrsStamina : MonoBehaviour
{
    public Image stamina;
    public PlayerInfo playerInfo;
    void Start()
    {
        stamina = GetComponent<Image>();
        playerInfo = FindObjectOfType<PlayerInfo>();
    }
    
    void Update()
    {
        stamina.fillAmount = playerInfo.stamina / playerInfo.maxstamina;
    }
}
