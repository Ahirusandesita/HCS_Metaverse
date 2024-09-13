using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewMoveDepth : MonoBehaviour
{
    [SerializeField]
    BoxCollider _myCollider = default;

    [SerializeField]
    BoxCollider _hitCollider = default;

    private SurfaceData _hitSurfaceData = default;

    private struct SurfaceData
    {
        public SurfaceData(Vector3 normal, Vector3[] vertexList)
        {
            this.normal = normal;
            this.vertexList = vertexList;
        }

        public Vector3 normal;

        public Vector3[] vertexList;
    }

    private struct ProjectionData
    {
        public ProjectionData(Vector3 position, float depth)
        {
            this.position = position;

            this.depth = depth;
        }

        public Vector3 position;

        public float depth;
    }

    private void Start()
    {
        Vector3[] myVertexes = GetVertexPositionList(transform, _myCollider);

        _hitSurfaceData = GetHitSurfaceData(myVertexes, _hitCollider);
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Return))
        //{
        Vector3[] myVertexes = GetVertexPositionList(transform, _myCollider);

        //_hitSurfaceData = GetHitSurfaceData(myVertexes, _hitCollider);

        ProjectionData[] projectionDatas = GetDepth(_hitCollider, myVertexes);

        int deepestIndex = 0;

        float deepestDepth = projectionDatas[0].depth;

        for (int i = 1; i < projectionDatas.Length; i++)
        {
            if (projectionDatas[i].depth > deepestDepth)
            {
                deepestDepth = projectionDatas[i].depth;

                deepestIndex = i;
            }
        }

        // í∏ì_
        if (CheckOnHitSurface(projectionDatas[deepestIndex]))
        {
            Vector3 moveVector = GetUpByDepth(projectionDatas[deepestIndex]);

            transform.position += moveVector;
        }

        // ï”
        if (CheckConnectingDeepestToOnSurf(deepestIndex, projectionDatas, out int onHitSurfaceVertexIndex))
        {

        }

        // ñ 

    }

    private ProjectionData[] GetDepth(BoxCollider hitCollider, Vector3[] vertexList)
    {
        Vector3 samplePosition = _hitSurfaceData.vertexList[0];

        ProjectionData[] projectionDatas = new ProjectionData[vertexList.Length];

        for (int i = 0; i < projectionDatas.Length; i++)
        {
            Vector3 sampleVector = samplePosition - vertexList[i];

            Vector3 projectionVector = _hitSurfaceData.normal * Vector3.Dot(sampleVector, _hitSurfaceData.normal);

            projectionDatas[i].position = vertexList[i] - projectionVector;

            float projectionDistance = Vector3.Distance(projectionDatas[i].position, vertexList[i]);

            if (Vector3.Dot(projectionVector, _hitSurfaceData.normal) < 0)
            {
                projectionDistance = -projectionDistance;
            }

            projectionDatas[i].depth = projectionDistance;
        }

        return projectionDatas;
    }

    private Vector3 GetUpByDepth(ProjectionData projectionData)
    {
        return _hitSurfaceData.normal * projectionData.depth;
    }

    private Vector3[] GetVertexPositionList(Transform transform, BoxCollider collider)
    {
        Vector3[] vertexList = new Vector3[8];

        Vector3 center = collider.center;

        Vector3 extents = collider.size * 0.5f;

        Matrix4x4 convertMatrix = transform.localToWorldMatrix;

        for (int i = 0; i < 8; i++)
        {
            int xSign = 1 - (i & 4) / 2;
            int ySign = 1 - (i & 2); 
            int zSign = 1 - (i & 1) * 2;

            Vector3 vertexPosition = new Vector3(extents.x * xSign, extents.y * ySign, extents.z * zSign);

            vertexList[i] = convertMatrix.MultiplyPoint3x4(center + vertexPosition);
        }

        return vertexList;
    }

    private int[] GetConnectingVertexIndexes(int parentIndex)
    {
        int[] connectingVertexIndexList = new int[3] 
        {
            (parentIndex ^ 1) & 2 & 4,
            (parentIndex ^ 2) & 4 & 1,
            (parentIndex ^ 4) & 1 & 2
        };

        return connectingVertexIndexList;
    }

    private int[] GetOnHitSurfaceVertexList(ProjectionData[] projectionDatas)
    {
        bool[] onHitSurfaceStates = new bool[projectionDatas.Length];
        int onHitSurfaceCount = 0;

        for (int i = 0; i < projectionDatas.Length; i++)
        {
            onHitSurfaceStates[i] = CheckOnHitSurface(projectionDatas[i]);
            onHitSurfaceCount++;
        }

        int[] onHitSurfaceIndexes = new int[onHitSurfaceCount];

        int setedIndex = 0;

        for (int i = 0; i < onHitSurfaceStates.Length; i++)
        {
            if (onHitSurfaceStates[i])
            {
                onHitSurfaceIndexes[setedIndex] = i;
                setedIndex++;
            }
        }

        return onHitSurfaceIndexes; 
    }

    private SurfaceData GetHitSurfaceData(Vector3[] vertexList, BoxCollider hitCollider)
    {
        Vector3 hitObjectCenter = hitCollider.transform.position;

        float nearestVerToCenterDistance = Vector3.Distance(hitObjectCenter, vertexList[0]);

        int nearestIndex = 0;

        for (int i = 1; i < vertexList.Length; i++)
        {
            float verToCenterDistance = Vector3.Distance(hitObjectCenter, vertexList[i]);

            if (verToCenterDistance < nearestVerToCenterDistance)
            {
                nearestVerToCenterDistance = verToCenterDistance;
                nearestIndex = i;
            }
        }

        Vector3 nearestVector = vertexList[nearestIndex] - hitObjectCenter;

        nearestVector.x /= hitCollider.size.x * 0.5f * hitCollider.transform.lossyScale.x;
        nearestVector.y /= hitCollider.size.y * 0.5f * hitCollider.transform.lossyScale.y;
        nearestVector.z /= hitCollider.size.z * 0.5f * hitCollider.transform.lossyScale.z;

        Vector3[] hitColliderNormals = BoxColliderData.GetColliderNormals(hitCollider.transform);

        float maxInnerPloduct = Vector3.Dot(nearestVector, hitColliderNormals[0]);
        int maxInnerPloductIndex = 0;

        for (int i = 1; i < hitColliderNormals.Length; i++)
        {
            float innerPloduct = Vector3.Dot(nearestVector, hitColliderNormals[i]);

            if (innerPloduct > maxInnerPloduct)
            {
                maxInnerPloduct = innerPloduct;
                maxInnerPloductIndex = i;
            }
        }

        Vector3 hitNormal = hitColliderNormals[maxInnerPloductIndex];

        Vector3[] vertexesOfSurface = BoxColliderData.GetVertexesOfSurface(maxInnerPloductIndex, hitCollider);

        Debug.Log($"<color=red>ñ ñ@ê¸ : {hitNormal}</color>");

        return new SurfaceData(hitNormal, vertexesOfSurface);
    }

    private bool CheckOnHitSurface(ProjectionData projectionData)
    {
        Vector3 side1 = _hitSurfaceData.vertexList[1] - _hitSurfaceData.vertexList[0];

        Vector3 side2 = _hitSurfaceData.vertexList[_hitSurfaceData.vertexList.Length - 1] - _hitSurfaceData.vertexList[0];

        Vector3 projectionVector = projectionData.position - _hitSurfaceData.vertexList[0];

        float innerToSide1 = Vector3.Dot(side1, projectionVector) / Vector3.Dot(side1, side1);
        float innerToSide2 = Vector3.Dot(side2, projectionVector) / Vector3.Dot(side2, side2);

        if (0 <= innerToSide1 && innerToSide1 <= 1 && 0 <= innerToSide2 && innerToSide2 <= 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool CheckConnectingDeepestToOnSurf(int DeepestIndex, ProjectionData[] projectionDatas, out int subjectIndex)
    {
        int[] connectingVertexes = GetConnectingVertexIndexes(DeepestIndex);

        int[] onHitSurfaceVertexes = GetOnHitSurfaceVertexList(projectionDatas);

        subjectIndex = -1;

        for (int i = 0; i < connectingVertexes.Length; i++)
        {
            for (int k = 0; k < onHitSurfaceVertexes.Length; k++)
            {
                if (connectingVertexes[i] == onHitSurfaceVertexes[k])
                {
                    if (subjectIndex < 0 || projectionDatas[subjectIndex].depth < projectionDatas[connectingVertexes[i]].depth)
                    {
                        subjectIndex = connectingVertexes[i];
                    }
                }
            }
        }

        return subjectIndex >= 0;
    }
}