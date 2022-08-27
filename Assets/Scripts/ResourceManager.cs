using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
// Если бы в игре было бы несколько языков - я бы засунул сюда
// считывание из XML, тут был бы Dictionary, из которого брались бы строки
//

public class ResourceManager : MonoBehaviour
{

    #region Singleton
    public static ResourceManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("Resources instance error!");
            return;
        }
        instance = this;
    }
    #endregion

    private System.Random _rng;

    public System.Random Rng { get => _rng; }

    // Start is called before the first frame update
    void Start()
    {
        _rng = new System.Random();
        //игнор всех коллизий для HitCollider, КРОМЕ Player (13)
        Physics.IgnoreLayerCollision(0, 16);
        Physics.IgnoreLayerCollision(10, 16);
        Physics.IgnoreLayerCollision(14, 16);
        Physics.IgnoreLayerCollision(15, 16);
        Physics.IgnoreLayerCollision(11, 16);
        Physics.IgnoreLayerCollision(16, 16);
    }

}
