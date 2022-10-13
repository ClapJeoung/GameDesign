using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.Rendering.Universal;

public enum Dimension { A, B }
public class Torch : MonoBehaviour
{
  private CircleCollider2D MyCol = null;  //원형 콜라이더
  [SerializeField] private Player_Move PlayerScript = null; //플레이어 스크립트
  [SerializeField] private Transform FireTrans;        //불 트랜스폼
  [SerializeField] private SpriteRenderer FireSpr;     //불 스프라이트랜더러
  private Color FireAlpha = Color.white;  //불 투명도
  [SerializeField] private float RechargeTime = 1.0f;            //재점화 속도(초)
  [SerializeField] private float ExtinguishTime = 20.0f;          //꺼지는 시간(초)
  private bool IsRecharging = false;                                  //재충전 중인지
  [SerializeField] private Light2D MyLight;                           //라이트
  [SerializeField] private float MaxIntensity = 2.5f; //최대밝기
  [SerializeField] private float MaxSize = 1.2f;      //최대크기
  [SerializeField] private Transform ParticleTransform = null;    //파티클 트랜스폼
  [SerializeField] private int IdleParticleCount = 10;
  [SerializeField] private ParticleSystem IdleParticle = null;  //기본 파티클
  private ParticleSystem.EmissionModule IdleParticle_emmision;
  [SerializeField] private ParticleSystem ChargedParticle = null;//풀차지 파티클
  private ParticleSystem.ShapeModule ChargedParticle_shape;
  [SerializeField] private ParticleSystem SmokeParticle = null; //사망 파티클
  private float firepower = 1.0f;//불 에너지
  private Interactable CurrentInteract = null;
  public float FirePower
  {
    get { return firepower; }
    set
    {
      firepower = value;
      firepower = Mathf.Clamp(FirePower, 0.0f, 1.0f);
      if (firepower <= 0.0f) { IdleParticle.Stop(); SmokeParticle.Play(); PlayerScript.Dead(); }
      else if (value >= 1.0f && firepower < 1.0f) ChargedParticle.Play();
    }
  }
  private void Start()
  {
    Setup();
  }
  public void Setup()
  {
    MyCol = GetComponent<CircleCollider2D>();
    IdleParticle_emmision = IdleParticle.emission;
    ChargedParticle_shape=ChargedParticle.shape;

    MyLight.intensity = 0.0f;             //죽어있는 상태로 시작
    FireTrans.localScale = Vector3.zero;
    firepower = 0.0f;
  }
  private void OnTriggerEnter2D(Collider2D collision)
  {
    if(FirePower <= 0.0f) return;

    if (collision.CompareTag("Interactable")) {CurrentInteract= collision.GetComponent<Interactable>();CurrentInteract.FireUp(); }
    else if (collision.CompareTag("Recharge")) IsRecharging = true;
    else if (collision.CompareTag("Water"))
    {
      if (FirePower > 0.0f)
      {
        FirePower = 0.0f;
        //  FireAlpha.a = 0.0f;
        //  FireSpr.color = FireAlpha;
        MyLight.intensity = 0.0f;
        FireTrans.localScale = Vector3.zero;
      }
    }
  }
  private void OnTriggerExit2D(Collider2D collision)
  {
    if (collision.CompareTag("Interactable")) { if (CurrentInteract != null) { CurrentInteract.FireDown(); CurrentInteract = null; } }
    else if (collision.CompareTag("Recharge")) IsRecharging = false;
  }
  private void Update()
  {
    return;
    if (FirePower <= 0) return;
    if (IsRecharging) FirePower += Time.deltaTime / RechargeTime; //충전중이면 쭉 증가
    else FirePower-=Time.deltaTime/ ExtinguishTime;               //그 외라면 쭉 감소

    FireTrans.localScale = Vector3.one * Mathf.Sqrt(FirePower);  //FirePower에 비례해 이미지 크기 조절
    FireAlpha.a = Mathf.Sqrt(FirePower);
    FireSpr.color = FireAlpha;                                    //FirePower에 비례해 투명도 조절
    MyLight.intensity = Mathf.Lerp(0.0f, MaxIntensity, FirePower);  //FirePower에 비례해 밝기 조절
    MyLight.pointLightOuterRadius = Mathf.Lerp(0.0f, MaxSize, FirePower); //FirePower에 비례해 크기 조절
                                                                          //  ParticleTransform.localScale=Vector3.one* Mathf.Lerp(0.0f, 1.0f, FirePower); //파티클의 트랜스폼 크기 자체를 조절
    IdleParticle_emmision.rateOverTime = Mathf.Lerp(0, IdleParticleCount, firepower);
  }
  public void Dead()
  {
    if (CurrentInteract != null) { CurrentInteract.FireDown(); CurrentInteract = null; }
  }
  public void Ignite()
  {
    StartCoroutine(ignite());
  }
  private IEnumerator ignite()
  {
    FireTrans.localScale = Vector3.one;
    ChargedParticle_shape.position = FireTrans.position;
    ChargedParticle.Play();
    float _time = 0.0f;
    while (_time < 0.1f)
    {
      MyLight.intensity = Mathf.Lerp(0.0f, MaxIntensity, _time / 0.1f);
      _time += Time.deltaTime;
      yield return null;
    }
    FirePower = 1.0f;
    IdleParticle.Play();
    MyLight.intensity = MaxIntensity;
  }
}
