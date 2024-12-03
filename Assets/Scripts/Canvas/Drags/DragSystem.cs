using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragSystem : MonoBehaviour,IBeginDragHandler,IEndDragHandler,IDragHandler
{
    [SerializeField]
    private float sensitivity = 1f;

    /// <summary>
    /// �X�N���[������Ώ�
    /// </summary>
    private List<IScrollable> scrollables = new List<IScrollable>();
    [SerializeField, InterfaceType(typeof(IScrollable))]
    private List<UnityEngine.Object> IScrollables = new List<Object>();
    private List<IScrollable> IScrollableList => IScrollables.OfType<IScrollable>().ToList();
    /// <summary>
    /// �Ō�̉�ʐڐG�_
    /// </summary>
    private Vector2 LastPointerPosition;
    /// <summary>
    /// �X�N���[���ł��邩�ǂ���
    /// </summary>
    private bool canScroll = false;


    private void Awake()
    {
        //EventTrigger�ɑΉ�����֐���o�^����///////////////////////////////////////////////////////
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
    /// �h���b�O���ꂽ�Ƃ�
    /// </summary>
    /// <param name="data"></param>
    private void OnDrag(PointerEventData data)
    {
        //�X�N���[���ł��Ȃ���ԂȂ�I������
        if (!canScroll)
        {
            return;
        }

        //�O��̐ڐG�_���猻�݂̐ڐG�_�������ĐڐG�_�̈ړ��ʂ����߂�
        Vector3 scrollMove = LastPointerPosition - data.position;

        if(scrollMove.x > 150f || scrollMove.y > 150f)
        {
            scrollMove = Vector3.zero;
        }

        //�X�N���[������ΏۂɈړ��ʂ�n���ăX�N���[��������
        foreach (IScrollable scrollable in scrollables)
        {
            scrollable.Scroll(scrollMove, sensitivity);
        }

        //�O��̐ڐG�_���X�V����
        LastPointerPosition = data.position;
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        OnDrag((PointerEventData)eventData);
    }

    /// <summary>
    /// �����ꂽ�Ƃ�
    /// </summary>
    /// <param name="data"></param>
    private void OnPinterDown(PointerEventData data)
    {
        //�O��̐ڐG�_�ɐݒ肵�āA�X�N���[���J�n�ł���悤�ɂ���
        LastPointerPosition = data.position;
        canScroll = true;
    }

    /// <summary>
    /// �����ꂽ�Ƃ�
    /// </summary>
    private void OnPointerUp()
    {
        //�X�N���[���ł��Ȃ�����
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
