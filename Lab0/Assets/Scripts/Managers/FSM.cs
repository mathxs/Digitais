using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Classe criada para uso do IA Controle de Movimento
//tive que tirar os complete, pois nao estou usando aquelas class, deu um confusao entender isso, pois nao tinha visto que tinha outras copias complete.

public class FSM : TankMovement
{
    protected Vector3 destPos;
    protected GameObject[] pointList;

    protected virtual void Initialize() { }
    protected virtual void FSMUpdate() { }
    protected virtual void FSMFixedUpdate() { }


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

    void FixedUpdate()
    {
        FSMFixedUpdate();
    }

}
