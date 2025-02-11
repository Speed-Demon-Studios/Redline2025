using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DifficultyButtonSwitch;
using UnityEngine.Events;

public class ButtonOnSelected : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public UnityEvent onSelectEvent;
    public UnityEvent onDeSelectEvent;

    public void OnDeselect(BaseEventData eventData)
    {
        onDeSelectEvent.Invoke();
    }

    public void OnSelect(BaseEventData eventData)
    {
        onSelectEvent.Invoke();
    }

}
