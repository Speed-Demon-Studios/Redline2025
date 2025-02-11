using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.ComponentModel.Design;

public class PlayerUiControl : MonoBehaviour
{
    [SerializeField] private RacerDetails rDetails;
    [SerializeField] private ShipsControls m_shipsControls;

    public Animator finishAnim;
    public Animator newLapAnim;
    public HUD hud;

    private void Update()
    {
        if (GameManager.gManager && !m_shipsControls.isTestShip)                                                                      
        {
            if (GameManager.gManager.raceStarted == true && GameManager.gManager.raceFinished == false)
            {
                float speed = ((int)m_shipsControls.RB.velocity.magnitude) * 7f;
                int pos = 0;
                if (GameManager.gManager.indexListSorted == true) // if the list of racers is sorted
                {
                    for (int i = 0; i < GameManager.gManager.pHandler.racers.Count; i++) // go through all racers and display the position 
                    {
                        if (GameManager.gManager.pHandler.racers[i] == rDetails) // if i is equal to this current object then display the position
                        {
                            pos = i + 1;
                        }
                    }
                }
                int currentLap = rDetails.currentLap;
                int totalLaps = GameManager.gManager.rManager.GetTotalLaps();
                bool isInRedline = m_shipsControls.ReturnIsInRedline;
                float energyfillValue = m_shipsControls.ReturnBoost / 3;
                float speedFillValue = m_shipsControls.RB.velocity.magnitude / m_shipsControls.GetDefaultMaxSpeed;
                hud.SetValues(speed, pos, currentLap, totalLaps, isInRedline, energyfillValue, speedFillValue, m_shipsControls.ReturnBoostLevel,
                    rDetails.currentLapTimeMINUTES, rDetails.currentLapTimeSECONDS, rDetails.quickestLapTimeMINUTES, rDetails.quickestLapTimeSECONDS, GameManager.gManager.allRacers.Count);
                hud.UpdateHUD();
            }
        }
    }

    public void FinishPopUp()
    {
        finishAnim.gameObject.SetActive(true);
        finishAnim.SetTrigger("Finished");
        StartCoroutine(WaitToHideFinish());
    }

    IEnumerator WaitToHideFinish()
    {
        yield return new WaitForSeconds(2f);
        finishAnim.gameObject.SetActive(false);
    }
}
