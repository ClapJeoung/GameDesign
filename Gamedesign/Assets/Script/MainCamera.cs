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
  [SerializeField] private SouldeathModule SDModule = null; //영혼사망 각종 데이터
  [SerializeField] private Transform FloodTrans = null; //카메라 잠길 홍수 트랜스폼
  [SerializeField] private ParticleSystem Flood_dustparticle = null;  //먼지 파티클
  [SerializeField] private ParticleSystem Flood_stoneparticle = null; //낙석 파티클
  private bool IsFlooded = false;                                     //침수 완료됐나요
  [SerializeField] private Light2D MyLight = null;
  private float OriginIntensity = 0.0f;
  private float OriginSize = 5.4f;                                    //카메라 기본 사이즈
  [HideInInspector] public float CurrentSizeRatio
  {
    get { return MyCamera.orthographicSize / OriginSize; }
  }
  [SerializeField] private SDEffect MySDE = null;
  private void Setup()
  {
    MyTransform = transform;
    IsDead = true;
    RP_start_emission = RP_start.emission;
    particle_world_shape = Particle_world.shape;
    particle_soul_shape = Particle_soul.shape;
    MyCamera = GetComponent<Camera>();
    OffsetDel = new Action(() => { });
    FloodTrans.GetComponent<SpriteRenderer>().enabled = false;
  }
  private void Start()
  {
    Setup();
  }
  private void LateUpdate()
  {
    if (finalwait) return;
    //  particle_world_shape.position = new Vector3(MyTransform.position.x, MyTransform.position.y + 5.5f, -1.0f);

    if (Input.anyKeyDown && IsFlooded) LoadNewGame(); //겜 끝나고 아무거나 누르면 재시작

    particle_soul_shape.position = new Vector3(MyTransform.position.x, MyTransform.position.y, 8.0f); //영혼계 파티클 위치 계속 업데이트


    if (!IsDead) UpdateOffset(); //죽으면 안움직임
  }
  private bool finalwait = false;
  public void LoadNewGame()
  {
    finalwait = true;
    UIManager.Instance.TurnOffRestartLogo();
  }
  public void UpdateOffset()
  {
    LastPos = MyTransform.position;
    Offset = Vector3.zero;
    Vector3 _newpos = Vector3.zero;
    _newpos = Vector3.Lerp(MyTransform.position, TargetTransform.position + TargetOffset+Vector3.up*0.5f, Time.deltaTime * CameraSpeed);
    _newpos = new Vector3(_newpos.x, Mathf.Clamp(_newpos.y, MinY, MaxY), -10.0f);
    Offset = _newpos - LastPos;
    OffsetDel();
    MyTransform.position += Offset;
  }

  private IEnumerator rockshakecoroutine;
  public void StartSpinRock()
  {
    rockshakecoroutine = rockshake();
    StartCoroutine(rockshakecoroutine);
  }
  private IEnumerator rockshake() //기둥 부수고 진동
  {
    Vector3 _originpos = MyTransform.position;
    Vector3 _offset = Vector3.zero;
    var _wait= new WaitForSeconds(1.0f / SDModule.Flood_shakecount);
    while (true)
    {
      _originpos = MyTransform.position;
      _offset = new Vector3(UnityEngine.Random.Range(-SDModule.Flood_shakedeg, SDModule.Flood_shakedeg), UnityEngine.Random.Range(-SDModule.Flood_shakedeg, SDModule.Flood_shakedeg));
      MyTransform.position = _originpos + _offset;
      yield return _wait;
    }
  }
  public void EndSpinRock() => StopCoroutine(rockshakecoroutine);

  #region CameraEvent 관련 함수

  private IEnumerator lerpoffset;
  private IEnumerator camerasize;
  private bool IsOffsetLerping = false;
  public void SetSpecialCamera(Vector2 targetpos,float distanceratio, float sizeratio,float sizetime)
  {
    //targetpos를 중심으로 distanceratio만큼 보간한 지점을 TargetOffset으로 업데이트하고 sizeratio만큼 배율 변화
   if(camerasize!=null)StopCoroutine(camerasize);
    lerpoffset = keellerp(targetpos, distanceratio);
    camerasize = changesize(OriginSize * sizeratio, sizetime);
    StartCoroutine(lerpoffset);
    StartCoroutine(camerasize);
  }
  public void SetSpecialCamera(Vector2 targetoffset,float sizeratio,float sizetime)
  {
    if (camerasize != null) StopCoroutine(camerasize);
    //targetoffset을 TargetOffset으로 지정하고 sizeratio만큼 배율 변화
    TargetOffset = targetoffset;
    camerasize = changesize(OriginSize * sizeratio, sizetime);
    StartCoroutine(camerasize);
  }
  public void ResetCamera()
  {
    if (IsOffsetLerping) { StopCoroutine(lerpoffset); IsOffsetLerping = false; }  //보간추적중이라면 코루틴을 정지
    TargetOffset = Vector3.zero;            //코루틴을 정지한다고 TargetOffset이 초기화되는것은 아니니 수동으로 초기화
    camerasize = changesize(OriginSize, 0.8f);
    StartCoroutine(camerasize); //카메라 사이즈는 원래대로
  }
  private IEnumerator keellerp(Vector3 targetpos,float ratio) //TargetOffset을 계속 보간하는 코루틴
  {
    IsOffsetLerping = true;
    float _movetime = 0.3f;
    float _time = 0.0f;
    float _currentratio = 1.0f;
    while (true)
    {
      _currentratio = 1 - (1 - ratio) * (_time / _movetime);
      TargetOffset = Vector3.Lerp(targetpos, MyTransform.position+Vector3.forward*10.0f, _currentratio) -MyTransform.position + Vector3.forward * 10.0f;
     if(_time<=_movetime) _time += Time.deltaTime;
      yield return null;
    }
  }
  private IEnumerator changesize(float targetsize,float sizetime)            //짧은 시간으로 사이즈를 변경하는 코루틴
  {
    float _time = 0.0f;
    float _originsize = MyCamera.orthographicSize;
    while (_time < sizetime)
    {
      MyCamera.orthographicSize = Mathf.Lerp(_originsize, targetsize, Mathf.Pow(_time / sizetime, 1.2f));
      _time += Time.deltaTime;
      yield return null;
    }
  }
  #endregion

  #region 카메라 목표설정
  public void SetTarget(Transform target,Vector3 offset) 
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
  #endregion

  #region 리스폰 관련
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
  #endregion

  #region 돌꽈짖
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
  #endregion

  #region 횃불 사망 홍수
  public void StartFlooda()
  {
    FloodTrans.GetComponent<SpriteRenderer>().enabled = true;
    IsDead = true;
    StartCoroutine(flood_move());
    StartCoroutine(flood_angle());
 //   UIManager.Instance.FadeOut(SDModule.Flood_FadeOutTime,true);
    Invoke("stopthat", SDModule.Flood_FadeOutTime + 1.0f);
  }
  private void stopthat()
  {
    StopAllCoroutines();
    StartCoroutine(flood_reset());
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
    while (_time < SDModule.Flood_time)
    {
      FloodTrans.position = _originpos + Vector3.up * Mathf.Lerp(SDModule.Flood_originpos, SDModule.Flood_targetpos, _time / SDModule.Flood_time);
      _time += Time.deltaTime;
      yield return null;
    }
    IsFlooded = true;
    Flood_dustparticle.Stop();
    Flood_stoneparticle.Stop();
  }
  private IEnumerator flood_angle() //홍수 날때 카메라 회전
  {
    float _time = 0.0f;
    while (_time < SDModule.Flood_time)
    {
      MyTransform.eulerAngles = Vector3.forward * Mathf.Lerp(0.0f, SDModule.Flood_angle,_time/ SDModule.Flood_time);
      FloodTrans.eulerAngles = Vector3.forward * Mathf.Lerp(0.0f, SDModule.Flood_angle / 5.0f, _time / SDModule.Flood_time);
      _time += Time.deltaTime;
      yield return null;
    }
  }
  private IEnumerator flood_shake() //홍수 날때 카메라 진동
  {
    Vector3 _originpos = MyTransform.position;
    Vector3 _offset = Vector3.zero;
    Transform[] _idles = new Transform[MySDE.Count];
    Transform[] _deletes = new Transform[MySDE.Count];
    Transform[] _lights=new Transform[MySDE.Count];
    Vector3[] _idles_origin=new Vector3[MySDE.Count];
    Vector3[] _deletes_origin=new Vector3[MySDE.Count];
    Vector3[] _lights_origin=new Vector3[MySDE.Count];
    for(int i=0;i<MySDE.Count; i++)
    {
      _idles[i] = MySDE.Particles_Idle[i].transform;_deletes[i] = MySDE.Particle_Delete[i].transform;
      _idles_origin[i] = _idles[i].position; _deletes_origin[i] = _deletes[i].position;
      _lights[i]=MySDE.Lights[i].transform; _lights_origin[i] = _lights[i].position;
    }
    var _waittime= new WaitForSecondsRealtime(1.0f / SDModule.Flood_shakecount);
    while (true)
    {
      _offset = new Vector3(UnityEngine.Random.Range(-SDModule.Flood_shakedeg, SDModule.Flood_shakedeg), UnityEngine.Random.Range(-SDModule.Flood_shakedeg, SDModule.Flood_shakedeg));
      MyTransform.position = _originpos + _offset;
   /*   for(int i = 0; i < MySDE.Count; i++)
      {
        _idles[i].position = _idles_origin[i] + _offset;
        _deletes[i].position = _deletes_origin[i] + _offset;
        _lights[i].position= _lights_origin[i] + _offset;
      }   */
      yield return _waittime;
    }
  }
  private IEnumerator flood_reset()
  {
    MyCamera.orthographicSize = 5.4f;
    MyTransform.eulerAngles = Vector3.zero;
    yield return null;
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
