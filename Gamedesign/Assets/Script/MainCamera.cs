using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using System;

public class MainCamera : MonoBehaviour
{
  private Transform MyTransform = null;     //카메라의 트랜스폼
  private Camera MyCamera = null; 
  [SerializeField] private float CameraSpeed = 1.0f;  //카메라 속도
  [SerializeField] private float MinY = 2.0f; //최소 바닥값
  [SerializeField] private float MaxY = 10.0f;//최대 바닥값
  [SerializeField] private float RespawnMovetime = 1.5f;
  private Vector3 Offset = Vector3.zero;    //1프레임 후 이동할 크기
  private Vector3 LastPos = Vector3.zero;   //1프레임 후 이동 전 위치
  private Transform TargetTransform = null; //카메라 이동 목표 트랜스폼
  private Vector3 TargetOffset = Vector3.zero;  //카메라 이동 목표 오프셋
  private Action OffsetDel = null;              //Offset 변동치 대리자
  private bool IsDead = false;
  [SerializeField] private ParticleSystem RP_start = null;  //리스폰 파티클
  private ParticleSystem.EmissionModule RP_start_emission;
  [SerializeField] private int MinParticle = 50;
  [SerializeField] private int MaxParticle = 180;
  [SerializeField] private ParticleSystem RP_end = null;    //리스폰 끝내는 파티클(안씀)
  [SerializeField] private ParticleSystem Particle_world = null;//일반계 환경 파티클(안씀)
  private ParticleSystem.ShapeModule particle_world_shape;
  [SerializeField] private ParticleSystem Particle_soul = null; //영혼계 환경 파티클
  private ParticleSystem.ShapeModule particle_soul_shape;
  [Space(5)]
  [SerializeField] private Transform FloodTrans = null; //카메라 잠길 홍수 트랜스폼
  [SerializeField] private float Flood_originpos = -12.0f;//홍수 시작 위치
  [SerializeField] private float Flood_targetpos = 0.5f;  //홍수 종료 위치
  [SerializeField] private float Flood_time = 5.0f;       //카메라가 다 잠기는 시간
  [SerializeField] private float Flood_shakedeg = 0.8f;   //카메라 진동 정도
  [SerializeField] private int Flood_shakecount = 8;      //초당 카메라 진동 횟수
  [SerializeField] private float Flood_angle = 15.0f;     //카메라 회전 크기
  [SerializeField] private float Flood_FadeOutTime = 7.0f;  //카메라 페이드아웃하는 시간
  [SerializeField] private ParticleSystem Flood_dustparticle = null;  //먼지 파티클
  [SerializeField] private ParticleSystem Flood_stoneparticle = null; //낙석 파티클
  private bool IsFlooded = false;                                     //침수 완료됐나요
  [SerializeField] private Light2D MyLight = null;
  private float OriginIntensity = 0.0f;
  private void Setup()
  {
    MyTransform = transform;
    IsDead = true;
    RP_start_emission = RP_start.emission;
    particle_world_shape = Particle_world.shape;
    particle_soul_shape = Particle_soul.shape;
    MyCamera = GetComponent<Camera>();
    OffsetDel = new Action(() => { });
  }
  private void Start()
  {
    Setup();
  }
  private void Update()
  {
    //  particle_world_shape.position = new Vector3(MyTransform.position.x, MyTransform.position.y + 5.5f, -1.0f);

    if (Input.anyKeyDown && IsFlooded) SceneManager.LoadScene(0); //겜 끝나고 아무거나 누르면 재시작

    particle_soul_shape.position = new Vector3(MyTransform.position.x, MyTransform.position.y, 8.0f); //영혼계 파티클 위치 계속 업데이트


    if (!IsDead) UpdateOffset(); //죽으면 안움직임
  }
  public void UpdateOffset()
  {
    LastPos = MyTransform.position;
    Offset = Vector3.zero;
    Vector3 _newpos = Vector3.zero;
    _newpos = Vector3.Lerp(MyTransform.position, TargetTransform.position + TargetOffset, Time.deltaTime * CameraSpeed);
    _newpos = new Vector3(_newpos.x, Mathf.Clamp(_newpos.y, MinY, MaxY), -10.0f);
    Offset = _newpos - LastPos;
    OffsetDel();
    MyTransform.position += Offset;
  }
  public void SetTarget(Transform target,Vector3 offset) //카메라 이동 목표 재설정
  {
    TargetTransform = target;
    TargetOffset = offset;
  }
  public void SetTarget(Vector3 offset)
  {
    TargetOffset=offset;
  }
  /// <summary>
  /// resetoffset 키면 TargetOffset 초기화
  /// </summary>
  /// <param name="target"></param>
  /// <param name="resetoffset"></param>
  public void SetTarget(Transform target,bool resetoffset)
  {
    TargetTransform = target;
    if (resetoffset) TargetOffset = Vector3.zero;
  }
  public float MoveToResapwn(Vector3 newpos,float waittime) //리스폰으로 이동
  {
    StartCoroutine(moveto(newpos,waittime));
    return RespawnMovetime + 0.1f;
  }
  public void StartRPParticle() { RP_start.Play(); AudioManager.Instance.PlayClip(1); } //리스폰 파티클 시작
  private IEnumerator moveto(Vector3 newpos,float waittime)
  {
    IsDead = true;
    float _time = 0.0f;
    Vector3 _originpos = MyTransform.position;
    Vector3 _targetpos = newpos + Vector3.back * 10.0f;
    Vector3 _newpos = Vector3.zero;
    float _particlerate = MinParticle;
    // RP_start_emission.rateOverTime = _particlerate;
    //  RP_start.Play();
    while (_time < RespawnMovetime)
    {
      _newpos = Vector3.Lerp(_originpos, _targetpos, Mathf.Pow(_time/ RespawnMovetime,2.5f));
      _newpos = new Vector3(_newpos.x, Mathf.Clamp(_newpos.y, MinY, MaxY), -10.0f);
      MyTransform.position = _newpos;
     // _particlerate = Mathf.Lerp(MinParticle, MaxParticle, Mathf.Pow((_time / RespawnMovetime), 2.0f));
     // RP_start_emission.rateOverTime=_particlerate;
      _time += Time.deltaTime;
      yield return null;
    }
    yield return new WaitForSeconds(waittime);
   RP_start.Stop();
  //  RP_end.Play();
    IsDead = false;
    TargetOffset = Vector3.zero;
  }

  public void RockPressed()
  {
    StartCoroutine(rockcoroutine());
  }
  private IEnumerator rockcoroutine()
  {
    float _waittime = 0.25f;
    OffsetDel += RockDown;
    yield return new WaitForSeconds(_waittime);
    OffsetDel -= RockDown;
  //  OffsetDel += RockUp;
  //  yield return new WaitForSeconds(_waittime);
  //  OffsetDel -= RockUp;
  }
  public void RockDown() => rockdel(true);
  private void RockUp() => rockdel(false);
  private void rockdel(bool isdown)
  {
    float _speed = 10.0f;
    TargetOffset += Vector3.up * _speed * Time.deltaTime * (isdown ? -1 : 1);
  }
  #region 횃불 사망 홍수
  public void StartFlood()
  {
    IsDead = true;
    StartCoroutine(flood_move());
    StartCoroutine(flood_angle());
    UIManager.Instance.FadeOut(Flood_FadeOutTime);
  }
  public void StartFloodParticle()
  {
    StartCoroutine(flood_shake());
    ParticleSystem.ShapeModule _shape = Flood_dustparticle.shape;
    _shape.position = MyTransform.position + new Vector3(0.0f, 5.5f, 3.0f);
    Flood_dustparticle.Play();
    _shape = Flood_stoneparticle.shape;
    _shape.position = MyTransform.position + new Vector3(0.0f, 5.5f, 3.0f);
    Flood_stoneparticle.Play();
  }
  private IEnumerator flood_move()  //홍수 오브젝트 이동
  {
    float _time = 0.0f;
    Vector3 _originpos = MyTransform.position + Vector3.forward * 3.0f;
    while (_time < Flood_time)
    {
      FloodTrans.position = _originpos + Vector3.up * Mathf.Lerp(Flood_originpos, Flood_targetpos, _time / Flood_time);
      _time += Time.deltaTime;
      yield return null;
    }
    yield return new WaitForSeconds(Flood_FadeOutTime - Flood_FadeOutTime);
    IsFlooded = true;
    Flood_dustparticle.Stop();
    Flood_stoneparticle.Stop();
  }
  private IEnumerator flood_angle() //홍수 날때 카메라 회전
  {
    float _time = 0.0f;
    while (_time < Flood_time)
    {
      MyTransform.eulerAngles = Vector3.forward * Mathf.Lerp(0.0f, Flood_angle,_time/Flood_time);
      FloodTrans.eulerAngles = Vector3.forward * Mathf.Lerp(0.0f, Flood_angle/5.0f, _time / Flood_time);
      _time += Time.deltaTime;
      yield return null;
    }
  }
  private IEnumerator flood_shake() //홍수 날때 카메라 진동
  {
    Vector3 _originpos = MyTransform.position;
    Vector3 _offset = Vector3.zero;
    while (true)
    {
      _offset = new Vector3(UnityEngine.Random.Range(-Flood_shakedeg, Flood_shakedeg), UnityEngine.Random.Range(-Flood_shakedeg, Flood_shakedeg));
      MyTransform.position = _originpos + _offset;
      yield return new WaitForSecondsRealtime(1.0f / Flood_shakecount);
    }
  }
  public void FinishFlood()
  {
    IsFlooded = false;
    StopAllCoroutines();
    FloodTrans.position = Vector2.up * 15.0f;
    FloodTrans.eulerAngles = Vector3.zero;
    MyTransform.eulerAngles = Vector3.zero;
  }
  #endregion

  #region 튜토리얼
  public void Tutorial_start(Vector2 startpos,float size) //튜토리얼 시작하면
  {
    IsDead = true;  //카메라 자동이동 끄고
    MyTransform.position = (Vector3)startpos + Vector3.back * 10.0f;  //튜토리얼 위치로 이동하고
    MyCamera.orthographicSize = size;     //카메라 사이즈 확대하고
    OriginIntensity = MyLight.intensity;  
    StartCoroutine(settinglight(0.0f));   //기본 배경 조명 끄고
  }
  public void Tutorial_finish(float targetsize,float targettime)  //튜토리얼 끝나면
  {
    StartCoroutine(tutorial_finish(targetsize,targettime));       //카메라 사이즈 다시 늘려주고
    StartCoroutine(settinglight(OriginIntensity));                //원래 있던 빛도 복구
  }
  #endregion

  #region 엔딩
  private IEnumerator settinglight(float targetint) //불조정
  {
    float _targettime = 0.5f;
    float _time = 0.0f;
    float _originint=MyLight.intensity;
    while (_time < _targettime)
    {
      MyLight.intensity = Mathf.Lerp(_originint, targetint, _time / _targettime);
      _time += Time.deltaTime;
      yield return null;
    }
    MyLight.intensity = targetint;
  }
  private IEnumerator tutorial_finish(float targetsize,float targettime)
  {
    float _time = 0.0f;
    float _originsize = MyCamera.orthographicSize;
    while (_time < targettime)
    {
      MyCamera.orthographicSize=Mathf.Lerp(_originsize, targetsize,Mathf.Sqrt(_time/targettime));
      _time += Time.deltaTime;
      yield return null;
    }
  }
  public void Ending_move(Vector2 newpos,float movetime)
  {
    StartCoroutine(ending_move((Vector3)newpos + Vector3.back * 10.0f,movetime));
  }
  public void Ending_shake(float shaketime)
  {
    StartCoroutine(ending_shake(shaketime));
  }
  private IEnumerator ending_move(Vector3 newpos,float movetime)
  {
    IsDead = true;
    Vector3 _originpos = MyTransform.position;
    float _time = 0.0f;
    while (_time < movetime)
    {
      MyTransform.position = Vector3.Lerp(_originpos, newpos, _time / movetime);
      _time += Time.deltaTime;
      yield return null;
    }
  }
  private IEnumerator ending_shake(float shaketime)
  {
    float _time = 0.0f;
    float _shakedegree = 0.06f;
    int _shakecount = 10;
     Vector3 _originpos = MyTransform.position;
    Vector3 _offset = Vector2.zero;
    while (_time < shaketime)
    {
      _offset = new Vector2(UnityEngine.Random.Range(-_shakedegree, _shakedegree), UnityEngine.Random.Range(-_shakedegree, _shakedegree));
      MyTransform.position = _originpos + _offset;
      _time += 1 / _shakecount;
      yield return new WaitForSeconds(1 / _shakecount);
    }
  }
  #endregion
}
