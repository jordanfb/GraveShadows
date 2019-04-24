using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EvidenceMono : MonoBehaviour
{
    [SerializeField]
    private Evidence _evidenceInfo;
    public bool isWaistLevel;
    [System.Serializable]


    public class EvidenceEvent : UnityEvent<Evidence>
    {

    }
    public EvidenceEvent onEvidenceCollected; // what gets invoked upon collecting this element of evidence

    private void Start()
    {
        setMatsToReg();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Evidence EvidenceInfo
    {
        get { return _evidenceInfo; }
        set { _evidenceInfo = value; }
    }

    public void CollectThisEvidence()
    {
        onEvidenceCollected.Invoke(EvidenceInfo);
        PlayerManager.instance.CollectEvidence(EvidenceInfo);
    }

    public void setMatsToReg() {
        Renderer mat = GetComponent<Renderer>();
        if (mat == null)
        {
            Renderer[] mats = GetComponentsInChildren<Renderer>();
            foreach (Renderer m in mats)
            {
                m.material.shader = Shader.Find("Standard");
                //m.material.SetFloat("_Outline", 0.005f);
                //m.material.SetColor("_Color", Color.white);
                //m.material.SetColor("_OutlineColor", Color.white);
            }
        }
        else
        {

            mat.material.shader = Shader.Find("Standard");
            //mat.material.SetFloat("_Outline", 0.005f);
            //mat.material.SetColor("_Color", Color.white);
            //mat.material.SetColor("_OutlineColor", Color.white);
        }

    }
    public void setMatsToOutline()
    {
        Renderer mat = GetComponent<Renderer>();
        if (mat == null)
        {
            Renderer[] mats = GetComponentsInChildren<Renderer>();
            foreach (Renderer m in mats)
            {
                m.material.shader = Shader.Find("Outlined/Silhouetted Diffuse");
                m.material.SetFloat("_Outline", 0.001f);
                m.material.SetColor("_Color", Color.white);
                m.material.SetColor("_OutlineColor", Color.white);
            }
        }
        else
        {

            mat.material.shader = Shader.Find("Outlined/Silhouetted Diffuse");
            mat.material.SetFloat("_Outline", 0.001f);
            mat.material.SetColor("_Color", Color.white);
            mat.material.SetColor("_OutlineColor", Color.white);
        }

    }
}
