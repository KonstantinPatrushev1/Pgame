using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    public float hp;
    public float maxhp;

    public float stamina;
    public float maxstamina;
    
    private float timer = 0f;

    private void Start()
    {
        maxhp = hp;
        stamina = 1440f;
        maxstamina = stamina;
    }

    void Update()
    {
        if (hp <= 0)
        {
            Debug.Log("Dead");
        }
        else
        {
            Debug.Log("HP: " + hp);
        }
        
        timer += Time.deltaTime;
        if (timer >= 1f && stamina > 0)
        {
            stamina--;
            timer = 0f;
        }
    }

    public void SetDamage(float newdamage)
    {
        hp -= newdamage;
    }

    public void SetStamina(float newstamina)
    {
        stamina -= newstamina;
    }
}
