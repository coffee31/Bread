using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource soundEffectSource;
    public AudioClip SoundClip; // Inspector���� ȿ���� ������ �Ҵ��մϴ�.

    public void PlaySoundEffect()
    {
        soundEffectSource.PlayOneShot(SoundClip);
    }
}
