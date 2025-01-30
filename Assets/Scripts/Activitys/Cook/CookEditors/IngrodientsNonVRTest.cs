using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
[CustomEditor(typeof(Ingrodients))]//Šg’£‚·‚éƒNƒ‰ƒX‚ðŽw’è
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
#endif