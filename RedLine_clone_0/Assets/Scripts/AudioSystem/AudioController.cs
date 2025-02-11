using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace EAudioSystem
{
    public class AudioController : MonoBehaviour
    {
        public float paramValue = 0.0f;

        public StudioEventEmitter a;
        public StudioGlobalParameterTrigger sgpt;
        public StudioGlobalParameterTrigger musicTrigger;
        public StudioGlobalParameterTrigger musicRaceFinishedTrigger;
        public StudioGlobalParameterTrigger musicLowpassValue;
        public StudioGlobalParameterTrigger masterVolumeValue;
        public GameObject musicParentGO;
        public GameObject musicTriggerGO;
        public GameObject musicRaceFinishedTriggerGO;
        public GameObject musicLowpassValueGO;
        public GameObject masterVolumeValueGO;
        public bool newValueChanged = false;
        public float newValue = 0.0f;
        public float masterVolume = 1.0f;

        public void SetParamValue(string paramName, float value)
        {
            GameManager.gManager.aC.musicTrigger.Value = value;
            GameManager.gManager.aC.musicTriggerGO.SetActive(false);
            GameManager.gManager.aC.musicTriggerGO.SetActive(true);
            GameManager.gManager.aC.musicTriggerGO.SetActive(false);
        }

        public void SetMasterVolume(float newValue)
        {
            masterVolume = newValue;
            GameManager.gManager.aC.masterVolumeValue.Value = newValue;
            GameManager.gManager.aC.masterVolumeValueGO.SetActive(false);
            GameManager.gManager.aC.masterVolumeValueGO.SetActive(true);
            GameManager.gManager.aC.masterVolumeValueGO.SetActive(false);
            GameManager.gManager.m_masterVolume = newValue;

        }

        private void Update()
        {
            if (GameManager.gManager.musicEmitter != null && GameManager.gManager.musicEmitter.IsPlaying() == false)
            {
                GameManager.gManager.musicEmitter.Play();
            }
        }
    }
}