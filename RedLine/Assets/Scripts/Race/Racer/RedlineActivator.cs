using UnityEngine;

public class RedlineActivator : MonoBehaviour
{
    public void ActivateRedline()
    {
        foreach (GameObject racerOBJ in GameManager.gManager.allRacers)
        {
            RedlineColliderSpawner redlineScript = racerOBJ.GetComponent<RacerDetails>().rCS;

            if (redlineScript != null)
            {
                if (redlineScript.enabled == false)
                {
                    redlineScript.enabled = true;
                }
            }
        }
        GameManager.gManager.redlineActivated = true;
    }

    public void DeactivateRedline()
    {
        foreach (GameObject racerOBJ in GameManager.gManager.allRacers)
        {
            RedlineColliderSpawner redlineScript = racerOBJ.GetComponentInChildren<RedlineColliderSpawner>();
            redlineScript.enabled = false;
        }
        GameManager.gManager.redlineActivated = false;
    }
}
