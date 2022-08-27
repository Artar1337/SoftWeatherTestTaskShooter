using UnityEngine;
using UnityEngine.AI;
using System.Collections;

//
//класс врага-преследователя игрока
//

public class Enemy : MonoBehaviour
{
    //трансформ, за которым идет нпс
    private Transform _target;
    //NavMesh Agent 
    private NavMeshAgent _agent;
    //рабочий аниматор врага
    private Animator _animator;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _target = GameObject.Find("FPS Controller").transform;
        _animator = GetComponent<Animator>();
        StartCoroutine(JumpCoroutine());
    }

    void FixedUpdate()
    {
        //задаем целью игрока
        _agent.destination = _target.position;
    }

    IEnumerator JumpCoroutine()
    {
        //бесконечный цикл
        while (true)
        {
            //если агент подошел к линку - воспроизводим анимацию
            if (_agent.isOnOffMeshLink)
            {
                _animator.SetTrigger("Jump");
                //0.4 сек. ждем чтобы был плавный переход между анимациями
                yield return new WaitForSeconds(0.4f);
                //потом ждем ещё до конца анимации, чтобы триггер не ставился несколько раз подряд
                yield return new WaitWhile(() => _animator.GetCurrentAnimatorStateInfo(0).IsTag("Jump"));
            }
            yield return null;
        }
    }
}
