using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MenuManagement;
using UnityEngine.UI;
using UnityEngine.Events;
using Cinemachine;
using DifficultyButtonSwitch;

public class SetMenu : MonoBehaviour
{
    public MenuType typeOfMenu;
    public List<GameObject> menuStartButtons;
    public List<GameObject> backGroundPanel;
    public SetMenu prevMenu;
    public UnityEvent back;

    public void OnBackButton()
    {
        back.Invoke();
    }
}
