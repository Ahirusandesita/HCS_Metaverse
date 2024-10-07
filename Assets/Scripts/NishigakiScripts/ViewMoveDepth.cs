using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewMoveDepth
{
    public ViewMoveDepth(BoxCollider myCollider, BoxCollider hitCollider, Transform viewTransform)
    {
        _myCollider = myCollider;

        _hitCollider = hitCollider;

        _viewTransform = viewTransform;

        _hitSurfaceData = GetHitSurfaceData(GetVertexPositionList(_myCollider.transform, _myCollider), _hitCollider);
    }

    private BoxCollider _myCollider = default;

    private BoxCollider _hitCollider = default;

    private Transform _viewTransform = default;

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

    public void HitCollider()
    {
        Vector3[] myVertexes = GetVertexPositionList(_myCollider.transform, _myCollider);

        _hitSurfaceData = GetHitSurfaceData(myVertexes, _hitCollider);
    }

    public void MoveDepth()
    {
        //if (Input.GetKeyDown(KeyCode.Return))
        //{
            Vector3[] colliderVertexes = GetVertexPositionList(_myCollider.transform, _myCollider);

            //_hitSurfaceData = GetHitSurfaceData(myVertexes, _hitCollider);

            ProjectionData[] projectionDatas = GetProjectionData(_hitCollider, colliderVertexes);

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

            Vector3 moveVector = default;

            //Debug.Log($"実体頂点:0<{colliderVertexes[0]}>,1<{colliderVertexes[1]}>,2<{colliderVertexes[2]}>,3<{colliderVertexes[3]}>,4<{colliderVertexes[4]}>,5<{colliderVertexes[5]}>,6<{colliderVertexes[6]}>,7<{colliderVertexes[7]}>");
            //Debug.Log($"射影頂点:0<{projectionDatas[0].position}>,1<{projectionDatas[1].position}>,2<{projectionDatas[2].position}>,3<{projectionDatas[3].position}>,4<{projectionDatas[4].position}>,5<{projectionDatas[5].position}>,6<{projectionDatas[6].position}>,7<{projectionDatas[7].position}>");
            //Debug.Log($"深度:0<{projectionDatas[0].depth}>,1<{projectionDatas[1].depth}>,2<{projectionDatas[2].depth}>,3<{projectionDatas[3].depth}>,4<{projectionDatas[4].depth}>,5<{projectionDatas[5].depth}>,6<{projectionDatas[6].depth}>,7<{projectionDatas[7].depth}>");

            // 頂点
            if (CheckOnHitSurface(projectionDatas[deepestIndex]))
            {
                //Debug.Log($"ちょーてん　{deepestIndex}番目");
                moveVector = GetMoveDepth(projectionDatas[deepestIndex].depth);
            }
            // その他
            else
            {
                //Debug.Log("へんとか");
                
                float intersectDepth = GetDeepestIntersectDepth(colliderVertexes, projectionDatas);

                float intoSurfaceDepth = GetDeepestIntoSurfaceDepth(colliderVertexes, projectionDatas);

                //Debug.Log("へん：" + intersectDepth + "  面：" + intoSurfaceDepth);

                if (intoSurfaceDepth <= intersectDepth)
                {
                    moveVector = GetMoveDepth(intersectDepth);
                }
                else
                {
                    moveVector = GetMoveDepth(intoSurfaceDepth);
                }
            }
        //Debug.Log("むーぶべくたー： x=" + moveVector.x + " , y=" + moveVector.y + " z=" + moveVector.z);

            _viewTransform.position = _myCollider.transform.position + moveVector;
        //}
    }

    private ProjectionData[] GetProjectionData(BoxCollider hitCollider, Vector3[] vertexList)
    {
        ProjectionData[] projectionDatas = new ProjectionData[vertexList.Length];

        for (int i = 0; i < projectionDatas.Length; i++)
        {
            Vector3 sampleVector = vertexList[i] - _hitSurfaceData.vertexList[0];

            Vector3 projectionVector = _hitSurfaceData.normal * Vector3.Dot(sampleVector, _hitSurfaceData.normal);

            projectionDatas[i].position = vertexList[i] - projectionVector;

            float projectionDistance = Vector3.Distance(projectionDatas[i].position, vertexList[i]);

            if (Vector3.Dot(projectionVector, _hitSurfaceData.normal) > 0)
            {
                projectionDistance = -projectionDistance;
            }

            projectionDatas[i].depth = projectionDistance;
        }

        return projectionDatas;
    }

    private Vector3 GetMoveDepth(ProjectionData projectionData)
    {
        return _hitSurfaceData.normal * projectionData.depth;
    }

    private Vector3 GetMoveDepth(float projectionData)
    {
        //Debug.Log($"設定深度：{projectionData}");
        return _hitSurfaceData.normal * projectionData;
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

        //Debug.Log($"<color=red>面法線 : {hitNormal}</color>");

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

    private float GetDeepestIntersectDepth(Vector3[] colliderVertexes, ProjectionData[] projectionDatas)
    {
        Vector3[] checkLineVertexes = new Vector3[2];

        Vector3[] projectionVertexes = new Vector3[projectionDatas.Length];

        for (int i = 0; i < projectionVertexes.Length; i++)
        {
            projectionVertexes[i] = projectionDatas[i].position;
        }

        float deepestDepth = default;

        for (int i = 0; i < 4; i++)
        {
            int[] checkLineVertexesIndex = BoxColliderData.ConnectiongSurfaceVertexesIndexes(i);

            checkLineVertexes[0] = _hitSurfaceData.vertexList[checkLineVertexesIndex[0]];
            checkLineVertexes[1] = _hitSurfaceData.vertexList[checkLineVertexesIndex[1]];

            //Debug.Log($"{i}辺　判定面の頂点の座標0:{checkLineVertexes[0]}");
            //Debug.Log($"{i}辺　判定面の頂点の座標1:{checkLineVertexes[1]}");

            for (int k = 0; k < 12; k++)
            {
                //Debug.Log($"辺の番号：{BoxColliderData.GetLineIndexes(k)}");
                if(FindClosestPoints(projectionVertexes, BoxColliderData.GetLineIndexes(k), checkLineVertexes, out Vector3 closestPointOnCheckLine, out float closestRatio))
                {
                    float depth = GetIntersectDepth(colliderVertexes, BoxColliderData.GetLineIndexes(k), closestPointOnCheckLine, closestRatio);

                    //Debug.Log($"{k}番目　当たってる　深度{depth}");

                    if (deepestDepth < depth)
                    {
                        deepestDepth = depth;
                    }
                }
                else
                {
                    //Debug.Log($"{k}番目　当たってない");
                }
            }
        }

        return deepestDepth;
    }

    private bool FindClosestPoints(Vector3[] colliderVertexes, int[] lineIndexes, Vector3[] checkLineVertexes, out Vector3 closestPointOnCheckLine, out float closestRatio)
    {
        Vector3 colliderVertex1 = colliderVertexes[lineIndexes[0]];
        Vector3 colliderVertex2 = colliderVertexes[lineIndexes[1]];

        Vector3 checkLineVertex1 = checkLineVertexes[0];
        Vector3 checkLineVertex2 = checkLineVertexes[1];

        Vector3 direction_ColliderLine = colliderVertex2 - colliderVertex1;
        Vector3 direction_CheckLine = checkLineVertex2 - checkLineVertex1;
        Vector3 temporaryLine = colliderVertex1 - checkLineVertex1;

        float dot_ColliderLine = Vector3.Dot(direction_ColliderLine, direction_ColliderLine);
        float dot_CheckLine = Vector3.Dot(direction_CheckLine, direction_CheckLine);
        float dot_CheckToTemporary = Vector3.Dot(direction_CheckLine, temporaryLine);

        float ratio_ColliderLine = default;
        float ratio_CheckLine = default;

        float dot_ColliderToTemporary = Vector3.Dot(direction_ColliderLine, temporaryLine);
        float dot_ColliderToCheck = Vector3.Dot(direction_ColliderLine, direction_CheckLine);
        float normalizer = dot_ColliderLine * dot_CheckLine - dot_ColliderToCheck * dot_ColliderToCheck;

        if (normalizer != 0f)
        {
            ratio_ColliderLine = (dot_ColliderToCheck * dot_CheckToTemporary - dot_ColliderToTemporary * dot_CheckLine) / normalizer;
        }
        else
        {
            ratio_ColliderLine = 0f;
        }

        ratio_CheckLine = (dot_ColliderToCheck * ratio_ColliderLine + dot_CheckToTemporary) / dot_CheckLine;

        closestPointOnCheckLine = checkLineVertex1 + ratio_CheckLine * direction_CheckLine;
        closestRatio = ratio_ColliderLine;

        //Debug.Log($"ratio_ColliderLine = {ratio_ColliderLine}  ,  ratio_CheckLine = {ratio_CheckLine}");

        if (ratio_ColliderLine <= 0 || 1 <= ratio_ColliderLine || ratio_CheckLine <= 0 || 1 <= ratio_CheckLine)
        {
            return false;
        }

        return true;
    }

    private float GetIntersectDepth(Vector3[] colliderVertexes, int[] lineIndexes, Vector3 closestPointOnCheckLine, float closestRatio)
    {
        Vector3 lineVector = colliderVertexes[lineIndexes[1]] - colliderVertexes[lineIndexes[0]];

        Vector3 closestPointOnColliderLine = colliderVertexes[lineIndexes[0]] + lineVector * closestRatio;

        Vector3 depthVector = closestPointOnColliderLine - closestPointOnCheckLine;

        //Debug.Log($"<color=red>LineVector={lineVector}  :  ClosestPointOnColliderLine={closestPointOnColliderLine}  :  ClosestPointOnCheckLine={closestPointOnCheckLine}  :  depthVector={depthVector}</color>");

        float depth = depthVector.magnitude;

        if (Vector3.Dot(depthVector, _hitSurfaceData.normal) >= 0)
        {
            depth *= -1;
        }

        return depth;
    }

    private float GetDeepestIntoSurfaceDepth(Vector3[] colliderVertexes, ProjectionData[] projectionDatas)
    {
        float deepestDepth = 0;

        Vector3[] projectionVertexes = new Vector3[projectionDatas.Length];

        Vector3[] surfaceNormalList = BoxColliderData.GetColliderNormals(_hitCollider.transform);

        Vector3 projectionVector = _hitSurfaceData.normal;

        for (int i = 0; i < projectionVertexes.Length; i++)
        {
            projectionVertexes[i] = projectionDatas[i].position;
        }

        int[,] intoSurfaceList = GetIntoSurfaceList(_hitSurfaceData.vertexList, projectionVertexes);

        if (intoSurfaceList == default)
        {
            //Debug.Log("めんにちょーてんあたってないよん");
            return default;
        }

        for (int i = 0; i < intoSurfaceList.GetLength(0); i++)
        {
            //Debug.Log($"頂点:{intoSurfaceList[i, 0]}   面：{intoSurfaceList[i, 1]}");
            int[] projectionSurfaceVertexes = BoxColliderData.GetSurfaceIndexes(intoSurfaceList[i, 1]);

            Vector3 surfaceNormal = surfaceNormalList[intoSurfaceList[i, 1]];

            Vector3 checkVertex = _hitSurfaceData.vertexList[intoSurfaceList[i, 0]];

            float projectionDot = Vector3.Dot(colliderVertexes[projectionSurfaceVertexes[0]] - checkVertex, surfaceNormal) / Vector3.Dot(projectionVector, surfaceNormal);

            Vector3 projectionPoint = checkVertex - projectionDot * projectionVector;

            Vector3 depthVector = checkVertex - projectionPoint;

            float depth = depthVector.magnitude;

            if (Vector3.Dot(depthVector, projectionVector) <= 0)
            {
                depth *= -1;
            }

            if (deepestDepth < depth)
            {
                deepestDepth = depth;
            }
        }

        return deepestDepth;
    }

    private int[,] GetIntoSurfaceList(Vector3[] checkVertexList, Vector3[] projectionVertexes)
    {
        bool[,] intoSurfaceStats = new bool[checkVertexList.Length,6];

        int intoCount = 0;

        for (int i = 0; i < checkVertexList.Length; i++)
        {
            if (CheckIntoSurface(checkVertexList[i], projectionVertexes, out int[] intoSurfaceIndex))
            {
                for (int k = 0; k < intoSurfaceIndex.Length; k++)
                {
                    intoSurfaceStats[i, intoSurfaceIndex[k]] = true;
                    intoCount++;
                }
            }
        }

        if (intoCount <= 0)
        {
            return default;
        }

        int[,] intoSurfaceList = new int[intoCount, 2];

        int listIndex = 0;

        for (int i = 0; i < intoSurfaceStats.GetLength(0); i++)
        {
            for (int k = 0; k < intoSurfaceStats.GetLength(1); k++)
            {
                if (intoSurfaceStats[i, k])
                {
                    intoSurfaceList[listIndex, 0] = i;
                    intoSurfaceList[listIndex, 1] = k;
                    listIndex++;
                }
            }
        }

        return intoSurfaceList;
    }

    private bool CheckIntoSurface(Vector3 checkVertexPosition, Vector3[] projectionVertexes, out int[] intoSurfaceIndex)
    {
        int[] intoList = new int[6];

        int intoCount = 0;

        for (int i = 0; i < 6; i++)
        {
            int[] surfaceIndexes = BoxColliderData.GetSurfaceIndexes(i);

            Vector3 firstLineVector = projectionVertexes[surfaceIndexes[1]] - projectionVertexes[surfaceIndexes[0]];

            Vector3 secondLineVector = projectionVertexes[surfaceIndexes[surfaceIndexes.Length - 1]] - projectionVertexes[surfaceIndexes[0]];

            Vector3 checkVector = checkVertexPosition - projectionVertexes[surfaceIndexes[0]];

            float firstDot = Vector3.Dot(checkVector, firstLineVector) / Vector3.Dot(firstLineVector, firstLineVector);

            float secondDot = Vector3.Dot(checkVector, secondLineVector) / Vector3.Dot(secondLineVector, secondLineVector);

            if (0 <= firstDot && firstDot <= 1 && 0 <= secondDot && secondDot <= 1)
            {
                intoList[intoCount] = i;
                intoCount++;
            }
        }

        intoSurfaceIndex = new int[intoCount];

        for (int i = 0; i < intoCount; i++)
        {
            intoSurfaceIndex[i] = intoList[i];
        }

        return intoCount > 0;
    }
}
