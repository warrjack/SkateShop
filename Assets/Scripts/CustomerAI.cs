using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CustomerAI : MonoBehaviour
{
    private NavMeshAgent agent;     //NavMeshAgent component
    public Vector3 destination;

    private List<Vector3> exitPoints = new List<Vector3>();

    private int randomInt = 0;
    private int randomChance = 0; 

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
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position != destination)
        {
            agent.SetDestination(destination);
        }
        else
        {
            setNewDestination();
        }
    }
    void setNewDestination()
    {
        //Choose if leaving or staying in store (0% chance leaving, inc by 3%)
        randomInt = Random.Range(0, 100);
        //Stay in store
        if (randomInt < randomChance)
        {

        }
    }
}
