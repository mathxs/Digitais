using UnityEngine;
using UnityEngine.UI;


//Neste primeiro progeto não escrevi nenhum pedaço de codigo, apenas acompanhei a explicação do uso no tutorial unity

//Os objetos public sao utilizados para interagir com o unity, no caso os AudioClip são usados para musica
//O rigidbody, para detectar colisoes e permitir tratar delas

public class TankShooting : MonoBehaviour
{
    public int m_PlayerNumber = 1;       
    public Rigidbody m_Shell;            
    public Transform m_FireTransform;    
    public Slider m_AimSlider;           
    public AudioSource m_ShootingAudio;  
    public AudioClip m_ChargingClip;     
    public AudioClip m_FireClip;         
    public float m_MinLaunchForce = 15f; 
    public float m_MaxLaunchForce = 30f; 
    public float m_MaxChargeTime = 0.75f;
    //Criada para uso do Nav Mesh
    public bool m_IsAI;


    private string m_FireButton;         
    private float m_CurrentLaunchForce;  
    private float m_ChargeSpeed;         
    private bool m_Fired;                
    
    //inicia variaveis ao ser ativado
    private void OnEnable()
    {
        m_CurrentLaunchForce = m_MinLaunchForce;
        m_AimSlider.value = m_MinLaunchForce;
    }


    private void Start()
    {
        m_FireButton = "Fire" + m_PlayerNumber;
        m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime;
    }


    //Atualiza em "tempo real" (ciclo do unity)
    //chama tanto o Fire para iniciar o tiro como mantem a logica dele até ele morrer.
    //Chama os sons conforme impacto, ou lancamento.
    private void Update()
    {   

        //Usado para retornar caso o IA esteja funcionando sem executar o update
        if (m_IsAI)
        {
            return; 
        }
        m_AimSlider.value = m_MinLaunchForce;

        if (m_CurrentLaunchForce >= m_MaxLaunchForce && !m_Fired)
        {
            m_CurrentLaunchForce = m_MaxLaunchForce;
            Fire();
        }

        else if (Input.GetButtonDown(m_FireButton))
        {
            m_Fired = false;
            m_CurrentLaunchForce = m_MinLaunchForce;
            m_ShootingAudio.clip = m_ChargingClip;
            m_ShootingAudio.Play();
        }

        else if (Input.GetButton(m_FireButton) && !m_Fired)
        {
            m_CurrentLaunchForce += m_ChargeSpeed * Time.deltaTime;
            m_AimSlider.value = m_CurrentLaunchForce;
        }

        else if (Input.GetButtonUp(m_FireButton) && !m_Fired)
        {
            Fire();
        }
    }

    //Atirando
    public void Fire()
    {
        //disparando sempre com forca media
        if (m_IsAI)
        {
            m_CurrentLaunchForce = m_MaxLaunchForce / 2.0f;
        }

        m_Fired = true;
        Rigidbody shellInstance = Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;
        shellInstance.velocity = m_CurrentLaunchForce * m_FireTransform.forward; ;

        m_ShootingAudio.clip = m_FireClip;
        m_ShootingAudio.Play();
        m_CurrentLaunchForce = m_MinLaunchForce;
    }
}