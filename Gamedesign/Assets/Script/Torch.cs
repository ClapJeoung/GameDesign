using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.Rendering.Universal;

public class Torch : MonoBehaviour
{
  private CircleCollider2D MyCol = null;  //원형 콜라이더
  [SerializeField] private Transform FireTrans;        //불 트랜스폼
  [SerializeField] private SpriteRenderer FireSpr;     //불 스프라이트랜더러
  private Color FireAlpha = Color.white;  //불 투명도
  [Range(0f, 1f)][SerializeField] private float FirePower = 1.0f; //불 에너지
  [SerializeField] private float RechargeTime  = 1.0f;            //재점화 속도(초)
  [SerializeField] private float ExtinguishTime = 20.0f;          //꺼지는 시간(초)
  private bool IsRecharging = false;                                  //재충전 중인지
  [SerializeField] private Light2D MyLight;                           //라이트
  [SerializeField] private float MaxIntensity = 2.5f; //최대밝기
  [SerializeField] private float MaxSize = 1.2f;      //최대크기
  [SerializeField] private Transform ParticleTransform = null;    //파티클 트랜스폼
  private void Awake()
  {
    Setup();
  }
  public void Setup()
  {
    MyCol = GetComponent<CircleCollider2D>();
  }
  private void OnTriggerEnter2D(Collider2D collision)
  {
    if (collision.CompareTag("Interactable")) collision.GetComponent<Interactable>().FireUp();
    else if (collision.CompareTag("Recharge")) IsRecharging = true;
  }
  private void OnTriggerExit2D(Collider2D collision)
  {
    if (collision.CompareTag("Interactable")) collision.GetComponent<Interactable>().FireDown();
    else if (collision.CompareTag("Recharge")) IsRecharging = false;
  }
  private void Update()
  {
    if (IsRecharging) FirePower += Time.deltaTime / RechargeTime; //충전중이면 쭉 증가
    else FirePower-=Time.deltaTime/ ExtinguishTime;               //그 외라면 쭉 감소
    FirePower = Mathf.Clamp(FirePower, 0.0f, 1.0f);               //FirePower의 범위는 0~1로 제한

    FireTrans.localScale = Vector3.one * Mathf.Sqrt(FirePower);  //FirePower에 비례해 이미지 크기 조절
    FireAlpha.a = Mathf.Sqrt(FirePower);
    FireSpr.color = FireAlpha;                                    //FirePower에 비례해 투명도 조절
    MyLight.intensity = Mathf.Lerp(0.0f, MaxIntensity, FirePower);  //FirePower에 비례해 밝기 조절
    MyLight.pointLightOuterRadius = Mathf.Lerp(0.0f, MaxSize, FirePower); //FirePower에 비례해 크기 조절
    ParticleTransform.localScale=Vector3.one* Mathf.Lerp(0.0f, 1.0f, FirePower); //파티클의 트랜스폼 크기 자체를 조절
  }
}
