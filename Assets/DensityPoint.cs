using TMPro;
using UnityEngine;

public class DensityPoint : MonoBehaviour
{
    public Vector3Int pointPosition;
    public float densityValue;

    public MeshRenderer sphereMeshRenderer;
    public TMP_Text text;

    public void SetDensity(Vector3Int position, float density)
    {
        pointPosition = position;
        densityValue = density;

        sphereMeshRenderer.material.color = Color.Lerp(Color.black, Color.white, Mathf.InverseLerp(-8f, 8f, density));
        text.text = density.ToString("F2");
    }
}
