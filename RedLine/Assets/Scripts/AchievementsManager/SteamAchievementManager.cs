using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteamAchievementManager : MonoBehaviour
{
    public void SetRichPresence(int index)
    {
        string pchStatus = "";

        switch (index)
        {
            case 1:
                {
                    pchStatus = "#Status_InRace";
                    break;
                }
            case 0:
                {
                    pchStatus = "#Status_AtMainMenu";
                    break;
                }
        }

        Steamworks.SteamFriends.ClearRichPresence();
        Steamworks.SteamFriends.SetRichPresence("steam_display", pchStatus);

    }

    private void Awake()
    {
        Steamworks.SteamAPI.Init();
        
        if (SteamAPI.RestartAppIfNecessary((AppId_t)3204410))
        {
            Debug.Log("[Steamworks.NET] Shutting down because RestartAppIfNecessary returned true. Steam will restart the application.");
        
            Application.Quit();
            return;
        }
    }


    public void GetAchievement(string AchievementID)
    {
        bool gotAchievement;
        
        Steamworks.SteamUserStats.GetAchievement(AchievementID, out gotAchievement);
        
        if (gotAchievement == false)
        {
            Steamworks.SteamUserStats.SetAchievement(AchievementID);
            Steamworks.SteamUserStats.StoreStats();
        }
        
        return;
    }

    public void CheckAllAchievementProgress(int p1Placement)
    {
        RacerDetails rDeets = GameManager.gManager.playerShips[0].GetComponent<RacerDetails>();
        bool gotAchievement;

        if (p1Placement >= 0)
        {
            // Checking for WIN related achievements
            if (p1Placement == 1)
            {
                int currentWins;
                Steamworks.SteamUserStats.GetStat("RaceWins", out currentWins);
                currentWins++;
                Debug.Log("current wins: " + currentWins);
                Steamworks.SteamUserStats.SetStat("RaceWins", currentWins);
                Steamworks.SteamUserStats.StoreStats();
        
                // "New Rider" - Win 1 race.
                Steamworks.SteamUserStats.GetAchievement("NewRider", out gotAchievement);
                if (currentWins == 1)
                {
                    Steamworks.SteamUserStats.SetAchievement("NewRider");
                }
                Steamworks.SteamUserStats.StoreStats();
        
                // "Redline Cadet" - Win 5 races.
                Steamworks.SteamUserStats.GetAchievement("RedlineCadet", out gotAchievement);
                if (currentWins == 5)
                {
                    Steamworks.SteamUserStats.SetAchievement("RedlineCadet");
                }
                Steamworks.SteamUserStats.StoreStats();
            }

            int completedRaces;
            Steamworks.SteamUserStats.GetStat("RacesCompleted", out completedRaces);
            Debug.Log("Completed Races: " + completedRaces);
            completedRaces++;
            Debug.Log("Added +1 To Completed Races. New Value: " + completedRaces);
            Steamworks.SteamUserStats.SetStat("RacesCompleted", completedRaces);
            Steamworks.SteamUserStats.StoreStats();

            // "First Flight" - Complete 1 race.
            Steamworks.SteamUserStats.GetAchievement("FirstFlight", out gotAchievement);
            if (completedRaces == 1)
            {
                Steamworks.SteamUserStats.SetAchievement("FirstFlight");
            }
            Steamworks.SteamUserStats.StoreStats();
        }
        return;
    }
}
