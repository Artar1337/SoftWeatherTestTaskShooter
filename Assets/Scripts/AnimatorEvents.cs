using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorEvents : MonoBehaviour
{
    [SerializeField]
    private bool _shouldBeDestroyedAfterNSeconds = false;
    [SerializeField]
    private float _secondsToLive = 60f;
    [SerializeField]
    private AudioClip[] _clips;

    private AudioSource _src;
    private PlayerMovement _playerMovement;

    private void Start()
    {
        TryGetComponent(out _src);
        if (_shouldBeDestroyedAfterNSeconds)
            StartCoroutine(DestroyAfterSeconds(_secondsToLive));
        _playerMovement = GameObject.Find("FPS Controller").GetComponent<PlayerMovement>();
    }

    public void PlaySound(AudioClip clip)
    {
        _src.PlayOneShot(clip);
    }

    public void PlayRandomSoundFromClipsIfIsOnGround()
    {
        if(_playerMovement.IsGrounded)
            _src.PlayOneShot(_clips[ResourceManager.instance.Rng.Next(0, _clips.Length)]);
    }

    private IEnumerator DestroyAfterSeconds(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }

    public void LoadMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu", 0);
    }
}
