using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTest : MonoBehaviour,IBullet
{
    private BulletInfo bulletInfo = new BulletInfo(Vector3.forward,1f,10f);

    public void Inject(BulletInfo bulletInfo)
    {
        this.bulletInfo = bulletInfo;
    }

    public void Shot()
    {

    }

    private void Update()
    {
        this.transform.Translate(bulletInfo.direction * bulletInfo.speed * Time.deltaTime);
    }
}
