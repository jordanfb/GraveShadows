using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ElevatorUpDown : MonoBehaviour
{
    public float speed;
    private bool goingUp;
    public bool isOn = false;
    private Vector3 maxHeight;
    private Vector3 minHeight;
    // Start is called before the first frame update
    void Start()
    {
        goingUp = false;
        maxHeight = transform.position;
        minHeight = new Vector3(transform.position.x, 0f, transform.position.z);
        //LetsMove();
    }

    // Update is called once per frame
    void Update()
    {

        

    }
   public void LetsMove()
    {
        StartCoroutine(Movement2(transform.position, maxHeight, minHeight, 3f));
    }
    //IEnumerator Movement(float whereAmI)
    //{
      //  print(" I Am " + whereAmI);
     //   print("maxHeight =" + maxHeight);
      //  if (whereAmI.y >= maxHeight.y || whereAmI.y <= minHeight.y)
      //  {
      //      speed *= -1;
      //      yield return new WaitForSeconds(3);
      //      transform.position = new Vector3(transform.position.x, whereAmI + speed, transform.position.z);


     //   }

       // transform.position = new Vector3(transform.position.x, whereAmI + speed, transform.position.z);

   // }
    IEnumerator Movement2(Vector3 whereIStart,  Vector3 max, Vector3 min, float time)
    {
        int counterman = 0;
        while (counterman < 10)
        {
            float elapsedTime = 0f;
            whereIStart = transform.position;
            while (elapsedTime < time)
            {
                transform.position = Vector3.Lerp(whereIStart, min, (elapsedTime / time));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            yield return new WaitForSeconds(2);
            elapsedTime = 0f;
            whereIStart = transform.position;
            while (elapsedTime < time)
            {
                transform.position = Vector3.Lerp(whereIStart, max, (elapsedTime / time));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            yield return new WaitForSeconds(2);
            yield return null;
        }

    }
}

