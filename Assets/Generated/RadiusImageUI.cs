using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class RadiusImageUI : MonoBehaviour
{
    private static readonly int Radius = Shader.PropertyToID("_Radius");

    [Tooltip("The radius value to apply to the image (0 to 1).")] 
    [SerializeField, Range(0f, 1f)]
    private float radius = 0.5f;

    private Image targetImage;

    private void OnValidate()
    {
        targetImage = GetComponent<Image>();
        if (targetImage != null) UpdateImageRadius();
    }

    private void UpdateImageRadius()
    {
        if (targetImage.material == null) targetImage.material = new Material(Shader.Find("UI/RoundedCorners"));

        var mat = targetImage.material;
        if (mat != null) mat.SetFloat(Radius, radius);
    }
}