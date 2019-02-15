using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class simplePlayerMovement : MonoBehaviour
{
    // Start is called before the first frame update
    private Camera mainCam;
    public GameObject player;
    private Rigidbody rb;
    public float speed;
    private Animator anim;
    public GameObject shadowPlane;
    private float shadowLeftBound;
    private float shadowRightBound;

    const float PLAYER_WIDTH = 0.5f;
    const float WALL_SPEED = 0.1f;

    public Collider currentWallCollider = null;

    [SerializeField]
    private bool isInShadowRealm = false;

    void Start()
    {
        mainCam = Camera.main;
        rb = GetComponent<Rigidbody>();
        anim = player.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float moveDirX = Input.GetAxis("Horizontal");
        float moveDirY = Input.GetAxis("Vertical");
        if (!isInShadowRealm) {
            thirdPersonMovement(moveDirX, moveDirY);
            //arbitrary speed in which player has to move, will need to change when I get Anims


        }
        else {
            wallMovement(moveDirX, moveDirY, currentWallCollider);
        }


    }

    void thirdPersonMovement(float _moveDirX, float _moveDirY) {

        rb.velocity = ((new Vector3(mainCam.transform.forward.x, 0f, mainCam.transform.forward.z).normalized * _moveDirY) + (mainCam.transform.right.normalized * _moveDirX)) * speed;
        if (rb.velocity.magnitude > 0.1f)
        {
            anim.SetBool("isWalking", true);
        }
        else
        {
            anim.SetBool("isWalking", false);
        }
    }
    bool touchingWall = true;
    void wallMovement(float _moveDirX, float _moveDirY, Collider _currentWallCollider) {


        if (_currentWallCollider == null) {
            Debug.Log("ERROR: current wall is null");
            return;

        }
        LayerMask mask = LayerMask.GetMask("WallLayer");
        RaycastHit hitWall;
        touchingWall = false;
        Vector3 nextPos = shadowPlane.transform.position + shadowPlane.transform.forward * -_moveDirX * WALL_SPEED - (Mathf.Sign(_moveDirX)* shadowPlane.transform.forward* PLAYER_WIDTH);
        if (Physics.Raycast(nextPos, -shadowPlane.transform.up, out hitWall, Mathf.Infinity, mask))
        {
            if (hitWall.collider == _currentWallCollider) {
                touchingWall = true;

            }
           
        }

        if (touchingWall)
        {

            shadowPlane.transform.position += shadowPlane.transform.forward * -_moveDirX * 0.1f;
            if (Mathf.Abs(_moveDirX) > 0.1){
                anim.SetBool("isWalking", true);
            }
            else {
                anim.SetBool("isWalking", false);
            }
        }
        







    }

    public bool toggleIsInShadowRealm() {
        isInShadowRealm = !isInShadowRealm;
        return isInShadowRealm;
    }
    public bool getIsInShadowRealm()
    {
        return isInShadowRealm;

    }

    public Vector2 setShadowBounds(float _shadowLeftBound, float _shadowRightBound) {

        shadowLeftBound = _shadowLeftBound;
        shadowRightBound = _shadowRightBound;
        return new Vector2(_shadowLeftBound, _shadowRightBound);
    }



    public Collider setCurrentWallCollider(Collider newCurrentWallCollider) {
        currentWallCollider = newCurrentWallCollider;
        return currentWallCollider;

    }

    private void OnTriggerEnter(Collider other)
    {
        if(Input.GetKeyDown(KeyCode.E) && other.gameObject.layer == 12 && !isInShadowRealm)
        {
            PlayerManager.instance.CollectEvidence(other.gameObject);
            Destroy(other.gameObject);
        }
    }
}
