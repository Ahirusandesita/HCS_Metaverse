using UnityEngine;
using System.Linq;

public class ReverseMesh : MonoBehaviour
{
    /// <summary>
    /// メッシュ反転を実行する
    /// </summary>
    void Start()
    {
        // メッシュを取得
        Mesh mesh = GetComponent<MeshFilter>().mesh;

        // ポリゴンを反転させて再代入
        mesh.triangles = mesh.triangles.Reverse().ToArray();
    }
}
