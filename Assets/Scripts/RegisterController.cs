using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegisterController : MonoBehaviour
{

    public List<Vector3> registerPositions = new List<Vector3>();               //List of positions for each spot in line   (Add Manually)
    public List<GameObject> customerLineUp = new List<GameObject>();            //List of customer game objects in line (Script)

    public List<Vector3> exitPoints = new List<Vector3>();             //Points declaring customer leaving scene

    private GameObject currentCustomer;
    //Add customer object to line up list
    public void AddToLineUp(GameObject customer)
    {
        customerLineUp.Add(customer);
        customer.GetComponent<CustomerAI>().destination = registerPositions[customerLineUp.Count - 1];
    }

    //Remove customer object from line up list
    public void LeaveLineUp()
    {
        currentCustomer = customerLineUp[0];
        customerLineUp.Remove(currentCustomer);
        currentCustomer.GetComponent<CustomerAI>().destination = exitPoints[Random.Range(0, exitPoints.Count)];
    }

    //Get next available position in line
    public Vector3 GetPositionInLine()
    {
        return registerPositions[customerLineUp.Count - 1];
    }

    //Shift entire line up one position
    public void shiftLineUp()
    {
        StartCoroutine(moveLine());
        
    }

    IEnumerator moveLine()
    {

        yield return new WaitForSeconds(1.1f);
        if (customerLineUp.Count > 0)
        {
            for (int i = 0; i < customerLineUp.Count; i++)
            {
                customerLineUp[i].GetComponent<CustomerAI>().destination = registerPositions[0];
            }
        }
    }

}
