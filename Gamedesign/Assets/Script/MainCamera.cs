using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
  private Transform PlayerTransform = null; //��ǥ �÷��̾� Ʈ������
  private Transform MyTransform = null;     //ī�޶��� Ʈ������
  private Camera MyCamera = null; 
  [SerializeField] private float CameraSpeed = 1.0f;  //ī�޶� �ӵ�
  [SerializeField] private float MinY = 2.0f; //�ּ� �ٴڰ�
  [SerializeField] private float MaxY = 10.0f;//�ִ� �ٴڰ�
  [SerializeField] private float RespawnMovetime = 1.5f;
  private Vector3 NewPos= Vector3.zero;
  private bool IsDead = false;
  [SerializeField] private ParticleSystem RP_start = null;
  private ParticleSystem.EmissionModule RP_start_emission;
  [SerializeField] private int MinParticle = 50;
  [SerializeField] private int MaxParticle = 180;
  [SerializeField] private ParticleSystem RP_end = null;
  [SerializeField] private ParticleSystem Particle_world = null;
  private ParticleSystem.ShapeModule particle_world_shape;
  [SerializeField] private ParticleSystem Particle_soul = null;
  private ParticleSystem.ShapeModule particle_soul_shape;
  [Space(5)]
  [SerializeField] private Transform FloodTrans = null; //ī�޶� ��� ȫ�� Ʈ������
  [SerializeField] private float Flood_originpos = -12.0f;//ȫ�� ���� ��ġ
  [SerializeField] private float Flood_targetpos = 0.5f;  //ȫ�� ���� ��ġ
  [SerializeField] private float Flood_time = 5.0f;       //ī�޶� �� ���� �ð�
  [SerializeField] private float Flood_shakedeg = 0.8f;   //ī�޶� ���� ����
  [SerializeField] private int Flood_shakecount = 8;      //�ʴ� ī�޶� ���� Ƚ��
  [SerializeField] private float Flood_angle = 15.0f;     //ī�޶� ȸ�� ũ��
  [SerializeField] private float Flood_FadeOutTime = 7.0f;  //ī�޶� ���̵�ƿ��ϴ� �ð�
  [SerializeField] private ParticleSystem Flood_dustparticle = null;  //���� ��ƼŬ
  [SerializeField] private ParticleSystem Flood_stoneparticle = null; //���� ��ƼŬ
  private bool IsFlooded = false;                                     //ħ�� �Ϸ�Ƴ���
  private void Setup()
  {
    MyTransform = transform;
    IsDead = true;
    RP_start_emission = RP_start.emission;
    particle_world_shape = Particle_world.shape;
    particle_soul_shape = Particle_soul.shape;
    MyCamera = GetComponent<Camera>();
  }
  private void Start()
  {
    Setup();
  }
  private void Update()
  {
    //  particle_world_shape.position = new Vector3(MyTransform.position.x, MyTransform.position.y + 5.5f, -1.0f);
    if (Input.anyKeyDown && IsFlooded) GameManager.Instance.StartTutorial();
    particle_soul_shape.position = new Vector3(MyTransform.position.x, MyTransform.position.y, 8.0f);
    if (IsDead) return;
    NewPos = Vector3.Lerp(MyTransform.position, PlayerTransform.position, Time.deltaTime * CameraSpeed);
    NewPos = new Vector3(NewPos.x, Mathf.Clamp(NewPos.y, MinY, MaxY), -10.0f);
    MyTransform.position = NewPos;
  }
  public void SetPlayer(Transform player)
  {
    PlayerTransform = player;
  }
  public float MoveToPosition(Vector3 newpos,float waittime)
  {
    StartCoroutine(moveto(newpos,waittime));
    return RespawnMovetime + 0.1f;
  }
  public void StartRPParticle() => RP_start.Play();
  private IEnumerator moveto(Vector3 newpos,float waittime)
  {
    IsDead = true;
    float _time = 0.0f;
    Vector3 _originpos = MyTransform.position;
    Vector3 _newpos = newpos + Vector3.back * 10.0f;
    float _particlerate = MinParticle;
   // RP_start_emission.rateOverTime = _particlerate;
  //  RP_start.Play();
    while (_time < RespawnMovetime)
    {
      NewPos = Vector3.Lerp(_originpos, _newpos,Mathf.Pow(_time/ RespawnMovetime,2.5f));
      NewPos = new Vector3(NewPos.x, Mathf.Clamp(NewPos.y, MinY, MaxY), -10.0f);
      MyTransform.position = NewPos;
     // _particlerate = Mathf.Lerp(MinParticle, MaxParticle, Mathf.Pow((_time / RespawnMovetime), 2.0f));
     // RP_start_emission.rateOverTime=_particlerate;
      _time += Time.deltaTime;
      yield return null;
    }
    yield return new WaitForSeconds(waittime);
   RP_start.Stop();
  //  RP_end.Play();
    IsDead = false;
  }

  public void StartFlood()
  {
    IsDead = true;
    StartCoroutine(flood_move());
    StartCoroutine(flood_angle());
    StartCoroutine(flood_shake());
    UIManager.Instance.FadeOut(Flood_FadeOutTime);
    ParticleSystem.ShapeModule _shape = Flood_dustparticle.shape;
    _shape.position = MyTransform.position + new Vector3(0.0f, 5.5f, 3.0f);
    Flood_dustparticle.Play();
    _shape=Flood_stoneparticle.shape;
    _shape.position = MyTransform.position + new Vector3(0.0f, 5.5f, 3.0f);
    Flood_stoneparticle.Play();
    
  }
  private IEnumerator flood_move()  //ȫ�� ������Ʈ �̵�
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
  private IEnumerator flood_angle() //ȫ�� ���� ī�޶� ȸ��
  {
    float _time = 0.0f;
    while (_time < Flood_time)
    {
      MyTransform.eulerAngles = Vector3.forward * Mathf.Lerp(0.0f, Flood_angle,_time/Flood_time);
      FloodTrans.eulerAngles = Vector3.forward * Mathf.Lerp(0.0f, Flood_angle/3.0f, _time / Flood_time);
      _time += Time.deltaTime;
      yield return null;
    }
  }
  private IEnumerator flood_shake() //ȫ�� ���� ī�޶� ����
  {
    Vector3 _originpos = MyTransform.position;
    Vector3 _offset = Vector2.zero;
    while (true)
    {
      _offset = new Vector2(Random.Range(-Flood_shakedeg, Flood_shakedeg),Random.Range(-Flood_shakedeg, Flood_shakedeg));
      MyTransform.position = _originpos + _offset;
      yield return new WaitForSeconds(1.0f / Flood_shakecount);
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
  public void Tutorial_start(Vector2 startpos,float size)
  {
    IsDead = true;
    MyTransform.position = (Vector3)startpos + Vector3.back * 10.0f;
    MyCamera.orthographicSize = size;
  }
  public void Tutorial_finish(float targetsize,float targettime)
  {
    StartCoroutine(tutorial_finish(targetsize,targettime));
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
      _offset = new Vector2(Random.Range(-_shakedegree, _shakedegree), Random.Range(-_shakedegree, _shakedegree));
      MyTransform.position = _originpos + _offset;
      _time += 1 / _shakecount;
      yield return new WaitForSeconds(1 / _shakecount);
    }
  }
}
