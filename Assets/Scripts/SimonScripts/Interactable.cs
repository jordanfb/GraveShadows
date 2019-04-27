using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Interactable : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject interactTextInGame;
    public GameObject interactTextPrefab;
    public string baseText;
    public string onActivateText;
    public GameObject Player;
    public bool hubFocus= false;
    private bool touchingCollider = false;
    private bool beenInteracted = false;

    void Start()
    {
        Player = GameObject.Find("Player");


    }

    ~Interactable() {

        Destroy(gameObject);
    }

    // Update is called once per frame
    Vector3 velocity = Vector3.zero;
    void Update()
    {

        if (interactTextInGame == null) {
            return;
        }



        if (!interactTextInGame.activeInHierarchy ) {
            return;
        }
        interactTextInGame.transform.position = Vector3.SmoothDamp(interactTextInGame.transform.position,
                                            Camera.main.WorldToScreenPoint(Player.transform.position + Vector3.up * 0.5f), ref velocity, 0.1f);




    }
    private void OnTriggerStay(Collider other) {
        if (other.gameObject.tag == "Player")
        {
            if(interactTextInGame == null) {
                return;
            }

            if (hubFocus)
            {
                //print("hub focus");
                interactTextInGame.SetActive(false);
            }
            else
            {
                interactTextInGame.SetActive(true);
            }
        }


    }


    private void OnTriggerEnter(Collider other)
    {
        touchingCollider = true;
        if (other.gameObject.tag == "Player")
        {
            if (interactTextInGame == null)
            {

                interactTextInGame = Instantiate(interactTextPrefab, GameObject.Find("Canvas").transform);
                if (beenInteracted)
                {
                    interactTextInGame.GetComponent<Text>().text = onActivateText;
                }
                else
                {
                    interactTextInGame.GetComponent<Text>().text = baseText;
                }
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
        touchingCollider = false;
        Destroy(interactTextInGame);

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
        if (interactTextInGame != null)
        {
            interactTextInGame.SetActive(false);
        } else
        {
            Debug.LogWarning("Error no interact text on this for some reason somehow. gameobject name: " + gameObject.name);
        }
    }
    public void enableText() {
        if (interactTextInGame != null)
        {
            interactTextInGame.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Error no interact text on this for some reason somehow. gameobject name: " + gameObject.name);
        }
    }

    public void setActivatedText() {
        if (interactTextInGame == null)
        {
            return;
        }
        beenInteracted = true;
        interactTextInGame.GetComponent<Text>().text = onActivateText;
    }





}
