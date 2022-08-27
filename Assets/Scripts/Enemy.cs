using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Enemy : MonoBehaviour
{
    //трансформ, за которым идет нпс
    private Transform _target;
    //NavMesh Agent 
    private NavMeshAgent _agent;
    //
    private Animator _animator;
    // Start is called before the first frame update
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _target = GameObject.Find("FPS Controller").transform;
        _animator = GetComponent<Animator>();
        StartCoroutine(JumpCoroutine());
    }

    void FixedUpdate()
    {
        //Follow the player
        _agent.destination = _target.position;
    }

    IEnumerator JumpCoroutine()
    {
        while (true)
        {
            if (_agent.isOnOffMeshLink)
            {
                _animator.SetTrigger("Jump");
                yield return new WaitForSeconds(0.4f);

                yield return new WaitWhile(() => _animator.GetCurrentAnimatorStateInfo(0).IsTag("Jump"));
            }
            yield return null;
        }
    }
}
