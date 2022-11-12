using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Lamp_event : MonoBehaviour, Interactable,Lightobj
{
  [SerializeField] private float RequireTime = 3.0f;  //점화에 걸리는 시간
  private float Progress = 0.0f;                      //불이 붙여진 시간
  private SpriteRenderer FireSpr = null;                //스프라이트렌더러
  private SpriteRenderer MySpr = null;
  [SerializeField] private Sprite Spr_event = null;   //트리거 램프 이미지
  [SerializeField] private Sprite Spr_world = null;   //현실계 램프 이미지
  [SerializeField] private Sprite Spr_soul = null;    //영혼계 램프 이미지
  private Transform MyTransform = null;               //트랜스폼(불)
  [SerializeField] private ParticleSystem BasicParticle = null;        //파티클-기본
  [SerializeField] private ParticleSystem FiredParticle = null;        //점화완료 파티클
  private bool Ignited = false;                       //지금 불이 붙었는지
  public bool Ignitable = true;                      //불이 끝까지 다 붙었는지
  private Color CurrentColor = Color.white;           //투명도를 조절할 변수
  [SerializeField] private Light2D MyLight;           //조명
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
    Progress += Time.deltaTime * (Ignited ? 1 : -1); //점화됐으면 Progress가 증가, 아니면 -1

    if (Progress < 0.0f) { Progress = 0.0f; MyTransform.localScale = Vector3.zero; CurrentColor.a = 0.0f; FireSpr.color = CurrentColor; return; }
    //진행도가 음수까지 떨어지면 이쯤에서 시마이

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
    }  //진행도가 최대치로 증가하면 끝

    MyTransform.localScale = Vector3.one * Mathf.Pow((Progress / RequireTime), 2);  //진행도에 비례해 크기 증가
    CurrentColor.a = Mathf.Sqrt(Progress / RequireTime);              //진행도의 루트 그래프 비율로 투명도 증가
    FireSpr.color = CurrentColor;                                       //투명도 적용
    MyLight.intensity = Mathf.Sqrt(Progress / RequireTime)* MaxIntensity;           //밝기 적용
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
