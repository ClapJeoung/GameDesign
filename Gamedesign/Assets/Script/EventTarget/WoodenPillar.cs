using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodenPillar : MonoBehaviour,Interactable
{
  [SerializeField] private float RequireTime = 2.0f; //한번 불태우는데 필요한 시간
  [SerializeField] private float IgniteTime = 2.0f;  //불 붙고 다 타는데 걸리는 시간
  private float Progress = 0.0f;                    //현재 진척도
  [SerializeField] private SpriteRenderer Spr_A = null; //일반 상태 이미지
  private bool IsFired = false;                     //불이 올라와있는지
  [SerializeField] private ParticleSystem SmokeParticle = null;//검은 연기 파티클
  [SerializeField] private ParticleSystem BurningParticle = null;//불타는 파티클
  [SerializeField] private ParticleSystem FiredParticle = null; //다 탔을떄 파티클
  private Vector3 CurrentTorchPos = Vector2.zero;       //실시간으로 콜라이더에 들어가있는 횃불 위치
  private Vector3 TargetTorchPos = Vector2.zero;        //불 시작할 위치
  [SerializeField] private float SmokeTime = 0.2f;      //연기 올라오는 시간
  private StageCollider MySC = null;
  private bool IsActive = true;
  [SerializeField] private SpinRock MyRock = null;      //이거 불타면 작동할 돌
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
      SmokeParticle.transform.position = CurrentTorchPos + Vector3.back;
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
    if (GameManager.Instance.CurrentSC != MySC || !IsActive) return;
    //현재 게임매니저에 있는 스테이지 콜라이더가 내 스테이지 콜라이더랑 다르거나 다 탄 상태면 중지

    if ((Progress / RequireTime) < 1.0f)
    {
      if (IsFired) Progress += Time.deltaTime;
      else Progress -= Time.deltaTime; 
      Progress = Mathf.Clamp(Progress, 0.0f, RequireTime);

      if ((Progress / RequireTime) >= 1.0f) //Ignite 임계점 넘어서면
      {
        TargetTorchPos = Vector2.Lerp(CurrentTorchPos, transform.position, 0.5f);  //TargetTorchPos에 값 할당
        SmokeParticle.Stop();                     //검은연기 파티클 종료
        BurningParticle.transform.position = TargetTorchPos + Vector3.back; //불타는 파티클 위치 설정
        StartCoroutine(getfired());
      }
      return;
    }

   /* if ((Progress / RequireTime) >= IgniteTime)//불이 붙는 수준
    {
      Progress += Time.deltaTime; //불이 있든 없든 계속 불탐
      if (SmokeParticle.isPlaying) SmokeParticle.Stop();

      if (Progress >= RequireTime) //점화 수치 최대
      {
      }
    }   */
  }
  private IEnumerator getfired()
  {
    BurningParticle.Play();
    Progress = 0.0f;
    IsActive = false;
    IsFired = false;
    ParticleSystem.EmissionModule _emission = BurningParticle.emission;
    ParticleSystem.ShapeModule _shape = BurningParticle.shape;
    float _minparticle = 30.0f, _maxparticle = 200.0f, _minsize = 0.1f, _maxsize = 1.0f, _time = 0.0f, _targettime = 2.5f;
    Vector3 _origin = _shape.position;
    _shape.scale = Vector3.one * _minsize;
    float _progress = 0.0f;
    while (_time < IgniteTime)
    {
      _progress = Mathf.Pow(_time / IgniteTime, 2.5f);
      _emission.rateOverTime = Mathf.Lerp(_minparticle, _maxparticle, _progress);
      _shape.scale = Vector3.one * Mathf.Lerp(_minsize,_maxsize,_progress);
      _shape.position = Vector3.Lerp(_origin, Vector3.zero, _progress);
      _time += Time.deltaTime;
      yield return null;
    }
    BurningParticle.Stop();
    FiredParticle.Play();
    Spr_A.enabled = false;
    gameObject.layer = LayerMask.NameToLayer("Default");
    yield return new WaitForSeconds(1.5f);
    MyRock.Active();
  }
  public void Setup()
  {
    MySC = transform.parent.GetComponent<StageCollider>().SetOrigin(this);
  }
  private void Start()
  {
    Setup();
  }
  public void TurnOn()
  {
  }
  public void TurnOff()
  {
  }
  public void ResetDimension()
  {
    StopAllCoroutines();
      FiredParticle.Play();
    Spr_A.enabled = true;
    Progress = 0.0f;
    IsActive = true;
    gameObject.layer = LayerMask.NameToLayer("Wall");
  }
}
