using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodenPillar : MonoBehaviour,Interactable
{
  [SerializeField] private float RequireTime = 2.0f; //�ѹ� ���¿�µ� �ʿ��� �ð�
  [SerializeField] private float IgniteTime = 2.0f;  //�� �ٰ� �� Ÿ�µ� �ɸ��� �ð�
  private float Progress = 0.0f;                    //���� ��ô��
  [SerializeField] private SpriteRenderer Spr_A = null; //�Ϲ� ���� �̹���
  private bool IsFired = false;                     //���� �ö���ִ���
  [SerializeField] private ParticleSystem SmokeParticle = null;//���� ���� ��ƼŬ
  [SerializeField] private ParticleSystem BurningParticle = null;//��Ÿ�� ��ƼŬ
  [SerializeField] private ParticleSystem FiredParticle = null; //�� ������ ��ƼŬ
  private Vector3 CurrentTorchPos = Vector2.zero;       //�ǽð����� �ݶ��̴��� ���ִ� ȶ�� ��ġ
  private Vector3 TargetTorchPos = Vector2.zero;        //�� ������ ��ġ
  [SerializeField] private float SmokeTime = 0.2f;      //���� �ö���� �ð�
  private StageCollider MySC = null;
  private bool IsActive = true;
  [SerializeField] private SpinRock MyRock = null;      //�̰� ��Ÿ�� �۵��� ��
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
      SmokeParticle.transform.position = CurrentTorchPos + Vector3.back;
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
    if (GameManager.Instance.CurrentSC != MySC || !IsActive) return;
    //���� ���ӸŴ����� �ִ� �������� �ݶ��̴��� �� �������� �ݶ��̴��� �ٸ��ų� �� ź ���¸� ����

    if ((Progress / RequireTime) < 1.0f)
    {
      if (IsFired) Progress += Time.deltaTime;
      else Progress -= Time.deltaTime; 
      Progress = Mathf.Clamp(Progress, 0.0f, RequireTime);

      if ((Progress / RequireTime) >= 1.0f) //Ignite �Ӱ��� �Ѿ��
      {
        TargetTorchPos = Vector2.Lerp(CurrentTorchPos, transform.position, 0.5f);  //TargetTorchPos�� �� �Ҵ�
        SmokeParticle.Stop();                     //�������� ��ƼŬ ����
        BurningParticle.transform.position = TargetTorchPos + Vector3.back; //��Ÿ�� ��ƼŬ ��ġ ����
        StartCoroutine(getfired());
      }
      return;
    }

   /* if ((Progress / RequireTime) >= IgniteTime)//���� �ٴ� ����
    {
      Progress += Time.deltaTime; //���� �ֵ� ���� ��� ��Ž
      if (SmokeParticle.isPlaying) SmokeParticle.Stop();

      if (Progress >= RequireTime) //��ȭ ��ġ �ִ�
      {
      }
    }   */
  }
  private IEnumerator getfired()
  {
    BurningParticle.Play();
    Progress = 0.0f;
    IsActive = false;
    IsFired = false;
    ParticleSystem.EmissionModule _emission = BurningParticle.emission;
    ParticleSystem.ShapeModule _shape = BurningParticle.shape;
    float _minparticle = 30.0f, _maxparticle = 200.0f, _minsize = 0.1f, _maxsize = 1.0f, _time = 0.0f, _targettime = 2.5f;
    Vector3 _origin = _shape.position;
    _shape.scale = Vector3.one * _minsize;
    float _progress = 0.0f;
    while (_time < IgniteTime)
    {
      _progress = Mathf.Pow(_time / IgniteTime, 2.5f);
      _emission.rateOverTime = Mathf.Lerp(_minparticle, _maxparticle, _progress);
      _shape.scale = Vector3.one * Mathf.Lerp(_minsize,_maxsize,_progress);
      _shape.position = Vector3.Lerp(_origin, Vector3.zero, _progress);
      _time += Time.deltaTime;
      yield return null;
    }
    BurningParticle.Stop();
    FiredParticle.Play();
    Spr_A.enabled = false;
    gameObject.layer = LayerMask.NameToLayer("Default");
    yield return new WaitForSeconds(1.5f);
    MyRock.Active();
  }
  public void Setup()
  {
    MySC = transform.parent.GetComponent<StageCollider>().SetOrigin(this);
  }
  private void Start()
  {
    Setup();
  }
  public void TurnOn()
  {
  }
  public void TurnOff()
  {
  }
  public void ResetDimension()
  {
    StopAllCoroutines();
      FiredParticle.Play();
    Spr_A.enabled = true;
    Progress = 0.0f;
    IsActive = true;
    gameObject.layer = LayerMask.NameToLayer("Wall");
  }
}
