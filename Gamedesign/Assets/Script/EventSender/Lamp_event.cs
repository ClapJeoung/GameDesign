using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Lamp_event : MonoBehaviour, Interactable,Lightobj
{
  [SerializeField] private float RequireTime = 3.0f;  //��ȭ�� �ɸ��� �ð�
  private float Progress = 0.0f;                      //���� �ٿ��� �ð�
  private SpriteRenderer FireSpr = null;                //��������Ʈ������
  private SpriteRenderer MySpr = null;
  [SerializeField] private Sprite Spr_event = null;   //Ʈ���� ���� �̹���
  [SerializeField] private Sprite Spr_world = null;   //���ǰ� ���� �̹���
  [SerializeField] private Sprite Spr_soul = null;    //��ȥ�� ���� �̹���
  private Transform MyTransform = null;               //Ʈ������(��)
  [SerializeField] private ParticleSystem BasicParticle = null;        //��ƼŬ-�⺻
  [SerializeField] private ParticleSystem FiredParticle = null;        //��ȭ�Ϸ� ��ƼŬ
  private bool Ignited = false;                       //���� ���� �پ�����
  public bool Ignitable = true;                      //���� ������ �� �پ�����
  private Color CurrentColor = Color.white;           //������ ������ ����
  [SerializeField] private Light2D MyLight;           //����
  [SerializeField] private float MaxIntensity = 3.0f;
  [SerializeField] private EventTarget[] MyActiveTargets = new EventTarget[0];
  [SerializeField] private EventTarget[] MyDeactiveTargets = new EventTarget[0];
  private enum LampType {Dimension,Event };
  [SerializeField] private LampType MyLampType;
  public void TurnOn()
  {
    MyLight.enabled = true;
  }
  public void TurnOff()
  {
    MyLight.enabled = false;
    //  StartCoroutine(turnoff());
  }
  private IEnumerator turnoff()
  {
    float _time = 0.0f;
    float _origin = MyLight.intensity;
    if(_origin!=0.0f)
    while (_time < 1.0f)
    {
        MyLight.intensity = _origin * (_time / 1.0f);
        _time += Time.deltaTime;
        yield return null;
    }
    MyLight.enabled = false;
  }
  public void Setup()
  {
    MyTransform = transform.GetChild(0).transform;
    FireSpr = MyTransform.GetComponent<SpriteRenderer>();
    MySpr = GetComponent<SpriteRenderer>();
    if (MyLampType == LampType.Dimension) { MySpr.sprite = Spr_world; CurrentColor = Color.white;
      ParticleSystem.MainModule _main = BasicParticle.main;
      _main.startColor = Color.blue;
      _main=FiredParticle.main;
      _main.startColor = Color.blue;
    }
    else { MySpr.sprite = Spr_event; CurrentColor = Color.red; }
    CurrentColor.a = 0.0f;
    FireSpr.color = CurrentColor;
    MyTransform.localScale = Vector3.zero;
    BasicParticle.Stop();
    MyLight.intensity = 0.0f;
    transform.parent.GetComponent<StageCollider>().SetOrigin(this);
    GameManager.Instance.AllLights.Add(this);
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

    if (Progress < 0.0f) { Progress = 0.0f; MyTransform.localScale = Vector3.zero; CurrentColor.a = 0.0f; FireSpr.color = CurrentColor; return; }
    //���൵�� �������� �������� ���뿡�� �ø���

    if (Progress > RequireTime)
    {
      Progress = RequireTime; FiredParticle.Play();
      // MyEventTarget.Active(); 
      if (MyLampType == LampType.Dimension)
      {
        if (GameManager.Instance.CurrentSC.CurrentDimension == Dimension.A)
        { GameManager.Instance.OpenMask(); MySpr.sprite = Spr_soul; }
        else { GameManager.Instance.CloseMask();MySpr.sprite = Spr_world; }
        Progress = 0.0f;
        Ignited = false;
      }
      else
      {
        foreach (var target in MyActiveTargets) if (target != null) target.Active();
        foreach (var target in MyDeactiveTargets) if (target != null) target.Deactive();
        Ignitable = false;
      }
      MyTransform.localScale = Vector3.zero;
    }  //���൵�� �ִ�ġ�� �����ϸ� ��

    MyTransform.localScale = Vector3.one * Mathf.Pow((Progress / RequireTime), 2);  //���൵�� ����� ũ�� ����
    CurrentColor.a = Mathf.Sqrt(Progress / RequireTime);              //���൵�� ��Ʈ �׷��� ������ ���� ����
    FireSpr.color = CurrentColor;                                       //���� ����
    MyLight.intensity = Mathf.Sqrt(Progress / RequireTime)* MaxIntensity;           //��� ����
  }

  private void OnDrawGizmos()
  {
    if (MyActiveTargets.Length > 0)
    {
      Gizmos.color = Color.green;
      for (int i = 0; i < MyActiveTargets.Length; i++)
       if(MyActiveTargets[i]!=null) Gizmos.DrawLine(transform.position, MyActiveTargets[i].transform.position);
    }
    if(MyDeactiveTargets.Length > 0)
    {
      Gizmos.color = Color.red;
      for (int i = 0; i < MyDeactiveTargets.Length; i++)
        if (MyDeactiveTargets[i] != null) Gizmos.DrawLine(transform.position, MyDeactiveTargets[i].transform.position);
    }
  }
  public void ResetLamp()
  {
    Ignitable = true;
    Progress = 0.0f;
    foreach (var target in MyActiveTargets)if(target!=null) target.Deactive();
    foreach (var target in MyDeactiveTargets) if (target != null) target.Active();
  }
}
