using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Virus : MonoBehaviour
{

    public static Virus instance;

    private bool wasdebugMode = false;
    private bool active = false;

    string personName = "buddy";
    float timer = 0;
    private bool addRBs = false;


    MeshRenderer[] meshRenderers = new MeshRenderer[0];

    string[] choices = { "HELLO PERSON HOW ARE YOU DOING TODAY??????", "I LOVE YOU PERSON", "Did you miss me?", "YOU've been HAxED", "AAAAAAAAAAAHHHH", "Peaches are for keepers", "Let's do this!", "YOU CAN WIN THIS I BELIEVE IN YOU",
    "PERSON what are you doing with your life?", "PERSON, why are you doing that?", "PERSON", "PERSON", "PERSON LOVES YOU", "I TOLD YOU I'D BE A GOOD WRITER", "HIRE ME GOOGLE", "HIRE ME ANYBODY", "WHAT's Up???", "I'm so random! *holds up spork*",
    "I DON't KNOW HOW MANY OF THESE TO WRITE", "I wrote a bunch of them in all caps for some reason", "Let's dance a jig, ehh?", "Are you an Avacado? Because you're 10/10", "I think we should call it YOUR DOOM!", "#meta", "#MakeMalwareMetaAgain", "#Hashtag",
    "#malware", "#virus", "#IJustPostedAllYourEmbarassingPhotosOnline", "##", "#PERSONSucks", "#PERSON", "#IsPERSONARealHumanBeing?", "You did all the work on this game", "#computersecurity", "RATE US WELL GAMEFEST", "GIVE US THE PRIZES"};

    private void SaveYourself()
    {
        if (instance == null || instance == this)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            active = true;
            SceneManager.sceneLoaded += OnSceneLoad;
        } else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // get whose computer this is
        string user = System.Environment.UserName;
        if (user.ToLower() == "jmanf")
        {
            personName = "Jordan";
        }
        else if (user.ToLower() == "simonhopkins" || user.ToLower() == "simon hopkins")
        {
            personName = "Simon";
        }
        else if (user.ToLower() == "erfranco" || user.ToLower() == "ericr")
        {
            // then we're okay to activate it
            personName = "Eric";
        }
        else if (user.ToLower() == "zack schwartz" || user.ToLower() == "schwaz3")
        {
            // then we're okay to activate it
            personName = "Zack";
        }
        else if (user.ToLower() == "shoem")
        {
            // then we're okay to activate it
            personName = "Amy";
        }
        else if (user.ToLower() == "gdoney" || user.ToLower() == "gdone")
        {
            // then we're okay to activate it
            personName = "Grant";
        }
        else
        {
            if (instance == this)
            {
                instance = null;
            }
            Destroy(gameObject);
        }
    }

    public void OnSceneLoad(Scene s, LoadSceneMode mode)
    {
        // get all the mesh renderers
        meshRenderers = GameObject.FindObjectsOfType<MeshRenderer>();
        GameObject ceilings = GameObject.Find("Ceilings");
        if (ceilings == null)
        {
            // try random names of ceiling objects...
            ceilings = GameObject.Find("Ceiling_GRp");
        }
        if (ceilings == null)
        {
            ceilings = GameObject.Find("Ceiling");
            if (ceilings != null)
            {
                ceilings.AddComponent<MeshCollider>().convex = true;
            }
        } else
        {
            // add mesh colliders to all the children
            for (int i =0; i < ceilings.transform.childCount; i++)
            {
                ceilings.transform.GetChild(i).gameObject.AddComponent<MeshCollider>().convex = true;
            }
        }

    }

    private string GetRandomStrnig()
    {
        string choice = choices[Random.Range(0, choices.Length)];
        return choice.Replace("PERSON", personName);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameplayManager.instance.debugMode)
        {
            wasdebugMode = true;

            // if we're active, then start changing things!
            if (active)
            {
                // also give a rigidbody to something if it has a meshrenderer and isn't a wall or a floor
                if (meshRenderers.Length > 0)
                {
                    // chose a random one to give a rigidbody to
                    int randomIndex = Random.Range(0, meshRenderers.Length);
                    if (meshRenderers[randomIndex].gameObject.name.ToLower().Contains("wall") || meshRenderers[randomIndex].gameObject.name.ToLower().Contains("floor") || meshRenderers[randomIndex].gameObject.name.ToLower().Contains("shadow") || meshRenderers[randomIndex].gameObject.name.ToLower().Contains("ceiling"))
                    {
                        // skip it since it's probably important?
                    }
                    else if (!meshRenderers[randomIndex].GetComponent<Rigidbody>())
                    {
                        // add one!
                        //Debug.Log("RB'd " + meshRenderers[randomIndex].gameObject.name);
                        if (!meshRenderers[randomIndex].GetComponent<Collider>() && !meshRenderers[randomIndex].GetComponent<BoxCollider>() && !meshRenderers[randomIndex].GetComponent<MeshCollider>())
                        {
                            // add a collider to it in the hopes that it'll become interactive I guess?
                            Bounds b = meshRenderers[randomIndex].gameObject.GetComponent<MeshFilter>().mesh.bounds;
                            BoxCollider bc = meshRenderers[randomIndex].gameObject.AddComponent<BoxCollider>();
                            bc.center = b.center;
                            bc.size = b.size;
                        } else if (meshRenderers[randomIndex].GetComponent<MeshCollider>())
                        {
                            // then set the collider to convex so that it works with the rigidbody
                            meshRenderers[randomIndex].GetComponent<MeshCollider>().convex = true;
                        }

                        Rigidbody rb = meshRenderers[randomIndex].gameObject.AddComponent<Rigidbody>();
                        bool useGravity = Random.Range(0, 2) == 1;
                        rb.useGravity = useGravity; // randomly use gravity or not
                        if (!useGravity)
                        {
                            // give it a random velocity!
                            rb.velocity = Random.insideUnitSphere;
                        }
                    }
                }



                timer -= Time.deltaTime;
                if (timer > 0)
                {
                    return;
                }
                // reset the timer and do stuff!
                timer = Random.Range(.5f, 1f);
                // change things
                // find a random text item and change the string to somethign
                Text[] texts = FindObjectsOfType<Text>();
                if (texts.Length > 0)
                {
                    texts[Random.Range(0, texts.Length)].text = GetRandomStrnig();
                }
                TextMeshPro[] textpro = FindObjectsOfType<TextMeshPro>();
                if (textpro.Length > 0)
                {
                    textpro[Random.Range(0, textpro.Length)].text = GetRandomStrnig();
                }
                TextMeshProUGUI[] textproUI = FindObjectsOfType<TextMeshProUGUI>();
                if (textproUI.Length > 0)
                {
                    textproUI[Random.Range(0, textproUI.Length)].text = GetRandomStrnig();
                }
            }
        }
        else
        {
            if (wasdebugMode)
            {
                // start the virus
                if (Input.GetKey(KeyCode.G))
                {
                    addRBs = true;
                }
                SaveYourself();
            }
            wasdebugMode = false;
        }
    }
}
