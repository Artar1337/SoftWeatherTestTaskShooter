using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
// ���� �� � ���� ���� �� ��������� ������ - � �� ������� ����
// ���������� �� XML, ��� ��� �� Dictionary, �� �������� ������� �� ������
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
        //����� ���� �������� ��� HitCollider, ����� Player (13)
        Physics.IgnoreLayerCollision(0, 16);
        Physics.IgnoreLayerCollision(10, 16);
        Physics.IgnoreLayerCollision(14, 16);
        Physics.IgnoreLayerCollision(15, 16);
        Physics.IgnoreLayerCollision(11, 16);
        Physics.IgnoreLayerCollision(16, 16);
    }

}
