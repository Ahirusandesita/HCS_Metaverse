using UnityEngine;

public class WarpPointer : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer = default;
    [SerializeField] private MeshRenderer warpSymbol = default;
    [SerializeField] private CharacterController characterController = default;
    [SerializeField] private LayerMask includeLayers = default;
    [SerializeField] private LayerMask excludeLayers = default;

    private readonly Color32 hitColor = new Color32(0, 0, 255, 255);
    private readonly Color32 neutralColor = new Color32(255, 0, 0, 255);


    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    private void Reset()
    {
        lineRenderer ??= GetComponent<LineRenderer>();
        warpSymbol ??= GetComponentInChildren<MeshRenderer>();
        characterController ??= GetComponentInParent<CharacterController>();
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
    /// <br>See <see href="https://qiita.com/kousaku-maron/items/106619d0c065be155bbb"/></br>
    /// </summary>
    /// <param name="origin">原点</param>
    /// <param name="direction">方向</param>
    /// <exception cref="System.InvalidOperationException">WarpPointerがどこにも当たらなかったとき（返す値が存在しないとき）</exception>
    /// <returns>ワープ可能かどうか</returns>
    public bool Draw(Vector3 origin, Vector3 direction, ref Vector3 warpPos)
    {
        // disable状態なら処理を終了
        if (!lineRenderer.enabled)
        {
            return false;
        }

        // y軸のdirectionをプラス値にクランプ
        direction.y = direction.y <= 0f
            ? 0.01f
            : direction.y;

        const float MAX_DISTANCE = 30f;
        const float MIN_DISTANCE = 1f;
        const float DEFAULT_DROP_HEIGHT = 1f;
        const float MIDDLE_DIRECTION_Y = 0.5f;

        // y軸のdirectionによってdistanceを変動させる（45°に近いほど長く、遠ざかるほど短くなる）
        float distance = MAX_DISTANCE - Mathf.Abs(MIDDLE_DIRECTION_Y - direction.y) * MAX_DISTANCE * 2;
        // distanceを最小値以上にクランプ
        distance = distance < MIN_DISTANCE
            ? MIN_DISTANCE
            : distance;

        // dropHeightを設定。y軸のdirectionが0に近いほど値を大きくする
        float dropHeight = DEFAULT_DROP_HEIGHT;
        dropHeight += direction.y < MIDDLE_DIRECTION_Y
            ? (MIDDLE_DIRECTION_Y - direction.y) * 10
            : 0;

        // ベジェ曲線の3つの基準点を設定
        // p0: 始点, p1: 中間点, p2: 目標点
        var p0 = origin;
        var p1 = origin + direction * distance / 2;
        var p2 = origin + direction * distance;
        p2.y = p0.y - dropHeight;

        // 目標点までの距離に応じてアンカーポイントを増やす
        const int ANCHOR_COUNT_COEFFICIENT = 10;
        int n = (int)Vector3.Distance(p0, p2) * ANCHOR_COUNT_COEFFICIENT;
        lineRenderer.positionCount = n;

        Vector3 prevb012 = p0;
        LayerMask layerMask = includeLayers == 0
            ? ~excludeLayers
            : includeLayers;

        // アンカーポイントごとにLineを描画する。ヒット判定があったら中断する
        for (int i = 0; i < n; i++)
        {
            // float値が欲しいためすべてキャスト。冗長ではない
            float t = (float)i / (float)(n - 1);
            // p012間を滑らかに移動する2点と、その2点間を移動する1点を求める
            var b01 = Vector3.Lerp(p0, p1, t);
            var b12 = Vector3.Lerp(p1, p2, t);
            var b012 = Vector3.Lerp(b01, b12, t);

            // 途中に障害物がないか判定
            if (Physics.Linecast(prevb012, b012, out RaycastHit hit, layerMask))
            {
                // アンカーポイントの数を修正
                lineRenderer.positionCount = i + 1;
                lineRenderer.SetPosition(i, hit.point);
                // Lineが当たった場所を取得
                warpPos = hit.point;

                // 当たった場所がワープ可能かどうか判定する
                bool canWarp = CheckWarpable(hit.normal, ref warpPos);
                if (canWarp)
                {
                    // ワープ前の初期化処理
                    CanWarp(warpPos);
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

            // forの最後はprev変数の値を変えたくない
            if (i == n - 1)
            {
                break;
            }

            prevb012 = b012;
        }

        // for文を抜けてこのコードに到達 = まだどこにもLineが当たっていない
        // なのでそこからはまっすぐRayを飛ばし、強制的にどこかに当てる
        // ぱっと見違和感ないので、ベジェ曲線が途切れた後は直線で補完
        if (Physics.Raycast(p2, (p2 - prevb012).normalized, out RaycastHit hit2, layerMask))
        {
            warpPos = hit2.point;

            // 当たった場所がワープ可能かどうか判定する
            bool canWarp = CheckWarpable(hit2.normal, ref warpPos);
            if (canWarp)
            {
                // ワープ前の初期化処理
                CanWarp(warpPos);
            }
            else
            {
                CantWarp();
            }

            return canWarp;
        }

        // どこにも当たらなかった場合
        return false;

        
        // 与えられた情報から、ワープ可能かどうか判定する
        bool CheckWarpable(Vector3 normal, ref Vector3 point)
        {
            var angle = Vector3.Angle(Vector3.up, normal);

            // ワープ可能な角度以内であれば、true
            // CharacterControllerの値を規定値とする
            if (angle <= characterController.slopeLimit)
            {
                return true;
            }

            // pointがある程度の高さであれば、壁を指していたとしても「その直下の地面にワープ可能」としたい
            // なので下方向に再度Rayを飛ばし、ある程度の高さ以内であるか判定する

            // tmpPoint => point（壁）と同義。再度飛ばしたRayが壁に当たらないよう法線方向に少しずらしている
            // distance => ある程度の高さ。characterControllerの高さを参考に適当な値を設定
            var tmpPoint = point + normal * 0.25f;
            float distance = characterController.height;

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

    /// <summary>
    /// ワープしたときに呼び出す（ビューのリセット）
    /// </summary>
    public void OnWarp()
    {
        lineRenderer.enabled = false;
        lineRenderer.startColor = neutralColor;
        lineRenderer.endColor = neutralColor;
        warpSymbol.enabled = false;
    }

    private void CanWarp(Vector3 warpPos)
    {
        lineRenderer.startColor = hitColor;
        lineRenderer.endColor = hitColor;
        warpSymbol.transform.position = warpPos;
        warpSymbol.enabled = true;
    }

    private void CantWarp()
    {
        lineRenderer.startColor = neutralColor;
        lineRenderer.endColor = neutralColor;
        warpSymbol.enabled = false;
    }
}
