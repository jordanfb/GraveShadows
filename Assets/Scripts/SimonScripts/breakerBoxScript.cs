using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class breakerBoxScript : Interactable
{
    private GameObject interactText;
    public GameObject interactTextPrefab;
    public List<GameObject> affiliatedLights;
    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject l in affiliatedLights)
        {
            l.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        displayUI();

    }


    public override void ColliderBehavior(Collider other)
    {
        interactText = Instantiate(interactText);
        if (other.gameObject.tag == "Player")
        {

            if (Input.GetKeyDown(KeyCode.E)) {
                other.GetComponent<simplePlayerMovement>().getAnim().SetTrigger("reachOver");
                foreach (GameObject l in affiliatedLights)
                {
                    l.SetActive(true);
                }

            }

        }
    }
}
