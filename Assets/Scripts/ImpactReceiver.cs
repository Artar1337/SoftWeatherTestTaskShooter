using UnityEngine;

//
// класс нужен для обработки импульса, который передает враг
// при ударе по игроку (CharacterController не поддерживает это сам по себе)
//

public class ImpactReceiver : MonoBehaviour
{
    //масса персонажа
    [SerializeField]
    private float _mass = 3.0f;
    //текущий вектор удара
    private Vector3 _impact = Vector3.zero;
    //контроллер
    private CharacterController _character;
    
    void Start()
    {
        _character = GetComponent<CharacterController>();
    }

    void FixedUpdate()
    {
        // применяем удар
        if (_impact.magnitude > 0.2f) 
            _character.Move(_impact * Time.fixedDeltaTime);
        // линейно интерполируем от удара до нуля
        _impact = Vector3.Lerp(_impact, Vector3.zero, 5 * Time.fixedDeltaTime);
    }

    // вызываем, если надо получить удар
    public void AddImpact(Vector3 dir, float force)
    {
        dir.Normalize();
        if (dir.y < 0) 
            dir.y = -dir.y;
        _impact += dir.normalized * force / _mass;
    }
}