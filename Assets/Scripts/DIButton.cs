using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
[CustomEditor(typeof(ReticleInitialize))]
public class ReticleDependencyInjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("�ˑ��f�[�^�𒍓�"))
        {
            ReticleInitialize reticleInitialize = (ReticleInitialize)target;
            reticleInitialize.Inject();
        }
    }
}

[CustomEditor(typeof(PokeableCanvasInHandInitialize))]
public class PokeableCanvasInHandDependencyInjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("�ˑ��f�[�^�𒍓�"))
        {
            PokeableCanvasInHandInitialize pokeableCanvasInHand = (PokeableCanvasInHandInitialize)target;
            pokeableCanvasInHand.Inject();
        }
    }
}

[CustomEditor(typeof(PlayerInitialize))]
public class PlayerProviderDependencyInjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("�ˑ��f�[�^�𒍓�"))
        {
            PlayerInitialize playerInitialize = (PlayerInitialize)target;
            playerInitialize.Inject();
        }
    }
}

#endif