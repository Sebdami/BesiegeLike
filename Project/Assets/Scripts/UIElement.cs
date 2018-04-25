using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class UIElement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    public static bool DisableDisabling = false;
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(!DisableDisabling)
            GameManager.instance.DisableBuildingControls = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!DisableDisabling)
            GameManager.instance.DisableBuildingControls = false;
    }

}
