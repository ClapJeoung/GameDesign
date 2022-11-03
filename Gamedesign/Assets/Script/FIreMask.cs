using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FIreMask : MonoBehaviour
{
  private Transform MyTransform = null;
  [SerializeField] private float EffectTime = 0.1f;
  [SerializeField] private float ClosePos = -16.0f;
  [SerializeField] private float OpenPos = -3.0f;
  [SerializeField] private ParticleSystem Particle_world = null;
  [SerializeField] private ParticleSystem Particle_soul = null;
  [SerializeField] private Animator ChangeEffectAnimator = null;
  private SpriteMask MyMask = null;
  [SerializeField] private Light2D PlayerLight = null;
  [SerializeField] private Transform FireTrans = null;
  private void Start()
  {
    Setup();
    MyMask = GetComponent<SpriteMask>();
  }
  public void Setup()
  {
    MyTransform= transform;
  }
  public void Open(float newsize)
  {
    StartCoroutine(open(newsize));
  }
  private IEnumerator open(float _size)
  {
    // Debug.Log(_size);
    AudioManager.Instance.PlayClip(0);
    float _originsize = FireTrans.localScale.x;
    FireTrans.localScale = Vector3.one*_size;
    ChangeEffectAnimator.SetTrigger("ToSoul");
    yield return new WaitForSeconds(0.5f);
    MyMask.enabled = true;
    yield return new WaitForSeconds(0.5f);
    FireTrans.localScale = Vector3.one * _originsize;
  }
  public void Close(float _size)
  {
    StartCoroutine(close(_size));
  }
  private IEnumerator close(float _size)
  {
    AudioManager.Instance.PlayClip(0);
    float _originsize = FireTrans.localScale.x;
    FireTrans.localScale = Vector3.one * _size;
    ChangeEffectAnimator.SetTrigger("ToWorld");
    yield return new WaitForSeconds(0.5f);
    MyMask.enabled = false;
    yield return new WaitForSeconds(0.5f);
    FireTrans.localScale = Vector3.one * _originsize;
  }
  private IEnumerator changepos(float targetpos,bool istoworld)
  {
    if (istoworld) Particle_world.Play();
    else Particle_soul.Play();
    float _time = 0.0f;
    Vector3 _originpos = Vector3.up*MyTransform.localPosition.y+Vector3.forward*9.0f;
    Vector3 _newpos = Vector3.up * targetpos + Vector3.forward * 9.0f;
  //  Debug.Log($"{_originsize} -> {targetsize}");
    while (_time < EffectTime)
    {
      MyTransform.localPosition = Vector3.Lerp(_originpos, _newpos, _time / EffectTime);
      _time += Time.deltaTime;
      yield return null;
    }
    if (istoworld) Particle_soul.Stop();
    else Particle_world.Stop();
  }
}
