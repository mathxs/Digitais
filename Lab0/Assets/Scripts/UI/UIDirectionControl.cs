using UnityEngine;

//Este codigo não foi alterado durante a criacao do jogo,
//utilizei tanto os slides do tidia quanto os videos do unity
//E este codigo foi importado pronto da loja do unity

public class UIDirectionControl : MonoBehaviour
{
    public bool m_UseRelativeRotation = true;  


    private Quaternion m_RelativeRotation;     


    private void Start()
    {
        m_RelativeRotation = transform.parent.localRotation;
    }


    private void Update()
    {
        if (m_UseRelativeRotation)
            transform.rotation = m_RelativeRotation;
    }
}
