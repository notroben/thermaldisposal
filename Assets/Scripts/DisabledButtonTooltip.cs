using UnityEngine;
using UnityEngine.EventSystems;

public class DisabledButtonTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Tooltip")]
    public GameObject tooltipObject;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tooltipObject != null) tooltipObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (tooltipObject != null) tooltipObject.SetActive(false);
    }
}
