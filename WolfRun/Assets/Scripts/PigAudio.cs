using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigAudio : MonoBehaviour
{
    [SerializeField] private AudioClip strawSound;
    [SerializeField] private AudioClip woodSound;
    [SerializeField] private AudioClip brickSound;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void constructionSound(WallType wallType)
    {
        if (audioSource.isPlaying)
            audioSource.Stop();
        switch (wallType)
        {
            case WallType.STRAW:
                audioSource.PlayOneShot(strawSound);
                break;
            case WallType.WOOD:
                audioSource.PlayOneShot(woodSound);
                break;
            case WallType.BRICK:
                audioSource.PlayOneShot(brickSound);
                break;
        }
    }

    public void stopSound()
    {
        audioSource.Stop();
    }
}
