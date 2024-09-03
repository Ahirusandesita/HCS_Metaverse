using UnityEngine;
using UnityEngine.EventSystems;

public class DragSystem : MonoBehaviour,IPointerDownHandler,IPointerUpHandler,IDragHandler
{
    [SerializeField]
    private float sensitivity = 1f;

    /// <summary>
    /// スクロールする対象
    /// </summary>
    private IScrollable[] scrollables;

    /// <summary>
    /// 最後の画面接触点
    /// </summary>
    private Vector2 LastPointerPosition;
    /// <summary>
    /// スクロールできるかどうか
    /// </summary>
    private bool canScroll = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        OnPinterDown((PointerEventData)eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        OnPointerUp();
    }

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

        scrollables = this.transform.GetComponentsInChildren<IScrollable>(true);
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
}
