using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_lamp_event : MonoBehaviour,Interactable
{
  [SerializeField] private float RequireTime = 3.0f;  //점화에 걸리는 시간
  private float Progress = 0.0f;                      //불이 붙여진 시간
  private SpriteRenderer FireSpr = null;                //스프라이트렌더러
  private SpriteRenderer MySpr = null;
  private Transform MyTransform = null;               //트랜스폼(불)
  [SerializeField] private ParticleSystem BasicParticle = null;        //파티클-기본
  [SerializeField] private ParticleSystem FiredParticle = null;        //점화완료 파티클
  private bool Ignited = false;                       //지금 불이 붙었는지
  public bool Ignitable = false;                      //불이 끝까지 다 붙었는지
  private Color CurrentColor = Color.white;           //투명도를 조절할 변수
  private TutorialManager MyManager = null;
  public void Setup()
  {
    MyManager=transform.parent.parent.GetComponent<TutorialManager>();
    MyTransform = transform.GetChild(0).transform;
    FireSpr = MyTransform.GetComponent<SpriteRenderer>();
    MySpr = GetComponent<SpriteRenderer>();
    MySpr.enabled = false;
    CurrentColor.a = 0.0f;
    FireSpr.color = CurrentColor;
    MyTransform.localScale = Vector3.zero;
    BasicParticle.Stop();
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
      FireSpr.enabled = false;
      MyManager.Fired();
      // MyEventTarget.Active(); 
        Ignitable = false;
    }  //진행도가 최대치로 증가하면 끝

    MyTransform.localScale = Vector3.one * Mathf.Pow((Progress / RequireTime), 2);  //진행도에 비례해 크기 증가
    CurrentColor.a = Mathf.Sqrt(Progress / RequireTime);              //진행도의 루트 그래프 비율로 투명도 증가
    FireSpr.color = CurrentColor;                                       //투명도 적용
  }

  public void Active()
  {
    MySpr.enabled = true;
    Ignitable = true;
    FiredParticle.Play();
  }
  public void DeActive()
  {
    FiredParticle.Play();
    MySpr.enabled = false;
    Invoke("asdf", 1.0f);
  }
  private void asdf() => Destroy(gameObject);
}
