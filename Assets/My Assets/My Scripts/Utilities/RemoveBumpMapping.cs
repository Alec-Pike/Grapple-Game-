// Script written by Grok

using UnityEngine;

public class DisableNormalMap : MonoBehaviour
{
    void Start()
    {
        // Find all renderers in the scene
        Renderer[] renderers = Object.FindObjectsByType<Renderer>(FindObjectsSortMode.None);
        foreach (Renderer rend in renderers)
        {
            foreach (Material mat in rend.sharedMaterials)
            {
                if (mat.shader.name.Contains("HDRP/Lit"))
                {
                    mat.SetTexture("_NormalMap", null); // Clear normal map
                    Debug.Log($"Normal map disabled for material: {mat.name}");
                }
            }
        }
    }
}