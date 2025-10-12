using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorPlatformChecker : MonoBehaviour
{
    private Material currentMaterial;
    private Renderer rend;
    private RespawnOnTrigger respawnScript; // ton script de respawn

    public Material pinkMaterial;
    public Material yellowMaterial;
    public Material blueMaterial;

    void Start()
    {
        rend = GetComponent<Renderer>();
        respawnScript = GetComponent<RespawnOnTrigger>();
        currentMaterial = rend.material;
    }

    void Update()
    {
        // Met à jour la couleur actuelle du joueur (au cas où elle change via MaterialSwitcher)
        currentMaterial = rend.material;
    }

    void OnCollisionEnter(Collision collision)
    {
        string tag = collision.gameObject.tag;

        // On vérifie la couleur du joueur et la plateforme touchée
        //if (tag == "PlatformBasic")
        //{

        //    return;
        //}
        if (tag == "PlatformPink" && currentMaterial != pinkMaterial)
        {
            respawnScript?.SendMessage("Respawn");
        }
        else if (tag == "PlatformYellow" && currentMaterial != yellowMaterial)
        {
            respawnScript?.SendMessage("Respawn");
        }
        else if (tag == "PlatformBlue" && currentMaterial != blueMaterial)
        {
            respawnScript?.SendMessage("Respawn");
        }

        //if (currentMaterial == pinkMaterial)
        //{
        //    if (tag != "PlatformPink" && tag != "PlatformBasic")
        //        respawnScript?.SendMessage("Respawn");
        //}
        //else if (currentMaterial == yellowMaterial)
        //{
        //    if (tag != "PlatformYellow" && tag != "PlatformBasic")
        //        respawnScript?.SendMessage("Respawn");
        //}
        //else if (currentMaterial == blueMaterial)
        //{
        //    if (tag != "PlatformBlue" && tag != "PlatformBasic")
        //        respawnScript?.SendMessage("Respawn");
        //}
    }
}
