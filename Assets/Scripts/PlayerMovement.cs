using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float _normalSpeed = 2.56f, _sprintSpeed = 5.12f;
    [SerializeField]
    private LayerMask _groundLayers;
    [SerializeField]
    private float _groundDistance = 0.15f;    
    [SerializeField]
    private AudioClip _playerIsTiredSound,_playerLandedSound;

    private float _jumpForce = 8f;
    private float _gravity = -10f;

    private Animator _walkAnimator;
    private AnimatorEvents _walkAnimatorEvents;

    private float _timeInAir = 0f, _maxTimeInAir = 0.2f;
    private bool _isGrounded = false;

    private bool _isRunning = false;
    private Transform _legPosition;
    private CharacterController _controller;

    private Vector3 _velocity;
    private int _currentLayers;

    private float _maxEndurance = 10f, _currentEndurance = 10f,
        _currentEnduranceCooldown = -0.01f, _enduranceCooldown = 2f;
    private UnityEngine.UI.Slider _enduranceSlider;
    private AudioSource _audioSource;

    public bool IsGrounded { get => _isGrounded; 
        set{
            bool old = _isGrounded;
            _isGrounded = value;
            //произошла смена isGroundeed => персонаж приземлился на ноги!
            if (old != value && _timeInAir > _maxTimeInAir)
            {
                _walkAnimatorEvents.PlaySound(_playerLandedSound);
                _timeInAir = 0f;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _currentLayers = _groundLayers;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _controller = GetComponent<CharacterController>();
        _legPosition = transform.Find("Leg Position").transform;
        
        _enduranceSlider = GameObject.Find("Main Canvas").transform.Find("Endurance").
            GetComponent<UnityEngine.UI.Slider>();
        _audioSource = GetComponent<AudioSource>();
        _walkAnimator = _legPosition.GetComponent<Animator>();
        _walkAnimatorEvents = _legPosition.GetComponent<AnimatorEvents>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_controller.enabled)
            return;

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = (transform.right * x + transform.forward * z);
        //определяем, идет ли персонаж
        _walkAnimator.SetFloat("Speed", Mathf.Abs(x) + Mathf.Abs(z));

        if (_isRunning)
            _controller.Move(move * _sprintSpeed * Time.deltaTime);
        else
            _controller.Move(move * _normalSpeed * Time.deltaTime);

        IsGrounded = Physics.CheckSphere(_legPosition.position, _groundDistance,
            _currentLayers, QueryTriggerInteraction.Ignore);

        if (IsGrounded)
            _timeInAir = 0f;
        else
            _timeInAir += Time.deltaTime;

        if (Input.GetButtonDown("Jump") && IsGrounded)
            _velocity.y = Mathf.Sqrt(_jumpForce);
        else if (IsGrounded && _velocity.y < 0)
            _velocity.y = -transform.position.y;

        _velocity.y += _gravity * Time.deltaTime;

        _controller.Move(_velocity * Time.deltaTime);

        //устанавливаем визуально выносливость
        if (_currentEndurance < 0f)
            _currentEndurance = 0f;
        else if (_currentEndurance > _maxEndurance)
            _currentEndurance = _maxEndurance;
        _enduranceSlider.value = _currentEndurance/_maxEndurance;
        //проверка на спринт
        
        if (_currentEnduranceCooldown > 0f)
        {
            _isRunning = false;
            _currentEnduranceCooldown -= Time.deltaTime;
            return;
        }

        float interaction = Input.GetAxis("Run");
        //в воздухе никто не может бежать!
        if (interaction > 0f && IsGrounded)
        {
            _isRunning = true;
            _walkAnimator.SetBool("IsRunning", true);
            //бежим
            if (_currentEndurance > 0f)
            {
                _currentEndurance -= 1.5f * Time.deltaTime;
                return;
            }
            //устал бежать, делаем cooldown для нажатия на run
            //и вопсроизводим звук усталости
            _currentEnduranceCooldown = _enduranceCooldown;
            _isRunning = false;
            _walkAnimator.SetBool("IsRunning", false);
            _audioSource.PlayOneShot(_playerIsTiredSound);
        }
        else
        {
            _isRunning = false;
            if (_currentEndurance < _maxEndurance)
            {
                _currentEndurance += Time.deltaTime;
            }
            _walkAnimator.SetBool("IsRunning", false);
        }
        
        if (_currentEndurance < _maxEndurance)
        {
            _currentEndurance += 1.5f * Time.deltaTime;
        }
    }
}
