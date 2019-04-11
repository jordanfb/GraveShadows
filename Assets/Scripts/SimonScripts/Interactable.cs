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

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!gameObject.activeInHierarchy) {
            return;
        }
        interactText.transform.position = Camera.main.WorldToScreenPoint(transform.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (interactText == null)
            {
                interactText = Instantiate(interactTextPrefab, GameObject.Find("Canvas").transform);
                interactText.GetComponent<Text>().text = baseText;
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
