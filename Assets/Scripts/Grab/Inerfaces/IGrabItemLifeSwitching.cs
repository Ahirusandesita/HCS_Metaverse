using UnityEngine;
public enum LifeSwitchType
{
    OFF,
    ON
}
public interface IGrabItemLifeSwitching
{
    void LifeSwitching(LifeSwitchType lifeSwitchType,IGrabItemRawMaterials grabItemRawMaterials);
    LifeSwitchType NowLifeType { get; }
}
public interface IGrabItemRawMaterials
{
    void Destruction();
}
