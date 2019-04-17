using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Interactable : MonoBehaviour
{
    // Start is called before the first frame update

    private GameObject interactTextInGame;
    public GameObject interactTextPrefab;
    public string baseText;
    public string onActivateText;
    public GameObject Player;

    void Start()
    {
        Player = GameObject.Find("Player");
    }


    // Update is called once per frame
    void Update()
    {
        if(interactTextInGame == null) {
            return;
        }
        if (!interactTextInGame.activeInHierarchy) {
            return;
        }

        Vector3 velocity = Vector3.zero;
        interactTextInGame.transform.position = Vector3.SmoothDamp(interactTextInGame.transform.position, 
                                                Camera.main.WorldToScreenPoint(Player.transform.position + Vector3.up * 0.5f), ref velocity, 0.01f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (interactTextInGame == null)
            {
                print(interactTextInGame);

                interactTextInGame = Instantiate(interactTextPrefab, GameObject.Find("Canvas").transform);
                print(interactTextInGame);
                interactTextInGame.GetComponent<Text>().text = baseText;
                interactTextInGame.SetActive(true);
            }
            else
            {
                interactTextInGame.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (interactTextInGame == null)
        {
            return;
        }
        if (other.gameObject.tag == "Player")
        {
            interactTextInGame.SetActive(false);
        }
    }

    public void DisableText()
    {
        interactTextInGame.SetActive(false);
    }

    public void setActivatedText() {
        if (interactTextInGame == null)
        {
            return;
        }
        interactTextInGame.GetComponent<Text>().text = onActivateText;
    }





}
