using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_FSM : MonoBehaviour
{
    //Five States
    public enum ENEMY_STATE { MOVING, CHASING, FIRE , DEAD, GAMEOVER};

    //Current state 
    [SerializeField]
    private ENEMY_STATE currentState;

    //set and get current state
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
                case ENEMY_STATE.DEAD:
                    StartCoroutine(EnemyDead());
                    break;
                case ENEMY_STATE.FIRE:
                    StartCoroutine(EnemyFire());
                    break;
                     
            }

        }
    }

    //Variable decleration and assigning 
    private CheckMyVision checkMyVision;
    private NavMeshAgent agent = null;
    public Transform playerTransform = null;
    private Transform moveDestination = null;
    private Health playerHealth = null;
    public float maxDamage = 10f;

    //Awake method 
    private void Awake()
    {
        checkMyVision = GetComponent<CheckMyVision>();
        agent = GetComponent<NavMeshAgent>();
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
    }

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] destinations = GameObject.FindGameObjectsWithTag("Dest");
        moveDestination = destinations[Random.Range(0, destinations.Length)].GetComponent<Transform>();
        CurrentState = ENEMY_STATE.MOVING;
    }

    public IEnumerator EnemyMoving()
    {
        // one transition of player visible to chasing state
        while (currentState == ENEMY_STATE.MOVING)
        {
            checkMyVision.sensitivity = CheckMyVision.enmSensitivity.HIGH;
            agent.isStopped = false;
            agent.SetDestination(moveDestination.position);
            while (agent.pathPending)
            {
                yield return null;
            }
            //playervisible
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
            //two transition (in range  to fire and out of sight to moving)
            checkMyVision.sensitivity = CheckMyVision.enmSensitivity.LOW;
            agent.isStopped = false;
            agent.SetDestination(checkMyVision.lastKnownSighting);
            while (agent.pathPending)
            {
                yield return null;
            }
            float dist = Vector3.Distance(agent.transform.position, GameObject.FindGameObjectWithTag("Player").transform.position);
            if (dist <= 1.0f)
            {
                agent.isStopped = true;
                //if out if sight go back to moving
                if (!checkMyVision.targetInSight)
                {
                    CurrentState = ENEMY_STATE.MOVING;
                }
                    //if in range the fire or attack
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
        //one transition (life lost  to dead and out of range to chasing)
        Debug.Log("Fire function here");
        while (currentState == ENEMY_STATE.FIRE)
        {
            agent.isStopped = false;
            agent.SetDestination(playerTransform.position);
            while (agent.pathPending)
               yield return null;
            Debug.Log("fire while statement");
            if (agent.remainingDistance > agent.stoppingDistance)
            {
                Debug.Log("Back to chasing");
                CurrentState = ENEMY_STATE.CHASING;
            }else
            {
                
                playerHealth.HealthPoints -= maxDamage * Time.deltaTime;
                while (playerHealthPoints == 0)
                {
                    CurrentState = ENEMY_STATE.Dead;
                }
                
            }
                
            yield return null;
        }
        yield break;
    }

  
    public IEnumerator EnemyDead()
    {
        //two transitions (no life to gameover and remaining life to moving)
        Debug.Log("Dead function here");
        yield break;
    }

    public IEnumerator EnemyGameover()
    {
   //finish levels stop the game
        Debug.Log("Gameover function here");
        GameOver();
        yield break;
    }
    public void GameOver()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene);
    }
   
    // Update is called once per frame
    void Update()
    {

    }
}
