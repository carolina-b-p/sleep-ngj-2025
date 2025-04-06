using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseTest : MonoBehaviour
{

    public GameObject houseFloor; // Reference to the house floor prefab
    public GameObject houseMiddle; // Reference to the house middle prefab

    public Vector3 flooroffset = new Vector3(0, 0, 0); // Offset for the house floor prefab multiplied by which floor it is
    public int minimumHouseHeight = 1; // Minimum height of the house
    public int maximumHouseHeight = 20; // Maximum height of the house

    public List<Material> materials = new List<Material>(); // List of materials to be applied to the house
    // Start is called before the first frame update
    void Start()
    {
        //move the house randomly in a direction
        // Vector3 randomDirection = Random.insideUnitSphere * .5f; // Random direction within a sphere of radius 10
        // randomDirection.y = -5; // Keep the y value zero to avoid moving up or down
        // transform.position += randomDirection; // Move the house in the random direction

        //scale the house randomly in y
        // Vector3 randomScale = new Vector3( Random.Range(8f, 8.33f), Random.Range(1f, 5f)*10, Random.Range(8f, 8.33f)); // Random scale in y direction
        // transform.localScale = randomScale; // Apply the random scale to the house

        //add some slight rotation around the y axis
        float randomRotation = Random.Range(0, 4) * 90; // Random rotation around the y axis
        transform.Rotate(0, randomRotation, 0); // Apply the random rotation to the house

        //set material to a random one from the list of materials
        // int randomIndex = Random.Range(0, materials.Count); // Get a random index from the list of materials    
        // Material randomMaterial = materials[randomIndex]; // Get the random material from the list
        // // Get all the renderers in the house and set their material to the random material
        // Renderer[] renderers = GetComponentsInChildren<Renderer>(); // Get all the renderers in the house
        // foreach (Renderer renderer in renderers) // Loop through all the renderers
        // {
        //     // Set the material of the renderer to the random material
        //     Material[] newMaterials = new Material[renderer.materials.Length]; // Create a new array of materials with the same length as the original materials
        //     for (int i = 0; i < renderer.materials.Length; i++) // Loop through all the materials in the renderer
        //     {
        //         newMaterials[i] = randomMaterial; // Set the new material to the random material
        //     }
        //     renderer.materials = newMaterials; // Apply the new materials to the renderer
        // }


        //instansiate the house floor and middle prefabs
        GameObject floor = Instantiate(houseFloor, transform.position, Quaternion.identity, transform); // Instantiate the house floor prefab at the position of the house with the specified offset

        for (int i = 0; i < Random.Range(minimumHouseHeight, maximumHouseHeight); i++) // Loop through the height of the house
        {
            GameObject middle = Instantiate(houseMiddle, transform.position + flooroffset * (i + 1), Quaternion.identity,transform); // Instantiate the house middle prefab at the position of the house with the specified offset
            
            randomRotation = Random.Range(0, 4) * 90; // Random rotation around the y axis
            middle.transform.Rotate(0, randomRotation, 0); // Apply the random rotation to the house
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
