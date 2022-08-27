using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//класс нужен чисто для получения урона от игрока, т.к. 
//в скелете кости находятся в разных местах в иерархии

public class PartOfBody : MonoBehaviour
{
    [SerializeField]
    private Stats _entityStats;

    public bool RecieveDamage(float damage)
    {
        return _entityStats.RecieveHit(damage);
    }
}
