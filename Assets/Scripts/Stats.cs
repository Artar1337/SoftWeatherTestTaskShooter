using System.Collections;
using UnityEngine;

//
// класс информации о здоровье (используется врагами и игроком) 
//

[RequireComponent (typeof(AudioSource))]
public class Stats : MonoBehaviour
{
    //в долях - 1 - полное хп, 0 (и меньше) - умер
    private float _health = 1f;
    //жив?
    private bool _isAlive = true;
    //свойство для здоровья
    public float Health { get => _health; set => _health = value; }
    //жив? (но свойство)
    public bool IsAlive { get => _isAlive; }
    //источник звука для воспроизведения ЗВУКОВ БОЛИ АХАХАХ
    private AudioSource _src;
    //является ли объект игроком?
    [SerializeField]
    private bool _isPlayer = false;
    //слайдер кол-ва здоровья
    [SerializeField]
    private UnityEngine.UI.Slider _healthSlider;
    //звуки получения урона
    [SerializeField]
    private AudioClip[] _hitSounds;
    //звук смерти
    [SerializeField]
    private AudioClip _deathSound;
    //система частиц для огненного смерча после смерти нпс
    [SerializeField]
    private GameObject _explosionAppearence;
    //порог сравнения здоровья
    const float EPSILON = 0.01f;

    private void Start()
    {
        _src = GetComponent<AudioSource>();
    }

    //вызываем, когда надо получить урон
    public bool RecieveHit(float damage)
    {
        //мертв - урон не получен
        if (!IsAlive)
            return false;
        //жив - отнимаем здоровье
        AudioClip clipToPlay = _hitSounds[ResourceManager.instance.Rng.Next(0,_hitSounds.Length)];
        _health -= damage;
        //здоровье на нуле? до свидания
        if (_health <= EPSILON)
        {
            _health = 0f;
            clipToPlay = _deathSound;
            Death();
        }
        UpdateHealth();
        //воспроизводим звук смерти/получения урона
        _src.PlayOneShot(clipToPlay);
        return true;
    }

    //обновление визуального кол-ва здоровья
    private void UpdateHealth()
    {
        if (_health > 1f)
            _health = 1f;
        else if (_health < EPSILON)
            _health = 0f;
        _healthSlider.value = _health;
    }

    //смерть
    public void Death()
    {
        Debug.Log(transform.name + " dead");
        _isAlive = false;
        //если это - игрок
        if (_isPlayer)
        {
            Transform cam = transform.Find("Main Camera");
            //смерть игрока - отключение скриптов Controller, Looker
            cam.GetComponent<PlayerLookDirectionChecker>().enabled = false;
            GetComponent<CharacterController>().enabled = false;
            //включаем UI проигрыша
            Transform root = GameObject.Find("Main Canvas").transform.Find("GameOverScreen");
            root.gameObject.SetActive(true);
            if(PreferencesHandler.instance.Record < GameManager.instance.Score)
            {
                PreferencesHandler.instance.Record = GameManager.instance.Score;
                root.Find("New Best").gameObject.SetActive(true);
            }
            root.Find("Score").GetComponent<TMPro.TMP_Text>().text = GameManager.instance.Score.ToString();
            //включить курсор
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            //камера "падает" вниз, отключается ствол
            cam.GetComponent<Animator>().enabled = true;
            cam.Find("Gun").gameObject.SetActive(false);
            return;
        }
        //смерть нпс
        StartCoroutine(DeathCoroutine());
    }

    //корутина для смерти НПС
    private IEnumerator DeathCoroutine()
    {
        //отключаем ИИ, нанесение урона, играем анимацию падения
        GetComponent<Animator>().SetTrigger("Death");
        GetComponent<Enemy>().enabled = false;
        GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
        transform.Find("mixamorig:Hips").GetComponent<BoxCollider>().enabled = false;
        _healthSlider.gameObject.SetActive(false);
        //чтобы успел проиграться звук смерти бота
        yield return new WaitForSeconds(1.6f);
        //ОГНЕННЫЙ СМЕРЧ
        Instantiate(_explosionAppearence, transform.position, transform.rotation);
        //для системы частиц еще подождем
        yield return new WaitForSeconds(0.4f);
        //смерть непися + спавн еще двоих
        //возвращаем все статы врага к исходным и копируем его дважды
        _healthSlider.gameObject.SetActive(true);
        Health = 1f;
        UpdateHealth();
        GetComponent<Enemy>().enabled = true;
        GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = true;
        transform.Find("mixamorig:Hips").GetComponent<BoxCollider>().enabled = true;
        //Х координата - от -1 до 0 и от 0 до -1, Z координата - от -1 до 1
        Vector2 pos1 = new Vector2((float)-ResourceManager.instance.Rng.NextDouble(), 
            (float)ResourceManager.instance.Rng.NextDouble()*2 - 1f),
            pos2 = new Vector2((float)ResourceManager.instance.Rng.NextDouble(), 
            (float)ResourceManager.instance.Rng.NextDouble() * 2 - 1f);
        //спавним двоих
        Instantiate(gameObject, new Vector3(transform.position.x + pos1.x, transform.position.y, 
            transform.position.z + pos1.y), transform.rotation);
        Instantiate(gameObject, new Vector3(transform.position.x + pos2.x, transform.position.y, 
            transform.position.z + pos2.y), transform.rotation);
        //уничтожаем изначальную копию
        Destroy(gameObject);
    }
}
