using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
[CustomEditor(typeof(ReticleInitialize))]
public class ReticleDependencyInjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("依存データを注入"))
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

        if (GUILayout.Button("依存データを注入"))
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

        if (GUILayout.Button("依存データを注入"))
        {
            PlayerInitialize playerInitialize = (PlayerInitialize)target;
            playerInitialize.Inject();
        }
    }
}

#endif