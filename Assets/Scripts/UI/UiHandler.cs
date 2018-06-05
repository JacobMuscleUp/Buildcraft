using UnityEngine;
using UnityEngine.EventSystems;

public class UiHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IEndDragHandler
{
    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        InputSignals.DoUiEnter();
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        InputSignals.DoUiExit();
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        InputSignals.DoUiDragBegin();
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        InputSignals.DoUiDragEnd();
    }
}
