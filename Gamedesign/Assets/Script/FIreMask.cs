using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FIreMask : MonoBehaviour
{
  private Transform MyTransform = null;
  [SerializeField] private float EffectTime = 0.1f;
  [SerializeField] private float ClosePos = -16.0f;
  [SerializeField] private float OpenPos = -3.0f;
  [SerializeField] private ParticleSystem Particle_world = null;
  [SerializeField] private ParticleSystem Particle_soul = null;
  [SerializeField] private Animator ChangeEffectAnimator = null;

  private void Start()
  {
    Setup();
  }
  public void Setup()
  {
    MyTransform= transform;
  }
  public void Open(Vector2 newpos)
  {
    ChangeEffectAnimator.SetTrigger("ToSoul");
   // MyTransform.position = newpos;
    StartCoroutine(changepos(OpenPos,false));
  }
  public void Close(Vector2 newpos)
  {
    ChangeEffectAnimator.SetTrigger("ToWorld");
  //  MyTransform.position = newpos;
    StartCoroutine(changepos(ClosePos,true));
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
