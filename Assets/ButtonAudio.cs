using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonAudio : MonoBehaviour, ISelectHandler, ISubmitHandler, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    private MenuAudio menuAudio;

    void Awake () {
        menuAudio = GetComponentInParent<MenuAudio>();
    }
    
    public void OnSelect(BaseEventData data)
    {
        menuAudio.HoverSound();
    }

    public void OnSubmit(BaseEventData data)
    {
        menuAudio.ClickSound();
    }

    public void OnPointerDown(PointerEventData eventData){}
    public void OnPointerUp(PointerEventData eventData){}
    public void OnPointerClick(PointerEventData eventData)
    {
        menuAudio.ClickSound();
    }
}
