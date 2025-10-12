using UnityEngine;

public class MaterialSwitcher : MonoBehaviour
{
    public Material[] materials; // 0 = rose, 1 = jaune, 2 = bleu
    private Renderer rend;

    public KeyCode pinkKey = KeyCode.A;    // touche pour rose
    public KeyCode yellowKey = KeyCode.W;  // touche pour jaune
    public KeyCode blueKey = KeyCode.D;    // touche pour bleu
    void Start()
    {
        rend = GetComponent<Renderer>();

        // Mettre le premier mat�riel par d�faut si disponible
        if (materials.Length > 0)
        {
            rend.material = materials[0];
        }
    }

    void Update()
    {
        if (materials.Length < 3) return; // s�curit�

        // Changer le mat�riau selon la touche appuy�e
        if (Input.GetKeyDown(pinkKey))
        {
            rend.material = materials[0]; // rose
        }
        else if (Input.GetKeyDown(yellowKey))
        {
            rend.material = materials[1]; // jaune
        }
        else if (Input.GetKeyDown(blueKey))
        {
            rend.material = materials[2]; // bleu
        }
    }
}
