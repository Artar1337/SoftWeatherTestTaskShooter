using UnityEngine;

//
// класс для движения игрока
//

public class PlayerMovement : MonoBehaviour
{
    //скорость обычного движения и спринта
    [SerializeField]
    private float _normalSpeed = 2.56f, _sprintSpeed = 5.12f;
    //слои пола (с которых можно прыгать, на которых isGrounded=true)
    [SerializeField]
    private LayerMask _groundLayers;
    //дистанция, на которой персонаж будет логически стоять на земле
    [SerializeField]
    private float _groundDistance = 0.15f; 
    //звуки стамины и приземления после прыжка
    [SerializeField]
    private AudioClip _playerIsTiredSound, _playerLandedSound;
    
    //сила прыжка и гравитация (округлил)
    private float _jumpForce = 8f;
    private float _gravity = -10f;

    //аниматор игрока (нужен для воспроизведения звука шагов в основном)
    private Animator _walkAnimator;
    private AnimatorEvents _walkAnimatorEvents;

    //нужны для того, чтобы персонаж не смог "ходить" по воздуху (воспр. звуки в воздухе)
    private float _timeInAir = 0f, _maxTimeInAir = 0.2f;
    //находится ли перс на земле
    private bool _isGrounded = false;
    //бежит ли перс
    private bool _isRunning = false;
    //позиция ног (от нее считаем isGrounded)
    private Transform _legPosition;
    //контроллер перса
    private CharacterController _controller;
    
    // для имитации гравитации
    private Vector3 _velocity;

    //выносливость текущая, максимальная и cooldown по полному истощению
    private float _maxEndurance = 10f, _currentEndurance = 10f,
        _currentEnduranceCooldown = -0.01f, _enduranceCooldown = 2f;
    //слайдер для вывода выносливости
    private UnityEngine.UI.Slider _enduranceSlider;
    //источник звука (усталость)
    private AudioSource _audioSource;

    //на полу ли сейчас персонаж
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

    void Start()
    {
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
        //считаем вектор, куда персонаж должен идти
        Vector3 move = (transform.right * x + transform.forward * z);
        //определяем, идет ли персонаж
        _walkAnimator.SetFloat("Speed", Mathf.Abs(x) + Mathf.Abs(z));

        if (_isRunning)
            _controller.Move(move * _sprintSpeed * Time.deltaTime);
        else
            _controller.Move(move * _normalSpeed * Time.deltaTime);

        //определяем, стоит ли игрок
        IsGrounded = Physics.CheckSphere(_legPosition.position, _groundDistance,
            _groundLayers, QueryTriggerInteraction.Ignore);

        //если не стоит - значит он в воздухе на time.deltatime больше секунд
        if (IsGrounded)
            _timeInAir = 0f;
        else
            _timeInAir += Time.deltaTime;

        //если перс прыгает - значит добавляем ему velocity
        if (Input.GetButtonDown("Jump") && IsGrounded)
            _velocity.y = Mathf.Sqrt(_jumpForce);
        else if (IsGrounded && _velocity.y < 0)
            _velocity.y = -transform.position.y;

        //учитываем гравитацию и двигаем персонажа
        _velocity.y += _gravity * Time.deltaTime;
        _controller.Move(_velocity * Time.deltaTime);

        //устанавливаем визуально выносливость
        if (_currentEndurance < 0f)
            _currentEndurance = 0f;
        else if (_currentEndurance > _maxEndurance)
            _currentEndurance = _maxEndurance;
        _enduranceSlider.value = _currentEndurance/_maxEndurance;
        //проверка на усталость
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
            //не бежим - у нас cooldown
            _isRunning = false;
            if (_currentEndurance < _maxEndurance)
            {
                _currentEndurance += Time.deltaTime;
            }
            _walkAnimator.SetBool("IsRunning", false);
        }
        //не бежим - восстанавливаем выносливость
        if (_currentEndurance < _maxEndurance)
        {
            _currentEndurance += 1.5f * Time.deltaTime;
        }
    }
}
