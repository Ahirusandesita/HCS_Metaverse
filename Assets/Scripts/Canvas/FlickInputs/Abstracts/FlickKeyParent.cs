using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// フリックキーの親　
/// </summary>
public abstract class FlickKeyParent : MonoBehaviour
{
    [SerializeField]
    private EventTrigger eventTrigger;

    protected FlickKeyboardManager flickManager;
    
    /// <summary>
    /// KeyのImage
    /// </summary>
    private Image keyImage;
    /// <summary>
    /// Keyの初期カラー
    /// </summary>
    private Color initialKeyColor;
    /// <summary>
    /// Keyの初期サイズ
    /// </summary>
    private Vector3 initialKeySize;
    /// <summary>
    /// Keyが押されたときのKeySize
    /// </summary>
    private Vector3 pushKeySize;
    /// <summary>
    /// Keyが押されたときにX軸を大きくする数値
    /// </summary>
    protected float push_xKeySize = 0.37f;

    protected virtual void Awake()
    {
        flickManager = this.transform.root.GetComponentInChildren<FlickKeyboardManager>(true);

        //EventTriggerに対応する関数を登録する////////////////////////////////////////
        EventTrigger.Entry entryPointerDown = new EventTrigger.Entry();
        entryPointerDown.eventID = EventTriggerType.PointerDown;
        entryPointerDown.callback.AddListener((x) => OnPointerDown());

        EventTrigger.Entry entryPointerUp = new EventTrigger.Entry();
        entryPointerUp.eventID = EventTriggerType.PointerUp;
        entryPointerUp.callback.AddListener((x) => OnPointerUp());

        EventTrigger.Entry entryPointerClick = new EventTrigger.Entry();
        entryPointerClick.eventID = EventTriggerType.PointerClick;
        entryPointerClick.callback.AddListener((x) => OnPointerClick());


        EventTrigger.Entry entryPointerEnter = new EventTrigger.Entry();
        entryPointerEnter.eventID = EventTriggerType.PointerEnter;
        entryPointerEnter.callback.AddListener((x) => OnPointerEnter());
        eventTrigger.triggers.Add(entryPointerEnter);

        eventTrigger.triggers.Add(entryPointerDown);
        eventTrigger.triggers.Add(entryPointerUp);
        eventTrigger.triggers.Add(entryPointerClick);
        /////////////////////////////////////////////////////////////////////////////

        keyImage = GetComponent<Image>();

        //キーの初期代入
        initialKeyColor = keyImage.color;
        initialKeySize = transform.localScale;

        //キーが押されたときの数値設定
        pushKeySize = initialKeySize;
        pushKeySize.x = push_xKeySize;
    }

    /// <summary>
    /// キーが押されたときのキーアニメーション
    /// </summary>
    protected void PointerDownAnimation()
    {
        keyImage.color = ButtonColor.PushColor;
        transform.localScale = pushKeySize;
    }
    /// <summary>
    /// キーが離されたときのキーアニメーション
    /// </summary>
    protected void PointerUpAnimation()
    {
        keyImage.color = initialKeyColor;
        transform.localScale = initialKeySize;
    }

    /// <summary>
    /// キーがImage範囲内に侵入したとき
    /// </summary>
    protected abstract void OnPointerEnter();
    /// <summary>
    /// キーが押されたとき
    /// </summary>
    protected abstract void OnPointerDown();
    /// <summary>
    /// キーが離されたとき
    /// </summary>
    protected abstract void OnPointerUp();
    /// <summary>
    /// キー押されて、Image範囲内で離されたとき
    /// </summary>
    protected abstract void OnPointerClick();
}
