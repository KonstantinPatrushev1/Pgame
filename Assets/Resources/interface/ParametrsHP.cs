using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParametrsHP : MonoBehaviour
{
    public Image healthBar;
    public PlayerInfo playerInfo;
    void Start()
    {
        healthBar = GetComponent<Image>();
        playerInfo = FindObjectOfType<PlayerInfo>();
    }
    
    void Update()
    {
        healthBar.fillAmount = playerInfo.hp / playerInfo.maxhp;
    }
}
