﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CustomerAI : MonoBehaviour
{
    private NavMeshAgent agent;     //NavMeshAgent component
    public Vector3 destination;     //Next position to move to 
    public Vector3 destinationCache = Vector3.zero;     //Save previous destination for rotation towards clothing before overridden
    private Vector3 targetRotation;     //Target rotation character is rotating towards
    private bool isMoving = false;      //Check if currently moving or just finished moving towards point

    private List<Vector3> exitPoints = new List<Vector3>();             //Points declaring customer leaving scene
    public List<GameObject> tableList = new List<GameObject>();         //List of parents of clothing object points
    private List<GameObject> clothePoints = new List<GameObject>();     //List of clothing objects to determin next position
    private GameObject currentClothing;                                 //Current Clothing position destination is set to
    private List<GameObject> clothesCarrying = new List<GameObject>();  //Clothes currently carrying
    private float stackSpacer = 0.2f;                   //Space between stacked clothes
    private Vector3 clothesCarryingPosition = new Vector3(0f, 0f, 1f);    //Position clothes start when being stacked
    private GameObject clothesGrabbing;
    private bool canGrab = true;
    private float initialStackSpacer = 0.66f;

    public Mesh messUpMesh;     //Mesh for messy clothing
    public Mesh brokenMesh;     //Mesh for broken clothing

    public int randomInt = 0;                  //Random number generated for chances of customer choosing something
    public float stayInStoreChance = 0f;       //Chance % staying in the store
    public float pickUpChance = 0f;            //Chance % in choosing picking up clothes
    public float messUpChance = 0f;            //Chance % in choosing messing up clothes
    public float damageChance = 0f;            //Chance % in choosing damaging clothes
    public float checkOutChance = 0f;          //Chance % in choosing checking out clothes

    private Vector3 beforePosition = Vector3.zero;  //Check position for before being stuck
    private Vector3 afterPosition = Vector3.zero;   //Check position for after being stuck
    private float rangePosition = 0.3f;             //Range of leeway position can be the same to be considered stuck

    // Start is called before the first frame update
    void Start()
    {
        // Initialize components
        agent = GetComponent<NavMeshAgent>();

        //Set current destination to starting point
        destination = transform.position;
        //Add exit points to exitPoints array
        exitPoints.Add(new Vector3(-5f, 0.75f, 9.31f));
        exitPoints.Add(new Vector3(5f, 0.75f, 9.31f));

        //Get all clothing placement points
        findAllClothingLocations();
        setNextMove();
        agent.updateRotation = false;
        //StartCoroutine(checkIfStuckDelay());
    }

    // Update is called once per frame
    void Update()
    {
        //If current position is not the destination
        if (transform.position != destination)
        {
            //Set destination to agent's next point
            agent.SetDestination(destination);
            //Character is currently moving
            isMoving = true;
        }
        else
        {
            //Check if just finished moving
            if(isMoving)
            {
                //Delay before setting next move/destination
                isMoving = false;
                StartCoroutine(DelayBeforeNextMove());
            }
            //Set looking position y to eye level
            destinationCache.y = transform.position.y;
            //Get rotation based on character position
            targetRotation = destinationCache - transform.position;
        }
        //If position is close as possible, set destination to current position to get new position
        if (CheckIfCloseAsPossible())
        {
            destination = transform.position;
        }
    }

    //Late frame movement
    void LateUpdate()
    {
        //Rotation for when agent is moving
        if (isMoving)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(agent.velocity.normalized), 1000f * Time.deltaTime);
        }
        //Rotation for when looking at clothing
        else
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(targetRotation), 500f*Time.deltaTime);
        }
    }

    //Check if character is as close to destination position as possible
    bool CheckIfCloseAsPossible()
    {
        //If currently moving and not met destination
        if (isMoving && !agent.pathPending && !agent.hasPath)
        {
            //Remove path
            agent.ResetPath();
            //Change destination to current position to declare destination was reached
            destination = transform.position;
            return true;
        }
        else
        {
            //Character is not close enough to final destination
            return false;
        }
    }


    //Get position for each clothing object
    void findAllClothingLocations()
    {
        //For each table
        foreach (GameObject child in tableList)
        {
            //For each item on table
            for (int i = 0; i < child.transform.childCount - 1; i++)
            {
                //If item is a placement game object
                if (child.transform.GetChild(i).name.Contains("Place"))
                {
                    //Add gameobject as potential position
                    clothePoints.Add(child.transform.GetChild(i).gameObject);
                }
            }
        }
    }

    //Delay before making next decision
    IEnumerator DelayBeforeNextMove()
    {
        yield return new WaitForSeconds(2);
        setNextMove();

    }

    //Decide next move
    void setNextMove()
    {
        //Choose if leaving or staying in store (0% chance leaving, inc by 3%)
        randomInt = Random.Range(0, 100);

        //If carrying
        if (clothesCarrying.Count > 0)
        {
            //Staying in store (true if staying)
            if (randomGenerator(checkOutChance))
            {
                //Pick new location
                //Pick up clothes
                if (randomGenerator(pickUpChance))
                {

                }
                //Mess up clothes
                if (randomGenerator(pickUpChance))
                {

                }
                //Damage clothes
                if (randomGenerator(damageChance))
                {

                }
                //Check out
                if (randomGenerator(checkOutChance))
                {

                }
            }
        }

        //If not carrying anything
        else
        {
            //If already chosen next position
            if(currentClothing != null)
            {
                //Deciding factor
                randomInt = Random.Range(0, 100);

                //If staying in store
                if (randomGenerator(stayInStoreChance))
                {
                    stayInStoreChance += 2f;
                    //If clothes are in placement position
                    if (currentClothing.transform.childCount > 0)
                    {
                        clothesGrabbing = currentClothing.transform.GetChild(0).gameObject;
                        if (randomInt < 33)
                        {
                            currentClothing.transform.GetChild(0).GetChild(0).GetComponent<MeshFilter>().mesh = messUpMesh;
                        }
                        else if (randomInt > 33 && randomInt < 66 && canGrab)
                        {
                            //Set clothes object to child of character to be carried around
                            clothesGrabbing.transform.SetParent(transform);
                            //Set clothes carrying position to be ontop of hands
                            clothesCarryingPosition.y = initialStackSpacer + stackSpacer * clothesCarrying.Count;
                            clothesGrabbing.transform.localPosition = clothesCarryingPosition;
                            //Turn off collider to interact with other elements while carrying clothes
                            clothesGrabbing.GetComponent<BoxCollider>().enabled = false;
                            //Add clothes object just picked up to list of objects currently carrying
                            clothesCarrying.Add(clothesGrabbing);
                            //canGrab = false;
                        }
                        else
                        {
                            currentClothing.transform.GetChild(0).GetChild(0).GetComponent<MeshFilter>().mesh = brokenMesh;
                        }
                    }
                }
                //If leaving store
                else
                {

                }
                
            }

        }
        //Stay in store
        //Pick up clothes
        //Damage clothes
        //Mess up clothes        

        //Pick new location
        currentClothing = clothePoints[Random.Range(0, clothePoints.Count - 1)];
        destination = currentClothing.transform.position;
        //Save destination for after destination is overridden
        destinationCache = destination;
    }

    bool randomGenerator(float chance)
    {
        randomInt = Random.Range(0, 100);
        if (chance < randomInt)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    IEnumerator checkIfStuckDelay()
    {
        beforePosition = transform.position;
        yield return new WaitForSeconds(4.1f);
        afterPosition = transform.position;
        checkIfStuck();
    }

    void checkIfStuck()
    {
        if (beforePosition.x > afterPosition.x - rangePosition && beforePosition.x < afterPosition.x + rangePosition)
        {
            Debug.Log(gameObject.name + " claimed Stuck");
            setNextMove();
            StartCoroutine(checkIfStuckDelay());
        }
        else if (beforePosition.y > afterPosition.y - rangePosition && beforePosition.y < afterPosition.y + rangePosition)
        {
            Debug.Log(gameObject.name + " claimed Stuck");
            setNextMove();
            StartCoroutine(checkIfStuckDelay());
        }
        else
        {
            Debug.Log(gameObject.name + " claimed Not Stuck");
            StartCoroutine(checkIfStuckDelay());
        }
    }
}
