using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupScene : MonoBehaviour
{

    public List<GameObject> Tables = new List<GameObject>();
    public GameObject prefab; 
    private GameObject currentCreation;
    private Transform currentPlacement;
    public List<Material> colourList = new List<Material>();

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < Tables.Count; i++)
        {
            for(int j = 0; j < Tables[i].transform.childCount; j++)
            {
                currentPlacement = Tables[i].transform.GetChild(j).transform;
                currentCreation = GameObject.Instantiate(prefab, currentPlacement.position, Quaternion.identity);
                currentCreation.name = "Clothes" + Tables[i].name;
                currentCreation.transform.GetChild(0).GetComponent<MeshRenderer>().material = colourList[i];
                currentCreation.transform.rotation = Quaternion.Euler(currentCreation.transform.rotation.x, currentPlacement.rotation.eulerAngles.y, currentCreation.transform.rotation.z);
                currentCreation.transform.parent = currentPlacement.transform;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
