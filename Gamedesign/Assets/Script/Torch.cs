using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.Rendering.Universal;

public enum Dimension { A, B }
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
  [SerializeField] private ParticleSystem IdleParticle = null;  //�⺻ ��ƼŬ
  private ParticleSystem.EmissionModule IdleParticle_emmision;
  [SerializeField] private ParticleSystem ChargedParticle = null;//Ǯ���� ��ƼŬ
  private ParticleSystem.ShapeModule ChargedParticle_shape;
  [SerializeField] private ParticleSystem SmokeParticle = null; //��� ��ƼŬ
  private float firepower = 1.0f;//�� ������
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

    MyLight.intensity = 0.0f;             //�׾��ִ� ���·� ����
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
