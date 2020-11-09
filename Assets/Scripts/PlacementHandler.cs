using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementHandler : MonoBehaviour
{
    public List<string> objectStacked = new List<string>();
    public bool receivableInfo = false;
    // Start is called before the first frame update
    void Start()
    {
        objectStacked.Add(gameObject.name);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
