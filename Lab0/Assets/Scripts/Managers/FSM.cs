using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Classe criada para uso do IA Controle de Movimento
public class FSM : Complete.TankMovement
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
