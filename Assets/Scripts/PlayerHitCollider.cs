using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitCollider : MonoBehaviour
{
    [SerializeField]
    private float _damage;

    private void OnTriggerEnter(Collider collider)
    {
        collider.transform.GetComponent<Stats>().RecieveHit(_damage);
        collider.transform.GetComponent<ImpactReceiver>().AddImpact(transform.forward, 1f);
    }
}
