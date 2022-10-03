using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.Rendering.Universal;

public class Torch : MonoBehaviour
{
  private CircleCollider2D MyCol = null;  //���� �ݶ��̴�
  [SerializeField] private Player_Move PlayerScript = null; //�÷��̾� ��ũ��Ʈ
  [SerializeField] private Transform FireTrans;        //�� Ʈ������
  [SerializeField] private SpriteRenderer FireSpr;     //�� ��������Ʈ������
  private Color FireAlpha = Color.white;  //�� ����
  [SerializeField] private float RechargeTime = 1.0f;            //����ȭ �ӵ�(��)
  [SerializeField] private float ExtinguishTime = 20.0f;          //������ �ð�(��)
  private bool IsRecharging = false;                                  //������ ������
  [SerializeField] private Light2D MyLight;                           //����Ʈ
  [SerializeField] private float MaxIntensity = 2.5f; //�ִ���
  [SerializeField] private float MaxSize = 1.2f;      //�ִ�ũ��
  [SerializeField] private Transform ParticleTransform = null;    //��ƼŬ Ʈ������
  [SerializeField] private int IdleParticleCount = 10;
  private ParticleSystem IdleParticle = null;  //�⺻ ��ƼŬ
  private ParticleSystem.EmissionModule IdleParticle_emmision;
  private ParticleSystem ChargedParticle = null;//Ǯ���� ��ƼŬ
  private ParticleSystem SmokeParticle = null; //��� ��ƼŬ
  private float firepower = 1.0f;//�� ������
  public float FirePower
  {
    get { return firepower; }
    set
    {
      if (firepower <= 0) return;
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
    ParticleSystem[] particles = GameManager.Instance.GetPlayerParticles();
    IdleParticle = particles[0];
    IdleParticle_emmision = IdleParticle.emission;
    ChargedParticle= particles[1];
    SmokeParticle = particles[2];
  }
  private void OnTriggerEnter2D(Collider2D collision)
  {
    if(FirePower <= 0.0f) return;

    if (collision.CompareTag("Interactable")) collision.GetComponent<Interactable>().FireUp();
    else if (collision.CompareTag("Recharge")) IsRecharging = true;
    else if (collision.CompareTag("Water"))
    {
      FirePower = 0.0f;
      FireAlpha.a = 0.0f;
      FireSpr.color = FireAlpha;
      MyLight.intensity = 0.0f;
      FireTrans.localScale = Vector3.zero;
    }
  }
  private void OnTriggerExit2D(Collider2D collision)
  {
    if (collision.CompareTag("Interactable")) collision.GetComponent<Interactable>().FireDown();
    else if (collision.CompareTag("Recharge")) IsRecharging = false;
  }
  private void Update()
  {
    if (FirePower <= 0) return;
    if (IsRecharging) FirePower += Time.deltaTime / RechargeTime; //�������̸� �� ����
    else FirePower-=Time.deltaTime/ ExtinguishTime;               //�� �ܶ�� �� ����

    FireTrans.localScale = Vector3.one * Mathf.Sqrt(FirePower);  //FirePower�� ����� �̹��� ũ�� ����
    FireAlpha.a = Mathf.Sqrt(FirePower);
    FireSpr.color = FireAlpha;                                    //FirePower�� ����� ���� ����
    MyLight.intensity = Mathf.Lerp(0.0f, MaxIntensity, FirePower);  //FirePower�� ����� ��� ����
    MyLight.pointLightOuterRadius = Mathf.Lerp(0.0f, MaxSize, FirePower); //FirePower�� ����� ũ�� ����
                                                                          //  ParticleTransform.localScale=Vector3.one* Mathf.Lerp(0.0f, 1.0f, FirePower); //��ƼŬ�� Ʈ������ ũ�� ��ü�� ����
    IdleParticle_emmision.rateOverTime = Mathf.Lerp(0, IdleParticleCount, firepower);
  }
}
