using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Wooden : MonoBehaviour,Interactable,Lightobj
{
  [SerializeField] private float RequireTime = 2.0f; //다 타는데 필요한 시간
 [Range(0.0f,1.0f)] [SerializeField] private float IgniteTime = 0.5f; //발화가 진행되는 순간 (0~1)
  private float Progress = 0.0f;                    //현재 진척도
  [SerializeField] private SpriteRenderer Spr_A = null; //일반 상태 이미지
  [SerializeField] private SpriteRenderer Spr_B = null; //다른 차원 이미지
  [SerializeField] private SpriteRenderer Spr_C = null; //대기 상태 이미지
  [SerializeField] private Transform FireTransform = null;//화염 이미지 트랜스폼
  private bool IsFired = false;                     //불이 올라와있는지
  [SerializeField] private ParticleSystem SmokeParticle = null;//검은 연기 파티클
  [SerializeField] private ParticleSystem BurningParticle = null;//불타는 파티클
  [SerializeField] private ParticleSystem FiredParticle = null; //다 탔을떄 파티클
  private Vector3 CurrentTorchPos = Vector2.zero;       //실시간으로 콜라이더에 들어가있는 횃불 위치
  private Vector3 TargetTorchPos = Vector2.zero;        //불 시작할 위치
  [SerializeField] private float SmokeTime = 0.2f;      //연기 올라오는 시간
  [SerializeField] private Transform MaskTransform = null;//마스크 트랜스폼
  [SerializeField] private float FireSize = 2.0f;
  [SerializeField] private Light2D MyLight = null;
  [SerializeField] private float MaxLightOuter = 2.0f;
  [SerializeField] private Dimension MyDimension;
  private StageCollider MySC = null;
  private bool isactive = true;
  private Dimension OriginDimension;
  public bool IsActive
  {
    get { if (GameManager.Instance.CurrentSC.CurrentDimension == Dimension.A) //월드가 A 차원일때
      {
        if (MyDimension == Dimension.A ) return true; //나도 A면 활성화

        return false; //아니면 비활성화
      }
    else if (GameManager.Instance.CurrentSC.CurrentDimension == Dimension.B)  //월드가 B 차원일때
      {
        if (MyDimension == Dimension.B) return true; //나도 B면 활성화

        return false; //아니면 비활성화
      }
      return true;
    }
    set { isactive = value; }
  }
  public void FireUp()
  {
    if (!IsActive) return;
    IsFired = true;
    StartCoroutine(smokecoroutine());
  }
  private IEnumerator smokecoroutine()
  {
    yield return new WaitForSeconds(SmokeTime);
    if (IsFired)
    {
      SmokeParticle.transform.position = CurrentTorchPos+Vector3.back;
      SmokeParticle.Play();
    }
  }
  public void FireDown()
  {
    if (!IsActive) return;
    IsFired = false;
    StopAllCoroutines();
    if (SmokeParticle.isPlaying) SmokeParticle.Stop();
  }
  private void OnTriggerStay2D(Collider2D collision)
  {
    if (collision.CompareTag("Torch")) CurrentTorchPos = collision.transform.position;
  }
  private void Update()
  {
    if (GameManager.Instance.CurrentSC!=MySC||!IsActive) return;
    //현재 게임매니저에 있는 스테이지 콜라이더가 내 스테이지 콜라이더랑 다르거나 다 탄 상태면 중지

    if ((Progress / RequireTime) < IgniteTime)  //연기가 나는 수준
    {
     if(IsFired) Progress+=Time.deltaTime;  //불이 있으면 진행도 증가
     else Progress-= Time.deltaTime;        //불이 없으면 진행도 감소
     Progress=Mathf.Clamp(Progress, 0.0f, RequireTime); //0 미만으로 떨어지지 않도록

      if ((Progress / RequireTime) >= IgniteTime) //Ignite 임계점 넘어서면
      {
        TargetTorchPos = Vector2.Lerp(CurrentTorchPos, transform.position, 0.5f);  //TargetTorchPos에 값 할당
     //   FireTransform.gameObject.SetActive(true); //불 이미지 활성화
     //   FireTransform.position = TargetTorchPos;  //불 위치 설정
     //   FireTransform.localScale = Vector3.zero;  //불 크기 초기값(0) 설정
     //   MaskTransform.position = TargetTorchPos;
        SmokeParticle.Stop();                     //검은연기 파티클 종료
        BurningParticle.transform.position = TargetTorchPos + Vector3.back; //불타는 파티클 위치 설정
        BurningParticle.Play();                   //불타는 파티클 실행
        MyLight.transform.position = TargetTorchPos;
        AudioManager.Instance.PlayFire();
      }

     return;
    }
    
    if ((Progress / RequireTime) >= IgniteTime)//불이 붙는 수준
    {
      Progress += Time.deltaTime; //불이 있든 없든 계속 불탐
      if(SmokeParticle.isPlaying) SmokeParticle.Stop();
    //  FireTransform.localScale = Vector3.one * Mathf.Lerp(0, FireSize, ((Progress / RequireTime) - IgniteTime)/(1-IgniteTime));
      MyLight.pointLightOuterRadius=MaxLightOuter* ((Progress / RequireTime) - IgniteTime) / (1 - IgniteTime);
      //Ignitetime ~ 1을 0 ~ 1로 변환시키고 크기에 대입

      if (Progress >= RequireTime) //점화 수치 최대
      {
        BurningParticle.Stop();
        if (MyDimension == Dimension.A) { Spr_A.enabled = false; Spr_B.enabled = true; MyDimension = Dimension.B; }
        else if (MyDimension == Dimension.B) { Spr_B.enabled = false; Spr_A.enabled = true; MyDimension = Dimension.A; }
        Progress = 0.0f;
        IsFired = false;
        StartCoroutine(burningcoroutine());
      }
    }
  }
  private IEnumerator burningcoroutine()
  {
    float _time = 0.0f;
    float _firetime = 0.1f;
    FiredParticle.Play();
  /*  while (_time < _firetime)
    {
      _time += Time.deltaTime;
      MaskTransform.localScale = Vector3.one * Mathf.Lerp(0, FireSize, _time / _firetime); //마지막 불까지 다 태웠으면
      yield return null;
    }
    _time = 0.0f; */
    while (_time < _firetime)
    {
      _time += Time.deltaTime;
      MyLight.pointLightOuterRadius = Mathf.Lerp(MaxLightOuter, 0.0f, _time / _firetime);
      yield return null;
    }
    AudioManager.Instance.StopFire(true);
  }
  public void Setup()
  {
    MySC = transform.parent.GetComponent<StageCollider>();
    MySC.SetOrigin(this);
    Spr_B.size = Spr_A.size;
    Spr_C.size = Spr_B.size;
    GetComponent<BoxCollider2D>().size= Spr_A.size;
    ParticleSystem.ShapeModule myshape = FiredParticle.shape;
    myshape.scale =new Vector3( Spr_A.size.x, Spr_A.size.y, 1.0f);
    if (MyDimension == Dimension.A) Spr_B.enabled = false;
    else if(MyDimension== Dimension.B) Spr_A.enabled = false;
    OriginDimension = MyDimension;
    GameManager.Instance.AllLights.Add(this);
    float _min = 15.0f, _max = 40.0f,_minsize=1.0f,_maxsize=10.0f,_size= Spr_A.size.x* Spr_A.size.y;
    ParticleSystem.EmissionModule _emi = FiredParticle.emission;
    _emi.rateOverTime = Mathf.Lerp(_min, _max,  _size/ _maxsize);
  }
  private void Start()
  {
    Setup();
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

  public void ResetDimension()
  {
   if( MyDimension != OriginDimension)
    {
      MyDimension = OriginDimension;
      FiredParticle.Play();
      if (MyDimension == Dimension.B) { Spr_A.enabled = false; Spr_B.enabled = true; }
      else if (MyDimension == Dimension.A) { Spr_B.enabled = false; Spr_A.enabled = true;  }
    }

  }
}
