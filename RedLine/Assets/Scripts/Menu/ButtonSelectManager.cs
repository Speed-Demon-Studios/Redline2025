using Cinemachine;
using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DifficultyButtonSwitch
{
    public class ButtonSelectManager : MonoBehaviour
    {
        [Header("Player Selections")]
        public int speedClass;
        public int m_manufacturer = 1;
        public bool timeTrial;

        [Header("Menus")]
        [SerializeField] private GameObject m_pressAToStart;
        [SerializeField] private GameObject m_title;
        [SerializeField] private GameObject m_speedClassSelect;
        [SerializeField] private GameObject m_transitionToLoadingScreen;
        [SerializeField] private CinemachineVirtualCamera[] m_cameras;

        [Header("Title")]
        [SerializeField] private Animator m_titleAnim;
        [SerializeField] private Animator m_titleGlowAnim;
        [SerializeField] private AnimationCurve m_titleFillCurve;
        [SerializeField] private AudioSource m_titleAudio;

        [Header("Class Select")]
        [SerializeField] private AnimationCurve m_barChangeTween;
        [SerializeField] private float m_barChangeDuration, m_barLength;
        [SerializeField] private Transform m_speedBar, m_speedBarRed1, m_speedBarRed2, m_competitionSkillBar, m_competitionSkillBarRed1, m_competitionSkillBarRed2;
        [SerializeField] private Animator m_debutAnimator, m_proAnimator, m_eliteAnimator;
        [SerializeField] private float m_debutSpeed, m_debutCompetition, m_proSpeed, m_proCompetition, m_eliteSpeed, m_eliteCompetition;

        [Header("Vehicle Select")]
        [SerializeField] private TextMeshProUGUI m_pressStartToJoinText;
        [SerializeField] private Transform m_topSpeedBar, m_topSpeedBarRed1, m_topSpeedBarRed2, m_accelerationBar, m_accelerationBarRed1, m_accelerationBarRed2, m_handlingBar, m_handlingBarRed1, m_handlingBarRed2;
        [SerializeField] private Animator m_splitwingAnim, m_fulcrumAnim, m_cutlassAnim, m_shipDisplayAnim, m_manufacturerDisplayAnim, m_shipSelectAnimator;
        [SerializeField] private GameObject[] m_SplitwingModel, m_FulcrumModel, m_CutlassModel;
        [SerializeField] private GameObject m_citadelShips, m_falconShips, m_monarchShips;
        [SerializeField] private Sprite m_citadelImage, m_falconImage, m_monarchImage;
        [SerializeField] private Image m_manufacturerImage, m_manufacturerImageRed;

        [Header("Other")]
        [SerializeField] private Material m_cursorMat;
        [SerializeField] private AnimationCurve m_cursorPulseCurve;
        [SerializeField] private float m_cursorPulseDuration;
        [SerializeField] private GameObject m_sunFlare;
        [SerializeField] private Animator m_blackoutAnimator;

        private void Update()
        {
            // For testing manufacturer change
            //if (Input.GetKeyDown(KeyCode.Space))
            //{
            //    //ManufacturerChange();
            //}
        }

        private void OnEnable()
        {
            m_cursorMat.color = Color.white;
            m_pressStartToJoinText.color = Color.white;
            Tween.Color(m_cursorMat, Color.red, m_cursorPulseDuration, 0, m_cursorPulseCurve, Tween.LoopType.Loop);
            Tween.Color(m_pressStartToJoinText, Color.red, m_cursorPulseDuration, 0, m_cursorPulseCurve, Tween.LoopType.Loop);
            
            foreach (CinemachineVirtualCamera cam in m_cameras)
            {
                cam.Priority = 0;
            }
            m_cameras[0].Priority = 1;

        }

        public void TransitionToTitle(bool doTransitionEffect)
        {
            m_pressAToStart.SetActive(false);
            m_title.SetActive(true);

            foreach (CinemachineVirtualCamera cam in m_cameras)
            {
                cam.Priority = 0;
            }
            m_cameras[1].Priority = 1;

            if (doTransitionEffect)
            {
                m_titleAnim.SetTrigger("TitleIn");
                m_titleGlowAnim.SetTrigger("TitleInGlow");
                if (m_titleAudio != null)
                    m_titleAudio.Play();
            }
        }


        public void TransitionToClassSelect(bool timeTrialSelected)
        {
            timeTrial = timeTrialSelected;
            foreach (CinemachineVirtualCamera cam in m_cameras)
            {
                cam.Priority = 0;
            }
            m_cameras[2].Priority = 1;
        }

        public void TransitionToShipSelect(Animator anim)
        {
            anim.SetTrigger("RotateShipDisplay");
        }

        public void Ready()
        {
            m_shipSelectAnimator.SetTrigger("Ready");
            Invoke("TransitionToLoadingScreen", 1f);
        }

        public void Ready2Player()
        {
            m_shipSelectAnimator.SetTrigger("Ready2Player");
            Invoke("TransitionToLoadingScreen", 1f);
        }

        public void Ready3Or4Player()
        {
            m_shipSelectAnimator.SetTrigger("Ready3Or4Player");
            Invoke("TransitionToLoadingScreen", 1f);
        }

        public void TransitionToLoadingScreen()
        {
            m_transitionToLoadingScreen.SetActive(true);
            m_sunFlare.SetActive(false);
            m_blackoutAnimator.SetTrigger("Blackout");


        }




        public void SpeedBarFill(float fillAmount)
        {
            Vector3 barPos = new Vector3(m_barLength - (m_barLength * fillAmount), 0, 0);
            Tween.LocalPosition(m_speedBar, barPos, m_barChangeDuration, 0.05f, m_barChangeTween);
            Tween.LocalPosition(m_speedBarRed1, barPos, m_barChangeDuration, 0, m_barChangeTween);
            Tween.LocalPosition(m_speedBarRed2, barPos, m_barChangeDuration, 0.1f, m_barChangeTween);
        }
    
        public void CompetitionSkillBarFill(float fillAmount)
        {
            Vector3 barPos = new Vector3(m_barLength - (m_barLength * fillAmount), 0, 0);
            Tween.LocalPosition(m_competitionSkillBar, barPos, m_barChangeDuration, 0.05f, m_barChangeTween);
            Tween.LocalPosition(m_competitionSkillBarRed1, barPos, m_barChangeDuration, 0, m_barChangeTween);
            Tween.LocalPosition(m_competitionSkillBarRed2, barPos, m_barChangeDuration, 0.1f, m_barChangeTween);
        }

        public void SelectClass(int speed) 
        {
            speedClass = speed;
        }

        // CLASS: 1 = Debut, 2 = Pro, 3 = Elite;
        public void SpeedClassInfoChange(int speed)
        {
            switch (speed)
            {
                case 1:
                    m_debutAnimator.SetTrigger("TransitionIn");
                    m_proAnimator.SetTrigger("TransitionOut");
                    m_eliteAnimator.SetTrigger("TransitionOut");
                    SpeedBarFill(m_debutSpeed);
                    CompetitionSkillBarFill(m_debutCompetition);
                    Debug.Log("Debut");
                    break;

                case 2:
                    m_debutAnimator.SetTrigger("TransitionOut");
                    m_proAnimator.SetTrigger("TransitionIn");
                    m_eliteAnimator.SetTrigger("TransitionOut");
                    SpeedBarFill(m_proSpeed);
                    CompetitionSkillBarFill(m_proCompetition);
                    Debug.Log("Pro");
                    break;

                case 3:
                    m_debutAnimator.SetTrigger("TransitionOut");
                    m_proAnimator.SetTrigger("TransitionOut");
                    m_eliteAnimator.SetTrigger("TransitionIn");
                    SpeedBarFill(m_eliteSpeed);
                    CompetitionSkillBarFill(m_eliteCompetition);
                    Debug.Log("Elite");
                    break;

            }
        }

        // SHIPS: 1 = Splitwing, 2 = Fulcrum, 3 = Cutlass
        public void VehicleInfoChange(int ship, Animator inAnimator, List<Animator> outAniators)
        {
            inAnimator.SetTrigger("TransitionIn");
            foreach(Animator anim in outAniators)
            {
                anim.SetTrigger("TransitionOut");
            }
            //switch (ship)
            //{
            //    case 1:
            //        m_splitwingAnim.SetTrigger("TransitionIn");
            //        m_fulcrumAnim.SetTrigger("TransitionOut");
            //        m_cutlassAnim.SetTrigger("TransitionOut");
            //        foreach (GameObject model in m_SplitwingModel)
            //        {
            //            model.SetActive(true);
            //        }
            //        foreach (GameObject model in m_FulcrumModel)
            //        {
            //            model.SetActive(false);
            //        }
            //        foreach (GameObject model in m_CutlassModel)
            //        {
            //            model.SetActive(false);
            //        }
            //        break;
            //
            //    case 2:
            //        m_splitwingAnim.SetTrigger("TransitionOut");
            //        m_fulcrumAnim.SetTrigger("TransitionIn");
            //        m_cutlassAnim.SetTrigger("TransitionOut");
            //        foreach (GameObject model in m_SplitwingModel)
            //        {
            //            model.SetActive(false);
            //        }
            //        foreach (GameObject model in m_FulcrumModel)
            //        {
            //            model.SetActive(true);
            //        }
            //        foreach (GameObject model in m_CutlassModel)
            //        {
            //            model.SetActive(false);
            //        }
            //        break;
            //
            //    case 3:
            //        m_splitwingAnim.SetTrigger("TransitionOut");
            //        m_fulcrumAnim.SetTrigger("TransitionOut");
            //        m_cutlassAnim.SetTrigger("TransitionIn");
            //        foreach (GameObject model in m_SplitwingModel)
            //        {
            //            model.SetActive(false);
            //        }
            //        foreach (GameObject model in m_FulcrumModel)
            //        {
            //            model.SetActive(false);
            //        }
            //        foreach (GameObject model in m_CutlassModel)
            //        {
            //            model.SetActive(true);
            //        }
            //        break;
            //
            //}
        }



        // MANUFACTURERS: 1 = Citadel, 2 = Falcon, 3 = Monarch
        public void ManufacturerChange(Sprite turnOn, Animator anim, Image img, Image imgRed)
        {
            anim.SetTrigger("ChangeManufacturer");
            img.sprite = turnOn;
            imgRed.sprite = turnOn;
            //switch (m_manufacturer)
            //{
            //
            //    case 1:
            //        m_manufacturer = 2;
            //        m_manufacturerDisplayAnim.SetTrigger("ChangeManufacturer");
            //        m_manufacturerImage.sprite = m_falconImage;
            //        m_manufacturerImageRed.sprite = m_falconImage;
            //        m_citadelShips.SetActive(false);
            //        m_falconShips.SetActive(true);
            //        m_monarchShips.SetActive(false);
            //
            //        break;
            //
            //    case 2:
            //        m_manufacturer = 3;
            //        m_manufacturerDisplayAnim.SetTrigger("ChangeManufacturer");
            //        m_manufacturerImage.sprite = m_monarchImage;
            //        m_manufacturerImageRed.sprite = m_monarchImage;
            //        m_citadelShips.SetActive(false);
            //        m_falconShips.SetActive(false);
            //        m_monarchShips.SetActive(true);
            //        break;
            //
            //    case 3:
            //        m_manufacturer = 1;
            //        m_manufacturerDisplayAnim.SetTrigger("ChangeManufacturer");
            //        m_manufacturerImage.sprite = m_citadelImage;
            //        m_manufacturerImageRed.sprite = m_citadelImage;
            //        m_citadelShips.SetActive(true);
            //        m_falconShips.SetActive(false);
            //        m_monarchShips.SetActive(false);
            //        break;
            //}
        }

        public void TopSpeedBarFill(float fillAmount)
        {
            Vector3 barPos = new Vector3(m_barLength - (m_barLength * fillAmount), 0, 0);
            Tween.LocalPosition(m_topSpeedBar, barPos, m_barChangeDuration, 0.05f, m_barChangeTween);
            Tween.LocalPosition(m_topSpeedBarRed1, barPos, m_barChangeDuration, 0, m_barChangeTween);
            Tween.LocalPosition(m_topSpeedBarRed2, barPos, m_barChangeDuration, 0.1f, m_barChangeTween);
        }

        public void AccelerationBarFill(float fillAmount)
        {
            Vector3 barPos = new Vector3(m_barLength - (m_barLength * fillAmount), 0, 0);
            Tween.LocalPosition(m_accelerationBar, barPos, m_barChangeDuration, 0.05f, m_barChangeTween);
            Tween.LocalPosition(m_accelerationBarRed1, barPos, m_barChangeDuration, 0, m_barChangeTween);
            Tween.LocalPosition(m_accelerationBarRed2, barPos, m_barChangeDuration, 0.1f, m_barChangeTween);
        }

        public void HandlingBarFill(float fillAmount)
        {
            Vector3 barPos = new Vector3(m_barLength - (m_barLength * fillAmount), 0, 0);
            Tween.LocalPosition(m_handlingBar, barPos, m_barChangeDuration, 0.05f, m_barChangeTween);
            Tween.LocalPosition(m_handlingBarRed1, barPos, m_barChangeDuration, 0, m_barChangeTween);
            Tween.LocalPosition(m_handlingBarRed2, barPos, m_barChangeDuration, 0.1f, m_barChangeTween);
        }
    }
}
