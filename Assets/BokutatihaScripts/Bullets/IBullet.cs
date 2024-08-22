using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BulletInfo
{
    public readonly Vector3 direction;
    public readonly float speed;
    public readonly float lifeTime;

    public BulletInfo(Vector3 direction,float speed,float lifeTime)
    {
        this.direction = direction;
        this.speed = speed;
        this.lifeTime = lifeTime;
    }
}
public interface IBullet
{
    void Shot();
    void Inject(BulletInfo bulletInfo);
}
