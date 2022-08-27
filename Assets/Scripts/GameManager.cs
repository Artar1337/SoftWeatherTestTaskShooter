using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [Range(0f,1f)]
    [SerializeField]
    private readonly float _bodyDamage = 0.2f, _headDamage = 0.5f;
    [SerializeField]
    private readonly int _bodyScoreAddition = 10, _headScoreAddition = 40;
    [SerializeField]
    private LayerMask _raycastedLayers;

    private int _score = 0;
    private int _ammo = 8;
    private readonly int _maxAmmo = 8;
    private Stats _stats;
    private readonly float _weaponRange = 200f;

    public int Score { get => _score; 
        set {
            _score = value;
            _scoreRepresentation.text = _score.ToString();
        }
    }

    public int Ammo { get => _ammo; set => _ammo = value; }
    public int MaxAmmo { get => _maxAmmo; }

    private Animator _gun;
    private Transform _bullets, _fire;
    private Transform _cam;
    private GameObject _case;
    private AudioSource _secondGunSource;
    private TMPro.TMP_Text _scoreRepresentation;

    [SerializeField]
    private AudioClip _gunNoAmmoClickSound;
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

    private void Shoot()
    {
        StartCoroutine(ThrowCase());
        Instantiate(_fireParticles, _fire);
        if (Physics.Raycast(_cam.position, _cam.forward, out RaycastHit hit, _weaponRange, _raycastedLayers))
        {
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Head"))
            {
                if(hit.transform.GetComponent<PartOfBody>().RecieveDamage(_headDamage))
                    Score += _headScoreAddition;
                Instantiate(_smokeHit, hit.point, Quaternion.LookRotation(hit.normal));
            }
            else if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Body"))
            {
                if(hit.transform.GetComponent<PartOfBody>().RecieveDamage(_bodyDamage))
                    Score = Score + _bodyScoreAddition;
                Instantiate(_smokeHit, hit.point, Quaternion.LookRotation(hit.normal));
            }
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

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _stats.RecieveHit(1f);
        }
    }
}
