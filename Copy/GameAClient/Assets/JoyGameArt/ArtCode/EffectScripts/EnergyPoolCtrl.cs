using UnityEngine;

public class EnergyPoolCtrl : MonoBehaviour
{
    public Renderer LiquidRenderer;
    public Transform Weapon;
    private float _liquidVolume = 0.5f;
    private Material _liquidMat;

    public float LiquidVolume
    {
        get { return _liquidVolume; }
        set
        {
            if (value != _liquidVolume)
            {
                _liquidVolume = Mathf.Clamp01(value);
                _liquidMat.SetFloat("_FillAmount", _liquidVolume);
            }
        }
    }

    void Awake()
    {
        _liquidMat = LiquidRenderer.material;
    }
}