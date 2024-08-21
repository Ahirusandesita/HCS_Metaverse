using UnityEngine;

namespace HCSMeta.Function.Initialize
{
    public class ReticleInitialize : InitializeBase
    {
        [SerializeField]
        private HandType handType;

        [SerializeField]
        private InitializeAsset initialize;

        [SerializeField, InterfaceType(typeof(IDependencyProvider<ReticleDependencyInformation>))]
        private UnityEngine.Object ReticleDependencyProvider;

        private IDependencyProvider<ReticleDependencyInformation> reticleDependencyProvider => ReticleDependencyProvider as IDependencyProvider<ReticleDependencyInformation>;

        private IDependencyInjector<ReticleDependencyInformation>[] dependencyInjectors;


        private void Awake()
        {
            foreach (GameObject gameObject in initialize.InitializeObjects)
            {
                GameObject instance = Instantiate(gameObject);

                dependencyInjectors = instance.GetComponentsInChildren<IDependencyInjector<ReticleDependencyInformation>>();

                foreach (IDependencyInjector<ReticleDependencyInformation> dependencyInjector in dependencyInjectors)
                {
                    dependencyInjector.Inject(reticleDependencyProvider.Information);
                }
            }
        }

        public override void Initialize()
        {
#if UNITY_EDITOR
            ReticleDependencyProvider[] reticleDependencyProviders = GameObject.FindObjectsOfType<ReticleDependencyProvider>();

            foreach (ReticleDependencyProvider @object in reticleDependencyProviders)
            {
                if (@object.HandType == handType)
                {
                    ReticleDependencyProvider = @object;
                }
            }

            string[] guids = InitializeAssetDatabase.Find();
            foreach (string guid in guids)
            {
                InitializeAsset asset = InitializeAssetDatabase.LoadAssetAtPathFromGuid(guid);

                if (asset.InitializeType == this)
                {
                    initialize = asset;
                }
            }

            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

        public static bool operator ==(InitializeType initializeTye, ReticleInitialize reticleInitialize)
        {
            return reticleInitialize.handType == HandType.Left && initializeTye == InitializeType.ReticleLeftHand || reticleInitialize.handType == HandType.Right && initializeTye == InitializeType.ReticleRightHand;
        }
        public static bool operator !=(InitializeType initializeTye, ReticleInitialize reticleInitialize)
        {
            return !(reticleInitialize.handType == HandType.Left && initializeTye == InitializeType.ReticleLeftHand || reticleInitialize.handType == HandType.Right && initializeTye == InitializeType.ReticleRightHand);
        }

        public override bool Equals(object obj) => this.Equals(obj as ReticleInitialize);
        public override int GetHashCode() => GetHashCode();
    }
}
