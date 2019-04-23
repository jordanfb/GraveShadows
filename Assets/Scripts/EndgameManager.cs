using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class EndgameManager : MonoBehaviour
{

    public List<Suspect> suspects;
    public GameObject Prefab;

    [System.NonSerialized]
    public UnityAction[] clickButtonEvent;
    private GameObject[] choices;

    public float radius = 1;
    
    // Start is called before the first frame update
    void Start()
    {
        GenerateStuff();
    }

    [ContextMenu("Generate stuff")]
    private void GenerateStuff()
    {
        if (choices != null)
        {
            for (int i = 0; i < choices.Length; i++)
            {
                Destroy(choices[i]);
            }
        }
        float degrees = 360 / suspects.Count;
        choices = new GameObject[suspects.Count];
        clickButtonEvent = new UnityAction[suspects.Count];
        float currentDegrees = 90; // start upright for pretty

        for (int i = 0; i < suspects.Count; i++)
        {
            GameObject go = Instantiate(Prefab, transform);
            choices[i] = go;
            Vector3 pos = new Vector3(Mathf.Cos(currentDegrees * Mathf.Deg2Rad), Mathf.Sin(currentDegrees * Mathf.Deg2Rad), 0) * radius;
            go.transform.localPosition = pos;

            EndgameComponent egc = go.GetComponent<EndgameComponent>();
            Suspect s = suspects[i];
            clickButtonEvent[i] += () => { SuspectChosen(s); };
            egc.SetStuff(suspects[i], clickButtonEvent[i]);
            egc.nameText.text = suspects[i].CodeName;

            currentDegrees += degrees;
        }
    }

    public void SuspectChosen(Suspect s)
    {
        Debug.Log("suspect chosen " + s.Name);
        Debug.Log("Real suspect " + EvidenceManager.instance.culprit.Name);
        if (EvidenceManager.instance.culprit == s)
        {
            Debug.Log("YOU WIN");
            EvidenceManager.instance.Generated = false;
            GameplayManager.instance.FadeOut(() => { SceneManager.LoadScene("WinScene"); });
        } else
        {
            Debug.Log("UGGGGH You lose");
            EvidenceManager.instance.Generated = false;
            GameplayManager.instance.FadeOut(() => { SceneManager.LoadScene("LoseScene"); });
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
