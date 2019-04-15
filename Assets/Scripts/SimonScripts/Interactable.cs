using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Interactable : MonoBehaviour
{
    // Start is called before the first frame update

    private GameObject interactText;
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
        if(interactText == null) {
            return;
        }
        if (!interactText.activeInHierarchy) {
            return;
        }
        print(interactText);
        interactText.transform.position = Camera.main.WorldToScreenPoint(Player.transform.position+ Vector3.up*0.5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (interactText == null)
            {
                interactText = Instantiate(interactTextPrefab, GameObject.Find("Canvas").transform);
                interactText.GetComponent<Text>().text = baseText;
                interactText.SetActive(true);
            }
            else
            {
                interactText.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            interactText.SetActive(false);
        }
    }

    public void setActivatedText() {
        interactText.GetComponent<Text>().text = onActivateText;
    }





}
