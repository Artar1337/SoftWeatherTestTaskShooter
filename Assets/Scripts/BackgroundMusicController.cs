using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusicController : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] _clips;
    private AudioSource _src;

    // Start is called before the first frame update
    void Start()
    {
        AudioListener.volume = 1;
        if (PreferencesHandler.instance.Sound < 0)
            AudioListener.volume = 0;
        if (PreferencesHandler.instance.Music > 0)
        {
            _src = GetComponent<AudioSource>();
            StartCoroutine(SoundCoroutine());
        }
    }

    IEnumerator SoundCoroutine()
    {
        int index = ResourceManager.instance.Rng.Next(0, _clips.Length);
        //бесконечно крутим по кругу треки, пока идет игра
        while (true)
        {
            if (index >= _clips.Length)
                index = 0;
            _src.PlayOneShot(_clips[index]);
            //ждем, пока играет музычка
            yield return new WaitWhile(() => _src.isPlaying);
            //пять секунд тишины, просто так)
            yield return new WaitForSeconds(5f);
            index++;
        }
    }
}
