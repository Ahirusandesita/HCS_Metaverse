using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Cysharp.Threading.Tasks;
using HCSMeta.Function.Initialize.Interface;

namespace HCSMeta.Function.Touch
{
    public interface ICanvasDeploymentAndConvergence
    {
        /// <summary>
        /// “WŠJ
        /// </summary>
        void Deployment();
        /// <summary>
        /// Žû‘©
        /// </summary>
        void Convergence();
        /// <summary>
        /// “WŠJ’†‚©
        /// </summary>
        bool IsDeployment { get; }
    }
}
namespace HCSMeta.Function.Touch
{
    public class DebutButton : MonoBehaviour, IInjectableSpecificType
    {
        private IAvailableSpecificType availableSpecificType;
        private OVRCanvasManager OVRCanvasManager;

        public void Inject(IAvailableSpecificType availableSpecificType)
        {
            this.availableSpecificType = availableSpecificType;
            FindOVRCanvasManager().Forget();
        }

        public void Selected()
        {
            OVRCanvasManager.ChangeCanvasDeployment();
        }

        private async UniTaskVoid FindOVRCanvasManager()
        {
            OVRCanvasManager = await availableSpecificType.WaitForSpecificTypeAsync<OVRCanvasManager>();
        }
    }
}