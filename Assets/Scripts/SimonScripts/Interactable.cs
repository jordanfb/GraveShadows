using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Interactable : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject interactTextInGame;
    public GameObject interactTextPrefab;
    public string baseText;
    public string onActivateText;
    public GameObject Player;
    public bool hubFocus = false;
    private bool touchingCollider = false;
    private bool beenInteracted = false;

    [Space]
    public bool replaceMaterialOnInteract = false; // this is used for lamps and breakerboxes which stop highlighting when they're done
    public MeshRenderer[] associatedMeshs;
    public Material[] associatedReplacementMaterials;

    void Start()
    {
        Player = GameObject.Find("Player");


    }

    [ContextMenu("Force All Find Nearby Mesh Renderers")]
    public void ForceAllFindNearbyMeshRenderers()
    {
        //Interactable[] i = GameObject.FindObjectsOfType<Interactable>();
        //foreach (Interactable interactable in i)
        //{
        //    interactable.FindNearbyMeshRenderers();
        //}
        if (transform.parent != null)
        {
            // then find all the interactables in your siblings
            int childCount = transform.parent.childCount;
            for (int i = 0; i < childCount; i++)
            {
                Interactable sib = transform.parent.GetChild(i).GetComponent<Interactable>();
                if (sib != null)
                {
                    if (sib.associatedMeshs == null || sib.associatedMeshs.Length == 0)
                    {
                        // only replace them if they don't already have associated meshes
                        sib.FindNearbyMeshRenderers();
                    }
                }
            }
        }
    }

    [ContextMenu("Find Nearby Mesh Renderers")]
    public void FindNearbyMeshRenderers()
    {
        MeshRenderer[] nearby = GameObject.FindObjectsOfType<MeshRenderer>();
        // look through them to find the closest one
        float distance = 0;
        MeshRenderer closest = null;
        foreach (MeshRenderer mr in nearby)
        {
            float thisDistance = Vector3.Distance(transform.position, mr.transform.position);
            if (closest == null || thisDistance < distance)
            {
                closest = mr;
                distance = thisDistance;
            }
        }
        associatedMeshs = new MeshRenderer[1];
        associatedMeshs[0] = closest;
    }

    private void OnDestroy()
    {
        if (interactTextInGame != null)
        {
            Destroy(interactTextInGame); // hopefully this will fix the press E to pick up bug?
            interactTextInGame = null;
        }
    }

    ~Interactable()
    {
        if (interactTextInGame != null)
        {
            Destroy(interactTextInGame); // hopefully this will fix the press E to pick up bug?
            interactTextInGame = null;
        }
    }

    // Update is called once per frame
    Vector3 velocity = Vector3.zero;
    void Update()
    {

        if (interactTextInGame == null)
        {
            return;
        }



        if (!interactTextInGame.activeInHierarchy)
        {
            return;
        }
        interactTextInGame.transform.position = Vector3.SmoothDamp(interactTextInGame.transform.position,
                                            Camera.main.WorldToScreenPoint(Player.transform.position + Vector3.up * 0.5f), ref velocity, 0.1f);




    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (interactTextInGame == null)
            {
                return;
            }

            if (hubFocus)
            {
                //print("hub focus");
                interactTextInGame.SetActive(false);
            }
            else
            {
                interactTextInGame.SetActive(true);
            }
        }


    }


    private void OnTriggerEnter(Collider other)
    {
        touchingCollider = true;
        if (other.gameObject.tag == "Player")
        {
            if (interactTextInGame == null)
            {

                interactTextInGame = Instantiate(interactTextPrefab, GameObject.Find("Canvas").transform);
                if (beenInteracted)
                {
                    interactTextInGame.GetComponent<Text>().text = onActivateText;
                }
                else
                {
                    interactTextInGame.GetComponent<Text>().text = baseText;
                }
                interactTextInGame.SetActive(true);
            }
            else
            {

                interactTextInGame.SetActive(true);


            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        touchingCollider = false;
        Destroy(interactTextInGame);
        interactTextInGame = null;

        //if (interactTextInGame == null)
        //{
        //    return;
        //}
        ////if (other.gameObject.tag == "Player")
        ////{
        ////    interactTextInGame.SetActive(false);
        ////}
    }

    public void DisableText()
    {
        if (interactTextInGame != null)
        {
            interactTextInGame.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Error no interact text on this for some reason somehow. gameobject name: " + gameObject.name); // this gets called in the tutorial FIX
        }
    }
    public void enableText()
    {
        if (interactTextInGame != null)
        {
            interactTextInGame.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Error no interact text on this for some reason somehow. gameobject name: " + gameObject.name);
        }
    }

    public void setActivatedText()
    {
        if (interactTextInGame == null)
        {
            return;
        }
        beenInteracted = true;
        interactTextInGame.GetComponent<Text>().text = onActivateText;
        StartCoroutine(ReplaceMaterialAfterTime());
    }

    private IEnumerator ReplaceMaterialAfterTime()
    {
        yield return new WaitForSeconds(.5f); // the lights turn on after .5 seconds so we're going to delay the highlight turning off too
        if (replaceMaterialOnInteract && associatedMeshs != null && associatedReplacementMaterials != null)
        {
            for (int i = 0; i < associatedMeshs.Length; i++)
            {
                associatedMeshs[i].material = associatedReplacementMaterials[i]; // replace the material
            }
        }
    }
}