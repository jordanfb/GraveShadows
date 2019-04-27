using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class YarnBoard : MonoBehaviour
{
    [SerializeField]
    private GameObject _pinPrefab;
    [SerializeField]
    private GameObject _evidencePrefab;
    [SerializeField]
    private GameObject _suspectPrefab;
    [SerializeField]
    private GameObject _boardCanvas;
    public GameObject _flavorTextPanel;
    [SerializeField]
    private Text _evidenceTitle;
    [SerializeField]
    private Text _flavorTextAsset;
    public GameObject _suspectInfoPanel;
    public GameObject _closeEvidenceButton;
    [SerializeField]
    private Text _suspectCodeName;
    [SerializeField]
    private Text _suspectBio;
    [SerializeField]
    private float _pinZOffset = 0.0f;
    [SerializeField]
    private float _offsetRatio;
    [Tooltip("Determines the x offset of character text meshes on the notebook paper.")]
    [SerializeField]
    private float _textPositionXOffset;
    [SerializeField]
    private int _charTextFontSize;
    [SerializeField]
    private YarnBoardCamera _yarnBoardCamera;
    [SerializeField]
    private float _photoScalar;
    [SerializeField]
    private Transform _yarnBoardParent;
    [SerializeField]
    private float _evidenceXOffset;
    [SerializeField]
    private float _evidenceYOffset;
    [SerializeField]
    private float _pinScale;
    [SerializeField]
    private float _pinOffset;
    [SerializeField]
    public Vector3 evidenceOffsetFromYarnboardHit = new Vector3(0, 0, .05f);

    [Space]
    [SerializeField]
    public GameObject removeFromYarnboardText; // used to show that you're removing an evidence from the yarnboard
    public LineDraw lineDraw;

    private List<GameObject> createdEvidenceOnBoard = new List<GameObject>();
    private Dictionary<SerializedEvidence, GameObject> yarnboardConnectionEvidenceLocation = new Dictionary<SerializedEvidence, GameObject>();
    private YarnBoardMode mode = YarnBoardMode.None;
    private Collider _collider;
    private Vector2 _minPinPos;
    private Vector2 _maxPinPos;

    // evidene moving info:
    private GameObject movingYarnboardItem;
    private SerializedEvidence movingYarnboardEvidence;
    private Vector3 movingStartPos;

    public enum YarnBoardMode
    {
        None, Displaying, Moving
    }

    // Start is called before the first frame update
    void Start()
    {
        GenerateContent(); // load the content from the evidence manager
    }

    public void GenerateContent()
    {
        // destroy the old ones:
        for (int i = 0; i < createdEvidenceOnBoard.Count; i++)
        {
            Destroy(createdEvidenceOnBoard[i]);
        }
        createdEvidenceOnBoard.Clear();
        lineDraw.DestroyAll();
        mode = YarnBoardMode.None;
        movingYarnboardItem = null;
        movingYarnboardEvidence = null;
        yarnboardConnectionEvidenceLocation.Clear();

        // then make the new ones!
        for (int i = 0; i < EvidenceManager.AllEvidence.Count; i++)
        {
            // spawn the evidence that's on the board or off it, and do nothing to the rest
            SerializedEvidence se = EvidenceManager.AllEvidence[i];
            YarnBoardEntity ybe = EvidenceManager.instance.ReferencedEntity(se);

            // We're only worried about evidence we have available for the yarn board
            if (se.evidenceState != SerializedEvidence.EvidenceState.OnYarnBoard)
                continue;

            Evidence e = ybe as Evidence;
            Suspect s = ybe as Suspect;
            GameObject go = null;
            if (e != null)
            {
                // it's regular evidence
                // spawn an evidence prefab:
                go = Instantiate(_evidencePrefab, transform.parent);
                EvidenceMono emono = go.GetComponentInChildren<EvidenceMono>();
                emono.EvidenceInfo = e;
                SpriteRenderer sr = go.GetComponentInChildren<SpriteRenderer>();
                sr.sprite = e.Photo;
                createdEvidenceOnBoard.Add(go);
            }
            else if (s != null)
            {
                // then create a suspect evidence thing
                // spawn an evidence prefab:
                go = Instantiate(_suspectPrefab, transform.parent);
                SuspectMono susmono = go.GetComponentInChildren<SuspectMono>();
                susmono.SuspectInfo = s;
                SpriteRenderer sr = go.GetComponentInChildren<SpriteRenderer>();
                sr.sprite = s.Photo;
                createdEvidenceOnBoard.Add(go);
            }

            if (go == null)
            {
                Debug.LogError("GameObject is null for some reason figure it out Alexander");
                continue;
            }
            yarnboardConnectionEvidenceLocation.Add(se, go);
            //if (se.evidenceState == SerializedEvidence.EvidenceState.OnYarnBoard)
            //{
            //    Debug.LogWarning("This is going to need to be fixed since it's using global position");
            //}
            go.transform.position = se.location; // new Vector3(se.location.x, se.location.y, 0);
        }

        // then add the yarn connections:
        // this is so inefficient but it's what I've got...
        for (int i = 0; i < EvidenceManager.AllEvidence.Count; i++)
        {
            // spawn the evidence that's on the board or off it, and do nothing to the rest
            SerializedEvidence se = EvidenceManager.AllEvidence[i];
            if (se.evidenceState != SerializedEvidence.EvidenceState.OnYarnBoard)
                continue;
            if (!yarnboardConnectionEvidenceLocation.ContainsKey(se))
            {
                Debug.LogError("Key not found somehow");
                continue;
            }
            // otherwise we connect it to all the evidence which is on the yarnboard
            for (int j = 0; j < se.connectedEvidence.Count; j++)
            {
                // connect it to all of the other ones that exist
                SerializedEvidence otherConnection = EvidenceManager.AllEvidence[se.connectedEvidence[j]];
                if (yarnboardConnectionEvidenceLocation.ContainsKey(otherConnection))
                {
                    lineDraw.CreateFullLine(yarnboardConnectionEvidenceLocation[se], yarnboardConnectionEvidenceLocation[otherConnection], se, otherConnection);
                }
            }
            // then remove it from the dictionary so that we don't get duplicate lines
            yarnboardConnectionEvidenceLocation.Remove(se);
        }
    }

    public void AddGeneratedGameobject(GameObject go)
    {
        // this is for the yarn items
        createdEvidenceOnBoard.Add(go);
    }

    private float PinOffset(Collider c)
    {
        return (1f / _offsetRatio) * (c.bounds.max.y - c.bounds.center.y);
    }

    private void Update()
    {
        if(GameplayManager.instance.debugMode && Input.GetKeyDown(KeyCode.T))
        {
            int next = Random.Range(0, EvidenceManager.AllEvidence.Count - 1);
            EvidenceManager.AllEvidence[next].evidenceState = SerializedEvidence.EvidenceState.OffYarnBoard;
            GenerateContent();
        }

        if (Input.GetMouseButtonDown(1) && mode == YarnBoardMode.None)
        {
            //Raycast to screen
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // On hit, check if it's a piece of evidence
            if (Physics.Raycast(ray, out hit))
            {
                GameObject evidence = hit.collider.gameObject;
                if (evidence.tag == "Evidence")
                {
                    _yarnBoardCamera.LookAtEvidence(evidence.transform);
                    EvidenceMono em = evidence.GetComponent<EvidenceMono>();
                    SuspectMono sm = evidence.GetComponent<SuspectMono>();
                    mode = YarnBoardMode.Displaying;
                    if (em != null)
                    {
                        // display it on the suspect board
                        _suspectCodeName.text = em.EvidenceInfo.Name;
                        _suspectBio.text = em.EvidenceInfo.FlavorText + "\n";
                        _suspectInfoPanel.SetActive(true);
                        _suspectInfoPanel.GetComponentInChildren<Scrollbar>().size = 0;
                        _suspectInfoPanel.GetComponentInChildren<Scrollbar>().value = 1;
                        _closeEvidenceButton.SetActive(true);
                    }
                    else if (sm != null)
                    {
                        _suspectCodeName.text = sm.SuspectInfo.CodeName + "\n(" + sm.SuspectInfo.Name + ")";
                        _suspectBio.text = sm.SuspectInfo.Bio + "\n";
                        _suspectInfoPanel.SetActive(true);
                        _suspectInfoPanel.GetComponentInChildren<Scrollbar>().size = 0;
                        _suspectInfoPanel.GetComponentInChildren<Scrollbar>().value = 1;
                        _closeEvidenceButton.SetActive(true);
                    }
                    else
                    {
                        // some weird error occured so just don't go anywhere
                        Debug.LogWarning("Couldn't find suspect or evidence monobehavior");
                        mode = YarnBoardMode.None;
                    }
                }
            }
        }
        else if (Input.GetMouseButtonDown(0) && mode == YarnBoardMode.None)
        {
            // move the item you clicked on
            //Debug.Log("Mtrying to oving now");
            //Raycast to screen
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // On hit, check if it's a piece of evidence
            if (Physics.Raycast(ray, out hit))
            {
                GameObject evidence = hit.collider.gameObject;
                if (evidence.tag == "Evidence")
                {
                    //Debug.Log("Moving now");
                    // get the correct object to move
                    movingYarnboardItem = hit.collider.transform.parent.gameObject;
                    movingStartPos = evidence.transform.position + evidenceOffsetFromYarnboardHit;
                    if (movingYarnboardItem.GetComponentInChildren<EvidenceMono>() != null)
                    {
                        movingYarnboardEvidence = EvidenceManager.instance.FindSerializedEvidence(movingYarnboardItem.GetComponentInChildren<EvidenceMono>().EvidenceInfo);
                    }
                    else if (movingYarnboardItem.GetComponentInChildren<SuspectMono>() != null)
                    {
                        movingYarnboardEvidence = EvidenceManager.instance.FindSerializedEvidence(movingYarnboardItem.GetComponentInChildren<SuspectMono>().SuspectInfo);
                    }
                    else
                    {
                        movingYarnboardEvidence = null;
                        Debug.LogError("ERROR: UNABLE TO GET EVIDENCE FOR MOVING IT");
                    }
                    mode = YarnBoardMode.Moving;
                }
            }
        }
        else if (Input.GetMouseButton(0) && mode == YarnBoardMode.Moving)
        {
            //Debug.Log("clicking annd holding");

            // move the item you clicked on

            //Raycast to screen
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // On hit, check if it's a piece of evidence
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("YarnBoard")))
            {
                movingYarnboardItem.transform.position = hit.point + evidenceOffsetFromYarnboardHit;
                removeFromYarnboardText.SetActive(false);
            } else
            {
                // highlight that you're going to remove it from the board! How the fuck? Probably interact text saying "Remove from board"
                removeFromYarnboardText.SetActive(true);
            }
        }
        else if (Input.GetMouseButtonUp(0) && mode == YarnBoardMode.Moving)
        {
            // move the item you clicked on
            //Debug.Log("nolonger clicking");
            removeFromYarnboardText.SetActive(false); // so that if we were removing it it's now no longer displayed
            //Raycast to screen
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // On hit, check if it's a piece of evidence
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("YarnBoard")))
            {
                Vector3 newPos = hit.point + evidenceOffsetFromYarnboardHit;
                movingYarnboardItem.transform.position = newPos;
                // then create a movement event!
                if (movingYarnboardEvidence != null)
                {
                    YarnBoardMoveEvidenceEvent moveEvent = new YarnBoardMoveEvidenceEvent(movingYarnboardEvidence.evidenceindex, movingYarnboardEvidence, movingStartPos, newPos);
                    moveEvent.Redo(); // set it in the location as well!
                    UndoRedoStack.AddEvent(moveEvent);
                }
            }
            else
            {
                // remove it from the yarnboard!
                YarnBoardAddToYarnBoardEvent undoredoevent = new YarnBoardAddToYarnBoardEvent(movingYarnboardEvidence.evidenceindex, movingYarnboardEvidence, true);
                // it's inverted since we're removing it
                UndoRedoStack.AddEvent(undoredoevent); // create the undo/redo event
                                                       // then redo it to remove it from the yarnboard
                undoredoevent.Redo(); // remove it from the yarnboard!
                GenerateContent(); // re-generate content to deal with removing it from the yarnboard!
            }
            mode = YarnBoardMode.None;
        }
    }

    private void RandomizePositionOnBoard(ref GameObject pin, int index)
    {
        float randX = Random.Range(_minPinPos.x * .75f, _maxPinPos.x * .75f);
        float randY = Random.Range(_minPinPos.y * .75f, _maxPinPos.y * .75f);
        pin.transform.position = new Vector3(randX + index, randY, transform.position.z - _pinZOffset);
        pin.GetComponentInChildren<Transform>().position = Vector3.zero;
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

    public Transform GetYarnboardParent()
    {
        return _yarnBoardParent;
    }

    public void DeactivateFlavorText()
    {
        _closeEvidenceButton.SetActive(false);
        _flavorTextPanel.SetActive(false);
        _flavorTextAsset.text = "";
        mode = YarnBoardMode.None;
    }

    public void DeactivateSuspectPanel()
    {
        _closeEvidenceButton.SetActive(false);
        _suspectInfoPanel.SetActive(false);
        _suspectCodeName.text = "";
        _suspectBio.text = "";
        mode = YarnBoardMode.None;
    }
}
