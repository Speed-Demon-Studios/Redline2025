using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RichPresence : MonoBehaviour
{
    public int status = 0;
    bool statusSet = false;

    private void Update()
    {
        if (GameManager.gManager.SAM != null && statusSet == false)
        {
            GameManager.gManager.SAM.SetRichPresence(status);
            statusSet = true;
        }
    }
}
