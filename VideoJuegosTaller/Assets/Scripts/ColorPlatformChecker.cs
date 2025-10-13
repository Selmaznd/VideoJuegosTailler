using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorPlatformChecker : MonoBehaviour
{

    private Renderer rend;
    private RespawnOnTrigger respawnScript;

    public Material pinkMaterial;
    public Material yellowMaterial;
    public Material blueMaterial;

    void Start()
    {
        rend = GetComponent<Renderer>();
        respawnScript = GetComponent<RespawnOnTrigger>();
    }

    void OnTriggerEnter(Collider other)
    {
        string currentMatName = rend.material.name;

        switch (other.tag)
        {
            case "PlatformPink":
                if (!currentMatName.Contains(pinkMaterial.name))
                    respawnScript?.SendMessage("Respawn");
                break;

            case "PlatformYellow":
                if (!currentMatName.Contains(yellowMaterial.name))
                    respawnScript?.SendMessage("Respawn");
                break;

            case "PlatformBlue":
                if (!currentMatName.Contains(blueMaterial.name))
                    respawnScript?.SendMessage("Respawn");
                break;
        }
    }

}
