using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class breakerBoxScript : MonoBehaviour
{

    public List<GameObject> affiliatedLights;
    public bool isElevator;
    private bool hasBeenActivated;

    // Start is called before the first frame update
    void Start()
    {
        hasBeenActivated = false;
        if (isElevator == false)
        {
            foreach (GameObject l in affiliatedLights)
            {
                l.SetActive(false);
            }
        }
 
    }





    private void OnTriggerStay(Collider other)
    {
        if (hasBeenActivated) {
            return;
        }
        if (other.gameObject.tag == "Player")
        {

            if (Input.GetKeyDown(KeyCode.E)) {

                other.GetComponent<simplePlayerMovement>().getAnim().SetTrigger("reachOver");
                if (isElevator == false)
                {
                    float counter = 0.0f;

                    foreach (GameObject l in affiliatedLights)
                    {

                        StartCoroutine(turnOnLight(0.5f + counter, l));
                        //l.SetActive(true);
                        GetComponent<Interactable>().setActivatedText();
                        counter += 0.3f;

                    }
                }
                else if (isElevator == true)
                {
                    foreach (GameObject l in affiliatedLights)
                    {
                        l.GetComponent<ElevatorUpDown>().LetsMove();
                    }
                }

                hasBeenActivated = true;
            }

        }
    }

    IEnumerator turnOnLight(float secs, GameObject assocLight)
    {

        yield return new WaitForSeconds(secs);
        assocLight.SetActive(true);
    }


}
