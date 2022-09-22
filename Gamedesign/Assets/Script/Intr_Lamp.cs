using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intr_Lamp :MonoBehaviour, Interactable   //램프의 스크립트
{
  [SerializeField] private float RequireTime = 2.0f;  //점화에 걸리는 시간
  private float Progress = 0.0f;                      //불이 붙여진 시간
  private SpriteRenderer MySpr = null;                //스프라이트렌더러
  private Transform MyTransform = null;               //트랜스폼
  private ParticleSystem MyParticle = null;           //파티클
  private bool Ignited = false;                       //지금 불이 붙었는지
  private bool Ignitable = true;                      //불이 다 붙었는지
  private Color CurrentColor = Color.white;           //투명도를 조절할 변수
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
    Progress += Time.deltaTime * (Ignited ? 1 : -1); //점화됐으면 Progress가 증가, 아니면 -1
    
    if (Progress < 0.0f) { Progress = 0.0f; MyTransform.localScale = Vector3.zero;CurrentColor.a = 0.0f;MySpr.color = CurrentColor; return; }
    //진행도가 음수까지 떨어지면 이쯤에서 시마이

    if (Progress > RequireTime) { Progress = RequireTime; Ignitable = false; }  //진행도가 최대치로 증가하면 끝
      MyTransform.localScale = Vector3.one * Mathf.Pow((Progress / RequireTime),2);  //진행도에 비례해 크기 증가
    CurrentColor.a = Mathf.Sqrt(Progress / RequireTime);              //진행도의 루트 그래프 비율로 투명도 증가
    MySpr.color = CurrentColor;                                       //투명도 적용


  }
}
