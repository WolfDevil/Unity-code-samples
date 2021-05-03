using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ColorSchemeManager : MonoBehaviour
{
    [Inject] private Settings _settings;
    public ColorScheme CurrentColorSceme { get; private set; }

    private Color? _firstBlockColor = null;
    private Color? _secondBlockColor = null;
    private int _lastHeightStepValue = 0;

    public void RandomizeColorSceme()
    {
        CurrentColorSceme = _settings.ColorSchemes.FindAll(scheme => !scheme.Equals(CurrentColorSceme)).RandomElement();
        _firstBlockColor = null;
        _secondBlockColor = null;
        _lastHeightStepValue = 0;
}

    public Color[] GetBGColors()
    {
        if (CurrentColorSceme.BackgroundGradientColors.Count == 2)
        {
            var topColor = CurrentColorSceme.BackgroundGradientColors[0];
            var bottomColor = CurrentColorSceme.BackgroundGradientColors[1];
            return new[] { topColor, bottomColor };
        }
        return null;
    }

    public Material GetCubeMaterial(float y)
    {
        var material = Instantiate(_settings.CubeMaterial);
        var colorsCount = CurrentColorSceme.CubeGradientColors.Count;

        int _heightStep = (int)(y / _settings.GradientStep);
        if (_firstBlockColor == null || _secondBlockColor == null)
        {
            _firstBlockColor = CurrentColorSceme.CubeGradientColors.RandomElement();
            _secondBlockColor = CurrentColorSceme.CubeGradientColors.FindAll(color => !color.Equals(_firstBlockColor)).RandomElement();
        }
        else if (_lastHeightStepValue < _heightStep)
        {
            _firstBlockColor = _secondBlockColor;
            _secondBlockColor = CurrentColorSceme.CubeGradientColors.FindAll(color => !color.Equals(_firstBlockColor)).RandomElement();
        }
        _lastHeightStepValue = _heightStep;

        var gradient = new Gradient();
        var colorKeys = new GradientColorKey[2];
        var alphaKeys = new GradientAlphaKey[2];
        colorKeys[0].color = (Color)_firstBlockColor;
        colorKeys[0].time = 0.0f;
        colorKeys[1].color = (Color)_secondBlockColor;
        colorKeys[1].time = 1.0f;
        gradient.SetKeys(colorKeys, alphaKeys);
        var timeByBlockHeight = Mathf.Repeat(y / _settings.GradientStep, 1.0f);
        material.color = gradient.Evaluate(timeByBlockHeight);
        return material;
    }

    [Serializable]
    public class ColorScheme
    {
        public List<Color> CubeGradientColors;
        public List<Color> BackgroundGradientColors;
    }

    [Serializable]
    public class Settings
    {
        public Material CubeMaterial;
        public float GradientStep;
        public List<ColorScheme> ColorSchemes;
    }
}
