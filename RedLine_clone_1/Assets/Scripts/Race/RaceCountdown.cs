using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RaceCountdown : MonoBehaviour
{
    [SerializeField] private int countdownLength = 2;
    private TextMeshProUGUI m_countdownText;

    //public bool m_readyForCountdown = false;
    public bool m_countdownCoroutineStarted = false;
    public bool m_countdownFinished = false;
    public bool m_countdownStarted = false;
    private bool m_countDownStarted;
    public Animator countDownAnim;

    public void SetCountdownTextObj(TextMeshProUGUI textOBJ)
    {
        m_countdownText = textOBJ;
        return;
    }

    public TextMeshProUGUI GetCountdownTextOBJ()
    {
        return m_countdownText;
    }

    public void Inistialize()
    {
        GameManager.gManager.raceCountdown = this;
        GameManager.gManager.raceStarted = false;
        GameManager.gManager.countdownIndex = countdownLength;
        m_countdownFinished = false;
        m_countdownStarted = false;
        m_countdownCoroutineStarted = false;
    }

    public IEnumerator RaceCountdownTimer()
    {
        // Tell the rest of the script that the coroutine has started.
        m_countdownCoroutineStarted = true;
        if (m_countdownStarted == false)
        {
            m_countdownStarted = true;
        }
        //if (m_countdownText.enabled == false)
        //{
        //    m_countdownText.enabled = true;
        //}

        //m_countdownText.text = GameManager.gManager.countdownIndex.ToString();

        if (GameManager.gManager.countdownIndex > 0)
        {
            GameManager.gManager.countdownIndex -= 1;
            Debug.Log("Countdown: " + GameManager.gManager.countdownIndex);
        }
        else if (GameManager.gManager.countdownIndex <= 0)
        {
            m_countdownFinished = true;
            m_countdownCoroutineStarted = false;
            //m_countdownText.enabled = false;
            GameManager.gManager.rManager.StartRace();
            StopCoroutine(RaceCountdownTimer());
        }
        yield return new WaitForSecondsRealtime(1);

        //m_countdownText.text = GameManager.gManager.countdownIndex.ToString();
        m_countdownCoroutineStarted = false;
        StopCoroutine(RaceCountdownTimer());
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.gManager.racersPlaced == true && GameManager.gManager.readyForCountdown == true && GameManager.gManager.startCamerasFinished == true && m_countdownFinished == false && m_countdownCoroutineStarted == false)
        {
            GameManager.gManager.raceStarted = false;
            if (!m_countDownStarted)
            {
                m_countDownStarted = true;
                countDownAnim.SetTrigger("CountDown");
            }
            StartCoroutine(RaceCountdownTimer());
        }
    }
}
