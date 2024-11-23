using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    public float hp;
    // Start is called before the first frame update
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
    }

    public void SetDamage(float newdamage)
    {
        hp -= newdamage;
    }
    
}
