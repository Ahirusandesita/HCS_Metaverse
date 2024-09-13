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

    
}
