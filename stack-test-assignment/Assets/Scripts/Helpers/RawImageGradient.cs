using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RawImageGradient : MonoBehaviour
{
    [SerializeField] private RawImage _img = null;

    private Texture2D _bgTexture;

    void Awake()
    {
        _img.color = Color.white;
        _bgTexture = new Texture2D(1, 2);
        _bgTexture.wrapMode = TextureWrapMode.Clamp;
        _bgTexture.filterMode = FilterMode.Bilinear;
        SetColor(Color.gray, Color.black);
    }

    public void SetColor(Color color1, Color color2)
    {
        _bgTexture.SetPixels(new Color[] { color1, color2 });
        _bgTexture.Apply();
        _img.texture = _bgTexture;
    }

    public void SetColor(Color[] color)
    {
        if (color.Length != 2 || color == null) return;
        _bgTexture.SetPixels(new Color[] { color[0], color[1] });
        _bgTexture.Apply();
        _img.texture = _bgTexture;
    }
}
