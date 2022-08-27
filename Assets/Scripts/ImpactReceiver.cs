using UnityEngine;

public class ImpactReceiver : MonoBehaviour
{
    [SerializeField]
    private float _mass = 3.0f; // defines the character mass
    private Vector3 _impact = Vector3.zero;
    private CharacterController _character;
    
    void Start()
    {
        _character = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // apply the impact force:
        if (_impact.magnitude > 0.2F) 
            _character.Move(_impact * Time.fixedDeltaTime);
        // consumes the impact energy each cycle:
        _impact = Vector3.Lerp(_impact, Vector3.zero, 5 * Time.fixedDeltaTime);
    }

    // call this function to add an impact force:
    public void AddImpact(Vector3 dir, float force)
    {
        dir.Normalize();
        if (dir.y < 0) 
            dir.y = -dir.y; // reflect down force on the ground
        _impact += dir.normalized * force / _mass;
    }
}