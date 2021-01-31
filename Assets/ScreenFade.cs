using System;
using UnityEngine;
 
public class ScreenFade : MonoBehaviour
{
    [SerializeField] private AnimationCurve FadeCurve;
    private static ScreenFade _instance;
    private static float _alpha = 1;
    private static bool _fadingOut;
    private static float _fadeDuration = 2f;
    private static Texture2D _texture;

    private void Start()
    {
        FadeIn();
    }

    private void Awake()
    {
        _texture = new Texture2D(1,1);
        _texture.SetPixel(0, 0, Color.black);
        _texture.Apply();
        _instance = this;
    }

    public static void FadeIn()
    {
        _fadingOut = false;
    }

    public static void FadeOut()
    {
        _fadingOut = true;
    }

    private void Update()
    {
        if (!_fadingOut)
            _alpha = Mathf.Clamp01(_alpha - Time.deltaTime / _fadeDuration);
        else
            _alpha = Mathf.Clamp01(_alpha + Time.deltaTime / _fadeDuration);
    }

    private void OnGUI()
    {
        GUI.color = new Color(1, 1, 1, FadeCurve.Evaluate(_alpha));
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), _texture);
        GUI.color = Color.white;
    }
}