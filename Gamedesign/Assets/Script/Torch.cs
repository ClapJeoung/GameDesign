using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.Rendering.Universal;

public class Torch : MonoBehaviour
{
  private CircleCollider2D MyCol = null;  //���� �ݶ��̴�
  [SerializeField] private Transform FireTrans;        //�� Ʈ������
  [SerializeField] private SpriteRenderer FireSpr;     //�� ��������Ʈ������
  private Color FireAlpha = Color.white;  //�� ����
  [Range(0f, 1f)][SerializeField] private float FirePower = 1.0f; //�� ������
  [SerializeField] private float RechargeTime  = 1.0f;            //����ȭ �ӵ�(��)
  [SerializeField] private float ExtinguishTime = 20.0f;          //������ �ð�(��)
  private bool IsRecharging = false;                                  //������ ������
  [SerializeField] private Light2D MyLight;                           //����Ʈ
  [SerializeField] private float MaxIntensity = 2.5f; //�ִ���
  [SerializeField] private float MaxSize = 1.2f;      //�ִ�ũ��
  [SerializeField] private Transform ParticleTransform = null;    //��ƼŬ Ʈ������
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
    if (IsRecharging) FirePower += Time.deltaTime / RechargeTime; //�������̸� �� ����
    else FirePower-=Time.deltaTime/ ExtinguishTime;               //�� �ܶ�� �� ����
    FirePower = Mathf.Clamp(FirePower, 0.0f, 1.0f);               //FirePower�� ������ 0~1�� ����

    FireTrans.localScale = Vector3.one * Mathf.Sqrt(FirePower);  //FirePower�� ����� �̹��� ũ�� ����
    FireAlpha.a = Mathf.Sqrt(FirePower);
    FireSpr.color = FireAlpha;                                    //FirePower�� ����� ���� ����
    MyLight.intensity = Mathf.Lerp(0.0f, MaxIntensity, FirePower);  //FirePower�� ����� ��� ����
    MyLight.pointLightOuterRadius = Mathf.Lerp(0.0f, MaxSize, FirePower); //FirePower�� ����� ũ�� ����
    ParticleTransform.localScale=Vector3.one* Mathf.Lerp(0.0f, 1.0f, FirePower); //��ƼŬ�� Ʈ������ ũ�� ��ü�� ����
  }
}
