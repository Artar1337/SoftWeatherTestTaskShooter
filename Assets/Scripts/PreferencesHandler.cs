using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// здесь обрабатываются записи из реестра

public class PreferencesHandler : MonoBehaviour
{

    #region Singleton
    public static PreferencesHandler instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("PREFS instance error!");
            return;
        }
        instance = this;
    }
    #endregion

    private string _record = "Record", _sens = "Sens", _sound = "Sound", _music = "Music";

    //рекорд хранится просто в реестре
    public int Record { 
        get
        {
            return GetIntParameter(_record);
        }
        set
        {
            if (GetIntParameter(_record) < value)
                SetIntParameter(_record, value);
        }
    }

    public void ResetRecord()
    {
        SetIntParameter(_record, 0);
    }

    //чувствительность мышки, меняем от 10 до 1000
    public int Sens
    {
        get
        {
            if (GetIntParameter(_sens) <= 0)
                SetIntParameter(_sens, 500);
            return GetIntParameter(_sens);
        }
        set
        {
            SetIntParameter(_sens, value);
        }
    }

    //-1 - выкл, 1 - вкл
    public int Sound
    {
        get
        {
            if (GetIntParameter(_sound) == 0)
                SetIntParameter(_sound, 1);
            return GetIntParameter(_sound);
        }
        set
        {
            SetIntParameter(_sound, value);
        }
    }

    //-1 - выкл, 1 - вкл
    public int Music
    {
        get
        {
            if (GetIntParameter(_music) == 0)
                SetIntParameter(_music, 1);
            return GetIntParameter(_music);
        }
        set
        {
            SetIntParameter(_music, value);
        }
    }

    private int GetIntParameter(string name)
    {
        if (!PlayerPrefs.HasKey(name))
            PlayerPrefs.SetInt(name, 0);
        return PlayerPrefs.GetInt(name);
    }

    private void SetIntParameter(string name, int value)
    {
        PlayerPrefs.SetInt(name, value);
    }

}
