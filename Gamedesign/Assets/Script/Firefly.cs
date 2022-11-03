using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Firefly : RespawnObj,Lightobj
{
  [SerializeField] private ParticleSystem MyParticle = null;
  private Color IdleColor= Color.white;
  [SerializeField] private Color ActiveColor = Color.white;
  [SerializeField] private Light2D MyLight = null;
  [SerializeField] private float TargetSize = 3.0f;
  private float MoveAmount = 0.8f;
  private float KeepMoveTime = 2.0f;
  private float LightAmount =1.2f;
  private float Light_aver = 1.4f;
  private float LightTime_min = 1.2f;
  private float LightTime_max = 2.0f;

  public void Setup()
  {
    ParticleSystem.MainModule _main = MyParticle.main;
    if (IsActive)
    {
      MyLight.pointLightOuterRadius = TargetSize;
      _main.startColor =  ActiveColor;
      StartCoroutine(keepmove());
      StartCoroutine(keeplight());
    }
    else
    {
      MyLight.pointLightOuterRadius = 0.0f;
    }
    GameManager.Instance.AllLights.Add(this);
  }
  public void TurnOn()
  {
    MyLight.enabled = true;
  }
  public void TurnOff()
  {
    MyLight.enabled = false;
  //  StartCoroutine(turnoff());
  }
  private IEnumerator turnoff()
  {
    float _time = 0.0f;
    float _origin = MyLight.intensity;
    if (_origin != 0.0f)
      while (_time < 1.0f)
      {
        MyLight.intensity = _origin * (_time / 1.0f);
        _time += Time.deltaTime;
        yield return null;
      }
    MyLight.enabled = false;
  }

  private void Start()
  {
    Setup();
  }
  private void OnTriggerEnter2D(Collider2D collision)
  {
    if (collision.CompareTag("Player") && !IsActive)
    {
      StartCoroutine(turnon());
      StartCoroutine(keepmove());
      StartCoroutine(keeplight());
    }
  }
  private IEnumerator turnon()
  {
    IsActive = true;
    GameManager.Instance.SetNewRespawn(this);
    ParticleSystem.MainModule _main = MyParticle.main;
    float _time = 0.0f;
    while (_time < 1.0f)
    {
      MyLight.pointLightOuterRadius = (_time / 1.0f) * TargetSize;
      _main.startColor=Color.Lerp(IdleColor, ActiveColor, (_time / 1.0f));
      _time += Time.deltaTime;
      yield return null;
    }
  }
  private IEnumerator keepmove()
  {
    Transform _transform = MyLight.transform;
    Vector2 _originpos = Vector2.zero;
    Vector2 _offset=_originpos+new Vector2(Random.Range(-MoveAmount, MoveAmount),Random.Range(-MoveAmount, MoveAmount));
    Vector2 _lastpos = Vector2.zero;
    int _signx = 1;
    int _signy = -1;
    float _time = 0.0f;
    int _quadrant = Random.Range(0, 4);
    while (true)
    {
      _time = 0.0f;
      _lastpos = _transform.localPosition;
      switch (_quadrant)
      {
        case 0: _signx = 1;_signy = 1; break;
        case 1: _signx = -1;_signy = 1;break;
        case 2: _signx = -1;_signy = -1;break;
        case 3: _signx = 1;_signy = -1;break;
      }
      _offset=new Vector2(Random.Range(0.0f,MoveAmount)*_signx,Random.Range(0.0f,MoveAmount)*_signy);
      while (_time < KeepMoveTime)
      {
        _transform.localPosition = Vector2.Lerp(_lastpos, _originpos + _offset, Mathf.Pow(_time / KeepMoveTime, 1.5f));
        _time += Time.deltaTime;
        yield return null;
      }
      _quadrant += Random.Range(1, 3);
      if (_quadrant < 3) _quadrant -= 4;
      yield return null;
    }
  }
  private IEnumerator keeplight()
  {
    float _time = 0.0f;
    float _lighttime = 0;
    float _lastlight = 0;
    float _targetlight = 0;
    bool _isup = true;
    while (true)
    {
      _lighttime = Random.Range(LightTime_min, LightTime_max);
      _lastlight = MyLight.intensity;
      _targetlight = Light_aver + (_isup ? Random.Range(0.0f, LightAmount) : Random.Range(-LightAmount, 0.0f));
      _time = 0.0f;
      while(_time< _lighttime)
      {
        MyLight.intensity=Mathf.Lerp(_lastlight,_targetlight,Mathf.Pow((_time/_lighttime),2.0f));
        _time += Time.deltaTime;
        yield return null;
      }
      _isup = !_isup;
      yield return null;
    }
  }
}
