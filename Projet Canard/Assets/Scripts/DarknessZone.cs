using UnityEngine;

public class DarknessZone : MonoBehaviour
{
    [Header("Suivi joueur")]
    public Transform playerHead;

    [Header("Paramètres d'obscurité")]
    [Range(0.5f, 5f)]  public float innerRadius   = 1.5f;
    [Range(1f,  10f)]  public float outerRadius   = 4.0f;
    [Range(0f,  1f)]   public float noiseStrength = 0.15f;

    [Header("Déformation directionnelle")]
    [Range(0f, 5f)]  public float lookBonus     = 2.0f; // Combien de mètres en plus devant
    [Range(1f, 16f)] public float lookSharpness = 4.0f; // Largeur du cône (haut = étroit)

    [Header("Réactivité à la lampe")]
    public Light flashlight;
    [Range(0f, 3f)] public float flashlightBonus = 1.2f;

    private Material _mat;
    private static readonly int InnerRadiusProp   = Shader.PropertyToID("_InnerRadius");
    private static readonly int OuterRadiusProp   = Shader.PropertyToID("_OuterRadius");
    private static readonly int LookDirectionProp = Shader.PropertyToID("_LookDirection");
    private static readonly int LookBonusProp     = Shader.PropertyToID("_LookBonus");
    private static readonly int LookSharpnessProp = Shader.PropertyToID("_LookSharpness");
    [Header("Dégradé progressif")]
    [Range(0.3f, 5f)] public float fogCurve = 2.0f;

    private static readonly int FogCurveProp = Shader.PropertyToID("_FogCurve");



    void Start()
    {
        _mat = GetComponent<Renderer>().material;
    }

    void LateUpdate()
    {
        transform.position = playerHead.position;

        _mat.SetFloat(InnerRadiusProp, innerRadius);
        _mat.SetFloat(OuterRadiusProp, outerRadius);

        // ✅ On envoie la direction du regard au shader
        _mat.SetVector(LookDirectionProp, playerHead.forward);
        _mat.SetFloat(LookBonusProp,     lookBonus);
        _mat.SetFloat(LookSharpnessProp, lookSharpness);

        _mat.SetFloat(FogCurveProp, fogCurve);
    }
}