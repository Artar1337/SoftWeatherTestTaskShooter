using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//����� ����� ����� ��� ��������� ����� �� ������, �.�. 
//� ������� ����� ��������� � ������ ������ � ��������

public class PartOfBody : MonoBehaviour
{
    [SerializeField]
    private Stats _entityStats;

    public bool RecieveDamage(float damage)
    {
        return _entityStats.RecieveHit(damage);
    }
}
