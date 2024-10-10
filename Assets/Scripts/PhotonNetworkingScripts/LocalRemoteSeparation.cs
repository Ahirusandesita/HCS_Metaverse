using UnityEngine;
using Fusion;
using Layer_lab._3D_Casual_Character;
using Cysharp.Threading.Tasks;
public class LocalRemoteSeparation : MonoBehaviour
{
    [SerializeField]
    private SeparationLifetimeScope separationLifetimeScope;

    [SerializeField]
    private GameObject localGameObject;

    [SerializeField]
    private NetworkPrefabRef remoteViewObjectPrefab;

    private RemoteView remoteViewInstance = null;
    public async void RemoteViewCreate(NetworkRunner networkRunner, PlayerRef playerRef)
    {
        NetworkObject remoteViewObject
            =await networkRunner.SpawnAsync(remoteViewObjectPrefab, Vector3.zero, Quaternion.identity);
        RemoteView remoteView = remoteViewObject.GetComponent<RemoteView>();

        if (playerRef == networkRunner.LocalPlayer)
        {
            CharacterControl characterController = remoteView.GetComponentInChildren<CharacterControl>();
            foreach (Renderer renderer in characterController.GetComponentsInChildren<Renderer>())
            {
                renderer.gameObject.SetActive(false);
            }
        }
        Instantiate(separationLifetimeScope).SeparationSetup(localGameObject, remoteView).Build();
        remoteViewInstance = remoteView;
    }
    private void Update()
    {
        Debug.LogError(existInstance);
    }
    public async UniTask<RemoteView> unko()
    {
        await UniTask.WaitUntil(() => remoteViewInstance);
        return remoteViewInstance;
    }
}