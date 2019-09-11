using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MiniMapClicked : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{

    public void OnDrag(PointerEventData eventData)
    {
        
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Player.player.ShowFinishDirection();
    }
}
