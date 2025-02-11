using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreatingLobbySettings : MonoBehaviour
{
    public TMP_Text playerNumber;

    public void OnValueChange(Slider slider)
    {
        playerNumber.text = slider.value.ToString();
    }
}
