using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICatchUpManager : MonoBehaviour
{
    List<GameObject> m_playerOBJS = new();
    public float multiplier;
    private float m_topSpeedCap;
    float m_catchUpChange;

    public void Inistialize()
    {
        m_topSpeedCap = GameManager.gManager.catchUpTopCap;
        multiplier = GameManager.gManager.catchUpMultiplier;
        m_playerOBJS = GameManager.gManager.players;
    }

    private void Update()
    {
        if (GameManager.gManager.raceStarted && !GameManager.gManager.raceFinished)
        {
            float score = 1f;
            foreach (GameObject AIOBJ in GameManager.gManager.racerObjects)
            {
                if(AIOBJ.GetComponent<RacerDetails>().finishedRacing || AIOBJ.GetComponent<RacerDetails>().currentLap != 0)
                    if (ChangeCatchUp(AIOBJ) > 0)
                        score *= ChangeCatchUp(AIOBJ);
            }

            float originalScore = score;
            float modFactor = m_topSpeedCap - (m_topSpeedCap / GameManager.gManager.racerObjects.Count);
            float makeupValue = (m_topSpeedCap - originalScore) * modFactor;
            float percentage = originalScore + (makeupValue * originalScore);

            m_catchUpChange = percentage;

            m_catchUpChange = Mathf.Clamp(m_catchUpChange, 0.5f, m_topSpeedCap);

            foreach (GameObject AIOBJ in GameManager.gManager.racerObjects)
            {
                AIOBJ.GetComponent<ShipsControls>().MaxSpeedCatchupChange(percentage);
            }
        }
    }

    float ChangeCatchUp(GameObject ai)
    {
        RacerDetails aIRacerDets = ai.GetComponent<RacerDetails>();
        float numberOfCheckPoints = GameManager.gManager.checkpointParent.GetNumberOfChildren();
        float score = 0f;

        foreach(GameObject playerObj in m_playerOBJS)
        {
            RacerDetails playerRacerDets = playerObj.GetComponent<RacerDetails>();

            float playerCheckpoint = playerRacerDets.currentCheckpoint;
            float playerLap = playerRacerDets.currentLap;

            float aiCheckpoint = aIRacerDets.currentCheckpoint;
            float aiLap = aIRacerDets.currentLap;

            float playerCheckPointPercentage = (playerCheckpoint / numberOfCheckPoints) + playerLap;
            float aiCheckPointPercentage = (aiCheckpoint / numberOfCheckPoints) + aiLap;

            if (playerCheckPointPercentage < 1)
                playerCheckPointPercentage += 1;
            if (aiCheckPointPercentage < 1)
                aiCheckPointPercentage += 1;

            float difference = aiCheckPointPercentage - playerCheckPointPercentage;
            if (difference > 0)
            {
                float reversDiff = 1 - difference;
                score += reversDiff * multiplier;
            }
        }

        float originalScore = score;
        float modFactor = 1 - (1 / m_playerOBJS.Count);
        float makeupValue = (1 - originalScore) * modFactor;
        float percentage = originalScore + (makeupValue * originalScore);

        return percentage;
    }
}
