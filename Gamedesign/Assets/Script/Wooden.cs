using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Wooden : MonoBehaviour,Interactable
{
  [SerializeField] private float RequireTime = 2.0f; //다 타는데 필요한 시간
 [Range(0.0f,1.0f)] [SerializeField] private float IgniteTime = 0.5f; //발화가 진행되는 순간 (0~1)
  private float Progress = 0.0f;                    //현재 진척도
  private bool IsActive = true;                    //활성화된 목재인지
 [SerializeField] private SpriteRenderer MySpr = null;              //내 스프라이트랜더러
  [SerializeField] private Transform FireTransform = null;//화염 이미지 트랜스폼
  private bool IsFired = false;                     //불이 올라와있는지
  [SerializeField] private ParticleSystem SmokeParticle = null;//검은 연기 파티클
  [SerializeField] private ParticleSystem BurningParticle = null;//불타는 파티클
  [SerializeField] private ParticleSystem FiredParticle = null; //다 탔을떄 파티클
  private Vector2 CurrentTorchPos = Vector2.zero;       //실시간으로 콜라이더에 들어가있는 횃불 위치
  private Vector2 TargetTorchPos = Vector2.zero;        //불 시작할 위치
  [SerializeField] private float SmokeTime = 0.2f;      //연기 올라오는 시간
  [SerializeField] private Transform MaskTransform = null;//마스크 트랜스폼
  [SerializeField] private float FireSize = 2.0f;
  [SerializeField] private Light2D MyLight = null;
  [SerializeField] private float MaxLightOuter = 2.0f;
  public void FireUp()
  {
    IsFired = true;
    StartCoroutine(smokecoroutine());
  }
  private IEnumerator smokecoroutine()
  {
    yield return new WaitForSeconds(SmokeTime);
    if (IsFired)
    {
      SmokeParticle.transform.position = CurrentTorchPos;
      SmokeParticle.Play();
    }
  }
  public void FireDown()
  {
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
    if (!IsActive) return;  //!IsActive면 다 탔다는 뜻

    if ((Progress / RequireTime) < IgniteTime)  //점화 시작 비율 이전
    {
     if(IsFired) Progress+=Time.deltaTime;  //불이 있으면 진행도 증가
     else Progress-= Time.deltaTime;        //불이 없으면 진행도 감소
     Progress=Mathf.Clamp(Progress, 0.0f, RequireTime); //0 미만으로 떨어지지 않도록

      if ((Progress / RequireTime) >= IgniteTime) //Ignite 임계점 넘어서면
      {
        TargetTorchPos = Vector2.Lerp(CurrentTorchPos, transform.position, 0.5f);  //TargetTorchPos에 값 할당
        FireTransform.gameObject.SetActive(true); //불 이미지 활성화
        FireTransform.position = TargetTorchPos;  //불 위치 설정
        FireTransform.localScale = Vector3.zero;  //불 크기 초기값(0) 설정
        MaskTransform.position = TargetTorchPos;
        SmokeParticle.Stop();                     //검은연기 파티클 종료
        BurningParticle.transform.position = TargetTorchPos; //불타는 파티클 위치 설정
        BurningParticle.Play();                   //불타는 파티클 실행
        MyLight.transform.position = TargetTorchPos;
      }

     return;
    }
    
    if ((Progress / RequireTime) >= IgniteTime)//점화 시작 비율 이후
    {
      Progress += Time.deltaTime; //불이 있든 없든 계속 불탐

      FireTransform.localScale = Vector3.one * Mathf.Lerp(0, FireSize, ((Progress / RequireTime) - IgniteTime)/(1-IgniteTime));
      MyLight.pointLightOuterRadius=MaxLightOuter* ((Progress / RequireTime) - IgniteTime) / (1 - IgniteTime);
      //Ignitetime ~ 1을 0 ~ 1로 변환시키고 크기에 대입

      if (Progress >= RequireTime) //점화 수치 최대
      {
        BurningParticle.Stop();
        gameObject.layer = 0; //게임오브젝트 레이어 제거
        MySpr.color = Color.gray;
        StartCoroutine(burningcoroutine());
        IsActive = false;
      }
    }
  }
  private IEnumerator burningcoroutine()
  {
    float _time = 0.0f;
    float _firetime = 0.1f;
    FiredParticle.Play();
    while (_time < _firetime)
    {
      _time += Time.deltaTime;
      MaskTransform.localScale = Vector3.one * Mathf.Lerp(0, FireSize, _time / _firetime); //마지막 불까지 다 태웠으면
      yield return null;
    }
    _time = 0.0f;
    while (_time < _firetime)
    {
      _time += Time.deltaTime;
      MyLight.pointLightOuterRadius = Mathf.Lerp(MaxLightOuter, 0.0f, _time / _firetime);
      yield return null;
    }

    yield return new WaitForSeconds(0.4f);

    for (int i = 0; i < transform.childCount; i++)
    {
      transform.GetChild(i).gameObject.SetActive(false);
    }
    gameObject.tag = "Untagged";
    this.enabled = false;  //자식 객체 전부 비활성화하고 스크립트도 비활성화
  }
  public void Setup()
  {
  }
  private void Awake()
  {
    Setup();
  }
}
