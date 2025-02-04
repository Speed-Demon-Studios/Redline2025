using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionHandler : MonoBehaviour
{
    public bool racersAdded = false;
    public List<RacerDetails> racers = new List<RacerDetails>();
    public IList<RacerDetails> racerFinder = new List<RacerDetails>();
    private bool racersSorted = false;

    public List<GameObject> aiRacePrefabs = new();
    public Nodes startNode;

    private void Awake()
    {
        GameManager.gManager.pHandler = this;
        racerFinder = new List<RacerDetails>();
        racers = new List<RacerDetails>();
        racersAdded = false;
        racersSorted = false;
    }

    public void OnRaceLoaded()
    {
        if (!GameManager.gManager.isTimeTrial)
        {
            for (int i = 0; i < 9; i++)
            {
                int index = Random.Range(0, aiRacePrefabs.Count);

                GameObject aiShip = Instantiate(aiRacePrefabs[index]);

                aiShip.GetComponent<AIMoveInputs>().desiredNode = startNode;
                aiShip.GetComponent<ShipsControls>().Initialize(true);
                aiShip.GetComponent<InitializeBeforeRace>().Initialize();
                racers.Add(aiShip.GetComponent<RacerDetails>());

                if (aiShip.GetComponent<RacerDetails>() != null)
                    aiShip.GetComponent<RacerDetails>().rCS.CallSpawnCollider();

                GameManager.gManager.racerObjects.Add(aiShip);
            }
        }

        foreach(GameObject players in GameManager.gManager.players)
        {
            racers.Add(players.GetComponent<RacerDetails>());
        }

        racersAdded = true;
        GameManager.gManager.racersAdded = true;
    }

    public IEnumerator SortRacers()
    {
        //foreach (RacerDetails rD in racers)
        //{
        //    rD.placement = racers.IndexOf(rD) + 1;
        //}

        for (int i = 0; i < racers.Count; i++)
        {
            racers[i].placement = (i + 1);
        }
        yield return null;
        StopCoroutine(SortRacers());
    }

    // Update is called once per frame
    void Update()
    {
        if (racersAdded == true)
        {
            GameManager.gManager.indexListSorted = false;
            racers.Sort((r1, r2) =>
            {
                racersSorted = false;
                if (r1.currentCheckpoint != r2.currentCheckpoint)
                {
                    return r1.currentCheckpoint;
                }
            
                if (r1.currentLap != r2.currentLap)
                {
                    return r1.currentCheckpoint;
                }
            
                if (r1.finishedRacing)
                {
                    return r1.currentCheckpoint;
                }
            
                if (r2.finishedRacing)
                {
                    return r2.currentCheckpoint;
                }
            
                StartCoroutine(SortRacers());
                racersSorted = true;
                return r1.NextCheckpointDistance().CompareTo(r2.NextCheckpointDistance());
            });
            GameManager.gManager.indexListSorted = true;
                
        }
    }
}
