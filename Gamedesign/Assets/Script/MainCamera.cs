using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using System;

public class MainCamera : MonoBehaviour
{
  private Transform MyTransform = null;     //ī�޶��� Ʈ������
  private Camera MyCamera = null; 
  [SerializeField] private float CameraSpeed = 1.0f;  //ī�޶� �ӵ�
  [SerializeField] private float MinY = 2.0f; //�ּ� �ٴڰ�
  [SerializeField] private float MaxY = 10.0f;//�ִ� �ٴڰ�
  [SerializeField] private float RespawnMovetime = 1.5f;
  private Vector3 Offset = Vector3.zero;    //1������ �� �̵��� ũ��
  private Vector3 LastPos = Vector3.zero;   //1������ �� �̵� �� ��ġ
  private Transform TargetTransform = null; //ī�޶� �̵� ��ǥ Ʈ������
  private Vector3 TargetOffset = Vector3.zero;  //ī�޶� �̵� ��ǥ ������
  private Action OffsetDel = null;              //Offset ����ġ �븮��
  private bool IsDead = false;
  [SerializeField] private ParticleSystem RP_start = null;  //������ ��ƼŬ
  private ParticleSystem.EmissionModule RP_start_emission;
  [SerializeField] private int MinParticle = 50;
  [SerializeField] private int MaxParticle = 180;
  [SerializeField] private ParticleSystem RP_end = null;    //������ ������ ��ƼŬ(�Ⱦ�)
  [SerializeField] private ParticleSystem Particle_world = null;//�Ϲݰ� ȯ�� ��ƼŬ(�Ⱦ�)
  private ParticleSystem.ShapeModule particle_world_shape;
  [SerializeField] private ParticleSystem Particle_soul = null; //��ȥ�� ȯ�� ��ƼŬ
  private ParticleSystem.ShapeModule particle_soul_shape;
  [Space(5)]
  [SerializeField] private SouldeathModule SDModule = null; //��ȥ��� ���� ������
  [SerializeField] private Transform FloodTrans = null; //ī�޶� ��� ȫ�� Ʈ������
  [SerializeField] private ParticleSystem Flood_dustparticle = null;  //���� ��ƼŬ
  [SerializeField] private ParticleSystem Flood_stoneparticle = null; //���� ��ƼŬ
  private bool IsFlooded = false;                                     //ħ�� �Ϸ�Ƴ���
  [SerializeField] private Light2D MyLight = null;
  private float OriginIntensity = 0.0f;
  private float OriginSize = 5.4f;                                    //ī�޶� �⺻ ������
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

    if (Input.anyKeyDown && IsFlooded) LoadNewGame(); //�� ������ �ƹ��ų� ������ �����

    particle_soul_shape.position = new Vector3(MyTransform.position.x, MyTransform.position.y, 8.0f); //��ȥ�� ��ƼŬ ��ġ ��� ������Ʈ


    if (!IsDead) UpdateOffset(); //������ �ȿ�����
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
  private IEnumerator rockshake() //��� �μ��� ����
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

  #region CameraEvent ���� �Լ�

  private IEnumerator lerpoffset;
  private IEnumerator camerasize;
  private bool IsOffsetLerping = false;
  public void SetSpecialCamera(Vector2 targetpos,float distanceratio, float sizeratio,float sizetime)
  {
    //targetpos�� �߽����� distanceratio��ŭ ������ ������ TargetOffset���� ������Ʈ�ϰ� sizeratio��ŭ ���� ��ȭ
   if(camerasize!=null)StopCoroutine(camerasize);
    lerpoffset = keellerp(targetpos, distanceratio);
    camerasize = changesize(OriginSize * sizeratio, sizetime);
    StartCoroutine(lerpoffset);
    StartCoroutine(camerasize);
  }
  public void SetSpecialCamera(Vector2 targetoffset,float sizeratio,float sizetime)
  {
    if (camerasize != null) StopCoroutine(camerasize);
    //targetoffset�� TargetOffset���� �����ϰ� sizeratio��ŭ ���� ��ȭ
    TargetOffset = targetoffset;
    camerasize = changesize(OriginSize * sizeratio, sizetime);
    StartCoroutine(camerasize);
  }
  public void ResetCamera()
  {
    if (IsOffsetLerping) { StopCoroutine(lerpoffset); IsOffsetLerping = false; }  //�����������̶�� �ڷ�ƾ�� ����
    TargetOffset = Vector3.zero;            //�ڷ�ƾ�� �����Ѵٰ� TargetOffset�� �ʱ�ȭ�Ǵ°��� �ƴϴ� �������� �ʱ�ȭ
    camerasize = changesize(OriginSize, 0.8f);
    StartCoroutine(camerasize); //ī�޶� ������� �������
  }
  private IEnumerator keellerp(Vector3 targetpos,float ratio) //TargetOffset�� ��� �����ϴ� �ڷ�ƾ
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
  private IEnumerator changesize(float targetsize,float sizetime)            //ª�� �ð����� ����� �����ϴ� �ڷ�ƾ
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

  #region ī�޶� ��ǥ����
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
  /// resetoffset Ű�� TargetOffset �ʱ�ȭ
  /// </summary>
  /// <param name="target"></param>
  /// <param name="resetoffset"></param>
  public void SetTarget(Transform target,bool resetoffset)
  {
    TargetTransform = target;
    if (resetoffset) TargetOffset = Vector3.zero;
  }
  #endregion

  #region ������ ����
  public float MoveToResapwn(Vector3 newpos,float waittime) //���������� �̵�
  {
    StartCoroutine(moveto(newpos,waittime));
    return RespawnMovetime + 0.1f;
  }
  public void StartRPParticle() { RP_start.Play(); AudioManager.Instance.PlayClip(1); } //������ ��ƼŬ ����
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

  #region ����¢
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

  #region ȶ�� ��� ȫ��
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
  private IEnumerator flood_move()  //ȫ�� ������Ʈ �̵�
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
  private IEnumerator flood_angle() //ȫ�� ���� ī�޶� ȸ��
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
  private IEnumerator flood_shake() //ȫ�� ���� ī�޶� ����
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

  #region Ʃ�丮��
  public void Tutorial_start(Vector2 startpos,float size) //Ʃ�丮�� �����ϸ�
  {
    IsDead = true;  //ī�޶� �ڵ��̵� ����
    MyTransform.position = (Vector3)startpos + Vector3.back * 10.0f;  //Ʃ�丮�� ��ġ�� �̵��ϰ�
    MyCamera.orthographicSize = size;     //ī�޶� ������ Ȯ���ϰ�
    OriginIntensity = MyLight.intensity;  
    StartCoroutine(settinglight(0.0f));   //�⺻ ��� ���� ����
  }
  public void Tutorial_finish(float targetsize,float targettime)  //Ʃ�丮�� ������
  {
    StartCoroutine(tutorial_finish(targetsize,targettime));       //ī�޶� ������ �ٽ� �÷��ְ�
    StartCoroutine(settinglight(OriginIntensity));                //���� �ִ� ���� ����
  }
  #endregion

  #region ����
  private IEnumerator settinglight(float targetint) //������
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
