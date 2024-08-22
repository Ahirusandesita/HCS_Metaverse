using UnityEngine;

public abstract class InitializeBase : MonoBehaviour
{
    public abstract void Initialize();
}
#if UNITY_EDITOR
public static class InitializeAssetDatabase
{
    public static string[] Find()
    {
        string[] guids = UnityEditor.AssetDatabase.FindAssets("t:InitializeAsset");
        if (guids.Length == 0)
        {
            throw new System.IO.FileNotFoundException("InitializeAsset does not found");
        }
        return guids;
    }

    public static InitializeAsset LoadAssetAtPathFromGuid(string guid)
    {
        string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
        return UnityEditor.AssetDatabase.LoadAssetAtPath<InitializeAsset>(path);
    }

    public static string GUIDToAssetPath(string guid)
    {
        return UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
    }

    public static InitializeAsset LoadAssetAtPath(string path)
    {
        return UnityEditor.AssetDatabase.LoadAssetAtPath<InitializeAsset>(path);
    }
}

public static class CommodityAssetDatabase
{
    public static string[] Find()
    {
        string[] guids = UnityEditor.AssetDatabase.FindAssets("t:CommodityAsset");
        if (guids.Length == 0)
        {
            throw new System.IO.FileNotFoundException("InitializeAsset does not found");
        }
        return guids;
    }

    public static CommodityAsset LoadAssetAtPathFromGuid(string guid)
    {
        string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
        return UnityEditor.AssetDatabase.LoadAssetAtPath<CommodityAsset>(path);
    }

    public static string GUIDToAssetPath(string guid)
    {
        return UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
    }

    public static CommodityAsset LoadAssetAtPath(string path)
    {
        return UnityEditor.AssetDatabase.LoadAssetAtPath<CommodityAsset>(path);
    }
}
#endif