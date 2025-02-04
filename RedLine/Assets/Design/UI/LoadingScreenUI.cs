using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreenUI : MonoBehaviour
{
    [SerializeField] private Animator m_UIAnimator;
    [SerializeField] private Image m_loadingBarFill;
    [SerializeField] private float m_loadingBarFillAmount;
    [SerializeField] private Material m_cursorMat;
    [SerializeField] private float m_cursorPulseDuration;
    [SerializeField] private AnimationCurve m_cursorPulseCurve;

    [Header("Testing")]
    [SerializeField] private AnimationCurve m_loadProgress;
    [SerializeField] private float m_loadDuration;





    private void OnEnable()
    {
        m_loadingBarFill.fillAmount = 0;
        m_UIAnimator.SetTrigger("TitleIn");

        m_cursorMat.color = Color.white;
        Tween.Color(m_cursorMat, Color.red, m_cursorPulseDuration, 0, m_cursorPulseCurve, Tween.LoopType.Loop);

        // TESTING ONLY
        Tween.Value(0f, 1f, FillLoadBar, m_loadDuration, 1f, m_loadProgress);
        //
    }

    private void FillLoadBar(float progress)
    {
        m_loadingBarFill.fillAmount = progress;
    }
}
