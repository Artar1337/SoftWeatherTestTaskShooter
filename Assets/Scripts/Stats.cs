using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(AudioSource))]
public class Stats : MonoBehaviour
{
    //� ����� - 1 - ������ ��, 0 (� ������) - ����
    private float _health = 1f;
    private bool _isAlive = true;
    public float Health { get => _health; set => _health = value; }
    public bool IsAlive { get => _isAlive; }
    private AudioSource _src;
    [SerializeField]
    private bool _isPlayer = false;
    [SerializeField]
    private UnityEngine.UI.Slider _healthSlider;
    [SerializeField]
    private AudioClip[] _hitSounds;
    [SerializeField]
    private AudioClip _deathSound;
    [SerializeField]
    private GameObject _explosionAppearence;

    const float EPSILON = 0.01f;

    private void Start()
    {
        _src = GetComponent<AudioSource>();
    }

    public bool RecieveHit(float damage)
    {
        if (!IsAlive)
            return false;
        AudioClip clipToPlay = _hitSounds[ResourceManager.instance.Rng.Next(0,_hitSounds.Length)];
        _health -= damage;
        if (_health <= EPSILON)
        {
            _health = 0f;
            clipToPlay = _deathSound;
            Death();
        }
        UpdateHealth();
        _src.PlayOneShot(clipToPlay);
        return true;
    }

    private void UpdateHealth()
    {
        if (_health > 1f)
            _health = 1f;
        else if (_health < EPSILON)
            _health = 0f;
        _healthSlider.value = _health;
    }

    public void Death()
    {
        Debug.Log(transform.name + " dead");
        _isAlive = false;
        if (_isPlayer)
        {
            Transform cam = transform.Find("Main Camera");
            //������ ������ - ���������� �������� Controller, Looker
            cam.GetComponent<PlayerLookDirectionChecker>().enabled = false;
            GetComponent<CharacterController>().enabled = false;
            //�������� UI ���������
            Transform root = GameObject.Find("Main Canvas").transform.Find("GameOverScreen");
            root.gameObject.SetActive(true);
            if(PreferencesHandler.instance.Record < GameManager.instance.Score)
            {
                PreferencesHandler.instance.Record = GameManager.instance.Score;
                root.Find("New Best").gameObject.SetActive(true);
            }
            root.Find("Score").GetComponent<TMPro.TMP_Text>().text = GameManager.instance.Score.ToString();
            //�������� ������
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            //������ "������" ����, ����������� �����
            cam.GetComponent<Animator>().enabled = true;
            cam.Find("Gun").gameObject.SetActive(false);
            return;
        }
        //������ ���
        StartCoroutine(DeathCoroutine());
    }

    private IEnumerator DeathCoroutine()
    {
        GetComponent<Animator>().SetTrigger("Death");
        GetComponent<Enemy>().enabled = false;
        GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
        transform.Find("mixamorig:Hips").GetComponent<BoxCollider>().enabled = false;
        _healthSlider.gameObject.SetActive(false);
        //����� ����� ����������� ���� ������ ����
        yield return new WaitForSeconds(1.6f);
        Instantiate(_explosionAppearence, transform.position, transform.rotation);
        //��� ������� ������ ��� ��������
        yield return new WaitForSeconds(0.4f);
        //������ ������ + ����� ��� �����
        _healthSlider.gameObject.SetActive(true);
        Health = 1f;
        UpdateHealth();
        GetComponent<Enemy>().enabled = true;
        GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = true;
        transform.Find("mixamorig:Hips").GetComponent<BoxCollider>().enabled = true;
        //� ���������� - �� -1 �� 0 � �� 0 �� -1, Z ���������� - �� -1 �� 1
        Vector2 pos1 = new Vector2((float)-ResourceManager.instance.Rng.NextDouble(), 
            (float)ResourceManager.instance.Rng.NextDouble()*2 - 1f),
            pos2 = new Vector2((float)ResourceManager.instance.Rng.NextDouble(), 
            (float)ResourceManager.instance.Rng.NextDouble() * 2 - 1f);
        Instantiate(gameObject, new Vector3(transform.position.x + pos1.x, transform.position.y, 
            transform.position.z + pos1.y), transform.rotation);
        Instantiate(gameObject, new Vector3(transform.position.x + pos2.x, transform.position.y, 
            transform.position.z + pos2.y), transform.rotation);
        Destroy(gameObject);
    }
}
