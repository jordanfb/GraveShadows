using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lampSwivelScript : MonoBehaviour
{

    public GameObject assocLamp;
    public GameObject assocLight;
    private bool isMoving = false;
    public float speed = 50f;
    public float angleSpunEachTime;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (!assocLamp.activeInHierarchy) {
            return;
        }
        if (isMoving) {
            return;
        }
        if (other.gameObject.tag == "Player")
        {
            if (Input.GetKeyDown(KeyCode.E)) {

                IEnumerator coroutine = rotateLightAndLamp(angleSpunEachTime);
                StartCoroutine(coroutine);
                //assocLight.transform.Rotate(assocLamp.transform.up, 90f);
            }
        }
    }


    IEnumerator rotateLightAndLamp(float amount) {

        float rotSoFar = 0;
        isMoving = true;
        while (rotSoFar < amount) {
            yield return new WaitForSeconds(0.01f);

            float rotAmount = speed * Time.deltaTime;
            rotSoFar += rotAmount;
            assocLamp.transform.RotateAround(assocLamp.transform.position, Vector3.up, rotAmount);
            assocLight.transform.RotateAround(assocLamp.transform.position, Vector3.up, rotAmount);
        }

        isMoving = false;

    }
}
