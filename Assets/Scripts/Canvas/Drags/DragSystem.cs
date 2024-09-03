using UnityEngine;
using UnityEngine.EventSystems;

public class DragSystem : MonoBehaviour,IPointerDownHandler,IPointerUpHandler,IDragHandler
{
    [SerializeField]
    private float sensitivity = 1f;

    /// <summary>
    /// �X�N���[������Ώ�
    /// </summary>
    private IScrollable[] scrollables;

    /// <summary>
    /// �Ō�̉�ʐڐG�_
    /// </summary>
    private Vector2 LastPointerPosition;
    /// <summary>
    /// �X�N���[���ł��邩�ǂ���
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

        scrollables = this.transform.GetComponentsInChildren<IScrollable>(true);
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
}
