using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class doorOpenScript : MonoBehaviour
{
    // Start is called before the first frame update


    public float degrees = 0;
    public float openSpeed = 50f;
    private float elapsedTime = 0;
    public float timeStep = 0.01f;
    public bool isMoving = false;
    public bool isOpen = false;
    public GameObject assocDoor;
    float lastOpenDir;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {



    }

    //dir 
    IEnumerator OpenDoorAnim(float dir) {


        while (Mathf.Abs(degrees) < 90) {
            isMoving = true;
            yield return new WaitForSeconds(timeStep);
            elapsedTime += timeStep;
            float degreesChange = openSpeed * Mathf.Sqrt(elapsedTime*20) * Time.deltaTime * dir;
            assocDoor.transform.Rotate(Vector3.up, degreesChange);
            degrees += degreesChange;
        }
        
        elapsedTime = 0;
        isMoving = false;
        isOpen = true;

    }

    IEnumerator CloseDoorAnim(float dir)
    {


        while (-dir*degrees > 0)
        {
            isMoving = true;
            yield return new WaitForSeconds(timeStep);
            elapsedTime += timeStep;
            float degreesChange = openSpeed * Mathf.Sqrt(elapsedTime * 20) * Time.deltaTime * dir;
            assocDoor.transform.Rotate(Vector3.up, degreesChange);
            degrees += degreesChange;
        }

        elapsedTime = 0;
        isMoving = false;
        isOpen = false;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (isMoving)
        {
            return;
        }
        Vector3 dirToPlayer = transform.position - other.transform.position;
        lastOpenDir = -Mathf.Sign(Vector3.Dot(dirToPlayer, transform.right));
    }


    private void OnTriggerStay(Collider other)
    {
        if (isMoving) {
            return;
        }
        if (other.gameObject.tag == "Player") {

            if (Input.GetKeyDown(KeyCode.E))
            {

                IEnumerator coroutine = OpenDoorAnim(lastOpenDir);
                StartCoroutine(coroutine);
            }

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (isMoving)
        {
            return;
        }
        if (other.gameObject.tag == "Player")
        {
            if (isOpen)
            {
                IEnumerator coroutine = CloseDoorAnim(-lastOpenDir);
                StartCoroutine(coroutine);
            }

        }
    }

}
