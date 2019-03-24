using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AITankController : FSM
{

    public TankShooting tankShooter;
    public TankHealth tankHealth;
    private bool isDead = false;
    private float elapsedTime = 0.0f;
    private float shootRate = 3.0f;
    private GameObject player = null;
    private NavMeshAgent navMeshAgent;

    public enum FSMState
    {
        None, Patrol, Attack, Dead,
    }

    public FSMState curState;

    protected override void Initialize()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        pointList = GameObject.FindGameObjectsWithTag("PatrolPoint");
        //base.Initialize();

        //A posicao de destino é randomica no inicio ? Pq ?
        int rndIndex = UnityEngine.Random.Range(0, pointList.Length);
        Debug.Log(pointList.Length);
        destPos = pointList[rndIndex].transform.position;
    }

    protected override void FSMUpdate()
    {
        //base.FSMUpdate();
        switch (curState)
        {
            case FSMState.Patrol:
                UpdatePatrolState();
                break;
            case FSMState.Attack:
                UpdateAttackState();
                break;
            case FSMState.Dead:
                UpdateDeadState();
                break;
        }
        elapsedTime += Time.deltaTime;

        //Tive que tirar o private dp m_currentHealth
        if(this.tankHealth.m_CurrentHealth <= 0)
        {
            curState = FSMState.Dead;
        }

    }
    
    private void UpdateDeadState()
    {
        if (!isDead)
        {
            Debug.Log("Dead");
        }
    }

    private void UpdateAttackState()
    {
        Collider[] players = Physics.OverlapSphere(transform.position, 15.0f, LayerMask.GetMask(" Players"));
        if(players.Length == 0)
        {
            curState = FSMState.Patrol;
            player = null;
            navMeshAgent.enabled = true;
            return;
        }
        player = players[0].gameObject;
        Vector3 _direction = (player.transform.position - transform.position).normalized;
        Quaternion _lookRotation = Quaternion.LookRotation(_direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, Time.deltaTime * 3);
        if(elapsedTime > shootRate)
        {
            //tive que tirar o protect do Fire()
            this.tankShooter.Fire();
            elapsedTime = 0;
        }
    }

    private void UpdatePatrolState()
    {
        Collider[] players = Physics.OverlapSphere(transform.position, 10.0f, LayerMask.GetMask(" Players"));
        if (players.Length > 0)
        {
            curState = FSMState.Attack;
            player = players[0].gameObject;
            navMeshAgent.enabled = false;
            return;
        }
        if (IsInCurrentRange(destPos))
        {
            int rndIndex = UnityEngine.Random.Range(0, pointList.Length);
            destPos = pointList[rndIndex].transform.position;
        }
        navMeshAgent.destination = destPos;
    }

    protected bool IsInCurrentRange(Vector3 pos)
    {
        float xPos = Mathf.Abs(pos.x - transform.position.x);
        float zPos = Mathf.Abs(pos.z - transform.position.z); 
        if(xPos <= 5 && zPos <= 5)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
