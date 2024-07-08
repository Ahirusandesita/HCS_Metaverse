using UnityEngine;

public class WarpPointer : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer = default;
    [SerializeField] private MeshRenderer warpSymbol = default;
    [SerializeField, CustomField("optional"), Tooltip("設定すると、 CharacterController の slopeLimit を参照してワープ可否を判断する")]
    private CharacterController characterController = default;

    private readonly Color32 hitColor = new Color32(255, 0, 0, 255);
    private readonly Color32 neutralColor = new Color32(0, 0, 255, 255);
    private const float MAX_DISTANCE = 30f;
    private const float MIN_DISTANCE = 1f;
    private const float DEFAULT_DROP_HEIGHT = 1f;


    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    private void Reset()
    {
        lineRenderer ??= GetComponent<LineRenderer>();
        warpSymbol ??= GetComponentInChildren<MeshRenderer>();
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
    /// ポインターを描画する
    /// </summary>
    /// <param name="origin">原点</param>
    /// <param name="direction">方向</param>
    /// <exception cref="System.InvalidOperationException">WarpPointerがどこにも当たらなかったとき（返す値が存在しないとき）</exception>
    /// <returns>ワープ可能かどうか</returns>
    public bool Draw(Vector3 origin, Vector3 direction, ref Vector3 warpPos)
    {
        if (!lineRenderer.enabled)
        {
            return false;
        }

        direction.y = direction.y <= 0f
            ? 0.01f
            : direction.y;

        float distance = MAX_DISTANCE - Mathf.Abs(0.5f - direction.y) * MAX_DISTANCE * 2;
        distance = distance < MIN_DISTANCE ? MIN_DISTANCE : distance;

        float dropHeight = DEFAULT_DROP_HEIGHT;
        dropHeight += direction.y < 0.5f ? (0.5f - direction.y) * 10 : 0;

        var p0 = origin;
        var p1 = origin + direction * distance / 2;
        var p2 = origin + direction * distance;
        p2.y = p0.y - dropHeight;

        const int ANCHOR_COUNT_COEFFICIENT = 10;
        int n = (int)Vector3.Distance(p0, p2) * ANCHOR_COUNT_COEFFICIENT;
        lineRenderer.positionCount = n;

        Vector3 prevb012 = p0;

        for (int i = 0; i < n; i++)
        {
            float t = (float)i / (float)(n - 1);
            var b01 = Vector3.Lerp(p0, p1, t);
            var b12 = Vector3.Lerp(p1, p2, t);
            var b012 = Vector3.Lerp(b01, b12, t);

            if (Physics.Linecast(prevb012, b012, out RaycastHit hit))
            {
                lineRenderer.positionCount = i + 1;
                lineRenderer.SetPosition(i, hit.point);
                warpPos = hit.point;

                bool canWarp = CheckWarpable(hit.normal, ref warpPos);
                if (canWarp)
                {
                    ChangeColorOnHit();
                }

                return canWarp;
            }
            else
            {
                lineRenderer.SetPosition(i, b012);
            }

            if (i == n - 1)
            {
                break;
            }

            prevb012 = b012;
        }

        if (Physics.Raycast(p2, (p2 - prevb012).normalized, out RaycastHit hit2))
        {
            warpPos = hit2.point;

            bool canWarp = CheckWarpable(hit2.normal, ref warpPos);
            if (canWarp)
            {
                ChangeColorOnHit();
            }

            return canWarp;
        }

        // この例外がthrowされることは想定していない
        throw new System.InvalidOperationException("WarpPointerがどこにも当たっていません。");


        bool CheckWarpable(Vector3 normal, ref Vector3 point)
        {
            var angle = Vector3.Angle(Vector3.up, normal);

            // ワープ可能な角度以内であれば、true
            // CharacterControllerがアタッチされているなら、その値を規定値とする
            bool withinSlopeLimit = characterController is null
                ? angle <= 45f
                : angle <= characterController.slopeLimit;

            if (withinSlopeLimit)
            {
                return true;
            }

            // 元のpointがある程度の高さであれば、壁を指していたとしても「その直下の地面にワープ可能」としたい
            // なので下方向に再度Rayを飛ばし、ある程度の高さ以内であるか判定する

            // tmpPoint => 元のpoint（壁）と同義。再度飛ばしたRayが壁に当たらないよう法線方向に少しずらしている
            // distance => ある程度の高さ。characterControllerの高さを参考に適当な値を設定
            var tmpPoint = point + normal * 0.1f;
            float distance = characterController is null
                ? 2f
                : characterController.height * 2;

            if (Physics.Raycast(tmpPoint, Vector3.down, out RaycastHit hit, distance))
            {
                point = hit.point;
                return true;
            }

            return false;
        }
    }

    public void SetActive(bool value)
    {
        lineRenderer.enabled = value;
    }

    private void ChangeColorOnHit()
    {
        lineRenderer.startColor = hitColor;
        lineRenderer.endColor = hitColor;
    }
}
