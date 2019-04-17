using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ElevatorUpDown : MonoBehaviour
{
    private bool moving;
    public bool isOn = false;
    private Vector3 maxHeight;
    private Vector3 minHeight;
    public GameObject childGate;
    public GameObject topGate;
    public GameObject bottomGate;
    // Start is called before the first frame update
    void Start()
    {
        moving = false;
        maxHeight = transform.position;
        minHeight = new Vector3(transform.position.x, 0f, transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {

    }
   public void LetsMove()
    {
        StartCoroutine(Movement2(transform.position, maxHeight, minHeight, 3f));
    }
    IEnumerator Movement2(Vector3 whereIStart,  Vector3 max, Vector3 min, float time)
    {
        //da big loop
        moving = true;
        while (moving == true)
        {
            float elapsedTime = 0f;
            whereIStart = transform.position;
            //going down
            while (elapsedTime < time) 
            {
                transform.position = Vector3.Lerp(whereIStart, min, (elapsedTime / time));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            childGate.SetActive(false);
            bottomGate.SetActive(false);
            yield return new WaitForSeconds(2);
            childGate.SetActive(true);
            bottomGate.SetActive(true);
            elapsedTime = 0f;
            whereIStart = transform.position;
            //going up
            while (elapsedTime < time)
            {
                transform.position = Vector3.Lerp(whereIStart, max, (elapsedTime / time));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            childGate.SetActive(false);
            topGate.SetActive(false);
            yield return new WaitForSeconds(2);
            childGate.SetActive(true);
            topGate.SetActive(true);
            yield return null;
        }

    }
}

