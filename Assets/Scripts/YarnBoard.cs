using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YarnBoard : MonoBehaviour
{
    [SerializeField]
    private GameObject _pinPrefab;
    [SerializeField]
    private float _pinnedObjectOffset;
    private List<GameObject> _pins;
    // Start is called before the first frame update
    void Start()
    {
        _pins = new List<GameObject>();
        InitializePins();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void InitializePins()
    {
        foreach(GameObject go in PlayerManager.instance.CollectedEvidence) {
            GameObject pin = Instantiate(_pinPrefab) as GameObject;
            //TODO: Change transform position (need to decide best way of doing so)
            _pins.Add(pin);

            Evidence evidence = go.GetComponent<EvidenceMono>().EvidenceInfo;
            switch(evidence.GetEvidenceType)
            {
                case EvidenceType.Object:
                    //Display photo associated with evidence SO & parent it to pin
                    GameObject photo = new GameObject(evidence.Name);
                    photo.transform.parent = pin.transform;
                    photo.transform.position = new Vector3(photo.transform.position.x, _pinnedObjectOffset, photo.transform.position.z);
                    SpriteRenderer pSprite = photo.AddComponent<SpriteRenderer>();
                    pSprite.sprite = evidence.Photo;
                    break;
                case EvidenceType.Conversation:
                    break;
                case EvidenceType.Document:
                    //Display the gameObject as it exists in the world
                    //Later on we might have to rotate it to face the camera
                    GameObject document = Instantiate(go) as GameObject;
                    document.name = evidence.Name;
                    document.transform.parent = pin.transform;
                    document.transform.position = new Vector3(document.transform.position.x, _pinnedObjectOffset, document.transform.position.z);
                    break;
            }
        }
    }
}
