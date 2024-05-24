using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// �t���b�N�L�[�̐e�@
/// </summary>
public abstract class FlickKeyParent : MonoBehaviour
{
    [SerializeField]
    private EventTrigger eventTrigger;

    protected FlickKeyboardManager flickManager;
    
    /// <summary>
    /// Key��Image
    /// </summary>
    private Image keyImage;
    /// <summary>
    /// Key�̏����J���[
    /// </summary>
    private Color initialKeyColor;
    /// <summary>
    /// Key�̏����T�C�Y
    /// </summary>
    private Vector3 initialKeySize;
    /// <summary>
    /// Key�������ꂽ�Ƃ���KeySize
    /// </summary>
    private Vector3 pushKeySize;
    /// <summary>
    /// Key�������ꂽ�Ƃ���X����傫�����鐔�l
    /// </summary>
    protected float push_xKeySize = 0.37f;

    protected virtual void Awake()
    {
        flickManager = this.transform.root.GetComponentInChildren<FlickKeyboardManager>(true);

        //EventTrigger�ɑΉ�����֐���o�^����////////////////////////////////////////
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

        //�L�[�̏������
        initialKeyColor = keyImage.color;
        initialKeySize = transform.localScale;

        //�L�[�������ꂽ�Ƃ��̐��l�ݒ�
        pushKeySize = initialKeySize;
        pushKeySize.x = push_xKeySize;
    }

    /// <summary>
    /// �L�[�������ꂽ�Ƃ��̃L�[�A�j���[�V����
    /// </summary>
    protected void PointerDownAnimation()
    {
        keyImage.color = ButtonColor.PushColor;
        transform.localScale = pushKeySize;
    }
    /// <summary>
    /// �L�[�������ꂽ�Ƃ��̃L�[�A�j���[�V����
    /// </summary>
    protected void PointerUpAnimation()
    {
        keyImage.color = initialKeyColor;
        transform.localScale = initialKeySize;
    }

    /// <summary>
    /// �L�[��Image�͈͓��ɐN�������Ƃ�
    /// </summary>
    protected abstract void OnPointerEnter();
    /// <summary>
    /// �L�[�������ꂽ�Ƃ�
    /// </summary>
    protected abstract void OnPointerDown();
    /// <summary>
    /// �L�[�������ꂽ�Ƃ�
    /// </summary>
    protected abstract void OnPointerUp();
    /// <summary>
    /// �L�[������āAImage�͈͓��ŗ����ꂽ�Ƃ�
    /// </summary>
    protected abstract void OnPointerClick();
}
