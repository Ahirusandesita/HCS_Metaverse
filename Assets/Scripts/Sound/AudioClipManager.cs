using UnityEngine;

public class AudioClipManager : MonoBehaviour
{
    [System.Serializable]
    public struct AvaterSE
    {
        public AudioClip _walk;
    }

    [Header("SE")]
    public AvaterSE _avaterSE = default;

    [Header("BGM")]
    public AudioClip _MainCityBGM = default;
}
