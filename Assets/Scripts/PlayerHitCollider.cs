using UnityEngine;

//
//класс нужен чисто для получения урона игроком от врага
//

public class PlayerHitCollider : MonoBehaviour
{
    [SerializeField]
    private float _damage;

    private void OnTriggerEnter(Collider collider)
    {
        //наносим урон
        collider.transform.GetComponent<Stats>().RecieveHit(_damage);
        //откидываем игрока в сторону
        collider.transform.GetComponent<ImpactReceiver>().AddImpact(transform.forward, 1f);
    }
}
