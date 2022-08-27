using System.Collections;
using UnityEngine;

//
// Данный скрипт - сборник методов, как правило, нужных для вызова в animation events
//

public class AnimatorEvents : MonoBehaviour
{
    //если нужно уничтожить объект
    [SerializeField]
    private bool _shouldBeDestroyedAfterNSeconds = false;
    //время жизни, если поле выше = true
    [SerializeField]
    private float _secondsToLive = 60f;
    //массив аудиодорожек для воспроизведения через PlayRandomSoundFromClips
    [SerializeField]
    private AudioClip[] _clips;

    //источник звука
    private AudioSource _src;
    //скрипт движения игрока
    private PlayerMovement _playerMovement;

    private void Start()
    {
        TryGetComponent(out _src);
        if (_shouldBeDestroyedAfterNSeconds)
            StartCoroutine(DestroyAfterSeconds(_secondsToLive));
        _playerMovement = GameObject.Find("FPS Controller").GetComponent<PlayerMovement>();
    }

    //играем один звук
    public void PlaySound(AudioClip clip)
    {
        _src.PlayOneShot(clip);
    }

    //играем один звук из _clips, если игрок стоит на земле
    public void PlayRandomSoundFromClipsIfIsOnGround()
    {
        if(_playerMovement.IsGrounded)
            _src.PlayOneShot(_clips[ResourceManager.instance.Rng.Next(0, _clips.Length)]);
    }

    //уничтожаем объект через time секунд
    private IEnumerator DestroyAfterSeconds(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }

    //загрузка в меню
    public void LoadMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu", 0);
    }
}
