using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

[CustomEditor(typeof(InitializeBase),true)]
public class InitializeInjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("�ˑ��f�[�^�𒍓�"))
        {
            InitializeBase Initialize = (InitializeBase)target;
            Initialize.Initialize();
        }
    }
}
#endif