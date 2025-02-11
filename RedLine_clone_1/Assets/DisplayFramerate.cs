using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplayFramerate : MonoBehaviour
{
    public float fps;
    public TextMeshProUGUI fpsCounter;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating(nameof(GetFPS), 0.06f, 0.05f);
    }

    public void GetFPS()
    {
        fps = (int)(1f / Time.unscaledDeltaTime);
        fpsCounter.text = fps.ToString();
    }

}
