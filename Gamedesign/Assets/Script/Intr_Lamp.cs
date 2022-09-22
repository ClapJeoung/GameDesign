using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intr_Lamp :MonoBehaviour, Interactable   //������ ��ũ��Ʈ
{
  [SerializeField] private float RequireTime = 2.0f;  //��ȭ�� �ɸ��� �ð�
  private float Progress = 0.0f;                      //���� �ٿ��� �ð�
  private SpriteRenderer MySpr = null;                //��������Ʈ������
  private Transform MyTransform = null;               //Ʈ������
  private ParticleSystem MyParticle = null;           //��ƼŬ
  private bool Ignited = false;                       //���� ���� �پ�����
  private bool Ignitable = true;                      //���� �� �پ�����
  private Color CurrentColor = Color.white;           //������ ������ ����
  public void Setup()
  {
    MyTransform = transform.GetChild(0).transform;
    MySpr = MyTransform.GetComponent<SpriteRenderer>();
    MyParticle = transform.GetChild(1).GetComponent<ParticleSystem>();
    CurrentColor.a = 0.0f;
    MySpr.color = CurrentColor;
    MyTransform.localScale = Vector3.one;
    MyParticle.Stop();
  }
  private void Awake()
  {
    Setup();
  }
  public void FireUp()
  {
    if (!Ignitable) return;
    Ignited = true;
    MyParticle.Play();
  }
  public void FireDown()
  {
    if (!Ignitable) return;
    Ignited = false;
    MyParticle.Stop();
  }
  private void Update()
  {
    if (!Ignitable) return;
    Progress += Time.deltaTime * (Ignited ? 1 : -1); //��ȭ������ Progress�� ����, �ƴϸ� -1
    
    if (Progress < 0.0f) { Progress = 0.0f; MyTransform.localScale = Vector3.zero;CurrentColor.a = 0.0f;MySpr.color = CurrentColor; return; }
    //���൵�� �������� �������� ���뿡�� �ø���

    if (Progress > RequireTime) { Progress = RequireTime; Ignitable = false; }  //���൵�� �ִ�ġ�� �����ϸ� ��
      MyTransform.localScale = Vector3.one * Mathf.Pow((Progress / RequireTime),2);  //���൵�� ����� ũ�� ����
    CurrentColor.a = Mathf.Sqrt(Progress / RequireTime);              //���൵�� ��Ʈ �׷��� ������ ���� ����
    MySpr.color = CurrentColor;                                       //���� ����


  }
}
