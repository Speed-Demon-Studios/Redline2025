using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsVolumeControl : MonoBehaviour
{
    [SerializeField] private Slider m_masterVolumeSlider;


    void Awake()
    {
        m_masterVolumeSlider.onValueChanged.AddListener(delegate { GameManager.gManager.aC.SetMasterVolume(m_masterVolumeSlider.value); });
        m_masterVolumeSlider.value = GameManager.gManager.m_masterVolume;
    }
}
