using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Currency : MonoBehaviour
{
    Text text;

    public int addAmount = 1;
    public int gold;
    
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        text.text = gold.ToString();
        // This code is to update the currency of the game via the screen 
        if (gold < 0)
        {
            gold = 0;
        }
        
    }
}

