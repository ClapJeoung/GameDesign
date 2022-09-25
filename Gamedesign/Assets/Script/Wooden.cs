using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Wooden : MonoBehaviour,Interactable
{
  [SerializeField] private float RequireTime = 2.0f; //�� Ÿ�µ� �ʿ��� �ð�
 [Range(0.0f,1.0f)] [SerializeField] private float IgniteTime = 0.5f; //��ȭ�� ����Ǵ� ���� (0~1)
  private float Progress = 0.0f;                    //���� ��ô��
  private bool IsActive = true;                    //Ȱ��ȭ�� ��������
 [SerializeField] private SpriteRenderer MySpr = null;              //�� ��������Ʈ������
  [SerializeField] private Transform FireTransform = null;//ȭ�� �̹��� Ʈ������
  private bool IsFired = false;                     //���� �ö���ִ���
  [SerializeField] private ParticleSystem SmokeParticle = null;//���� ���� ��ƼŬ
  [SerializeField] private ParticleSystem BurningParticle = null;//��Ÿ�� ��ƼŬ
  [SerializeField] private ParticleSystem FiredParticle = null; //�� ������ ��ƼŬ
  private Vector2 CurrentTorchPos = Vector2.zero;       //�ǽð����� �ݶ��̴��� ���ִ� ȶ�� ��ġ
  private Vector2 TargetTorchPos = Vector2.zero;        //�� ������ ��ġ
  [SerializeField] private float SmokeTime = 0.2f;      //���� �ö���� �ð�
  [SerializeField] private Transform MaskTransform = null;//����ũ Ʈ������
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
    if (!IsActive) return;  //!IsActive�� �� ���ٴ� ��

    if ((Progress / RequireTime) < IgniteTime)  //��ȭ ���� ���� ����
    {
     if(IsFired) Progress+=Time.deltaTime;  //���� ������ ���൵ ����
     else Progress-= Time.deltaTime;        //���� ������ ���൵ ����
     Progress=Mathf.Clamp(Progress, 0.0f, RequireTime); //0 �̸����� �������� �ʵ���

      if ((Progress / RequireTime) >= IgniteTime) //Ignite �Ӱ��� �Ѿ��
      {
        TargetTorchPos = Vector2.Lerp(CurrentTorchPos, transform.position, 0.5f);  //TargetTorchPos�� �� �Ҵ�
        FireTransform.gameObject.SetActive(true); //�� �̹��� Ȱ��ȭ
        FireTransform.position = TargetTorchPos;  //�� ��ġ ����
        FireTransform.localScale = Vector3.zero;  //�� ũ�� �ʱⰪ(0) ����
        MaskTransform.position = TargetTorchPos;
        SmokeParticle.Stop();                     //�������� ��ƼŬ ����
        BurningParticle.transform.position = TargetTorchPos; //��Ÿ�� ��ƼŬ ��ġ ����
        BurningParticle.Play();                   //��Ÿ�� ��ƼŬ ����
        MyLight.transform.position = TargetTorchPos;
      }

     return;
    }
    
    if ((Progress / RequireTime) >= IgniteTime)//��ȭ ���� ���� ����
    {
      Progress += Time.deltaTime; //���� �ֵ� ���� ��� ��Ž

      FireTransform.localScale = Vector3.one * Mathf.Lerp(0, FireSize, ((Progress / RequireTime) - IgniteTime)/(1-IgniteTime));
      MyLight.pointLightOuterRadius=MaxLightOuter* ((Progress / RequireTime) - IgniteTime) / (1 - IgniteTime);
      //Ignitetime ~ 1�� 0 ~ 1�� ��ȯ��Ű�� ũ�⿡ ����

      if (Progress >= RequireTime) //��ȭ ��ġ �ִ�
      {
        BurningParticle.Stop();
        gameObject.layer = 0; //���ӿ�����Ʈ ���̾� ����
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
      MaskTransform.localScale = Vector3.one * Mathf.Lerp(0, FireSize, _time / _firetime); //������ �ұ��� �� �¿�����
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
    this.enabled = false;  //�ڽ� ��ü ���� ��Ȱ��ȭ�ϰ� ��ũ��Ʈ�� ��Ȱ��ȭ
  }
  public void Setup()
  {
  }
  private void Awake()
  {
    Setup();
  }
}
