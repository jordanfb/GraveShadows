using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class elevatorOpener : MonoBehaviour
{
    public float ammoutToOpen;
    public float openSpeed = 50f;
    private float elapsedTime = 0;
    public float timeStep = 0.01f;
    public bool isMoving = false;
    public bool isOpen = false;
    public GameObject assocLeftDoor;
    public GameObject assocRightDoor;
    float lastOpenDir;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {



    }

    //dir 
    IEnumerator OpenDoorAnim(float dir)
    {
        float movedSoFar = 0;
        print(movedSoFar);
        while (movedSoFar < ammoutToOpen)
        {

            isMoving = true;
            yield return new WaitForSeconds(timeStep);
            elapsedTime += timeStep;
            float change = openSpeed * Time.deltaTime;
            movedSoFar += change;
            assocLeftDoor.transform.position -= assocLeftDoor.transform.right* change;
            assocRightDoor.transform.position += assocLeftDoor.transform.right * change;

        }

        elapsedTime = 0;
        isMoving = false;
        isOpen = true;

    }

    IEnumerator CloseDoorAnim(float dir)
    {

        float movedSoFar = 0;
        while (movedSoFar < ammoutToOpen)
        {
            print(movedSoFar);
            isMoving = true;
            yield return new WaitForSeconds(timeStep);
            elapsedTime += timeStep;
            float change = openSpeed * Time.deltaTime;
            movedSoFar += change;
            assocLeftDoor.transform.position += assocLeftDoor.transform.right * change;
            assocRightDoor.transform.position -= assocLeftDoor.transform.right * change;

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
        if (isOpen) {
            return;
        }
        if (other.gameObject.tag == "Player")
        {



            IEnumerator coroutine = OpenDoorAnim(lastOpenDir);
            StartCoroutine(coroutine);


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
