using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionBox : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.name.Substring(0, 6) == "Clothe")
        {
            Debug.Log("DONE");
        }

    }

    private void OnTriggerExit(Collider other)
    {

    }
}
