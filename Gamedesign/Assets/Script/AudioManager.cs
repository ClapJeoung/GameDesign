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
  [SerializeField] private AudioClip[] WalkingClips = null;

  private AudioSource BackgroundSource_0 = null;
  private AudioSource BackgroundSource_1 = null;
  private AudioSource[] EffectSources = null;
  private AudioSource FireSource = null;
  private AudioSource SpecialSource = null;
  private int CurrentSource = 0;

  private void Awake()
  {
    instance = this;
    AudioSource[] _temp = GetComponents<AudioSource>();
    EffectSources=new AudioSource[_temp.Length-4];
    for(int i = 0; i < _temp.Length; i++)
    {
      if (i == 0) BackgroundSource_0 = _temp[i];
      else if (i == 1) BackgroundSource_1 = _temp[i];
      else if (i == 2) FireSource = _temp[i];
      else if (i == 3) SpecialSource = _temp[i];
      else EffectSources[i - 4] = _temp[i];
    }
    BackgroundSource_0.clip = Background;
    BackgroundSource_0.volume = BackgroundVolume;
    BackgroundSource_0.Play();
  }
  public void PlayClip(int i) //통용되는 소리 재생기
  {
    EffectSources[CurrentSource].clip = MyClipis[i];
    EffectSources[CurrentSource].Play();
    CurrentSource++;
    if (CurrentSource == EffectSources.Length) CurrentSource = 0;
  }
  public void PlayClip(AudioClip clip) //통용되는 소리 재생기
  {
    EffectSources[CurrentSource].clip = clip; 
    EffectSources[CurrentSource].Play();
    CurrentSource++;
    if (CurrentSource == EffectSources.Length) CurrentSource = 0;
  }
  private List<int> BubbleList=new List<int>();
  /// <summary>
  /// 물 떨어지는 소리
  /// </summary>
  /// <param name="i"></param>
  /// <param name="id"></param>
  public void PlayClip(int i, int id)
  {
    if (BubbleList.Contains(id)) return;
    BubbleList.Add(id);
    StartCoroutine(erasebubble(MyClipis[i].length, id));

    EffectSources[CurrentSource].clip = MyClipis[i];
    EffectSources[CurrentSource].Play();
    CurrentSource++;
    if (CurrentSource == EffectSources.Length) CurrentSource = 0;
  }
  public void PlayWalk()  //걷기 소리 하나하나 랜덤으로 출력
  {
    PlayClip(WalkingClips[Random.Range(0, WalkingClips.Length)]);
  }
  public void PlayFire()
  {
    FireSource.clip = MyClipis[8];
    FireSource.Play();
  }
  public void StopFire(bool isdestroy)
  {
    FireSource.Stop();
    if (isdestroy)
    {
      FireSource.clip = MyClipis[9];
      FireSource.Play();
    }
  }
  private bool specialisdone = false;
  public void PlaySpecial(int clip)
  {
    if (SpecialSource.isPlaying) return;
    specialisdone = false;
    SpecialSource.volume = 1.0f;
    SpecialSource.clip = MyClipis[clip];
    SpecialSource.Play();
  }
  public void StopSpecial()
  {
    if (specialisdone) return;
    specialisdone = true;
    StartCoroutine(turnoffspecial());
  }
  private IEnumerator turnoffspecial()
  {
    float _time = 0.0f,_targettime = 0.5f;
    while (_time < _targettime)
    {
      SpecialSource.volume =1.0f - _time / _targettime;
      _time += Time.deltaTime;
      yield return null;
    }
  }
  private IEnumerator erasebubble(float length,int target)
  {
    yield return new WaitForSeconds(length);
    BubbleList.Remove(target);
  }
}
