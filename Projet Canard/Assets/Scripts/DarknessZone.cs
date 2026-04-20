using UnityEngine;

public class DarknessZone : MonoBehaviour
{
    [Header("Suivi joueur")]
    public Transform playerHead; // Assigner la MainCamera VR

    [Header("Paramètres d'obscurité")]
    [Range(0.5f, 5f)]  public float innerRadius   = 1.5f; // Zone visible autour du joueur
    [Range(1f,  10f)]  public float outerRadius   = 4.0f; // Rayon total de la sphère
    [Range(0f,  1f)]   public float noiseStrength = 0.15f;

    [Header("Réactivité à la lampe")]
    public Light flashlight;
    [Range(0f, 3f)] public float flashlightBonus  = 1.2f; // Bonus de rayon dans la direction lampe

    private Material _mat;
    private static readonly int InnerRadiusProp = Shader.PropertyToID("_InnerRadius");
    private static readonly int OuterRadiusProp = Shader.PropertyToID("_OuterRadius");

    void Start()
    {
        _mat = GetComponent<Renderer>().material;
    }

    void LateUpdate()
    {
        // Coller la sphère au joueur
        transform.position = playerHead.position;

        // Passer les valeurs au shader
        _mat.SetFloat(InnerRadiusProp, innerRadius);
        _mat.SetFloat(OuterRadiusProp, outerRadius);
    }
}