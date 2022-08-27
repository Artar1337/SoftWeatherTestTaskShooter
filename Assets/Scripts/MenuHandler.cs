using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MenuHandler : MonoBehaviour
{
    [SerializeField]
    private bool _isMain = false;
    GameObject _canvas, _main, _settings, _loading;
    Toggle _music;
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

    public void SetMusicStatus(bool status)
    {
        if (!status)
            PreferencesHandler.instance.Music = -1;
        else
            PreferencesHandler.instance.Music = 1;
    }

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

    public void SetSens(float sens)
    {
        PreferencesHandler.instance.Sens = (int)sens;
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void OpenMenu(bool main)
    {
        _main.SetActive(main);
        _settings.SetActive(!main);
    }

    public void StartGame()
    {
        StartCoroutine(LoadCoroutine());
    }

    public void ResetScore()
    {
        PreferencesHandler.instance.ResetRecord();
        _main.transform.Find("Best Score").GetComponent<TMPro.TMP_Text>().text =
            "BEST SCORE: 0";
    }

    private IEnumerator LoadCoroutine()
    {
        _loading.SetActive(true);
        yield return null;
        SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }
}
