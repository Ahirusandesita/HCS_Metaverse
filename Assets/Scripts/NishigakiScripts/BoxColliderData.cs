using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BoxColliderData
{
    public static Vector3[] GetColliderNormals(Transform colliderTransform)
    {
        Vector3[] hitColliderNormals = new Vector3[6]
        {
            colliderTransform.right,
            -colliderTransform.right,
            colliderTransform.up,
            -colliderTransform.up,
            colliderTransform.forward,
            -colliderTransform.forward
        };

        return hitColliderNormals;
    }

    public static int[] GetSurfaceIndexes(int surfaceIndex)
    {
        switch (surfaceIndex)
        {
            case 0:
                return new int[] { 0, 1, 3, 2 };

            case 1:
                return new int[] { 7, 6, 4, 5 };

            case 2:
                return new int[] { 0, 1, 5, 4 };

            case 3:
                return new int[] { 7, 3, 2, 6 };

            case 4:
                return new int[] { 0, 2, 6, 4 };

            case 5:
                return new int[] { 7, 5, 1, 3 };

            default:
                // 例外
                return default;
        }
    }

    public static int[] GetLineIndexes(int lineIndex)
    {
        switch (lineIndex)
        {
            case 0:
                return new int[] { 0, 1 };

            case 1:
                return new int[] { 0, 2 };

            case 2:
                return new int[] { 0, 4 };

            case 3:
                return new int[] { 1, 3 };

            case 4:
                return new int[] { 1, 5 };

            case 5:
                return new int[] { 2, 3 };

            case 6:
                return new int[] { 2, 6 };

            case 7:
                return new int[] { 3, 7 };

            case 8:
                return new int[] { 4, 5 };

            case 9:
                return new int[] { 4, 6 };

            case 10:
                return new int[] { 5, 7 };

            case 11:
                return new int[] { 6, 7 };

            default:
                // 例外
                return default;
        }
    }

    public static int[] GetConnectingVertexIndexes(int parentIndex)
    {
        int[] connectingVertexIndexList = new int[3]
        {
            (parentIndex ^ 1) & 2 & 4,
            (parentIndex ^ 2) & 4 & 1,
            (parentIndex ^ 4) & 1 & 2
        };

        return connectingVertexIndexList;
    }

    public static Vector3[] GetVertexesOfSurface(int surfaceIndex, BoxCollider collider)
    {
        Vector3[] vertexList = new Vector3[4];

        Vector3 center = collider.center;

        Vector3 extents = collider.size * 0.5f;

        Matrix4x4 convertMatrix = collider.transform.localToWorldMatrix;

        Vector2 indexSign = default;

        switch (surfaceIndex)
        {
            case 0:
                for (int i = 0; i < 4; i++)
                {
                    indexSign = VertexIndexSigns(i);
                    Vector3 vertexPosition = new Vector3(extents.x, extents.y * indexSign.x, extents.z * indexSign.y);
                    vertexList[i] = convertMatrix.MultiplyPoint3x4(center + vertexPosition);
                    Debug.Log(i + "番目： x = " + vertexList[i].x + " , y = " + vertexList[i].y + " , z = " + vertexList[i].z);

                }
                break;

            case 1:
                for (int i = 0; i < 4; i++)
                {
                    indexSign = VertexIndexSigns(i);
                    Vector3 vertexPosition = new Vector3(-extents.x, extents.y * indexSign.x, extents.z * indexSign.y);
                    vertexList[i] = convertMatrix.MultiplyPoint3x4(center + vertexPosition);
                    Debug.Log(i + "番目： x = " + vertexList[i].x + " , y = " + vertexList[i].y + " , z = " + vertexList[i].z);

                }
                break;

            case 2:
                for (int i = 0; i < 4; i++)
                {
                    indexSign = VertexIndexSigns(i);
                    Vector3 vertexPosition = new Vector3(extents.x * indexSign.x, extents.y, extents.z * indexSign.y);
                    vertexList[i] = convertMatrix.MultiplyPoint3x4(center + vertexPosition);
                    Debug.Log(i + "番目： x = " + vertexList[i].x + " , y = " + vertexList[i].y + " , z = " + vertexList[i].z);

                }
                break;

            case 3:
                for (int i = 0; i < 4; i++)
                {
                    indexSign = VertexIndexSigns(i);
                    Vector3 vertexPosition = new Vector3(extents.x * indexSign.x, -extents.y, extents.z * indexSign.y);
                    vertexList[i] = convertMatrix.MultiplyPoint3x4(center + vertexPosition);
                    Debug.Log(i + "番目： x = " + vertexList[i].x + " , y = " + vertexList[i].y + " , z = " + vertexList[i].z);

                }
                break;

            case 4:
                for (int i = 0; i < 4; i++)
                {
                    indexSign = VertexIndexSigns(i);
                    Vector3 vertexPosition = new Vector3(extents.x * indexSign.x, extents.y * indexSign.y, extents.z);
                    vertexList[i] = convertMatrix.MultiplyPoint3x4(center + vertexPosition);
                    Debug.Log(i + "番目： x = " + vertexList[i].x + " , y = " + vertexList[i].y + " , z = " + vertexList[i].z);

                }
                break;

            case 5:
                for (int i = 0; i < 4; i++)
                {
                    indexSign = VertexIndexSigns(i);
                    Vector3 vertexPosition = new Vector3(extents.x * indexSign.x, extents.y * indexSign.y, -extents.z);
                    vertexList[i] = convertMatrix.MultiplyPoint3x4(center + vertexPosition);
                    Debug.Log(i + "番目： x = " + vertexList[i].x + " , y = " + vertexList[i].y + " , z = " + vertexList[i].z);

                }
                break;
        }

        return vertexList;
    }

    public static Vector2 VertexIndexSigns(int vertexIndex)
    {
        Vector2 vertexSigns = default;

        switch (vertexIndex)
        {
            case 0:
                vertexSigns.x = 1;
                vertexSigns.y = 1;
                break;

            case 1:
                vertexSigns.x = 1;
                vertexSigns.y = -1;
                break;

            case 2:
                vertexSigns.x = -1;
                vertexSigns.y = -1;
                break;

            case 3:
                vertexSigns.x = -1;
                vertexSigns.y = 1;
                break;
        }

        return vertexSigns;
    }

    public static int[] ConnectiongSurfaceVertexesIndexes(int LineIndex)
    {
        switch (LineIndex)
        {
            case 0:

                return new int[] { 0, 1 };

            case 1:

                return new int[] { 1, 2 };

            case 2:

                return new int[] { 2, 3 };

            case 3:

                return new int[] { 3, 0 };

            default:
                // 例外
                return default;
        }
    }
        
}
