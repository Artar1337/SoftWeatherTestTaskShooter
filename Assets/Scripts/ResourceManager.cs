using UnityEngine;

//
// Тут только устанавливаются игноры для коллизий триггера, в котором игрок получает урон,
// а также рандом (к единому рандому удобнее обращаться, да и много рандомов не работает)
// Если бы в игре было бы несколько языков (а в реальном проекте было бы) - я бы засунул сюда
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
