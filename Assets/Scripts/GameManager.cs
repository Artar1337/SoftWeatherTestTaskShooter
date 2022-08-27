using System.Collections;
using UnityEngine;

//
// класс отвечает за подсчет очков и за все, что связано с оружием
//

public class GameManager : MonoBehaviour
{

    #region Singleton
    public static GameManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("GAME MANAGER instance error!");
            return;
        }
        instance = this;
    }
    #endregion

    //урон по телу/голове
    [Range(0f,1f)]
    [SerializeField]
    private readonly float _bodyDamage = 0.2f, _headDamage = 0.5f;
    //плюс к очкам за попадание в тело/голову
    [SerializeField]
    private readonly int _bodyScoreAddition = 10, _headScoreAddition = 40;
    //слои, на которые пуля (рейкаст) обратит внимание
    [SerializeField]
    private LayerMask _raycastedLayers;

    //текущий счет
    private int _score = 0;
    //текущее кол-во патронов
    private int _ammo = 8;
    //максимальное кол-во патронов (для ПМ - 8)
    private readonly int _maxAmmo = 8;
    //статы игрока
    private Stats _stats;
    //эффективная дальность стрельбы
    private readonly float _weaponRange = 200f;

    //свойство для очков
    public int Score { get => _score; 
        set {
            _score = value;
            _scoreRepresentation.text = _score.ToString();
        }
    }
    //свойство для патронов
    public int Ammo { get => _ammo; set => _ammo = value; }
    //макс. кол-во патронов
    public int MaxAmmo { get => _maxAmmo; }

    //аниматор оружия
    private Animator _gun;
    //визуальная репрезентация кол-ва патронов
    private Transform _bullets;
    //место, откуда вылетает пламя от выстрела
    private Transform _fire;
    //main camera
    private Transform _cam;
    //гильза
    private GameObject _case;
    //источник звука (с него воспроизводится только щелчок, когда нет патронов)
    private AudioSource _secondGunSource;
    //кол-во очков
    private TMPro.TMP_Text _scoreRepresentation;

    //звук, воспроизводится когда нет патронов, а игрок жмет на выстрел
    [SerializeField]
    private AudioClip _gunNoAmmoClickSound;
    //FX - дырка от пули, красный туман (при выстреле во врага),
    //частицы от стен, когда в них попадает пуля, частицы огня из ствола
    [SerializeField]
    private GameObject _bulletHole, _smokeHit, _bulletParticles, _fireParticles;

    private void Start()
    {
        _cam = transform.Find("Main Camera").transform;
        _gun = _cam.Find("Gun").GetComponent<Animator>();
        _bullets = GameObject.Find("Main Canvas").transform.Find("Bullets");
        _stats = GetComponent<Stats>();
        _secondGunSource = _gun.transform.Find("Trigger").GetComponent<AudioSource>();
        _scoreRepresentation = GameObject.Find("Main Canvas").transform.
            Find("Score").transform.Find("Value").GetComponent<TMPro.TMP_Text>();
        _case = _gun.transform.Find("Body").Find("Case").gameObject;
        _fire = _gun.transform.Find("Body").Find("Barrel");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    //выстрел
    private void Shoot()
    {
        //запускаем гильзу 
        StartCoroutine(ThrowCase());
        //спавним огонь из ствола
        Instantiate(_fireParticles, _fire);
        //кидаем луч из камеры в центр экрана
        if (Physics.Raycast(_cam.position, _cam.forward, out RaycastHit hit, _weaponRange, _raycastedLayers))
        {
            //луч попал в голову
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Head"))
            {
                //+ к очкам, если был получен урон
                if(hit.transform.GetComponent<PartOfBody>().RecieveDamage(_headDamage))
                    Score += _headScoreAddition;
                //красный туман
                Instantiate(_smokeHit, hit.point, Quaternion.LookRotation(hit.normal));
            }
            //луч попал в тело
            else if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Body"))
            {
                //+ к очкам, если был получен урон
                if (hit.transform.GetComponent<PartOfBody>().RecieveDamage(_bodyDamage))
                    Score = Score + _bodyScoreAddition;
                //красный туман
                Instantiate(_smokeHit, hit.point, Quaternion.LookRotation(hit.normal));
            }
            //луч попал в стену
            if(hit.collider.CompareTag("Damagable"))
            {
                //дырка от пули
                Instantiate(_bulletHole, hit.point, Quaternion.LookRotation(hit.normal));
                //частицы
                Instantiate(_bulletParticles, hit.point, Quaternion.LookRotation(hit.normal));
            }
        }
    }

    private IEnumerator ThrowCase()
    {
        //ждем 5 кадров
        for (int i = 0; i < 5; i++)
            yield return null;
        //копируем гильзу и пуляем её вправо
        GameObject g = Instantiate(_case, _case.transform.position, _case.transform.rotation);
        g.SetActive(true);
        g.GetComponent<Rigidbody>().AddForce(_cam.right, ForceMode.Impulse);
    }

    //апдейт визуального количества пуль на экране
    private void UpdateAmmoCount()
    {
        for (int i = 0; i < _bullets.childCount; i++)
        {
            _bullets.GetChild(i).gameObject.SetActive(false);
        }
        for (int i = 0; i < _ammo; i++)
        {
            _bullets.GetChild(i).gameObject.SetActive(true);
        }
    }

    //логика пистолета здесь

    private void PistolLogic()
    {
        //если пистолет стреляет/перезаряжается - то ничего не делаем
        if (!_gun.GetCurrentAnimatorStateInfo(0).IsTag("Idle"))
            return;

        float input = Input.GetAxis("Fire1");
        if (input > 0f)
        {
            if (_ammo <= 0)
            {
                //проиграть звук щелканья спускового крючка
                if (!_secondGunSource.isPlaying)
                    _secondGunSource.PlayOneShot(_gunNoAmmoClickSound);
                return;
            }

            Shoot();
            _gun.SetTrigger("Shoot");
            _ammo--;

            UpdateAmmoCount();
            return;
        }

        input = Input.GetAxis("Reload");
        if (input > 0f && _ammo < _maxAmmo + 1)
        {
            //если нет патронов - нужно передернуть затвор
            if (_ammo == 0)
            {
                _gun.SetTrigger("FullReload");
                _ammo = _maxAmmo;
            }
            else
            {
                _gun.SetTrigger("Reload");
                //ЕЩЕ ОДИН ПАТРОН В ПАТРОННИКЕ (ЕСЛИ ПИСТОЛЕТ НЕ ОТСТРЕЛЯЛ ВСЁ)
                _ammo = _maxAmmo + 1;
            }

            UpdateAmmoCount();
            return;
        }
    }
    
    private void Update()
    {
        if (!_stats.IsAlive)
            return;

        PistolLogic();

        //если нажимаем escape - игрок мертв.
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _stats.RecieveHit(1f);
        }
    }
}
