using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CountdownTextSetter : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (GameManager.gManager.raceCountdown != null)
        {
            if (GameManager.gManager.raceCountdown.GetCountdownTextOBJ() == null)
            {
                GameManager.gManager.raceCountdown.SetCountdownTextObj(this.gameObject.GetComponent<TextMeshProUGUI>());
            }    
        }
    }
}
