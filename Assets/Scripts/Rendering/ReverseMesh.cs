using UnityEngine;
using System.Linq;

public class ReverseMesh : MonoBehaviour
{
    /// <summary>
    /// ���b�V�����]�����s����
    /// </summary>
    void Start()
    {
        // ���b�V�����擾
        Mesh mesh = GetComponent<MeshFilter>().mesh;

        // �|���S���𔽓]�����čđ��
        mesh.triangles = mesh.triangles.Reverse().ToArray();
    }
}
