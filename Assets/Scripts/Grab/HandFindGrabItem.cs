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
        //Ray�̍쐬�@�@�@�@�@�@�@��Ray���΂����_�@�@�@��Ray���΂�����
        Ray ray = new Ray(transform.position, transform.forward);

        //Ray�����������I�u�W�F�N�g�̏������锠
        RaycastHit hit;

        //Ray�̔�΂��鋗��
        int distance = 10;

        //Ray�̉���    ��Ray�̌��_�@�@�@�@��Ray�̕����@�@�@�@�@�@�@�@�@��Ray�̐F
        Debug.DrawLine(ray.origin, ray.direction * distance, Color.red);

        //����Ray�ɃI�u�W�F�N�g���Փ˂�����
        //                  ��Ray  ��Ray�����������I�u�W�F�N�g ������
        if (Physics.Raycast(ray, out hit, distance))
        {
            //Ray�����������I�u�W�F�N�g��tag��Player��������
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
