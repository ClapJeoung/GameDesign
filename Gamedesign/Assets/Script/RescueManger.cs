using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RescueManger : MonoBehaviour
{
  [SerializeField] private MainCamera MyCamera = null;
  [SerializeField] private Transform Outline = null;          //선
  [SerializeField] private Transform Inside = null;           //내용물
  private float CurrentPRG = 0.0f;  //현재 진행도
  private float TargetPRG = 0.0f;   //목표 진행도
  private float Min = 1.2f, Max = 1.9f; //최소~최대
  private float Range = 0.2f;       //오차범위
  [SerializeField] private float Speed = 1.0f; //진행도 증가 속도
  private float Offset = 0.2f;      //진행도랑 오브젝트 크기 오차범위
  [SerializeField] private ParticleSystem Success = null;
  [SerializeField] private ParticleSystem Fail = null;
  [SerializeField] private Torch MyTorch = null;[SerializeField] private Player_Move MyPlayer = null;
  private bool IsPlaying = true;
  private int RescueStack = 0;
  [SerializeField] private ParticleSystem PlayerFired = null;
  [SerializeField] private SpriteRenderer SpaceSpr = null;
  private Color Space_Idle = Color.white;
  [SerializeField] private Color Space_Pressed = Color.white;

  private void Start()
  {
    IsPlaying = false;
    enabled = false;
  }
  private void Update()
  {
    if (!IsPlaying) return;
    if (Input.GetKeyDown(KeyCode.Space)) { Pressed(); SpaceSpr.color = Space_Pressed; }
    if (Input.GetKeyUp(KeyCode.Space)) SpaceSpr.color = Space_Idle ;
  }
  public void Pressed()
  {
    StopAllCoroutines();
    StartCoroutine(check());
  }
  private IEnumerator check()
  {
    AudioManager.Instance.PlayClip(0);
    if (RescueStack == 2)
    {
      MyTorch.Reborn();
      MyCamera.CloseRescue();
      PlayerFired.Play();
      MyPlayer.transform.localScale = Vector3.zero;
      Speed *= 2.0f;
      StartCoroutine(destroyfire());
      yield return new WaitForSeconds(0.5f);
      GameManager.Instance.Dead_body(); //현재 차원 및 오브젝트 초기화
      GameManager.Instance.PlayRPParticle();  //화면 회전하는 파티클
      GameManager.Instance.Respawn();                   //리스폰
      GameManager.Instance.RespawnHindi();
      IsPlaying = false;
      enabled = false;
      yield break;
    }
    if (CurrentPRG >= (TargetPRG - Range) && CurrentPRG <= (TargetPRG + Range))
    {
      RescueStack++;
      yield return new WaitForSeconds(0.25f);
      ParticleSystem.ShapeModule _shape = Success.shape;
      _shape.scale = Vector3.one * CurrentPRG;
      Success.Play();
      StartCoroutine(startrescue());
    }
    else
    {
      IsPlaying = false;
      ParticleSystem.ShapeModule _shape = Fail.shape;
      _shape.scale = Vector3.one * CurrentPRG;
      Fail.Play();
      MyCamera.CloseRescue();
      MyPlayer.Dead_soul_1();
      StartCoroutine(destroyfire());
      enabled = false;
      yield break;
    }
  }
  public void OpenRescue()
  {
    MyCamera.OpenRescue();
    RescueStack = 0;
    SpaceSpr.enabled = true;
    StartCoroutine(startrescue());
  }
  private IEnumerator startrescue()
  {
    IsPlaying = true;
    TargetPRG = Random.Range(Min, Max); //범위 정하고
    Outline.localScale = Vector3.one * TargetPRG;         //범위 크기 설정
    Outline.GetComponent<SpriteRenderer>().enabled = true;
    Inside.localScale = Vector3.zero;                     //내부 크기는 0으로
    Inside.GetComponent<SpriteRenderer>().enabled = true;
    CurrentPRG = 0.0f;
    while (CurrentPRG < TargetPRG + 1.0f) //범위 넘어갈때까지
    {
      Debug.Log($"Current : {CurrentPRG}   Target : {TargetPRG}");
      CurrentPRG += Time.deltaTime * Speed;               //계속 증가
      Inside.localScale = Vector3.one * CurrentPRG;        //내부 크기 업데이트
      yield return null;
    }
    IsPlaying = false;
    ParticleSystem.ShapeModule _shape = Fail.shape;
    _shape.scale = Vector3.one * CurrentPRG;
    Fail.Play();
    MyPlayer.Dead_soul_1();
    MyCamera.CloseRescue();
    StartCoroutine(destroyfire());
  }
  private IEnumerator destroyfire()
  {
    float _time = 0.0f, _targettime = 0.3f;
    SpriteRenderer _outline = Outline.GetComponent<SpriteRenderer>();
    SpriteRenderer _inside = Inside.GetComponent<SpriteRenderer>();
    Color _outcol = _outline.color;
    Color _incol = _inside.color;
    Color _spacecol = Color.white;
    while (_time < _targettime)
    {
      _outcol.a = 1 - (_time / _targettime);
      _incol.a = 1 - (_time / _targettime);
      _spacecol.a = 1 - (_time / _targettime);
      _outline.color = _outcol;
      _inside.color = _incol;
      SpaceSpr.color = _spacecol;
      _time += Time.deltaTime;
      yield return null;
    }
    _outline.enabled = false;
    _inside.enabled = false;
    SpaceSpr.enabled = false;
    _outcol.a = 1.0f;
    _incol.a = 1.0f;
    _spacecol.a = 1.0f;
    _outline.color = _outcol;
    _inside.color = _incol;
    SpaceSpr.color = _spacecol;
  }
}
