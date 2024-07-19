using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class Installation : MonoBehaviour
{
    [SerializeField]
    private float vertical;
    [SerializeField]
    private float beside;
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }
    [SerializeField]
    private Direction direction;
    public struct OneAxis
    {
        public readonly Vector3 Start;
        public readonly Vector3 End;
        public OneAxis(Vector3 start,Vector3 end)
        {
            this.Start = start;
            this.End = end;
        }
    }
    public struct Range
    {
        public readonly OneAxis FirstVertical;
        public readonly OneAxis FirstBeSide;

        public readonly OneAxis SecondVertical;
        public readonly OneAxis SecondBeSide;

        public Range(OneAxis firstVertical,OneAxis firstBeSide,OneAxis secondVertical,OneAxis secondBeSide)
        {
            this.FirstVertical = firstVertical;
            this.FirstBeSide = firstBeSide;

            this.SecondVertical = secondVertical;
            this.SecondBeSide = secondBeSide;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Vector3 position = this.transform.position;

        Gizmos.color = Color.yellow;
        //Gizmos.DrawLine(points[i], points[i + 1]);
    }
#endif
    private void DDD()
    {
        Vector3 position = this.transform.position;
        switch (direction)
        {
            case Direction.Up:
                //new Range(new OneAxis(position,position+new Vector3(0f,vertical,0f),new OneAxis(position+position)        
                break;
            case Direction.Down:
                break;
            case Direction.Left:
                break;
            case Direction.Right:
                break;
        }
    }
}
