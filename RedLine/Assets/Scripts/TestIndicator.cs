using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestIndicator : MonoBehaviour
{
    public Camera main;
    public int targetIndex;
    public RawImage image;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.gManager.players.Count > targetIndex - 1 || GameManager.gManager.players[targetIndex - 1] != null)
        {
            this.gameObject.SetActive(true);
            Vector3 screenPosTemp = main.WorldToScreenPoint(GameManager.gManager.players[targetIndex - 1].transform.position);
            Vector2 screenPos = Vector3.zero;
            switch (GameManager.gManager.players.Count)
            {
                case 1:
                    screenPos = new Vector2(screenPosTemp.x, screenPosTemp.y);
                    break;
                case 2:
                    screenPos = new Vector2(screenPosTemp.x / 2, screenPosTemp.y);
                    break;
                case 3:
                    screenPos = new Vector2(screenPosTemp.x / 2, screenPosTemp.y / 2);
                    break;
                case 4:
                    screenPos = new Vector2(screenPosTemp.x / 2, screenPosTemp.y / 2);
                    break;
            }
            image.rectTransform.localPosition = screenPos;
        }
        else
        {
            this.gameObject.SetActive(false);
        }

    }
}
