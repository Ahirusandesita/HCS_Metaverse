using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSubItem : MonoBehaviour
{
    bool isSelect = false;
    [SerializeField]
    private GameObject parent;

    private Transform handTransform;
    public void Select()
    {
        Debug.Log("Select");
        isSelect = true;
    }

    public void UnSelect()
    {
        isSelect = false;
    }
    private void Awake()
    {

        InteractorDetailEventIssuer interactorDetailEventIssuer = GameObject.FindObjectOfType<InteractorDetailEventIssuer>();
        interactorDetailEventIssuer.OnInteractor += (a) =>
        {
            if (a.InteractorType == InteractorType.Select && isSelect)
            {
                Debug.Log(a.HandType);
                handTransform = a.HandTransform;
            }
        };
    }
    private void LateUpdate()
    {
        Vector3 parentPos = parent.transform.position;
        parentPos.x += 0.1f;
        if (isSelect)
        {
            

            float distance = Vector3.Distance(parent.transform.position, handTransform.position);
            parentPos.x += distance - 0.1f;
            this.transform.position = parentPos;
        }

        else
        {
            this.transform.position = parentPos;
        }
    }

}
