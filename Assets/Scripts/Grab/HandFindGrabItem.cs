using UnityEngine;

public class HandFindGrabItem : MonoBehaviour
{
    private GameObject lastHitGrabItem;

    private GameObject grabItem;
    private IGrabItemLifeSwitching itemLifeSwitching;
    private void Update()
    {
        RayTest();
    }
    void RayTest()
    {
        //Rayの作成　　　　　　　↓Rayを飛ばす原点　　　↓Rayを飛ばす方向
        Ray ray = new Ray(transform.position, transform.forward);

        //Rayが当たったオブジェクトの情報を入れる箱
        RaycastHit hit;

        //Rayの飛ばせる距離
        int distance = 10;

        //Rayの可視化    ↓Rayの原点　　　　↓Rayの方向　　　　　　　　　↓Rayの色
        Debug.DrawLine(ray.origin, ray.direction * distance, Color.red);

        //もしRayにオブジェクトが衝突したら
        //                  ↓Ray  ↓Rayが当たったオブジェクト ↓距離
        if (Physics.Raycast(ray, out hit, distance))
        {
            //Rayが当たったオブジェクトのtagがPlayerだったら
            if (hit.collider.gameObject.TryGetComponent<IGrabCopyItemPresenter>(out IGrabCopyItemPresenter grabCopyItemPresenter))
            {
                if (hit.collider.gameObject != lastHitGrabItem)
                {
                    lastHitGrabItem = grabCopyItemPresenter.CopyItem();

                    grabItem = Instantiate(lastHitGrabItem);
                    grabItem.transform.parent = this.transform;
                    itemLifeSwitching = grabItem.GetComponent<IGrabItemLifeSwitching>();
                    itemLifeSwitching.LifeSwitching(LifeSwitchType.OFF, lastHitGrabItem.GetComponent<IGrabItemRawMaterials>());
                }
            }
        }
        else
        {
            lastHitGrabItem = null;
            if(itemLifeSwitching?.NowLifeType == LifeSwitchType.OFF)
            {
                Destroy(lastHitGrabItem);
                itemLifeSwitching = null;
            }
        }
    }

}
