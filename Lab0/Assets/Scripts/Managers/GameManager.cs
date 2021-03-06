﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

//Neste primeiro progeto não escrevi nenhum pedaço de codigo, apenas acompanhei a explicação do uso no tutorial unity

//Controla a jogabilidade, iniciando e finalizado os rounds e controlando o jogo
//Fazendo inicio e fim do jogo assim como controlando os textos

public class GameManager : MonoBehaviour
{
    public int m_NumRoundsToWin = 5;        
    public float m_StartDelay = 3f;         
    public float m_EndDelay = 3f;           
    public CameraControl m_CameraControl;   
    public Text m_MessageText;              
    public GameObject m_TankPrefab;
    
    //de uso pela IA
    public GameObject m_TankAIPrefab;
    public TankManager[] m_Tanks;
    private int controle;

    private int m_RoundNumber;              
    private WaitForSeconds m_StartWait;     
    private WaitForSeconds m_EndWait;       
    private TankManager m_RoundWinner;
    private TankManager m_GameWinner;
    //controlando rotacao do IA (nao consegui fazer funcionar direito)
    public AITankController passandoMetodo;

    //materiais do tank e seu controne
    public Material[] m_Materials = new Material[9];
    private int gameState;
    private Boolean teste;

    private void Start()
    {
        m_StartWait = new WaitForSeconds(m_StartDelay);
        m_EndWait = new WaitForSeconds(m_EndDelay);

        SpawnAllTanks();
        SetCameraTargets();

        gameState = 0;
        teste = true;
        StartCoroutine(GameLoop());
        controle = m_Tanks.Length;
    }


    private void SpawnAllTanks()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            //Este é o caso da IA
            //inclui o codigo depois o || para poder iniciar dois IA e dois Jogadores, caso seja maior que 3 a quantidade de jogador
            if ((i == 0) || (i == 1) && (m_Tanks.Length>2))
            {

                m_Tanks[i].m_Instance =
                Instantiate(m_TankPrefab, m_Tanks[i].m_SpawnPoint.position, m_Tanks[i].m_SpawnPoint.rotation) as GameObject;
                m_Tanks[i].m_PlayerNumber = i + 1;
                m_Tanks[i].Setup();
                m_Tanks[i].texto("P" + (i + 1) + "Win" + m_Tanks[i].m_Wins);

            }
            else
            {
                m_Tanks[i].m_Instance =
                Instantiate(m_TankAIPrefab, m_Tanks[i].m_SpawnPoint.position, m_Tanks[i].m_SpawnPoint.rotation) as GameObject;
                m_Tanks[i].m_PlayerNumber = i + 1;
                m_Tanks[i].Setup();
                m_Tanks[i].texto("P" + (i + 1) + "Win" + m_Tanks[i].m_Wins);

            }
        }
    }


    private void SetCameraTargets()
    {
        Transform[] targets = new Transform[m_Tanks.Length];
        for (int i = 0; i < targets.Length; i++)
        {
            targets[i] = m_Tanks[i].m_Instance.transform;
        }
        m_CameraControl.m_Targets = targets;
    }


    private IEnumerator GameLoop()
    {
        //chamar material
        if (teste)
        {
            yield return StartCoroutine(Textura());
        }
        teste = false;

        yield return StartCoroutine(RoundStarting());
        yield return StartCoroutine(RoundPlaying());
        yield return StartCoroutine(RoundEnding());

        if (m_GameWinner != null)
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            StartCoroutine(GameLoop());
        }

        //tentando controlar rotacao
        if (controle != m_Tanks.Length)
        {
            passandoMetodo.Morreu(controle - m_Tanks.Length);
            controle = m_Tanks.Length;
        }
    }


    private IEnumerator RoundStarting()
    {
        ResetAllTanks();
        DisableTankControl();
        m_CameraControl.SetStartPositionAndSize();
        m_RoundNumber++;
        m_MessageText.text = "Partida " + m_RoundNumber;
        yield return m_StartWait;
    }


    private IEnumerator RoundPlaying()
    {
        EnableTankControl();
        m_MessageText.text = string.Empty;
        while (!OneTankLeft())
        {
            yield return null;
        }
    }


    private IEnumerator RoundEnding()
    {
        DisableTankControl();
        m_RoundWinner = null;
        m_RoundWinner = GetRoundWinner();
        if (m_RoundWinner != null)
            m_RoundWinner.m_Wins++;

        m_GameWinner = GetGameWinner();
        string message = EndMessage();
        m_MessageText.text = message;
        yield return m_EndWait;
    }

    private bool OneTankLeft()
    {
        int numTanksLeft = 0;
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_Instance.activeSelf)
                numTanksLeft++;
        }
        return numTanksLeft <= 1;
    }


    private TankManager GetRoundWinner()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_Instance.activeSelf)
                return m_Tanks[i];
        }
        return null;
    }


    private TankManager GetGameWinner()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_Wins == m_NumRoundsToWin)
            {
                return m_Tanks[i];
            }
            m_Tanks[i].texto("P" + (i + 1) + "Win" + m_Tanks[i].m_Wins);
        }

        return null;
    }

    //Editar as mensagems
    private string EndMessage()
    {
        string message = "DRAW!";

        if (m_RoundWinner != null)
            message = m_RoundWinner.m_ColoredPlayerText + " venceu a partida!";

        message += "\n\n\n\n";
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            message += m_Tanks[i].m_ColoredPlayerText + ": " + m_Tanks[i].m_Wins + " vitorias\n";
        }

        if (m_GameWinner != null)
            message = m_GameWinner.m_ColoredPlayerText + " é o maioral!";

        return message;
    }

    private void ResetAllTanks()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].Reset();
        }
    }


    private void EnableTankControl()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].EnableControl();
        }
    }


    private void DisableTankControl()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].DisableControl();
        }
    }

    //Escolhendo a textura
    private IEnumerator Textura()
    {
        //mantem o jogo parado para as escolhas
        ResetAllTanks();
        DisableTankControl();
        m_CameraControl.SetStartPositionAndSize();

        m_MessageText.text = "Escolha a textura com os botoes de 1 - 9, e aperte 0 para continuar para o proximo tank: ";

        //passo as 9 texturas e deixo escolhar para cada tank uma vez
        while ((gameState < m_Tanks.Length))
        {
            if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                m_Tanks[gameState].Material(m_Materials[0]);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                m_Tanks[gameState].Material(m_Materials[1]);
            }
            else if(Input.GetKeyDown(KeyCode.Keypad3))
            {
                m_Tanks[gameState].Material(m_Materials[2]);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad4))
            {
                m_Tanks[gameState].Material(m_Materials[3]);
            }
            else if(Input.GetKeyDown(KeyCode.Keypad5))
            {
                m_Tanks[gameState].Material(m_Materials[4]);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad6))
            {
                m_Tanks[gameState].Material(m_Materials[5]);
            }
            else if(Input.GetKeyDown(KeyCode.Keypad7))
            {
                m_Tanks[gameState].Material(m_Materials[6]);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad8))
            {
                m_Tanks[gameState].Material(m_Materials[7]);
            }
            else if(Input.GetKeyDown(KeyCode.Keypad9))
            {
                m_Tanks[gameState].Material(m_Materials[8]);
            }
            //permite sair para todos os tanks depois que escolhe todas as texturas
            if (Input.GetKeyDown(KeyCode.Keypad0)) 
            {
                //vida e jogador

                m_Tanks[gameState].texto("P" + (gameState+1) + "Win" + m_Tanks[gameState].m_Wins);
                gameState++;
            }
            yield return null;
        }
        //yield return null;
    }
}