using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class MarkManager : MonoBehaviour
{
    private List<Mark> marks = new List<Mark>();
    public void MarkInCamera(Mark[] marks)
    {
        List<Mark> existMark = marks.Except(this.marks).ToList();
        foreach(Mark mark in existMark)
        {
            mark.MarkInCamera();
            this.marks.Add(mark);
        }

        List<Mark> notExistMarks = this.marks.Except(marks).ToList();
        foreach(Mark notExistMark in notExistMarks)
        {
            notExistMark.MarkOutCamera();
            this.marks.Remove(notExistMark);
        }
    }

    public void Instance(Mark mark,Vector3 position)
    {
        Mark instance = Instantiate(mark);
        instance.transform.position = position;
        instance.TransformInject(this.transform);
    }

    private void Start()
    {
        foreach(Mark mark in FindObjectsOfType<Mark>())
        {
            mark.TransformInject(this.transform);
        }
    }
}
