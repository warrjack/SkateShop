using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player1Controller : MonoBehaviour
{

    //Self
    private float movementSpeed = 1800.0f;
    private Rigidbody rbody;
    private GameObject selfGameObject;

    //Check Interaction
    private bool inRegisterRange = false;
    private bool inClothesRange = false;
    private bool inFoldingStationRange = false;
    private bool inPlacementRange = false;
    private List<string> clothesInRange = new List<string>();
    private Transform tempTrans;
    private GameObject clothesObject;
    public static List<string> clothesCarrying = new List<string>();
    private bool isCarryingClothes = false;

    private List<string> foldingStationsInRange = new List<string>();
    private GameObject foldingStationObject;

    private List<string> placementInRange = new List<string>();
    private GameObject placementObject;


    // Start is called before the first frame update
    void Start()
    {
        rbody = GetComponent<Rigidbody>();
        selfGameObject = GameObject.Find(gameObject.name);
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        Interaction();
    }

    //Player 1 Movement Input
    void Movement()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputZ = Input.GetAxisRaw("Vertical");

        float moveX = inputX * movementSpeed * Time.deltaTime;
        float moveZ = inputZ * movementSpeed * Time.deltaTime;

        transform.LookAt(transform.position + new Vector3(moveX, 0, moveZ));

        rbody.velocity = new Vector3(moveX, 0, moveZ);
    }

    void Interaction()
    {
        //Pick up
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log(inFoldingStationRange+", "+inPlacementRange+ ", "+isCarryingClothes + ", " + inClothesRange + ", " + clothesCarrying.Count);
            if (inRegisterRange == true)
            {
                Debug.Log("Register");
            }


            //Picking up Clothes in Range
            else if (inClothesRange == true && clothesCarrying.Count <= 3)
            {
                
                
                var stackHeight = 0.2f;
                var stackSpacer = 0.1f;

                var directionLooking = selfGameObject.transform.rotation.eulerAngles.y;
                clothesObject = GameObject.Find(clothesInRange[clothesInRange.Count - 1]);
                ChangeParent(selfGameObject.GetComponent<Collider>(), clothesObject.GetComponent<Collider>());
                clothesCarrying.Add(clothesObject.gameObject.name);

                if (inFoldingStationRange)
                {
                    var objectsStackedOnCollider = foldingStationObject.GetComponent<FoldingStationHandler>().objectStacked;
                    objectsStackedOnCollider.RemoveAt(objectsStackedOnCollider.Count - 1);
                }
                else if (inPlacementRange)
                {
                    var objectsStackedOnCollider = placementObject.GetComponent<PlacementHandler>().objectStacked;
                    objectsStackedOnCollider.RemoveAt(objectsStackedOnCollider.Count - 1);
                }
                //Right
                if (directionLooking == 90)
                {
                    clothesObject.transform.position = new Vector3(selfGameObject.transform.position.x + 1.0f, selfGameObject.transform.position.y + stackSpacer + (stackHeight * clothesCarrying.Count), selfGameObject.transform.position.z);
                }
                //Down
                else if (directionLooking == 180)
                {
                    clothesObject.transform.position = new Vector3(selfGameObject.transform.position.x, selfGameObject.transform.position.y + stackSpacer + (stackHeight * clothesCarrying.Count), selfGameObject.transform.position.z - 1.0f);
                }
                //Left
                else if (directionLooking == 270){
                    clothesObject.transform.position = new Vector3(selfGameObject.transform.position.x - 1.0f, selfGameObject.transform.position.y + stackSpacer + (stackHeight * clothesCarrying.Count), selfGameObject.transform.position.z);
                }
                else
                {
                    clothesObject.transform.position = new Vector3(selfGameObject.transform.position.x, selfGameObject.transform.position.y + stackSpacer + (stackHeight * clothesCarrying.Count), selfGameObject.transform.position.z + 1.0f);
                }
                isCarryingClothes = true;
            }
        }

        //Drop off
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log(isCarryingClothes + ", " + inFoldingStationRange + ", " + clothesCarrying.Count);
            //If carrying an object
            if (isCarryingClothes)
            {
                clothesObject = GameObject.Find(clothesCarrying[clothesCarrying.Count - 1]);
                //If at folding station
                if (inFoldingStationRange)
                {
                    var objectsStackedOnCollider = foldingStationObject.GetComponent<FoldingStationHandler>().objectStacked;
                    //If station has 3 or less
                    if (objectsStackedOnCollider.Count <= 3)
                    {
                        foldingStationObject = GameObject.Find(foldingStationsInRange[0]);
                        RevertParent(clothesObject.GetComponent<Collider>());
                        var stackHeight = 0.2f;

                        clothesObject.transform.position = new Vector3(foldingStationObject.transform.position.x, foldingStationObject.transform.position.y + (stackHeight * objectsStackedOnCollider.Count), foldingStationObject.transform.position.z);
                        objectsStackedOnCollider.Add(clothesCarrying[clothesCarrying.Count - 1]);
                        clothesCarrying.RemoveAt(clothesCarrying.Count - 1);
                    }
                }
                //If at empty placement
                else if (inPlacementRange)
                {
                    var objectsStackedOnCollider = placementObject.GetComponent<PlacementHandler>().objectStacked;
                    if (objectsStackedOnCollider.Count == 0)
                    {
                        placementObject = GameObject.Find(placementInRange[0]);
                        RevertParent(clothesObject.GetComponent<Collider>());
                        clothesObject.transform.position = placementObject.transform.position;
                        objectsStackedOnCollider.Add(clothesCarrying[clothesCarrying.Count - 1]);
                        clothesCarrying.RemoveAt(clothesCarrying.Count - 1);
                    }
                }
                //If carrying nothing
                if (clothesCarrying.Count == 0)
                {
                    isCarryingClothes = false;
                }
            }
        }

    }

    void OnTriggerEnter(Collider other)
    {
        //Name Formatting
        var clothesName = other.gameObject.name.Remove(other.gameObject.name.Length - 4);
        var stationName = other.gameObject.name.Remove(other.gameObject.name.Length - 1);

        //If Register
        if (other.gameObject.name == "Register" || other.gameObject.name == "Register2")
        {
            inRegisterRange = true;
        }

        //If Going to pick up Clothes
        else if (clothesName == "Clothes")
        {
            inClothesRange = true;
            clothesInRange.Add(other.gameObject.name);
            //If more than clothing in range
            if (clothesInRange.Count == 2)
            {
                var distanceA = GameObject.Find(clothesInRange[0]).transform.position;
                var distanceB = GameObject.Find(clothesInRange[1]).transform.position;
                float closestA = Vector3.Distance(distanceA, selfGameObject.transform.position);
                float closestB = Vector3.Distance(distanceB, selfGameObject.transform.position);
                if (closestB >= closestA)
                {
                    clothesInRange.RemoveAt(1);
                }
                else
                {
                    clothesInRange.RemoveAt(0);
                }
            }

            else if (clothesInRange.Count == 3)
            {
                clothesInRange.RemoveAt(0);
            }

            clothesObject = GameObject.Find(clothesInRange[0]);

        }

        //If at empty clothing space
        else if (clothesName == "Place")
        {
            inPlacementRange = true;
            placementInRange.Add(other.gameObject.name);
            if (placementInRange.Count == 2)
            {
                var distanceA = GameObject.Find(placementInRange[0]).transform.position;
                var distanceB = GameObject.Find(placementInRange[1]).transform.position;
                float closestA = Vector3.Distance(distanceA, selfGameObject.transform.position);
                float closestB = Vector3.Distance(distanceB, selfGameObject.transform.position);
                if (closestB >= closestA)
                {
                    placementInRange.RemoveAt(1);
                }
                else
                {
                    placementInRange.RemoveAt(0);
                }
            }

            else if (placementInRange.Count == 3)
            {
                placementInRange.RemoveAt(0);
            }
            Debug.Log("Placement In Range");
            placementInRange.ForEach(Debug.Log);
            placementObject = GameObject.Find(placementInRange[0]);
        }

        //If at Folding Station
        else if (stationName == "FoldingStation")
        {
            inFoldingStationRange = true;
            foldingStationsInRange.Add(other.gameObject.name);
            if (foldingStationsInRange.Count == 2)
            {
                var distanceA = GameObject.Find(foldingStationsInRange[0]).transform.position;
                var distanceB = GameObject.Find(foldingStationsInRange[1]).transform.position;
                float closestA = Vector3.Distance(distanceA, selfGameObject.transform.position);
                float closestB = Vector3.Distance(distanceB, selfGameObject.transform.position);
                if (closestB >= closestA)
                {
                    foldingStationsInRange.RemoveAt(1);
                }
                else
                {
                    foldingStationsInRange.RemoveAt(0);
                }
            }

            else if (foldingStationsInRange.Count == 3)
            {
                foldingStationsInRange.RemoveAt(0);
            }
            Debug.Log("Station In Range");
            foldingStationsInRange.ForEach(Debug.Log);
            foldingStationObject = GameObject.Find(foldingStationsInRange[0]);
        }


    }

    void OnTriggerExit(Collider other)
    {
        var clothesName = other.gameObject.name.Remove(other.gameObject.name.Length - 4);
        var stationName = other.gameObject.name.Remove(other.gameObject.name.Length - 1);

        if (other.gameObject.name == "Register" || other.gameObject.name == "Register2")
        {
            inRegisterRange = false;
        }

        else if (clothesName == "Clothes")
        {
            inClothesRange = false;
            clothesInRange.Remove(other.gameObject.name);
        }

        else if (clothesName == "Place")
        {
            inPlacementRange = false;
            placementInRange.Remove(other.gameObject.name);
        }

        else if (stationName == "FoldingStation")
        {
            inFoldingStationRange = false;
            foldingStationsInRange.Remove(other.gameObject.name);
            other.gameObject.GetComponent<FoldingStationHandler>().receivableInfo = false;
        }
    }
    void ChangeParent(Collider parent, Collider child)
    {
        tempTrans = child.transform.parent;
        child.transform.parent = parent.transform;
    }

    //Revert the parent of object 2.
    void RevertParent(Collider child)
    {
        child.transform.parent = tempTrans;

    }
}

