using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class YarnBoard : MonoBehaviour
{
    [SerializeField]
    private GameObject _pinPrefab;
    [SerializeField]
    private GameObject _boardCanvas;
    [SerializeField]
    private GameObject _flavorTextPanel;
    [SerializeField]
    private Text _flavorTextAsset;
    [SerializeField]
    private float _pinnedObjectOffset;
    [Tooltip("Determines the x offset of character text meshes on the notebook paper.")]
    [SerializeField]
    private float _textPositionXOffset;
    [SerializeField]
    private int _charTextFontSize;
    [SerializeField]
    private YarnBoardCamera _yarnBoardCamera;

    private List<GameObject> _pins;
    private bool displaying = false;
    private Renderer _renderer;
    private Vector2 _minPinPos;
    private Vector2 _maxPinPos;

    // Start is called before the first frame update
    void Start()
    {
        //For use with randomly placing pins
        _renderer = GetComponent<Renderer>();
        _minPinPos = new Vector2(_renderer.bounds.min.x, _renderer.bounds.min.y);
        _maxPinPos = new Vector2(_renderer.bounds.max.x, _renderer.bounds.max.y);

        //Initialize Pins
        _pins = new List<GameObject>();
        foreach (GameObject go in PlayerManager.instance.CollectedEvidence)
        {
            GameObject pin = Instantiate(_pinPrefab) as GameObject;
            RandomizePositionOnBoard(ref pin);
            _pins.Add(pin);

            Evidence evidence = go.GetComponent<EvidenceMono>().EvidenceInfo;
            switch (evidence.GetEvidenceType)
            {
                case EvidenceType.Object:
                    //Display photo associated with evidence SO & parent it to pin
                    GameObject photo = new GameObject(evidence.Name);
                    photo.tag = "Evidence";
                    photo.transform.parent = pin.transform;
                    photo.transform.localPosition = Vector3.zero;
                    photo.AddComponent<SpriteRenderer>().sprite = evidence.Photo;
                    photo.AddComponent<BoxCollider>();
                    photo.AddComponent<EvidenceMono>().EvidenceInfo = evidence;
                    break;
                case EvidenceType.Conversation:
                    //Create a generic piece of paper sprite
                    GameObject paper = new GameObject(evidence.Name);
                    paper.tag = "Evidence";
                    paper.transform.parent = pin.transform;
                    paper.transform.localPosition = Vector3.zero;
                    paper.AddComponent<SpriteRenderer>().sprite = evidence.Photo;
                    paper.AddComponent<BoxCollider>();
                    paper.AddComponent<EvidenceMono>().EvidenceInfo = evidence;
                    //Create text object to display over paper
                    GameObject text = new GameObject(evidence.Name + "Characters");
                    text.transform.parent = paper.transform;
                    text.transform.localPosition = new Vector3(0f, 0f, -1f);
                    DisplayCharacters(ref text, evidence);
                    break;
                case EvidenceType.Document:
                    //Display the gameObject as it exists in the world
                    //Later on we might have to rotate it to face the camera
                    //Because we are instantiating a stored go and not making a new one, 
                    //adding a collider and EvidenceMono is unnecessary.
                    GameObject document = Instantiate(go) as GameObject;
                    document.tag = "Evidence";
                    document.name = evidence.Name;
                    document.transform.parent = pin.transform;
                    // CHANGE WHEN ACTUAL DOCUMENT MODELS ARE AVAILABLE
                    document.transform.localPosition = new Vector3(0f, 6f, 0f);
                    break;
            }
        }
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0) && !displaying)
        {
            //Raycast to screen
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            
            // On hit, check if it's a piece of evidence
            if(Physics.Raycast(ray, out hit))
            {
                GameObject evidence = hit.collider.gameObject;
                if(evidence.tag == "Evidence")
                {
                    _yarnBoardCamera.LookAtEvidence(evidence.transform);
                    _flavorTextPanel.SetActive(true);
                    _flavorTextAsset.text = evidence.GetComponent<EvidenceMono>().EvidenceInfo.FlavorText;
                    displaying = true;
                }
            }
        }
    }

    private void RandomizePositionOnBoard(ref GameObject pin)
    {
        float randX = Random.Range(_minPinPos.x * .75f, _maxPinPos.y * .75f);
        float randY = Random.Range(_minPinPos.y * .75f, _maxPinPos.y * .75f);
        pin.transform.position = new Vector3(randX, randY, 0f);
    }

    //Isolated for readability sake
    private void DisplayCharacters(ref GameObject go, Evidence ev)
    {
        //Compile a string with all characters
        string charText = "";
        foreach(string ch in ev.Characters)
        {
            charText += (ch + "\n");
        }

        //Create a new textmesh with parameters
        TextMesh t = go.AddComponent<TextMesh>();
        t.text = charText;
        t.color = Color.black;
        t.fontSize = _charTextFontSize;
    }

    public void DeactivateFlavorText()
    {
        _flavorTextPanel.SetActive(false);
        _flavorTextAsset.text = "";
        displaying = false;
    }
}
