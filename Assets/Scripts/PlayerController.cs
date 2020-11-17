using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    //Variable Decleration
    private CharacterController characterController;    //CharacterController Component
    private float playerSpeed = 6.0f;                   //Speed character moves at (WASD)
    private float playerRotationSpeed = 1.5f;
    private Vector3 moveDirection = Vector3.zero;

    private string inRageOf = "";                       //What can interact with ("", Register, Folding, Placing)

    private GameObject clothesObject;           //Clothes object interacting with

    private List<string> clothesCarrying = new List<string>();  //Clothes currently carrying
    private List<string> objectsInRange = new List<string>();   //Objects in range of interacting


    

    
    // Start is called before the first frame update
    void Start()
    {
        //Initialize
        characterController = GetComponent<CharacterController>();

    }

    // Update is called once per frame
    void Update()
    {
        moveDirection.x = Input.GetAxisRaw("Horizontal");
        moveDirection.z = Input.GetAxisRaw("Vertical");
        characterController.Move(moveDirection.normalized * playerSpeed * Time.deltaTime);

        if(Input.GetKey(KeyCode.W) | Input.GetKey(KeyCode.A) | Input.GetKey(KeyCode.S) | Input.GetKey(KeyCode.D))
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(moveDirection), playerRotationSpeed);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            interactWithObject();
        }
    }

    void interactWithObject()
    {

    }
}
