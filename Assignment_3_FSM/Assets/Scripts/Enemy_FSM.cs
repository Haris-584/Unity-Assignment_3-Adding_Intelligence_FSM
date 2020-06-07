using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_FSM : MonoBehaviour
{
    public enum ENEMY_STATE { MOVING, CHASING, FIRE , DEAD, GAMEOVER};
    [SerializeField]
    private ENEMY_STATE currentState;
    public ENEMY_STATE CurrentState
    {
        get
        {
            return currentState;

        }
        set
        {

            currentState = value;
            StopAllCoroutines();

            switch (currentState)
            {
                case ENEMY_STATE.MOVING:
                    StartCoroutine(EnemyMoving());
                    break;
                case ENEMY_STATE.CHASING:
                    StartCoroutine(EnemyChasing());
                    break;
                case ENEMY_STATE.FIRE:
                    StartCoroutine(EnemyFire());
                    break;
                    /*
                case ENEMY_STATE.DEAD:
                    StartCoroutine(EnemyDead());
                    break;
                case ENEMY_STATE.FIRE:
                    StartCoroutine(EnemyFire());
                    break;
                     */
            }

        }
    }

    private CheckMyVision checkMyVision;
    private NavMeshAgent agent = null;
    public Transform playerTransform = null;
    private Transform patrolDestination = null;
   // private Health playerHealth = null;
    public float maxDamage = 10f;
    private void Awake()
    {
        checkMyVision = GetComponent<CheckMyVision>();
        agent = GetComponent<NavMeshAgent>();
        //playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
        //playerTransform = playerHealth.GetComponent<Transform>();
    }

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] destinations = GameObject.FindGameObjectsWithTag("Dest");
        patrolDestination = destinations[Random.Range(0, destinations.Length)].GetComponent<Transform>();
        CurrentState = ENEMY_STATE.MOVING;
    }

    public IEnumerator EnemyMoving()
    {
        while (currentState == ENEMY_STATE.MOVING)
        {
            checkMyVision.sensitivity = CheckMyVision.enmSensitivity.HIGH;
            agent.isStopped = false;
            agent.SetDestination(patrolDestination.position);
            while (agent.pathPending)
            {
                yield return null;
            }

            if (checkMyVision.targetInSight)
            {
                agent.isStopped = true;
                CurrentState = ENEMY_STATE.CHASING;
                yield break;
            }
            yield return null;
        }

    }
    public IEnumerator EnemyChasing()
    {
        while (currentState == ENEMY_STATE.CHASING)
        {
            checkMyVision.sensitivity = CheckMyVision.enmSensitivity.LOW;
            agent.isStopped = false;
            agent.SetDestination(checkMyVision.lastKnownSighting);
            while (agent.pathPending)
            {
                yield return null;
            }
            Debug.Log(agent.stoppingDistance);
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                agent.isStopped = true;
                if (!checkMyVision.targetInSight)
                {
                    CurrentState = ENEMY_STATE.MOVING;
                }
                else
                {
                    CurrentState = ENEMY_STATE.FIRE;
                }
                yield break;
            }
            yield return null;
        }
        yield break;
    }
    public IEnumerator EnemyFire()
    {
        while (currentState == ENEMY_STATE.FIRE)
        {
            Debug.Log("test");
            agent.isStopped = false;
            agent.SetDestination(playerTransform.position);
            while (agent.pathPending)
                yield return null;
            if (agent.remainingDistance > agent.stoppingDistance)
            {
                CurrentState = ENEMY_STATE.CHASING;
            }
            else
            {
                //playerHealth.HealthPoints -= maxDamage * Time.deltaTime;
            }
            yield return null;
        }
        yield break;
    }

  /*
    public IEnumerator EnemyDead()
    {
        yield break;
    }

    public IEnumerator EnemyGameover()
    {
        yield break;
    }
   * 
   */
   
    // Update is called once per frame
    void Update()
    {

    }
}
