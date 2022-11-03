using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Tutorial_Wooden : MonoBehaviour, Interactable
{
  [SerializeField] private float RequireTime = 2.0f; //�� Ÿ�µ� �ʿ��� �ð�
  [Range(0.0f, 1.0f)][SerializeField] private float IgniteTime = 0.5f; //��ȭ�� ����Ǵ� ���� (0~1)
  private float Progress = 0.0f;                    //���� ��ô��
  [SerializeField] private SpriteRenderer Spr_A = null; //�Ϲ� ���� �̹���
  [SerializeField] private SpriteRenderer Spr_B = null; //�ٸ� ���� �̹���
  [SerializeField] private SpriteRenderer Spr_C = null; //��� ���� �̹���
  [SerializeField] private Transform FireTransform = null;//ȭ�� �̹��� Ʈ������
  private bool IsFired = false;                     //���� �ö���ִ���
  [SerializeField] private ParticleSystem SmokeParticle = null;//���� ���� ��ƼŬ
  [SerializeField] private ParticleSystem BurningParticle = null;//��Ÿ�� ��ƼŬ
  [SerializeField] private ParticleSystem FiredParticle = null; //�� ������ ��ƼŬ
  [SerializeField] private float SmokeTime = 0.2f;      //���� �ö���� �ð�
  private Dimension MyDimension=Dimension.A;
  private TutorialManager MyManager = null;
  private bool isactive = true;
  public bool IsActive
  {
    get
    {
      if (MyManager.TutorialDimension == Dimension.A) //���尡 A �����϶�
      {
        if (MyDimension == Dimension.A) return true; //���� A�� Ȱ��ȭ

        return false; //�ƴϸ� ��Ȱ��ȭ
      }
      else if (MyManager.TutorialDimension == Dimension.B)  //���尡 B �����϶�
      {
        if (MyDimension == Dimension.B) return true; //���� B�� Ȱ��ȭ

        return false; //�ƴϸ� ��Ȱ��ȭ
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
    //���� ���ӸŴ����� �ִ� �������� �ݶ��̴��� �� �������� �ݶ��̴��� �ٸ��ų� �� ź ���¸� ����

    if ((Progress / RequireTime) < IgniteTime)  //���Ⱑ ���� ����
    {
      if (IsFired) Progress += Time.deltaTime;  //���� ������ ���൵ ����
      else Progress -= Time.deltaTime;        //���� ������ ���൵ ����
      Progress = Mathf.Clamp(Progress, 0.0f, RequireTime); //0 �̸����� �������� �ʵ���

      if ((Progress / RequireTime) >= IgniteTime) //Ignite �Ӱ��� �Ѿ��
      {
        SmokeParticle.Stop();                     //�������� ��ƼŬ ����
        BurningParticle.Play();                   //��Ÿ�� ��ƼŬ ����
      }
      return;
    }

    if ((Progress / RequireTime) >= IgniteTime)//���� �ٴ� ����
    {
      Progress += Time.deltaTime; //���� �ֵ� ���� ��� ��Ž
      if (SmokeParticle.isPlaying) SmokeParticle.Stop();
      //  FireTransform.localScale = Vector3.one * Mathf.Lerp(0, FireSize, ((Progress / RequireTime) - IgniteTime)/(1-IgniteTime));
      //Ignitetime ~ 1�� 0 ~ 1�� ��ȯ��Ű�� ũ�⿡ ����

      if (Progress >= RequireTime) //��ȭ ��ġ �ִ�
      {
        BurningParticle.Stop();
        FiredParticle.Play();
        if (MyDimension == Dimension.A) { Spr_A.enabled = false; Spr_B.enabled = true; MyDimension = Dimension.B; }
        else if (MyDimension == Dimension.B) { Spr_B.enabled = false; Spr_A.enabled = true; MyDimension = Dimension.A; }
        Progress = 0.0f;
        IsFired = false;
        MyManager.Fired();  //ȭ����������
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
