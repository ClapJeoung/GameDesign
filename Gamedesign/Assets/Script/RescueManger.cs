using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RescueManger : MonoBehaviour
{
  [SerializeField] private MainCamera MyCamera = null;
  [SerializeField] private Transform Outline = null;          //��
  [SerializeField] private Transform Inside = null;           //���빰
  private float CurrentPRG = 0.0f;  //���� ���൵
  private float TargetPRG = 0.0f;   //��ǥ ���൵
  private float Min = 1.2f, Max = 1.9f; //�ּ�~�ִ�
  private float Range = 0.2f;       //��������
  [SerializeField] private float Speed = 1.0f; //���൵ ���� �ӵ�
  private float Offset = 0.2f;      //���൵�� ������Ʈ ũ�� ��������
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
      GameManager.Instance.Dead_body(); //���� ���� �� ������Ʈ �ʱ�ȭ
      GameManager.Instance.PlayRPParticle();  //ȭ�� ȸ���ϴ� ��ƼŬ
      GameManager.Instance.Respawn();                   //������
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
    TargetPRG = Random.Range(Min, Max); //���� ���ϰ�
    Outline.localScale = Vector3.one * TargetPRG;         //���� ũ�� ����
    Outline.GetComponent<SpriteRenderer>().enabled = true;
    Inside.localScale = Vector3.zero;                     //���� ũ��� 0����
    Inside.GetComponent<SpriteRenderer>().enabled = true;
    CurrentPRG = 0.0f;
    while (CurrentPRG < TargetPRG + 1.0f) //���� �Ѿ������
    {
      Debug.Log($"Current : {CurrentPRG}   Target : {TargetPRG}");
      CurrentPRG += Time.deltaTime * Speed;               //��� ����
      Inside.localScale = Vector3.one * CurrentPRG;        //���� ũ�� ������Ʈ
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
