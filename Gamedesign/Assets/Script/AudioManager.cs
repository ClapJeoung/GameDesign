using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;
  public static AudioManager Instance { get { return instance; } }
  [SerializeField] private float EffectVolume = 0.7f;
  [SerializeField] private float BackgroundVolume = 1.0f;
  [Space(5)]
  [SerializeField] private AudioClip Background = null;
  [SerializeField] private AudioClip[] MyClipis = null;

  private AudioSource BackgroundSource_0 = null;
  private AudioSource BackgroundSource_1 = null;
  private AudioSource[] EffectSources = null;

  private void Awake()
  {
    instance = this;
    AudioSource[] _temp = GetComponents<AudioSource>();
    EffectSources=new AudioSource[_temp.Length];
    for(int i = 0; i < _temp.Length; i++)
    {
      if (i == 0) BackgroundSource_0 = _temp[i];
      else if (i == 1) BackgroundSource_1 = _temp[i];
      else EffectSources[i - 2] = _temp[i];
    }
//    BackgroundSource_0.clip = Background;
    BackgroundSource_0.volume = BackgroundVolume;
    BackgroundSource_0.Play();
  }
  public void PlayClip(int i) //통용되는 소리 재생기
  {
    foreach(AudioSource clip in EffectSources)
    {
      if (clip.isPlaying) continue;
      else { clip.clip = MyClipis[i];clip.Play(); break; }
    }
  }
  private List<int> BubbleList=new List<int>();
  public void PlayClip(int i, int id)
  {
    if (BubbleList.Contains(id)) return;
    BubbleList.Add(id);
    StartCoroutine(erasebubble(MyClipis[i].length, id));
    foreach (AudioSource clip in EffectSources)
    {
      if (clip.isPlaying) continue;
      else { clip.clip = MyClipis[i]; clip.Play(); break; }
    }
  }
  private IEnumerator erasebubble(float length,int target)
  {
    yield return new WaitForSeconds(length);
    BubbleList.Remove(target);
  }
}
