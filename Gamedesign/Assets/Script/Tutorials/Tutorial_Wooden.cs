using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Tutorial_Wooden : MonoBehaviour, Interactable
{
  [SerializeField] private float RequireTime = 2.0f; //다 타는데 필요한 시간
  [Range(0.0f, 1.0f)][SerializeField] private float IgniteTime = 0.5f; //발화가 진행되는 순간 (0~1)
  private float Progress = 0.0f;                    //현재 진척도
  [SerializeField] private SpriteRenderer Spr_A = null; //일반 상태 이미지
  [SerializeField] private SpriteRenderer Spr_B = null; //다른 차원 이미지
  [SerializeField] private SpriteRenderer Spr_C = null; //대기 상태 이미지
  [SerializeField] private Transform FireTransform = null;//화염 이미지 트랜스폼
  private bool IsFired = false;                     //불이 올라와있는지
  [SerializeField] private ParticleSystem SmokeParticle = null;//검은 연기 파티클
  [SerializeField] private ParticleSystem BurningParticle = null;//불타는 파티클
  [SerializeField] private ParticleSystem FiredParticle = null; //다 탔을떄 파티클
  [SerializeField] private float SmokeTime = 0.2f;      //연기 올라오는 시간
  private Dimension MyDimension=Dimension.A;
  private TutorialManager MyManager = null;
  private bool isactive = true;
  public bool IsActive
  {
    get
    {
      if (MyManager.TutorialDimension == Dimension.A) //월드가 A 차원일때
      {
        if (MyDimension == Dimension.A) return true; //나도 A면 활성화

        return false; //아니면 비활성화
      }
      else if (MyManager.TutorialDimension == Dimension.B)  //월드가 B 차원일때
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
  private void Update()
  {
    if (!IsActive) return;
    //현재 게임매니저에 있는 스테이지 콜라이더가 내 스테이지 콜라이더랑 다르거나 다 탄 상태면 중지

    if ((Progress / RequireTime) < IgniteTime)  //연기가 나는 수준
    {
      if (IsFired) Progress += Time.deltaTime;  //불이 있으면 진행도 증가
      else Progress -= Time.deltaTime;        //불이 없으면 진행도 감소
      Progress = Mathf.Clamp(Progress, 0.0f, RequireTime); //0 미만으로 떨어지지 않도록

      if ((Progress / RequireTime) >= IgniteTime) //Ignite 임계점 넘어서면
      {
        SmokeParticle.Stop();                     //검은연기 파티클 종료
        BurningParticle.Play();                   //불타는 파티클 실행
      }
      return;
    }

    if ((Progress / RequireTime) >= IgniteTime)//불이 붙는 수준
    {
      Progress += Time.deltaTime; //불이 있든 없든 계속 불탐
      if (SmokeParticle.isPlaying) SmokeParticle.Stop();
      //  FireTransform.localScale = Vector3.one * Mathf.Lerp(0, FireSize, ((Progress / RequireTime) - IgniteTime)/(1-IgniteTime));
      //Ignitetime ~ 1을 0 ~ 1로 변환시키고 크기에 대입

      if (Progress >= RequireTime) //점화 수치 최대
      {
        BurningParticle.Stop();
        FiredParticle.Play();
        if (MyDimension == Dimension.A) { Spr_A.enabled = false; Spr_B.enabled = true; MyDimension = Dimension.B; }
        else if (MyDimension == Dimension.B) { Spr_B.enabled = false; Spr_A.enabled = true; MyDimension = Dimension.A; }
        Progress = 0.0f;
        IsFired = false;
        MyManager.Fired();  //화끈해진레후
      }
    }
  }
  public void Setup()
  {
    MyManager = transform.parent.parent.GetComponent<TutorialManager>();
  }
  private void Start()
  {
    Setup();
  }
  public void Active()
  {
    Spr_A.enabled = true;
    Spr_B.enabled = true;
    Spr_C.enabled = true;
    FiredParticle.Play();
  }
  public void DeActive()
  {
    FiredParticle.Play();
    Spr_A.enabled = false;
    Spr_B.enabled = false;
    Spr_C.enabled = false;
    Invoke("asdf", 1.0f);
  }
  private void asdf() => Destroy(gameObject);
}
