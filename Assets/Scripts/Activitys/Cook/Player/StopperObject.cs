using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopperObject : MonoBehaviour
{
    [SerializeField, Tooltip("ÚG”»’è‚ğs‚¤Collider")]
    private Collider _knifeCollider = default;

    private void Update()
    {
        // ÚG‚µ‚½Collider‚ğ”»’è‚µ‚ÄŠi”[‚·‚é
        Collider[] hitColliders = Physics.OverlapBox(_knifeCollider.bounds.center, _knifeCollider.bounds.size, this.transform.rotation);

        // ÚG‚µ‚½Collider‚ª‚È‚©‚Á‚½ê‡
        if (hitColliders is null)
        {
            // ‚È‚É‚à‚µ‚È‚¢
            Debug.Log($"‚È‚É‚à“–‚½‚Á‚Ä‚È‚¢‚æ‚ñ");
            return;
        }

        // ÚG‚µ‚½Collider‚·‚×‚Ä‚É”»’è‚ğs‚¤
        foreach (Collider hitCollider in hitColliders)
        {
            // Stoppable‚ğ‚Á‚Ä‚¢‚È‚¢ê‡
            if (!hitCollider.transform.root.TryGetComponent<Stoppable>(out var stoppable))
            {
                // Ÿ‚ÌCollider‚Ö
                continue;
            }

            // StopData‚ğ‚Á‚Ä‚¢‚éê‡
            if (hitCollider.transform.root.TryGetComponent<StopData>(out var stopData))
            {
                // StopData‚Ì’â~ƒtƒ‰ƒO‚ğ—§‚Ä‚é
                stopData.StopEnd(true);
            }
            // StopData‚ğ‚Á‚Ä‚¢‚È‚¢ê‡
            else
            {
                // ÚG‚µ‚Ä‚¢‚éƒIƒuƒWƒFƒNƒg‚ÉStopData‚ğ‰Á‚¦‚é
                hitCollider.transform.root.gameObject.AddComponent<StopData>();

                // ’â~‚Ìˆ—‚ğÀs‚·‚é
                stoppable.StoppingEvent();
            }
        }
    }
}
