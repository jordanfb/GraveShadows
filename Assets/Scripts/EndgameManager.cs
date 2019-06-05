using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class EndgameManager : MonoBehaviour
{

    public List<Suspect> suspects;
    public GameObject Prefab;

    [System.NonSerialized]
    public UnityAction[] clickButtonEvent;
    private GameObject[] choices;
    public Text chooseLeaderText;

    public float radius = 1;
    public float speed = 1;

    private float currentRadiusPercent = 0;

    public bool open = false; // instead of enabling/disabling this use this instead?
    
    // Start is called before the first frame update
    void Start()
    {
        GenerateStuff();
    }

    private void OnEnable()
    {
        currentRadiusPercent = 0;
    }

    public void SetOpen(bool open)
    {
        this.open = open;
        if (open)
        {
            if (currentRadiusPercent <= 0)
            {
                // then disable everything
                for (int i = 0; i < suspects.Count; i++)
                {
                    choices[i].SetActive(true);
                }
                chooseLeaderText.gameObject.SetActive(true);
            }
        }
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
            //Vector3 pos = new Vector3(Mathf.Cos(currentDegrees * Mathf.Deg2Rad), Mathf.Sin(currentDegrees * Mathf.Deg2Rad), 0) * 0;
            //go.transform.localPosition = pos;
            go.transform.localPosition = Vector3.zero; // with the animated opening just start it at 0, 0, 0
            go.transform.localScale = Vector3.zero;

            EndgameComponent egc = go.GetComponent<EndgameComponent>();
            Suspect s = suspects[i];
            clickButtonEvent[i] += () => { SuspectChosen(s); };
            egc.SetStuff(suspects[i], clickButtonEvent[i]);
            egc.nameText.text = suspects[i].CodeName;

            currentDegrees += degrees;
            print("Creating suspect " + i);
        }
    }

    public void SuspectChosen(Suspect s)
    {
        //Debug.Log("suspect chosen " + s.Name);
        //Debug.Log("Real suspect " + EvidenceManager.instance.culprit.Name);
        if (EvidenceManager.instance.culprit == s)
        {
            //Debug.Log("YOU WIN");
            EvidenceManager.instance.Generated = false;
            GameplayManager.instance.FadeOut(() => { SceneManager.LoadScene("winScene2"); });
        } else
        {
            //Debug.Log("UGGGGH You lose");
            EvidenceManager.instance.Generated = false;
            GameplayManager.instance.FadeOut(() => { SceneManager.LoadScene("loseScene2"); });
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (open && currentRadiusPercent < 1)
        {
            currentRadiusPercent += Time.deltaTime * speed;
            currentRadiusPercent = Mathf.Min(1, currentRadiusPercent); // limit it at 1
            float currentRadius = DeskDayDescriptionItem.Smootherstep(currentRadiusPercent) * radius;

            float degrees = 360 / suspects.Count;
            float currentDegrees = 90; // start upright for pretty
            for (int i = 0; i < suspects.Count; i++)
            {
                Vector3 pos = new Vector3(Mathf.Cos(currentDegrees * Mathf.Deg2Rad), Mathf.Sin(currentDegrees * Mathf.Deg2Rad), 0) * currentRadius;
                choices[i].transform.localPosition = pos;
                choices[i].transform.localScale = Vector3.one * DeskDayDescriptionItem.Smootherstep(currentRadiusPercent);

                currentDegrees += degrees;
            }
            string leaderText = "Pick the Leader";
            chooseLeaderText.text = leaderText.Substring(0, Mathf.Min((int)(leaderText.Length * currentRadiusPercent * 2), leaderText.Length));
        } else if (!open && currentRadiusPercent > 0)
        {
            currentRadiusPercent -= Time.deltaTime * speed;
            currentRadiusPercent = Mathf.Max(0, currentRadiusPercent); // limit it at 1
            float currentRadius = DeskDayDescriptionItem.Smootherstep(currentRadiusPercent) * radius;

            float degrees = 360 / suspects.Count;
            float currentDegrees = 90; // start upright for pretty
            for (int i = 0; i < suspects.Count; i++)
            {
                Vector3 pos = new Vector3(Mathf.Cos(currentDegrees * Mathf.Deg2Rad), Mathf.Sin(currentDegrees * Mathf.Deg2Rad), 0) * currentRadius;
                choices[i].transform.localPosition = pos;
                choices[i].transform.localScale = Vector3.one * DeskDayDescriptionItem.Smootherstep(currentRadiusPercent);

                currentDegrees += degrees;
            }
            string leaderText = "Pick the Leader";
            chooseLeaderText.text = leaderText.Substring(0, Mathf.Min((int)(leaderText.Length * currentRadiusPercent * 2), leaderText.Length));
            if (currentRadiusPercent <= 0)
            {
                // then disable everything
                for (int i = 0; i < suspects.Count; i++)
                {
                    choices[i].SetActive(false);
                }
                chooseLeaderText.gameObject.SetActive(false);
            }
        }
    }
}
