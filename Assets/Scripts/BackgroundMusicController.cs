using System.Collections;
using UnityEngine;

//
// контроллер музыки на фоне
//

public class BackgroundMusicController : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] _clips;
    private AudioSource _src;

    //если не включен звук - убираем возможность слышать звуки через audiolistener
    //если не включена музыка - не включаем корутину
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

    //бесконечно крутим по кругу треки (начало на рандомном треке, порядок - всегда один)
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
