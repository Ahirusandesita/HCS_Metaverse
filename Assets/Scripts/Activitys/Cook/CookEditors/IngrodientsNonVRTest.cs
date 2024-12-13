using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Ingrodients))]//�g������N���X���w��
public class IngrodientsNonVRTest : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Ingrodients ingrodients = target as Ingrodients;
        if (EditorApplication.isPlaying)
        {
            if (GUILayout.Button("NonVRTest_GrabAndRelese"))
            {
                ingrodients.NonVRTest_GrabAndRelease();
            }
        }

    }

}