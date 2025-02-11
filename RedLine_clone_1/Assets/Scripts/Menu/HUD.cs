using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [SerializeField]
    private Image energyBarFill, energyLeadingEdgeLower, energyLeadingEdgeUpper, speedBarFill, speedbarRedFill;
    [SerializeField]
    private GameObject energyFullSegment1, energyFullSegment2, energyFullSegment3;
    [SerializeField]
    private TextMeshProUGUI m_speedText, m_posText, m_lapText, m_lapTimeText, m_bestLapTime;
    [SerializeField]
    [Range(0f, 1f)]
    private float m_energyBarFillAmount, m_speedBarFillAmount;

    [SerializeField]
    private float m_leadingEdgeWidth;

    [SerializeField]
    private bool m_gainingRedline;

    [Range(0, 3)]
    private int m_currentBoostLevel;

    [SerializeField]
    private AnimationCurve energySegmentPulse;
    [SerializeField]
    private float pulseLength;

    private Color flickeringColor = Color.white, pulsingColor = new Color(0,0,0,0.5f), fullSegment1Color, fullSegment2Color, fullSegment3Color;

    [SerializeField]
    public Vector2 energyBarFillRange, energyLeadingEdgeFillRange, speedBarFillRange;

    private int m_position, m_totalPositions, m_lap, m_totalLaps;
    private float m_kph;

    private float m_currentLapTimeMiuntes;
    private float m_currentLapTimeSeconds;
    private float m_bestLapMiuntes;
    private float m_bestLapSeconds;


    private void OnEnable()
    {
        fullSegment1Color = energyFullSegment1.GetComponent<Image>().color;
        fullSegment2Color = energyFullSegment2.GetComponent<Image>().color;
        fullSegment3Color = energyFullSegment3.GetComponent<Image>().color;
        Tween.Value(new Color(0, 0, 0, 0), new Color(0, 0, 0, 1), PulsingColor, pulseLength, 0, energySegmentPulse, Tween.LoopType.Loop);
    }

    void PulsingColor(Color color)
    {
        pulsingColor = color;
    }

    // Re-maps one value range to another
    public float map(float s, float a1, float a2, float b1, float b2)
    {
        return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
    }

    // Updates the HUD display with current values
    public void UpdateHUD()
    {
        // Energy bar
        float energyFill = map(m_energyBarFillAmount, 0, 1, energyBarFillRange.x, energyBarFillRange.y);
        energyBarFill.fillAmount = energyFill;

        // Energy bar full segments

        Color currentBoostColor = Color.white;

        switch (m_currentBoostLevel)
        {
            case 0:
                energyFullSegment1.SetActive(false);
                energyFullSegment2.SetActive(false);
                energyFullSegment3.SetActive(false);
                break;

            case 1:
                energyFullSegment1.SetActive(true);
                energyFullSegment2.SetActive(false);
                energyFullSegment3.SetActive(false);

                energyFullSegment1.GetComponent<Image>().color = fullSegment1Color - pulsingColor; 
                break;

            case 2:
                energyFullSegment1.SetActive(true);
                energyFullSegment2.SetActive(true);
                energyFullSegment3.SetActive(false);

                energyFullSegment1.GetComponent<Image>().color = fullSegment2Color - pulsingColor;
                energyFullSegment2.GetComponent<Image>().color = fullSegment2Color - pulsingColor;
                break;

            case 3:
                energyFullSegment1.SetActive(true);
                energyFullSegment2.SetActive(true);
                energyFullSegment3.SetActive(true);

                energyFullSegment1.GetComponent<Image>().color = fullSegment3Color - pulsingColor;
                energyFullSegment2.GetComponent<Image>().color = fullSegment3Color - pulsingColor;
                energyFullSegment3.GetComponent<Image>().color = fullSegment3Color - pulsingColor;
                break;

            default:
                energyFullSegment1.SetActive(false);
                energyFullSegment2.SetActive(false);
                energyFullSegment3.SetActive(false);
                break;

        }

        

        // Glowing leading edge of energy bar
        float intensity = Random.Range(0.5f, 1f);
        flickeringColor.a = intensity;

        if (m_gainingRedline)
        {
            float energyLELowerFill = map(m_energyBarFillAmount - m_leadingEdgeWidth, 0, 1, energyLeadingEdgeFillRange.x, energyLeadingEdgeFillRange.y);
            float energyLEUpperFill = map(m_energyBarFillAmount, 0, 1, energyBarFillRange.x, energyBarFillRange.y);
            energyLeadingEdgeLower.fillAmount = energyLELowerFill;
            energyLeadingEdgeUpper.fillAmount = energyLEUpperFill;

            energyLeadingEdgeUpper.color = flickeringColor;
        }
        else
        {
            float energyLELowerFill = 0;
            float energyLEUpperFill = 0;
            energyLeadingEdgeLower.fillAmount = energyLELowerFill;
            energyLeadingEdgeUpper.fillAmount = energyLEUpperFill;
        }

        // Speed bar
        float speedFill = map(m_speedBarFillAmount, 0, 1, speedBarFillRange.x, speedBarFillRange.y);
        speedBarFill.fillAmount = speedFill;
        speedbarRedFill.color = flickeringColor;

        m_speedText.text = "<b>" + m_kph.ToString() + "</b> Kph";

        m_posText.text = m_position.ToString() + " / " + m_totalPositions.ToString();

        m_lapText.text = m_lap.ToString() + " / " + m_totalLaps.ToString();

        m_lapTimeText.text = string.Format("{0:00}", m_currentLapTimeMiuntes) + ":" + string.Format("{0:00.00}", m_currentLapTimeSeconds);
        m_bestLapTime.text = string.Format("{0:00}", m_bestLapMiuntes) + ":" + string.Format("{0:00.00}", m_bestLapSeconds);
    }

    public void SetValues(float speed, int pos, int laps, int totalLaps, bool isInRedline, float energyFillAmount,
        float speedFillAmount, int currentBoostLevel, float currentLapTimeMiuntes, float currentLapTimeSeconds,
        float bestLapTimeMinutes, float bestLapTimeSeconds, int totalPos)
    {
        m_kph = speed;
        m_position = pos;
        m_lap = laps;
        m_totalLaps = totalLaps;
        m_gainingRedline = isInRedline;
        m_energyBarFillAmount = energyFillAmount;
        m_speedBarFillAmount = speedFillAmount;
        m_currentBoostLevel = currentBoostLevel;
        m_currentLapTimeMiuntes = currentLapTimeMiuntes;
        m_currentLapTimeSeconds = currentLapTimeSeconds;
        m_bestLapMiuntes = bestLapTimeMinutes;
        m_bestLapSeconds = bestLapTimeSeconds;
        m_totalPositions = totalPos;
    }
}
