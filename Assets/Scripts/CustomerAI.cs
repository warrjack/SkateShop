using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CustomerAI : MonoBehaviour
{
    private NavMeshAgent agent;     //NavMeshAgent component
    public Vector3 destination;     //Next position to move to 
    private bool isMoving = false;

    private List<Vector3> exitPoints = new List<Vector3>();             //Points declaring customer leaving scene
    private List<GameObject> clothesCarrying = new List<GameObject>();  //Clothes currently carrying
    public List<GameObject> tableList = new List<GameObject>();         //List of parents of clothing object points
    private List<GameObject> clothePoints = new List<GameObject>();     //List of clothing objects to determin next position

    private int randomInt = 0;                  //Random number generated for chances of customer choosing something
    private float stayInStoreChance = 0f;       //Chance % staying in the store
    private float pickUpChance = 0f;            //Chance % in choosing picking up clothes
    private float messUpChance = 0f;            //Chance % in choosing messing up clothes
    private float damageChance = 0f;            //Chance % in choosing damaging clothes
    private float checkOutChance = 0f;          //Chance % in choosing checking out clothes
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
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position != destination)
        {
            agent.SetDestination(destination);
            isMoving = true;
        }
        else
        {
            isMoving = false;
            StartCoroutine(DelayBeforeNextMove());
        }
        Debug.Log("Is close as poss: " + CheckIfCloseAsPossible());
    }

    bool CheckIfCloseAsPossible()
    {
        if (isMoving && !agent.pathPending && !agent.hasPath)
        {
            Debug.Log("reached destination");
            isMoving = false;
            agent.ResetPath();
            return true;
        }
        else
        {
            return false;
        }
    }


    //Get position for each clothing object
    void findAllClothingLocations()
    {
        foreach (GameObject child in tableList)
        {
            if (child.name.Contains("Placement"))
            {
                clothePoints.Add(child);
            }
        }
    }

    IEnumerator DelayBeforeNextMove()
    {
        Debug.Log("BeforeDelay");
        yield return new WaitForSeconds(4);
        Debug.Log("After");
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
            //Pick new location
            destination = clothePoints[Random.Range(0, clothesCarrying.Count - 1)].transform.position;
            //if (agent.FindClosestEdge())

        }
        //Stay in store
        //Pick up clothes
        //Damage clothes
        //Mess up clothes        
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
}
