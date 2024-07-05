using UnityEngine;

public class WarpPointer
{
    private readonly LineRenderer lineRenderer = default;
    private const float MAX_DISTANCE = 30f;
    private const float MIN_DISTANCE = 1f;


    public WarpPointer(GameObject user)
    {
        var gameObject = new GameObject("WarpPointer");
        gameObject.transform.SetParent(user.transform);
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.alignment = LineAlignment.View;
        lineRenderer.receiveShadows = false;
        lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        lineRenderer.loop = false;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
    }

    /// <summary>
    /// ポインターを描画する
    /// </summary>
    /// <param name="origin">原点</param>
    /// <param name="direction">方向</param>
    /// <exception cref="System.InvalidOperationException">WarpPointerがどこにも当たらなかったとき（返す値が存在しないとき）</exception>
    /// <returns>ポインターが当たった座標</returns>
    public Vector3 Draw(Vector3 origin, Vector3 direction)
    {
        if (!lineRenderer.enabled)
        {
            return Vector3.zero;
        }

        direction.y = direction.y <= 0f
            ? 0.01f
            : direction.y;

        float distance = MAX_DISTANCE - Mathf.Abs(0.5f - direction.y) * MAX_DISTANCE * 2;
        distance = distance < MIN_DISTANCE ? MIN_DISTANCE : distance;

        float dropHeight = 0f;
        dropHeight = direction.y < 0.5f ? dropHeight + (0.5f - direction.y) * 10 : dropHeight;

        var p0 = origin;
        var p1 = origin + direction * distance / 2;
        var p2 = origin + direction * distance;
        p2.y = p0.y - dropHeight;

        const int ANCHOR_COUNT_COEFFICIENT = 10;
        int n = (int)Vector3.Distance(p0, p2) * ANCHOR_COUNT_COEFFICIENT;
        lineRenderer.positionCount = n + 1;

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
                return hit.point;
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
            lineRenderer.SetPosition(n, hit2.point);
            return hit2.point;
        }

        // この例外がthrowされることは想定していない
        throw new System.InvalidOperationException("WarpPointerがどこにも当たっていません。");
    }

    public void SetActive(bool value)
    {
        lineRenderer.enabled = value;
    }
}
