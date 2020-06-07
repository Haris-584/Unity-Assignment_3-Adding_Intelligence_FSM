using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_FSM : MonoBehaviour
{
    public enum ENEMY_STATE { MOVING, ChASING , FIRING , DEAD , GAMEOVER };
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
                case ENEMY_STATE.FIRING:
                    StartCoroutine(EnemyFiring());
                    break;
                case ENEMY_STATE.DEAD:
                    StartCoroutine(EnemyAttack());
                    break;
                case ENEMY_STATE.GAMEOVER:
                    StartCoroutine(EnemyGameover());
                    break;
            }

        }
    }

    private CheckMyVision checkMyVision;
    private NavMeshAgent agent = null;
   // public Transform playerTransform = null;
   // private Transform patrolDestination = null;
   // public float maxDamage = 10f;
    private void Awake()
    {
        checkMyVision = GetComponent<CheckMyVision>();
        agent = GetComponent<NavMeshAgent>();
       
    }

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] destinations = GameObject.FindGameObjectsWithTag("Dest");
   //     patrolDestination = destinations[Random.Range(0, destinations.Length)].GetComponent<Transform>();
        CurrentState = ENEMY_STATE.MOVING;
    }

    public IEnumerator EnemyMoving()
    {
        yield break;
    }
    public IEnumerator EnemyChasing()
    {
        yield break;
    }
    
    public IEnumerator EnemyFiring()
    {
        yield break;
    }
    public IEnumerator EnemyDead()
    {
        yield break;
    }
    public IEnumerator EnemyGameover()
    {
        yield break;
    }
     
    // Update is called once per frame
    void Update()
    {

    }
}
