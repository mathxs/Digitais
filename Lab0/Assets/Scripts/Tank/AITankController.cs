using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AITankController : FSM
{

    //tive que tirar os complete, pois nao estou usando aquelas class, deu um confusao entender isso, pois nao tinha visto que tinha outras copias complete.
    //tem um bug da detecção, o tank do ia etava se detectando como alvo por isso nao estava andando, por conta disso tive que modificar alguns contadores, para ele sóperceber o 2 tank dentro do alvo
    //comentei em cima dos lugares em que eu modifiquei

    public TankShooting tankShooter;
    public TankHealth tankHealth;
    private bool isDead = false;
    private float elapsedTime = 0.0f;
    private float shootRate = 3.0f;
    private GameObject player = null;
    private NavMeshAgent navMeshAgent;
    private int controle;

    public enum FSMState
    {
        None, Patrol, Attack, Dead,
    }

    public FSMState curState;

    protected override void Initialize()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        pointList = GameObject.FindGameObjectsWithTag("PatrolPoint");
        Debug.Log(pointList.Length);
        //base.Initialize();

        //A posicao de destino é randomica no inicio.
        int rndIndex = UnityEngine.Random.Range(0, pointList.Length);
        
        destPos = pointList[rndIndex].transform.position;
        controle = 0;
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
        Collider[] players = Physics.OverlapSphere(transform.position, 15.0f, LayerMask.GetMask("Players"));
        Debug.Log(players.Length);
        //aumentei de 0 para 1, pois a ia estava se enxergando como alvo e nao parava de atirar
        if (players.Length == 1)
        {
            curState = FSMState.Patrol;
            player = null;
            navMeshAgent.enabled = true;
            return;
        }
        //com mais de um jogador a rotacao do tank buga, ele se perde, estou tentando consertar
        player = players[controle].gameObject;
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
        Collider[] players = Physics.OverlapSphere(transform.position, 10.0f, LayerMask.GetMask("Players"));
        //aumentei de 0 para 1, pois a ia estava se enxergando como alvo e nao parava de atirar
        if (players.Length > 1)
        {
            curState = FSMState.Attack;
            player = players[0].gameObject;
            navMeshAgent.enabled = false;
            return;
        }
        if (IsInCurrentRange(destPos))
        {
            //curState = FSMState.Patrol;
            //é uma ia burra por conta dos random
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
        Initialize();

    }

    // Update is called once per frame
    void Update()
    {
        FSMUpdate();
    }
    public void Morreu(int i)
    {
        controle = i;
        Debug.Log(controle);

    }
}
