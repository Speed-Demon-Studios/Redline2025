using MenuManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundPanelsController : MonoBehaviour
{
    [SerializeField] List<GameObject> m_pressAToStart, m_title, m_selectClass, m_selectShip, m_options, m_credits, m_allMenus;
    [SerializeField] GameObject m_1PlayerShipSelect, m_2PlayerShipSelect, m_3PlayerShipSelect, m_4PlayerShipSelect;

    public void ChangeShipSelectBGPanels(int numberOfPlayers)
    {
        m_1PlayerShipSelect.SetActive(false);
        m_2PlayerShipSelect.SetActive(false);
        m_3PlayerShipSelect.SetActive(false);
        m_4PlayerShipSelect.SetActive(false);

        switch (numberOfPlayers)
        {
            case 1:
                m_1PlayerShipSelect.SetActive(true);
                break;

            case 2:
                m_2PlayerShipSelect.SetActive(true);
                break;

            case 3:
                m_3PlayerShipSelect.SetActive(true);
                break;

            case 4:
                m_4PlayerShipSelect.SetActive(true);
                break;
        }
    }

    public void ChangeBGPanels()
    {
        MenuType currentMenu = GameManager.gManager.uiCInput.GetMenuManager().GetCurrentType();
        List<GameObject> currentBGPanels = new();

        switch (currentMenu)
        {
            case MenuType.PlayerOneJoin:
                currentBGPanels = m_pressAToStart;
                break;

            case MenuType.Main:
                currentBGPanels = m_title;
                break;

            case MenuType.Difficulty:
                currentBGPanels = m_selectClass;
                break;

            case MenuType.ShipSelectionReady:
                currentBGPanels = m_selectShip;
                break;

            case MenuType.Option:
                currentBGPanels = m_options;
                break;

            case MenuType.Credits:
                currentBGPanels = m_credits;
                break;
        }

        foreach (GameObject obj in m_allMenus)
        {
            obj.gameObject.SetActive(false);
        }

        foreach (GameObject obj in currentBGPanels)
        {
            obj.SetActive(true);
        }
    }

    

}
