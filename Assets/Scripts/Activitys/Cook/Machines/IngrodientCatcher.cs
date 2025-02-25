using UnityEngine;
using Fusion;

public class IngrodientCatcher
{
    public bool SearchIngrodient(Vector3 hitBoxCenter, Vector3 hitBoxSize, Quaternion hitBoxRotation, out NetworkObject processingObject)
    {
        // processingObjectの初期化
        processingObject = default;

        // オブジェクトの取得範囲を形成して接触しているColliderを取得する
        Collider[] hitColliders = Physics.OverlapBox(hitBoxCenter, hitBoxSize, hitBoxRotation);

        // 何も当たっていなかった場合
        if (hitColliders is null)
        {
            // Falseを返して終了
            Debug.Log($"なにも当たってないよん");
            return false;
        }

        // 範囲内のオブジェクトをすべて探索する
        foreach (Collider hitCollider in hitColliders)
        {
            // Ingrodientsがついていた場合
            if (hitCollider.transform.root.TryGetComponent<Ingrodients>(out var igrodient))
            {
                // RigidbodyのKinematicがついている場合
                if (hitCollider.transform.root.GetComponent<Rigidbody>().isKinematic)
                {
                    // 次のオブジェクトに移る
                    continue;
                }

                // 固定するオブジェクトを取得する
                processingObject = igrodient.GetComponent<NetworkObject>();

                // Ingrodientと当たったからTrueを返して終了する
                return true;
            }
        }

        // Ingrodientと当たらなかったからfalseを返して終了する
        return false;
    }


}
