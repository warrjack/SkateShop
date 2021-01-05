using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{

    //Variable Decleration
    private CharacterController characterController;    //CharacterController Component
    private float playerSpeed = 7.0f;                   //Speed character moves at (WASD)
    private float playerRotationSpeed = 4.5f;           //Speed of character rotation
    private Vector3 moveDirection = Vector3.zero;       //Set next character position
    private Ray ray;                                    //Ray to detect what's in front
    private RaycastHit rayHit;                          //The point the ray cast hits
    
    private GameObject clothesObject;                   //Clothes object interacting with

    private List<GameObject> clothesCarrying = new List<GameObject>();  //Clothes currently carrying
    private float stackSpacer = 0.2f;                   //Space between stacked clothes
    private Vector3 clothesCarryingPosition = new Vector3(0f, 0f, 0.8f);    //Position clothes start when being stacked
    public Mesh foldedMesh;

    private bool inputInUse = false;                    //Get controller input on down instance

    public Slider slider;                               //Repair or Folding progress bar
    public Image fill;
    private ProgressBar progressBarClass;               //Progress bar script
    private Vector3 screenToWorldSliderPos = Vector3.zero;  //Get screen position in 3D world space
    private bool incrementFoldingBar = false;           //Check if holding down space to enable slider progression

    private bool usingRegister = false;

    private List<GameObject> customerLineUpFromRegister = new List<GameObject>();
    private int numberClothesScanning = 0;
    private RegisterController registerInUse;


    // Start is called before the first frame update
    void Start()
    {
        //Initialize
        characterController = GetComponent<CharacterController>();      //Get self Character Controller component
        progressBarClass = slider.GetComponent<ProgressBar>();          //Get Slider Porgress Bar Script component
        slider.gameObject.SetActive(false);                             //Hide Slider
    }

    // Update is called once per frame
    void Update()
    {
        //Get horizontal and vertical movement (keybaord & controller)
        moveDirection.x = Input.GetAxisRaw("Horizontal");
        moveDirection.z = Input.GetAxisRaw("Vertical");
        //Move player in direction of moveDirection at playerSpeed over time
        characterController.Move(moveDirection.normalized * playerSpeed * Time.deltaTime);

        //If any movement button is pressed, character will rotate towards movement direction, otherwise won't move
        //if(Input.GetKey(KeyCode.W) | Input.GetKey(KeyCode.A) | Input.GetKey(KeyCode.S) | Input.GetKey(KeyCode.D))
        if (moveDirection.x != 0 | moveDirection.z != 0)
        {
            //Rotate character from current rotation to movement direction at playerRotationSpeed rate
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(moveDirection), playerRotationSpeed);
        }

        //Pick up item ([E] or X)
        if (Input.GetAxisRaw("Pick Up") != 0 && !inputInUse)
        {
            inputInUse = true;
            //Cast ray from player center forward relative to player
            ray = new Ray(transform.position, transform.forward);
            //if ray hits point
            if (Physics.Raycast(ray, out rayHit))
            {
                Debug.DrawLine(ray.origin, rayHit.point, Color.green);

                //Check if rayHit point is close enough to interact with
                if (rayHit.distance <= 1.1f)
                {
                    Debug.Log(rayHit.collider.name);
                    //Handle interaction with object passing object ray hit
                    interactWithObject(rayHit.collider.gameObject);
                }
            }
        }
        //Drop item ([Q] or O)
        else if (Input.GetAxisRaw("Put Down") != 0 && !inputInUse)
        {
            inputInUse = true;
            //Cast ray from player center forward relative to player
            ray = new Ray(transform.position, transform.forward);
            //if ray hits point
            if (Physics.Raycast(ray, out rayHit))
            {
                Debug.DrawLine(ray.origin, rayHit.point, Color.blue);
                //Check if rayHit point is close enough to interact with
                if (rayHit.distance <= 1.1f)
                {
                    //Handle dropping object with object passing object ray hit
                    if (clothesCarrying.Count > 0)
                    {
                        dropObject(rayHit.collider.gameObject);
                    }
                }
            }
        }

        //Fold or Undamage product ([Space] or Square)
        else if (Input.GetAxisRaw("Interact") != 0 && clothesCarrying.Count == 0 && !inputInUse)
        {
            inputInUse = true;
            //Cast ray from player center forward relative to player
            ray = new Ray(transform.position, transform.forward);
            //if ray hits point
            if (Physics.Raycast(ray, out rayHit))
            {
                Debug.DrawLine(ray.origin, rayHit.point, Color.blue);
                //Check if rayHit point is close enough to interact with
                if (rayHit.distance <= 1.1f)
                {
                    //Check if interacting with clothing
                    if (rayHit.collider.gameObject.name.Contains("Clothe"))
                    {
                        //If the product is unfolded and in folding station
                        if (rayHit.collider.transform.GetChild(0).gameObject.GetComponent<MeshFilter>().mesh.name.Contains("Unfolded") && rayHit.collider.transform.parent.name.Contains("FoldingStation"))
                        {

                            EnableSlider();
                        }
                        //If the product is damaged and in repair station
                        else if (rayHit.collider.transform.GetChild(0).gameObject.GetComponent<MeshFilter>().mesh.name.Contains("Damaged") && rayHit.collider.transform.parent.name.Contains("RepairStation"))
                        {

                            EnableSlider();
                        }
                        else
                        {
                            //Not sure what we're interacting with...
                            Debug.Log(rayHit.collider.transform.GetChild(0).gameObject.GetComponent<MeshFilter>().mesh.name);
                        }
                    }
               
                    else if (rayHit.collider.gameObject.name.Contains("Register"))
                    {
                        customerLineUpFromRegister = rayHit.collider.gameObject.GetComponent<RegisterController>().customerLineUp;
                        if (customerLineUpFromRegister.Count > 0)
                        {
                            usingRegister = true;
                            registerInUse = rayHit.collider.gameObject.GetComponent<RegisterController>();
                            numberClothesScanning = customerLineUpFromRegister[0].GetComponent<CustomerAI>().clothesCarrying.Count;
                            EnableSlider();
                        }
                    }
                }
            }

        }

        //Reset input use so buttons can be pressed again
        if (Input.GetAxisRaw("Pick Up") == 0 && Input.GetAxisRaw("Put Down") == 0)
        {
            inputInUse = false;
        }


        //Enable Slider
        void EnableSlider()
        {
            //Is pressing Interact button down
            incrementFoldingBar = true;
            //Get screen position of player
            screenToWorldSliderPos = Camera.main.WorldToScreenPoint(this.transform.position);
            //Set slider position 40 units above player
            screenToWorldSliderPos.y += 40f;
            //Move slider to new Position
            slider.transform.position = screenToWorldSliderPos;
        }
        //If Interact button is down and is interacting with something
        
        if (incrementFoldingBar)
        {
            //Show slider
            slider.gameObject.SetActive(true);
            if (usingRegister)
            {
                progressBarClass.IncrementProgress(0.1f/numberClothesScanning);
                fill.color = new Color(204, 144, 40, 255);
            }
            //Increment Slider using connected class at 0.10f units/frame
            else
            {
                fill.color = new Color(40, 204, 179, 255);
                progressBarClass.IncrementProgress(0.1f);
            }
            //If slider is full
            if (slider.value >= 1)
            {

                if(usingRegister)
                {
                    registerInUse.LeaveLineUp();
                    registerInUse.shiftLineUp();
                }
                else
                {
                    //Fold/Repair clothing interacting with
                    rayHit.collider.transform.GetChild(0).gameObject.GetComponent<MeshFilter>().mesh = foldedMesh;
                }
                //Hide Slider
                incrementFoldingBar = false;
            }
        }

        //If Interact button is up
        if (Input.GetAxisRaw("Interact") == 0 | incrementFoldingBar == false | inputInUse)
        {
            //No longer holding down
            incrementFoldingBar = false;
            //Set Slider to 0
            progressBarClass.ResetProgress();
            //Hide slider
            slider.gameObject.SetActive(false);
        }

        //Check if sprinting ([LShift] or R2)
        if (Input.GetAxisRaw("Speed Up") != 0)
        {
            //Sprint speed
            playerSpeed = 10.0f;
        }
        else
        {
            //Walk speed (default)
            playerSpeed = 7.0f;
        }
    }

    //Handle interaction with object hit by ray
    void interactWithObject(GameObject collider)
    {
        //If interacting with clothes
        if (collider.name.Contains("Clothes"))
        {
            //Set clothes object to child of character to be carried around
            collider.transform.SetParent(transform);
            //Set clothes carrying position to be ontop of hands
            clothesCarryingPosition.y = stackSpacer * clothesCarrying.Count;
            collider.transform.localPosition = clothesCarryingPosition;
            //Turn off collider to interact with other elements while carrying clothes
            collider.GetComponent<BoxCollider>().enabled = false;
            //Add clothes object just picked up to list of objects currently carrying
            clothesCarrying.Add(collider);
        }
    }
    //Handle dropping clothes off
    void dropObject(GameObject collider)
    {
        //If trying to interact with empty clothes placement
        if (collider.name.Contains("Place"))
        {
            //Recognize most recent clothes object
            clothesObject = clothesCarrying[clothesCarrying.Count - 1];
            //Check if clothes are being placed in correct area (by checking last 3 letters of both placement object and clothes object
            if(clothesObject.name.Substring(clothesObject.name.Length - 3) == collider.name.Substring(collider.name.Length - 3))
            {
                transferObjectParent(clothesObject, collider);
            }
        }

        //If placing object down at folding station
        else if (collider.name.Contains("FoldingStation") | collider.name.Contains("RepairStation"))
        {
            //Recognize most recent clothes object
            clothesObject = clothesCarrying[clothesCarrying.Count - 1];
            transferObjectParent(clothesObject, collider);
        }
    }

    //Put object down, changing clothes from one parent to another
    void transferObjectParent(GameObject clothing, GameObject collider)
    {
        //Put clothes in same position as placement hitbox
        clothesObject.transform.position = collider.transform.position;
        //Set clothes collider to true, to be able to intereact with them again
        clothesObject.GetComponent<BoxCollider>().enabled = true;
        //Transfer clothes from player parent to placement parent
        clothesObject.transform.SetParent(collider.transform);
        //Remove clohtes from list of objects player is carrying 
        clothesCarrying.Remove(clothesObject);
    }
}

