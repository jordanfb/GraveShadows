using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    // Start is called before the first frame update
    bool UIshowing =false;
    public GameObject UIElement;
    public string test;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {


    }

    public void displayUI() {


    }

    public virtual void ColliderBehavior(Collider other) {
        print("ColliderBehavior() not implimented");
    }

    private void OnTriggerStay(Collider other) {
        ColliderBehavior(other);
        displayUI();
    }
}
