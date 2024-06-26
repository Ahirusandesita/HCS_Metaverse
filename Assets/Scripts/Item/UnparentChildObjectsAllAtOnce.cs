using UnityEngine;

public class UnparentChildObjectsAllAtOnce : MonoBehaviour
{

    Transform[] transforms;
    private void Start()
    {
        transforms = new Transform[transform.childCount];
        for(int i = 0; i < transform.childCount; i++)
        {
            transforms[i] = transform.GetChild(i);
        }

        foreach(Transform child in transforms)
        {
            child.parent = null;
        }

        transforms = null;

        Destroy(gameObject);

    }
}
