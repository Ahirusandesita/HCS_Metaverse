using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragSystem : MonoBehaviour,IBeginDragHandler,IEndDragHandler,IDragHandler
{
    [SerializeField]
    private float sensitivity = 1f;

    /// <summary>
    /// スクロールする対象
    /// </summary>
    private List<IScrollable> scrollables = new List<IScrollable>();
    [SerializeField, InterfaceType(typeof(IScrollable))]
    private List<UnityEngine.Object> IScrollables = new List<Object>();
    private List<IScrollable> IScrollableList => IScrollables.OfType<IScrollable>().ToList();
    /// <summary>
    /// 最後の画面接触点
    /// </summary>
    private Vector2 LastPointerPosition;
    /// <summary>
    /// スクロールできるかどうか
    /// </summary>
    private bool canScroll = false;


    private void Awake()
    {
        //EventTriggerに対応する関数を登録する///////////////////////////////////////////////////////
        //EventTrigger trigger = GetComponent<EventTrigger>();
        //EventTrigger.Entry entryDrag = new EventTrigger.Entry();
        //entryDrag.eventID = EventTriggerType.Drag;
        //entryDrag.callback.AddListener((data) => { OnDrag((PointerEventData)data); });

        //EventTrigger.Entry entryPointerDown = new EventTrigger.Entry();
        //entryPointerDown.eventID = EventTriggerType.PointerDown;
        //entryPointerDown.callback.AddListener((data) => OnPinterDown((PointerEventData)data));

        //EventTrigger.Entry entryPointerUp = new EventTrigger.Entry();
        //entryPointerUp.eventID = EventTriggerType.PointerUp;
        //entryPointerUp.callback.AddListener((x) => OnPointerUp());

        //trigger.triggers.Add(entryDrag);
        //trigger.triggers.Add(entryPointerDown);
        //trigger.triggers.Add(entryPointerUp);
        /////////////////////////////////////////////////////////////////////////////////////////////

        scrollables = new List<IScrollable>(this.transform.GetComponentsInChildren<IScrollable>(true));
        foreach(IScrollable scrollable in IScrollableList)
        {
            scrollables.Add(scrollable);
        }
    }
    public void ScrollableInject(List<IScrollable> scrollables)
    {
        foreach(IScrollable scrollable in scrollables)
        {
            this.scrollables.Add(scrollable);
        }
    }
    public void ScrollableInject(IScrollable scrollable)
    {
        scrollables.Add(scrollable);
    }

    /// <summary>
    /// ドラッグされたとき
    /// </summary>
    /// <param name="data"></param>
    private void OnDrag(PointerEventData data)
    {
        //スクロールできない状態なら終了する
        if (!canScroll)
        {
            return;
        }

        //前回の接触点から現在の接触点を引いて接触点の移動量を求める
        Vector3 scrollMove = LastPointerPosition - data.position;

        if(scrollMove.x > 150f || scrollMove.y > 150f)
        {
            scrollMove = Vector3.zero;
        }

        //スクロールする対象に移動量を渡してスクロールさせる
        foreach (IScrollable scrollable in scrollables)
        {
            scrollable.Scroll(scrollMove, sensitivity);
        }

        //前回の接触点を更新する
        LastPointerPosition = data.position;
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        OnDrag((PointerEventData)eventData);
    }

    /// <summary>
    /// 押されたとき
    /// </summary>
    /// <param name="data"></param>
    private void OnPinterDown(PointerEventData data)
    {
        //前回の接触点に設定して、スクロール開始できるようにする
        LastPointerPosition = data.position;
        canScroll = true;
    }

    /// <summary>
    /// 離されたとき
    /// </summary>
    private void OnPointerUp()
    {
        //スクロールできなくする
        canScroll = false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        OnPointerUp();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        OnPinterDown((PointerEventData)eventData);
    }
}
