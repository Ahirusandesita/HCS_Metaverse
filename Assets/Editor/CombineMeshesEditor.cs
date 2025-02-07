using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class CombineMeshesEditor : MonoBehaviour
{
    // �G�f�B�^���j���[�ɁuTools/Combine Selected Meshes�v��ǉ�
    [MenuItem("Tools/Combine Selected Meshes")]
    static void CombineSelectedMeshes()
    {
        // �V�[�����őI������Ă���I�u�W�F�N�g���擾
        GameObject[] selectedObjects = Selection.gameObjects;
        if (selectedObjects.Length == 0)
        {
            Debug.LogWarning("�I�u�W�F�N�g���I������Ă��܂���B");
            return;
        }

        List<CombineInstance> combineInstances = new List<CombineInstance>();
        Material sharedMaterial = null;

        // �I�����ꂽ�e�I�u�W�F�N�g���烁�b�V�����擾
        foreach (GameObject go in selectedObjects)
        {
            MeshFilter mf = go.GetComponent<MeshFilter>();
            MeshRenderer mr = go.GetComponent<MeshRenderer>();
            if (mf == null || mr == null)
            {
                Debug.LogWarning($"�I�u�W�F�N�g {go.name} �� MeshFilter �܂��� MeshRenderer ������܂���B�X�L�b�v���܂��B");
                continue;
            }

            // 1�ڂ̃I�u�W�F�N�g�̃}�e���A�����g�p�i�S�ē����}�e���A���ł��邱�Ƃ�O��j
            if (sharedMaterial == null)
            {
                sharedMaterial = mr.sharedMaterial;
            }

            // CombineInstance ���쐬
            CombineInstance ci = new CombineInstance();
            ci.mesh = mf.sharedMesh;
            // ���[�J�����烏�[���h�ւ̕ϊ��s����w��
            ci.transform = mf.transform.localToWorldMatrix;
            combineInstances.Add(ci);
        }

        if (combineInstances.Count == 0)
        {
            Debug.LogWarning("�����\�ȃ��b�V����������܂���ł����B");
            return;
        }

        // CombineInstances �z�񂩂�V�������b�V�����쐬
        Mesh combinedMesh = new Mesh();
        combinedMesh.name = "CombinedMesh";
        combinedMesh.CombineMeshes(combineInstances.ToArray());

        // �V���� GameObject ���쐬���AMeshFilter �� MeshRenderer ��ǉ�
        GameObject combinedObj = new GameObject("CombinedMesh");
        MeshFilter combinedMF = combinedObj.AddComponent<MeshFilter>();
        combinedMF.mesh = combinedMesh;
        MeshRenderer combinedMR = combinedObj.AddComponent<MeshRenderer>();
        combinedMR.sharedMaterial = sharedMaterial;

        // �I�v�V����: ���̃I�u�W�F�N�g�𖳌�������
        foreach (GameObject go in selectedObjects)
        {
            go.SetActive(false);
        }

        Debug.Log($"�������������܂����B{combineInstances.Count} �̃��b�V�����P�ɓ������܂����B");
    }
}