using Cysharp.Threading.Tasks;
using UnityEngine;

public class WarpPointer : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer = default;
    [SerializeField] private MeshRenderer warpSymbol = default;
    [SerializeField] private CharacterController characterController = default;
    [SerializeField] private LayerMask includeLayers = default;
    [SerializeField] private LayerMask excludeLayers = default;
    [Space]
    [SerializeField] private Color32 hitColor = default;
    [SerializeField] private Color32 neutralColor = default;
    [SerializeField] private float maxDistance = 30f;

    private Transform symbolTransform = default;
    private Vector3 prevWarpPos = Vector3.zero;
    private float movedDistance = 0f;


    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    private void Reset()
    {
        lineRenderer ??= GetComponent<LineRenderer>();
        warpSymbol ??= GetComponentInChildren<MeshRenderer>();
        characterController ??= GetComponentInParent<CharacterController>();
    }

    private void Awake()
    {
        symbolTransform = warpSymbol.transform;
    }

    private void Start()
    {
        lineRenderer.startColor = neutralColor;
        lineRenderer.endColor = neutralColor;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;

        warpSymbol.enabled = false;
    }

    /// <summary>
    /// �|�C���^�[��`�悷��
    /// <br>See <see href="https://qiita.com/kousaku-maron/items/106619d0c065be155bbb"/></br>
    /// </summary>
    /// <param name="origin">���_</param>
    /// <param name="direction">����</param>
    /// <exception cref="System.InvalidOperationException">WarpPointer���ǂ��ɂ�������Ȃ������Ƃ��i�Ԃ��l�����݂��Ȃ��Ƃ��j</exception>
    /// <returns>���[�v�\���ǂ���</returns>
    public bool Draw(Vector3 origin, Vector3 direction, ref Vector3 warpPos, Vector2 inputDir)
    {
        // disable��ԂȂ珈�����I��
        if (!lineRenderer.enabled)
        {
            return false;
        }

        // Vibration
        CheckVibration(warpPos);

        const float MIN_DIRECTION_Y = 0.025f;

        // y����direction���v���X�l�ɃN�����v
        direction.y = direction.y < MIN_DIRECTION_Y
            ? MIN_DIRECTION_Y
            : direction.y;

        const float MIN_DISTANCE = 1f;
        const float DEFAULT_DROP_HEIGHT = 1f;
        const float MIDDLE_DIRECTION_Y = 0.5f;

        // y����direction�ɂ����distance��ϓ�������i45���ɋ߂��قǒ����A��������قǒZ���Ȃ�j
        float distance = maxDistance - Mathf.Abs(MIDDLE_DIRECTION_Y - direction.y) * maxDistance * 2;
        // distance���ŏ��l�ȏ�ɃN�����v
        distance = distance < MIN_DISTANCE
            ? MIN_DISTANCE
            : distance;

        // dropHeight��ݒ�By����direction��0�ɋ߂��قǒl��傫������
        float dropHeight = DEFAULT_DROP_HEIGHT;
        dropHeight += direction.y < MIDDLE_DIRECTION_Y
            ? (MIDDLE_DIRECTION_Y - direction.y) * 5f
            : 0;

        // �x�W�F�Ȑ���3�̊�_��ݒ�
        // p0: �n�_, p1: ���ԓ_, p2: �ڕW�_
        Vector3 p0 = origin;
        Vector3 p1 = origin + direction * distance / 2;
        Vector3 p2 = origin + direction * distance;
        p2.y = p0.y - dropHeight;

        // �ڕW�_�܂ł̋����ɉ����ăA���J�[�|�C���g�𑝂₷
        const int ANCHOR_COUNT_COEFFICIENT = 10;
        int n = (int)Vector3.Distance(p0, p2) * ANCHOR_COUNT_COEFFICIENT;
        lineRenderer.positionCount = n;

        Vector3 prevb012 = p0;
        LayerMask layerMask = includeLayers == 0
            ? ~excludeLayers
            : includeLayers;

        // �A���J�[�|�C���g���Ƃ�Line��`�悷��B�q�b�g���肪�������璆�f����
        for (int i = 0; i < n; i++)
        {
            // float�l���~�������߂��ׂăL���X�g�B�璷�ł͂Ȃ�
            float t = (float)i / (float)(n - 1);
            // p012�Ԃ����炩�Ɉړ�����2�_�ƁA����2�_�Ԃ��ړ�����1�_�����߂�
            Vector3 b01 = Vector3.Lerp(p0, p1, t);
            Vector3 b12 = Vector3.Lerp(p1, p2, t);
            Vector3 b012 = Vector3.Lerp(b01, b12, t);

            // �r���ɏ�Q�����Ȃ�������
            if (Physics.Linecast(prevb012, b012, out RaycastHit hit, layerMask))
            {
                // �A���J�[�|�C���g�̐����C��
                lineRenderer.positionCount = i + 1;
                lineRenderer.SetPosition(i, hit.point);
                // Line�����������ꏊ���擾
                warpPos = hit.point;

                // ���������ꏊ�����[�v�\���ǂ������肷��
                bool canWarp = CheckWarpable(hit.normal, ref warpPos);
                if (canWarp)
                {
                    // ���[�v�O�̏���������
                    CanWarp(warpPos, inputDir);
                }
                else
                {
                    CantWarp();
                }

                return canWarp;
            }
            else
            {
                lineRenderer.SetPosition(i, b012);
            }

            // for�̍Ō��prev�ϐ��̒l��ς������Ȃ�
            if (i == n - 1)
            {
                break;
            }

            prevb012 = b012;
        }

        // Mathf.Infinity���ƃp�t�H�[�}���X��낵���Ȃ��̂ŁA�^��Infinity�̈ӁB
        const float PSEUDO_INFINITY = 10f;

        // for���𔲂��Ă��̃R�[�h�ɓ��B = �܂��ǂ��ɂ�Line���������Ă��Ȃ�
        // �Ȃ̂ł�������͂܂�����Ray���΂��A�����I�ɂǂ����ɓ��Ă�
        // �ς��ƌ���a���Ȃ��̂ŁA�x�W�F�Ȑ����r�؂ꂽ��͒����ŕ⊮
        if (Physics.Raycast(p2, (p2 - prevb012).normalized, out RaycastHit hit2, PSEUDO_INFINITY, layerMask))
        {
            warpPos = hit2.point;

            // ���������ꏊ�����[�v�\���ǂ������肷��
            bool canWarp = CheckWarpable(hit2.normal, ref warpPos);
            if (canWarp)
            {
                // ���[�v�O�̏���������
                CanWarp(warpPos, inputDir);
            }
            else
            {
                CantWarp();
            }

            return canWarp;
        }
        // �ǂ��ɂ�������Ȃ������ꍇ
        else
        {
            CantWarp();
            return false;
        }


        // �^����ꂽ��񂩂�A���[�v�\���ǂ������肷��
        bool CheckWarpable(Vector3 normal, ref Vector3 warpPos)
        {
            float angle = Vector3.Angle(Vector3.up, normal);

            // ���[�v�\�Ȋp�x�ȓ��ł���΁Atrue
            // CharacterController�̒l���K��l�Ƃ���
            if (angle <= characterController.slopeLimit)
            {
                return true;
            }

            // point��������x�̍����ł���΁A�ǂ��w���Ă����Ƃ��Ă��u���̒����̒n�ʂɃ��[�v�\�v�Ƃ�����
            // �Ȃ̂ŉ������ɍēxRay���΂��A������x�̍����ȓ��ł��邩���肷��

            // tmpPoint => point�i�ǁj�Ɠ��`�B�ēx��΂���Ray���ǂɓ�����Ȃ��悤�@�������ɏ������炵�Ă���
            // distance => ������x�̍����BcharacterController�̍������Q�l�ɓK���Ȓl��ݒ�
            Vector3 tmpPoint = warpPos + normal * characterController.radius;
            float distance = characterController.height;

            if (Physics.Raycast(tmpPoint, Vector3.down, out RaycastHit hit, distance, layerMask))
            {
                warpPos = hit.point;
                return true;
            }

            return false;
        }

        // �������������ɂ���ĐU����^����
        void CheckVibration(Vector3 warpPos)
        {
            const float DISTANCE_INTERVAL = 7.5f;

            movedDistance += Vector3.Distance(warpPos, prevWarpPos);
            prevWarpPos = warpPos;
            if (movedDistance > DISTANCE_INTERVAL)
            {
                movedDistance = 0f;
                OnMovedVibration().Forget();
            }
        }
    }

    public void SetActive(bool value)
    {
        lineRenderer.enabled = value;
        warpSymbol.enabled = false;
    }

    private void CanWarp(Vector3 warpPos, Vector2 inputDir)
    {
        lineRenderer.startColor = hitColor;
        lineRenderer.endColor = hitColor;
        symbolTransform.position = warpPos;
        symbolTransform.LocalRotateBy2DVector(inputDir, Vector3.down);
        warpSymbol.enabled = true;
    }

    private void CantWarp()
    {
        lineRenderer.startColor = neutralColor;
        lineRenderer.endColor = neutralColor;
        warpSymbol.enabled = false;
    }

    private async UniTaskVoid OnMovedVibration()
    {
        OVRInput.SetControllerVibration(0.1f, 0.15f, OVRInput.Controller.LTouch);
        await UniTask.Delay(System.TimeSpan.FromSeconds(0.04));
        OVRInput.SetControllerVibration(0f, 0f);
    }
}