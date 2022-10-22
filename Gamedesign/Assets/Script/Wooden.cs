using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Wooden : MonoBehaviour,Interactable
{
  [SerializeField] private float RequireTime = 2.0f; //�� Ÿ�µ� �ʿ��� �ð�
 [Range(0.0f,1.0f)] [SerializeField] private float IgniteTime = 0.5f; //��ȭ�� ����Ǵ� ���� (0~1)
  private float Progress = 0.0f;                    //���� ��ô��
  [SerializeField] private SpriteRenderer Spr_A = null; //�Ϲ� ���� �̹���
  [SerializeField] private SpriteRenderer Spr_B = null; //�ٸ� ���� �̹���
  [SerializeField] private SpriteRenderer Spr_C = null; //��� ���� �̹���
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
  [SerializeField] private Dimension MyDimension;
  private StageCollider MySC = null;
  private bool isactive = true;
  private Dimension OriginDimension;
  public bool IsActive
  {
    get { if (GameManager.Instance.CurrentSC.CurrentDimension == Dimension.A) //���尡 A �����϶�
      {
        if (MyDimension == Dimension.A ) return true; //���� A�� Ȱ��ȭ

        return false; //�ƴϸ� ��Ȱ��ȭ
      }
    else if (GameManager.Instance.CurrentSC.CurrentDimension == Dimension.B)  //���尡 B �����϶�
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
      SmokeParticle.transform.position = CurrentTorchPos;
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
    if (GameManager.Instance.CurrentSC!=MySC||!IsActive) return;
    //���� ���ӸŴ����� �ִ� �������� �ݶ��̴��� �� �������� �ݶ��̴��� �ٸ��ų� �� ź ���¸� ����

    if ((Progress / RequireTime) < IgniteTime)  //���Ⱑ ���� ����
    {
     if(IsFired) Progress+=Time.deltaTime;  //���� ������ ���൵ ����
     else Progress-= Time.deltaTime;        //���� ������ ���൵ ����
     Progress=Mathf.Clamp(Progress, 0.0f, RequireTime); //0 �̸����� �������� �ʵ���

      if ((Progress / RequireTime) >= IgniteTime) //Ignite �Ӱ��� �Ѿ��
      {
        TargetTorchPos = Vector2.Lerp(CurrentTorchPos, transform.position, 0.5f);  //TargetTorchPos�� �� �Ҵ�
     //   FireTransform.gameObject.SetActive(true); //�� �̹��� Ȱ��ȭ
     //   FireTransform.position = TargetTorchPos;  //�� ��ġ ����
     //   FireTransform.localScale = Vector3.zero;  //�� ũ�� �ʱⰪ(0) ����
     //   MaskTransform.position = TargetTorchPos;
        SmokeParticle.Stop();                     //�������� ��ƼŬ ����
        BurningParticle.transform.position = TargetTorchPos; //��Ÿ�� ��ƼŬ ��ġ ����
        BurningParticle.Play();                   //��Ÿ�� ��ƼŬ ����
        MyLight.transform.position = TargetTorchPos;
      }

     return;
    }
    
    if ((Progress / RequireTime) >= IgniteTime)//���� �ٴ� ����
    {
      Progress += Time.deltaTime; //���� �ֵ� ���� ��� ��Ž

    //  FireTransform.localScale = Vector3.one * Mathf.Lerp(0, FireSize, ((Progress / RequireTime) - IgniteTime)/(1-IgniteTime));
      MyLight.pointLightOuterRadius=MaxLightOuter* ((Progress / RequireTime) - IgniteTime) / (1 - IgniteTime);
      //Ignitetime ~ 1�� 0 ~ 1�� ��ȯ��Ű�� ũ�⿡ ����

      if (Progress >= RequireTime) //��ȭ ��ġ �ִ�
      {
        BurningParticle.Stop();
        if (MyDimension == Dimension.A) { Spr_A.enabled = false; Spr_B.enabled = true; MyDimension = Dimension.B; }
        else if (MyDimension == Dimension.B) { Spr_B.enabled = false; Spr_A.enabled = true; MyDimension = Dimension.A; }
        Progress = 0.0f;
        IsFired = false;
        StartCoroutine(burningcoroutine());
      }
    }
  }
  private IEnumerator burningcoroutine()
  {
    float _time = 0.0f;
    float _firetime = 0.1f;
    FiredParticle.Play();
  /*  while (_time < _firetime)
    {
      _time += Time.deltaTime;
      MaskTransform.localScale = Vector3.one * Mathf.Lerp(0, FireSize, _time / _firetime); //������ �ұ��� �� �¿�����
      yield return null;
    }
    _time = 0.0f; */
    while (_time < _firetime)
    {
      _time += Time.deltaTime;
      MyLight.pointLightOuterRadius = Mathf.Lerp(MaxLightOuter, 0.0f, _time / _firetime);
      yield return null;
    }

  }
  public void Setup()
  {
    MySC = transform.parent.GetComponent<StageCollider>();
    MySC.SetOrigin(this);
    Spr_B.size = Spr_A.size;
    Spr_C.size = Spr_B.size;
    GetComponent<BoxCollider2D>().size= Spr_A.size;
    ParticleSystem.ShapeModule myshape = FiredParticle.shape;
    myshape.scale =new Vector3( Spr_A.size.x,0.3f,1.0f);
    if (MyDimension == Dimension.A) Spr_B.enabled = false;
    else if(MyDimension== Dimension.B) Spr_A.enabled = false;
    OriginDimension = MyDimension;
  }
  private void Start()
  {
    Setup();
  }
  public void ResetDimension()
  {
   if( MyDimension != OriginDimension)
    {
      MyDimension = OriginDimension;
      FiredParticle.Play();
      if (MyDimension == Dimension.B) { Spr_A.enabled = false; Spr_B.enabled = true; }
      else if (MyDimension == Dimension.A) { Spr_B.enabled = false; Spr_A.enabled = true;  }
    }

  }
}
