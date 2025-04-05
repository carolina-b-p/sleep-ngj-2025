using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //find all gameobjects with tag "Building" and check for gameobjects that are closer to each other than one unit
        //for each set of close gameobjects delete a random of the two
        GameObject[] buildings = GameObject.FindGameObjectsWithTag("Building"); // Find all gameobjects with tag "Building"
        HashSet<int> alreadyDestroyed = new HashSet<int>(); // Create a HashSet to keep track of already destroyed buildings
        for (int i = 0; i < buildings.Length; i++) // Loop through all the buildings
        {
            if (alreadyDestroyed.Contains(i)) // Check if the building has already been destroyed
                continue; // Skip to the next iteration if it has been destroyed

            for (int j = i + 1; j < buildings.Length; j++) // Loop through all the buildings again starting from the next one
            {
                if (Vector3.Distance(buildings[i].transform.position, buildings[j].transform.position) < 5f) // Check if the distance between the two buildings is less than 1 unit
                {
                    // Delete one of the two buildings randomly
                    if (Random.Range(0, 2) == 0) // Randomly choose one of the two buildings to delete
                    {
                        Destroy(buildings[i]); // Delete the first building
                        break;
                    }
                    else
                    {
                        Destroy(buildings[j]); // Delete the second building
                        alreadyDestroyed.Add(j); // Add the index of the destroyed building to the HashSet to avoid deleting it again or deleting both buildings
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
