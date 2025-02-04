using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StartCameras;

namespace StartCameras {
    public class StartCameraManager : MonoBehaviour
    {
        public List<StartCamera> cameras = new();
        StartCamera m_currentCamera;
        StartCamera m_prevCamera;
        public Animator preRaceCanvasAnim;
        int m_cameraIndex = 0;
        bool m_switching;

        // Start is called before the first frame update
        void Start()
        {
            this.gameObject.SetActive(true);
            m_currentCamera = cameras[0];
            m_currentCamera.gameObject.SetActive(true);
            preRaceCanvasAnim.SetTrigger("PreRace");
        }

        // Update is called once per frame
        void Update()
        {
            if (!GameManager.gManager.readyForCountdown)
            {
                m_currentCamera.gameObject.transform.position += m_currentCamera.moveDirection * m_currentCamera.speed * Time.deltaTime;
                if (!m_switching)
                    StartCoroutine(SwitchIndex());
            }
        }

        IEnumerator SwitchIndex()
        {
            m_switching = true;
            yield return new WaitForSeconds(2f);
            m_cameraIndex++;
            if (m_cameraIndex > cameras.Count - 1)
            {
                preRaceCanvasAnim.SetTrigger("PreRaceFadeOut");
                GameManager.gManager.readyForCountdown = true;
                GameManager.gManager.startCamerasFinished = true;
                m_cameraIndex = 0;
            }

            if (!GameManager.gManager.readyForCountdown)
            {
                m_prevCamera = m_currentCamera;
                m_prevCamera.gameObject.SetActive(false);
                m_currentCamera = cameras[m_cameraIndex];
                m_currentCamera.gameObject.SetActive(true);
            }
            else
            {
                m_prevCamera = m_currentCamera;
                m_prevCamera.gameObject.SetActive(false);
                m_currentCamera = cameras[m_cameraIndex];
                m_currentCamera.gameObject.SetActive(false);
                this.gameObject.SetActive(false);
            }

            m_switching = false;
        }
    }
}
