using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachScriptToObjects : MonoBehaviour
{
    public GameObject plane; // The plane GameObject

    void Start()
    {
        // Get all renderers in the scene
        Renderer[] renderers = FindObjectsOfType<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            // Check if the renderer, the sharedMaterial, or the shader are null
            if (renderer != null && renderer.sharedMaterial != null && renderer.sharedMaterial.shader != null)
            {
                // Check if the renderer's material uses the correct shader
                if (renderer.sharedMaterial.shader.name == "CrossSection/OnePlaneBSP")
                {
                    // Attach the script to the GameObject
                    OnePlaneCuttingController controller = renderer.gameObject.AddComponent<OnePlaneCuttingController>();

                    // Set the plane field of the OnePlaneCuttingController script
                    controller.plane = plane;
                }
            }
        }
    }
}