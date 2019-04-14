using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class breakerBoxScript : MonoBehaviour
{

    public List<GameObject> affiliatedLights;
    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject l in affiliatedLights)
        {
            l.SetActive(false);
        }
    }





    private void OnTriggerStay(Collider other)
    {





        if (other.gameObject.tag == "Player")
        {

            if (Input.GetKeyDown(KeyCode.E)) {
                other.GetComponent<simplePlayerMovement>().getAnim().SetTrigger("reachOver");
                float counter = 0.0f;
                print("Hey");
                foreach (GameObject l in affiliatedLights)
                {

                    StartCoroutine(turnOnLight(0.5f + counter, l));
                    //l.SetActive(true);
                    GetComponent<Interactable>().setActivatedText();
                    counter += 0.3f;
                    
                }

            }

        }
    }

    IEnumerator turnOnLight(float secs, GameObject assocLight)
    {

        yield return new WaitForSeconds(secs);
        assocLight.SetActive(true);
    }


}
