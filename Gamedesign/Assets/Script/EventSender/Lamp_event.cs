using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Lamp_event : MonoBehaviour, Interactable
{
  [SerializeField] private float RequireTime = 3.0f;  //��ȭ�� �ɸ��� �ð�
  private float Progress = 0.0f;                      //���� �ٿ��� �ð�
  private SpriteRenderer MySpr = null;                //��������Ʈ������
  private Transform MyTransform = null;               //Ʈ������
  [SerializeField] private ParticleSystem BasicParticle = null;        //��ƼŬ-�⺻
  [SerializeField] private ParticleSystem FiredParticle = null;        //��ȭ�Ϸ� ��ƼŬ
  private bool Ignited = false;                       //���� ���� �پ�����
  public bool Ignitable = true;                      //���� ������ �� �پ�����
  private Color CurrentColor = Color.white;           //������ ������ ����
  [SerializeField] private Light2D MyLight;           //����
  [SerializeField] private EventTarget MyEventTarget=null;
  public void Setup()
  {
    MyTransform = transform.GetChild(0).transform;
    MySpr = MyTransform.GetComponent<SpriteRenderer>();
    CurrentColor.a = 0.0f;
    MySpr.color = CurrentColor;
    MyTransform.localScale = Vector3.zero;
    BasicParticle.Stop();
    MyLight.intensity = 0.0f;
  }
  private void Start()
  {
    Setup();
  }
  public void FireUp()
  {
    if (!Ignitable) return;
    Ignited = true;
    BasicParticle.Play();
  }
  public void FireDown()
  {
    if (!Ignitable) return;
    Ignited = false;
    BasicParticle.Stop();
  }
  private void Update()
  {
    if (!Ignitable) return;
    Progress += Time.deltaTime * (Ignited ? 1 : -1); //��ȭ������ Progress�� ����, �ƴϸ� -1

    if (Progress < 0.0f) { Progress = 0.0f; MyTransform.localScale = Vector3.zero; CurrentColor.a = 0.0f; MySpr.color = CurrentColor; return; }
    //���൵�� �������� �������� ���뿡�� �ø���

    if (Progress > RequireTime)
    { Progress = RequireTime; FiredParticle.Play(); Ignitable = false; MyEventTarget.Active(); }  //���൵�� �ִ�ġ�� �����ϸ� ��

    MyTransform.localScale = Vector3.one * Mathf.Pow((Progress / RequireTime), 2);  //���൵�� ����� ũ�� ����
    CurrentColor.a = Mathf.Sqrt(Progress / RequireTime);              //���൵�� ��Ʈ �׷��� ������ ���� ����
    MySpr.color = CurrentColor;                                       //���� ����
    MyLight.intensity = Mathf.Sqrt(Progress / RequireTime);           //��� ����
  }

  private void OnDrawGizmos()
  {
    if (MyEventTarget == null) return;

    Gizmos.color = Color.red;
    Gizmos.DrawLine(transform.position, MyEventTarget.transform.position);
  }
}
