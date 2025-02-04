using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NameRandomiser : MonoBehaviour
{
    public string[] nameList;
    public List<string> usedNames = new List<string>();
    private bool assigningNames = false;

    private void Awake()
    {
        assigningNames = false;
        usedNames = new List<string>();
        GameManager.gManager.nRandomiser = this;
    }

    public void AssignRacerNames()
    {
        assigningNames = true;
        usedNames = new List<string>();
        bool isPlayer = false;
        foreach (GameObject racerOBJ in GameManager.gManager.racerObjects)
        {
            isPlayer = false;

            foreach (GameObject playerOBJ in GameManager.gManager.players)
            {
                if (racerOBJ == playerOBJ)
                {
                    isPlayer = true;
                }
            }

            if (isPlayer == false)
            {
                DecideName(racerOBJ);
            }
        }

        assigningNames = false;
        GameManager.gManager.namesAssigned = true;
    }

    void DecideName(GameObject racerOBJ)
    {
        RacerDetails rDeets = racerOBJ.GetComponent<RacerDetails>();

        int nameIndex = (int)Random.Range(0, nameList.Count() - 1);
        string assignedName = nameList[nameIndex];

        if (CheckName(assignedName) == false)
        {
            rDeets.RacerName = assignedName;
            usedNames.Add(assignedName);
        }
        else if (CheckName(assignedName) == true)
        {
            DecideName(racerOBJ);
        }
        return;
    }

    bool CheckName(string assignedName)
    {
        bool nameUsed = false;
        foreach (string name in usedNames)
        {
            if (assignedName == name)
            {
                nameUsed = true;
            }
        }

        if (nameUsed == false)
        {
            return false;
        }
        else if (nameUsed == true)
        {
            return true;
        }

        return default;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //if (GameManager.gManager.CurrentScene == "Race" && GameManager.gManager.racersAdded == true && GameManager.gManager.raceStarted == false && assigningNames == false && GameManager.gManager.namesAssigned == false)
        //{
        //    GameManager.gManager.nRandomiser.AssignRacerNames();
        //}
    }
}
