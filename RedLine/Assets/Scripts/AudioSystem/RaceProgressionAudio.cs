using FMODUnity;
using Unity.VisualScripting;
using UnityEngine;

public class RaceProgressionAudio : MonoBehaviour
{
    public StudioGlobalParameterTrigger paramTrigger;
    public GameObject parentGO;

    float musicVolume = 1.0f;

    public float tempParamValue = 1.0f;
    public bool UpdateParamValue = false;

    public void SetParamValue(float value)
    {
        if (UpdateParamValue == true)
        {
            UpdateParamValue = false;
            parentGO.SetActive(true);
            paramTrigger.Value = tempParamValue;
            parentGO.SetActive(false);

        }
        else
        {
            parentGO.SetActive(true);
            paramTrigger.Value = value;
            parentGO.SetActive(false);
        }
    }


    private void Update()
    {
        if (GameManager.gManager.musicEmitter != null)
        {
            if (parentGO != null && paramTrigger != null)
            {
                if (parentGO.activeSelf == true && parentGO.GetComponent<StudioGlobalParameterTrigger>().Value != tempParamValue)
                {
                    paramTrigger.Value = tempParamValue;
                    parentGO.SetActive(false);
                    parentGO.SetActive(true);
                }
                //if (UpdateParamValue == true)
                //{
                //
                //
                //    //SetParamValue(tempParamValue);
                //}
            }
        }
        GameManager.gManager.musicEmitter.EventInstance.setVolume(musicVolume * GameManager.gManager.m_musicVolume);
    }
}
