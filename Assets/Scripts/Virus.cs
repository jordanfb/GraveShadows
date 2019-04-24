using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Virus : MonoBehaviour
{

    public static Virus instance;

    private bool wasdebugMode = false;
    private bool active = false;

    string personName = "buddy";
    float timer = 0;

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
        else if (user.ToLower() == "simonhopkins")
        {
            personName = "Simon";
        }
        else if (user.ToLower() == "erfranco")
        {
            // then we're okay to activate it
            personName = "Eric";
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
                SaveYourself();
            }
            wasdebugMode = false;
        }
    }
}
