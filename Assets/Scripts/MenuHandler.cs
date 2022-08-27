using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//
// обработчик событий в меню
//

public class MenuHandler : MonoBehaviour
{
    //если true - то этот обработчик стоит на корневом Canvas
    [SerializeField]
    private bool _isMain = false;
    //объекты корневого canvas, главного экрана, настроек и загрузки
    private GameObject _canvas, _main, _settings, _loading;
    //тогглбокс для музыки
    private Toggle _music;
    // Start is called before the first frame update
    void Start()
    {
        _canvas = gameObject;
        if(!_isMain)
            _canvas = GameObject.Find("Canvas");
        _main = _canvas.transform.Find("Main").gameObject;
        _main.transform.Find("Best Score").GetComponent<TMPro.TMP_Text>().text = 
            "BEST SCORE:\n" + PreferencesHandler.instance.Record.ToString();
        _settings = _canvas.transform.Find("Settings").gameObject;
        _loading = _canvas.transform.Find("Loading").gameObject;
        _music = _settings.transform.Find("Music").GetComponent<Toggle>();
        if (!_isMain)
            return;
        _music.isOn = PreferencesHandler.instance.Music > 0;
        _settings.transform.Find("Sound").GetComponent<Toggle>().isOn =
            PreferencesHandler.instance.Sound > 0;
        _settings.transform.Find("Sens").GetComponent<Slider>().value =
            PreferencesHandler.instance.Sens;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    //вкл/выкл музыку
    public void SetMusicStatus(bool status)
    {
        if (!status)
            PreferencesHandler.instance.Music = -1;
        else
            PreferencesHandler.instance.Music = 1;
    }

    //вкл/выкл звук (также включает/выключает и музыку)
    public void SetSoundStatus(bool status)
    {
        if (!status)
        {
            PreferencesHandler.instance.Sound = -1;
            PreferencesHandler.instance.Music = -1;
            if (_music == null)
                return;
            _music.isOn = false;
            _music.enabled = false;
        }
        else
        {
            PreferencesHandler.instance.Sound = 1;
            PreferencesHandler.instance.Music = 1;
            if (_music == null)
                return;
            _music.isOn = true;
            _music.enabled = true;
        }
    }

    //устанавливает чувствительность мыши
    public void SetSens(float sens)
    {
        PreferencesHandler.instance.Sens = (int)sens;
    }

    //выход из приложухи
    public void Exit()
    {
        Application.Quit();
    }

    //открывает/закрывает главное меню/настройки
    public void OpenMenu(bool main)
    {
        _main.SetActive(main);
        _settings.SetActive(!main);
    }

    //старт игры
    public void StartGame()
    {
        StartCoroutine(LoadCoroutine());
    }

    //сброс счета
    public void ResetScore()
    {
        PreferencesHandler.instance.ResetRecord();
        _main.transform.Find("Best Score").GetComponent<TMPro.TMP_Text>().text =
            "BEST SCORE: 0";
    }

    //загрузка игры
    private IEnumerator LoadCoroutine()
    {
        //активируем объект "загрузка"
        _loading.SetActive(true);
        //ждем один фрейм
        yield return null;
        //и только потом грузимся на уровень
        SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }
}
