using UnityEngine;
using System.Collections;

public class AnimatedTexture : MonoBehaviour
{
    public float fps = 30.0f;
    public Sprite[] frames;

    private int frameIndex;
    private SpriteRenderer rendererMy;

    void Start()
    {
        rendererMy = GetComponent<SpriteRenderer>();
        NextFrame();
        InvokeRepeating("NextFrame", 1 / fps, 1 / fps);
    }

    void NextFrame()
    {
        rendererMy.sprite = frames[frameIndex];
        frameIndex = (frameIndex + 1) % frames.Length;
    }
}