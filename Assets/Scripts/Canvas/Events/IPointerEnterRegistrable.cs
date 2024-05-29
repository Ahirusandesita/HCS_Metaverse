using UnityEngine.EventSystems;

public interface IPointerEnterRegistrable
{
    void OnPointerEnter(PointerEventData data);
}
public interface IPointerDownRegistrable
{
    void OnPointerDown(PointerEventData data);
}
public interface IPointerUpRegistrable
{
    void OnPointerUp(PointerEventData data);
}
public interface IPointerExitRegistrable
{
    void OnPointerExit(PointerEventData data);
}
public interface IPointerClickRegistrable
{
    void OnPointerClick(PointerEventData data);
}
public interface IDragRegistrable
{
    void OnDrag(PointerEventData data);
}
public interface IDragRegistrableToParent
{
    void OnParentDrag(PointerEventData data);
}
