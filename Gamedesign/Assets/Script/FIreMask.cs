using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FIreMask : MonoBehaviour
{
  private Transform MyTransform = null;
  [SerializeField] private float EffectTime = 0.8f;
  [SerializeField] private float ClosePos = -16.0f;
  [SerializeField] private float OpenPos = -3.0f;
  [SerializeField] private ParticleSystem Particle_world = null;
  [SerializeField] private ParticleSystem Particle_soul = null;
  [SerializeField] private Animator ChangeEffectAnimator = null;
  private SpriteMask MyMask = null;
  [SerializeField] private Light2D PlayerLight = null;
  [SerializeField] private Transform FireTrans = null;
  [SerializeField] private ParticleSystem Particle_Change = null; //ÆÄÆ¼Å¬
  private void Start()
  {
    Setup();
    MyMask = GetComponent<SpriteMask>();
  }
  public void Setup()
  {
    MyTransform= transform;
  }
  public void Open(Vector2 newpos)
  {
    StartCoroutine(open(newpos));
  }
  private IEnumerator open(Vector2 newpos)
  {
    AudioManager.Instance.PlayClip(0);
    float _targetradius = GetLength(newpos);
    MyTransform.position = newpos;
    Particle_Change.transform.position = (Vector3)newpos + Vector3.back * 8.0f;
    ParticleSystem.ShapeModule _shape = Particle_Change.shape;
    _shape.radius = 0.0f;
    MyTransform.localScale = Vector3.one*0.0f;
    Particle_Change.Play();
    float _time = 0.0f;
    while(_time< EffectTime)
    {
      MyTransform.localScale = Vector3.one * Mathf.Lerp(0.0f, _targetradius * 2.0f, Mathf.Pow((_time / EffectTime), 2.0f));
      _shape.radius = Mathf.Lerp(0.0f, _targetradius, Mathf.Pow((_time / EffectTime), 2.0f));
      _time += Time.deltaTime; yield return null;
    }
    Particle_Change.Stop();
  }
  private IEnumerator open_old(float _size)
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
  public void Close(Vector2 newpos)
  {
    StartCoroutine(close(newpos));
  }
  private IEnumerator close(Vector2 newpos)
  {
    float _targetradius = GetLength(newpos);
    AudioManager.Instance.PlayClip(0);
    MyTransform.position = newpos;
    Particle_Change.transform.position = (Vector3)newpos+Vector3.back*8.0f;
    ParticleSystem.ShapeModule _shape = Particle_Change.shape;
    _shape.radius = _targetradius;
    MyTransform.localScale = Vector3.one * _targetradius*2.0f;
    Particle_Change.Play();
    float _time = 0.0f;
    while (_time < EffectTime)
    {
      MyTransform.localScale = Vector3.one *Mathf.Lerp(_targetradius*2.0f, 0.0f, Mathf.Pow(_time / EffectTime,2.0f));
      _shape.radius =  Mathf.Lerp(_targetradius , 0.0f, Mathf.Pow((_time / EffectTime), 2.0f))/1.5f;
      _time += Time.deltaTime; yield return null;
    }
    MyTransform.localScale = Vector3.zero;
    Particle_Change.Stop();
  }
  private IEnumerator close_old(float _size)
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
  public float GetLength(Vector2 pos)
  {
    Vector2 _pos = pos - (Vector2)MyTransform.position;
    float _size=5.4f/Camera.main.orthographicSize, _width = 9.6f* _size + Mathf.Abs(_pos.x), _height = 5.4f * _size + Mathf.Abs(_pos.y);
//    Debug.Log($"width : {_width}  height : {_height}  length : { Vector2.Distance(Vector2.zero, new Vector2(_width, _height))}");
    return Vector2.Distance(Vector2.zero,new Vector2(_width, _height))+0.1f;
  }
}
