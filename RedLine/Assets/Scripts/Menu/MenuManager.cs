using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using MenuManagement;
using System;
using DifficultyButtonSwitch;

namespace MenuManagement
{
    public enum MenuType
    {
        PlayerOneJoin,
        PlayersJoined,
        Main,
        Option,
        Credits,
        Difficulty,
        ShipSelectionUnready,
        ShipSelectionReady
    }
    public class MenuManager : MonoBehaviour
    {
        private bool m_gameLoadedAndStarted;
        private MenuType m_currentMenuType;

        public SetMenu start;
        public SetMenu mainMenu;
        private SetMenu m_currentMenu;

        public ButtonSelectManager bSelect;

        public void SetGameLoaded(bool change) { m_gameLoadedAndStarted = change; }
        public MenuType GetCurrentType() { return m_currentMenuType; }
        public SetMenu GetCurrentMenu() { return m_currentMenu; }

        // Start is called before the first frame update
        public void Inistialize()
        {
            m_currentMenu = start;
            if (GameManager.gManager.firstLoadIntoGame)
            {
                m_currentMenuType = start.typeOfMenu;
                foreach(GameObject player in GameManager.gManager.players)
                {
                    player.GetComponent<PlayerInputScript>().Inistialize();
                }
                m_gameLoadedAndStarted = false;
                PressStart();
            }
        }

        public void PressStart()
        {
            if (!m_gameLoadedAndStarted)
            {
                SwitchMenu(mainMenu);
                GameManager.gManager.uiCInput.ResetFirstButton(0, mainMenu.menuStartButtons[0]);
                bSelect.TransitionToTitle(true);
                m_gameLoadedAndStarted = true;
            }
        }

        public void BackOutMenu(int playerNumber)
        {
            if(m_currentMenuType != MenuType.ShipSelectionReady)
                m_currentMenu.OnBackButton();
            else
            {
                GameManager.gManager.uiCInput.UnReadyPlayer(playerNumber);
            }

        }

        public void SwitchMenu(SetMenu switchingTo)
        {
            GameObject switchingToGameObject = switchingTo.gameObject;

            foreach(GameObject panel in m_currentMenu.backGroundPanel)
            {
                panel.SetActive(false);
            }

            m_currentMenu.gameObject.SetActive(false);
            switchingToGameObject.SetActive(true);

            m_currentMenu = switchingTo;

            m_currentMenuType = switchingTo.typeOfMenu;

            if (m_currentMenuType != MenuType.ShipSelectionReady)
            {
                foreach (GameObject panel in m_currentMenu.backGroundPanel)
                {
                    panel.SetActive(true);
                }
            }


            SetButtons(switchingTo);
        }

        public void BackGroundPanelForSelection()
        {
            foreach (GameObject panel in m_currentMenu.backGroundPanel)
            {
                panel.SetActive(false);
            }

            int index = 0;
            int numberOfPlayers = GameManager.gManager.players.Count - 1;
            foreach (GameObject panel in m_currentMenu.backGroundPanel)
            {
                if(index == numberOfPlayers)
                {
                    panel.SetActive(true);
                }
                else if(index > 3)
                {
                    panel.SetActive(true);
                }

                index++;
            }
        }

        public void SetButtons(SetMenu menu)
        {
            int index = 0;
            foreach (GameObject startButton in menu.menuStartButtons)
            {
                GameManager.gManager.uiCInput.ResetFirstButton(index, startButton);
                index++;
            }
        }

        public void Back()
        {
            SwitchMenu(m_currentMenu.prevMenu);
        }
    }
}
