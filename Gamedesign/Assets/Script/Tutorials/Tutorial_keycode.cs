using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Tutorial_keycode : MonoBehaviour //Ʃ�丮�� A�� D �̹���
{
  [SerializeField] private SpriteRenderer MySpr = null;       //�� ��������Ʈ������
  [SerializeField] private Sprite UnPressed = null;
  [SerializeField] private Sprite Pressed = null;
  [SerializeField] private ParticleSystem MyParticle = null;  //�� ��ƼŬ
  private ParticleSystem.EmissionModule MyEmission;           //�� ��ƼŬ ���̼�
  [SerializeField] private float IdleCount = 0;               //�⺻ ��ƼŬ ����
  [SerializeField] private float PressCount = 0;              //�������� ��ƼŬ ����
  [SerializeField] private Light2D MyLight = null;
  public void Setup()
  {
    MyEmission = MyParticle.emission;
    MyEmission.rateOverTime = IdleCount;
  }
  public void Start()
  {
    Setup();
  }
  private void Awake()
  {
    Color _color = Color.white;
    _color.a = 0.0f;
    MySpr.color = _color;
    MyLight.intensity = 0.0f;
  }
  public void StartTutorial()
  {
    MyParticle.Play();
    StartCoroutine(fadespr(true));
    StartCoroutine(setlight(true));
  }
  public void EndTutorial()
  {
    MyParticle.Stop();
    StartCoroutine(fadespr(false));
    StartCoroutine(setlight(false));
  }
  private IEnumerator startup() //��ƼŬ ���� 0������ IdleCount���� �ø�
  {
    float _targettime = 0.3f;
    float _time = 0.0f;
    while (_time < _targettime)
    {
      MyEmission.rateOverTime = IdleCount * _time / _targettime;
      _time += Time.deltaTime;
      yield return null;
    }
  }
  private IEnumerator fadespr(bool isin)
  {
    float _targettime = 0.2f;
    float _time = 0.0f;
    float _startA = isin ? 0.0f : 1.0f;
    float _endA = isin ? 1.0f : 0.0f;
    Color _currentcolor = Color.white;
    while (_time < _targettime)
    {
      _currentcolor.a = Mathf.Lerp(_startA, _endA, Mathf.Pow(_time / _targettime, 2.0f));
      MySpr.color = _currentcolor;
      _time += Time.deltaTime;
      yield return null;
    }
    _currentcolor.a = _endA;
    MySpr.color = _currentcolor;
  }
  private IEnumerator setlight(bool ison)
  {
    float _targettime = 0.2f;
    float _time = 0.0f;
    float _startA = ison ? 0.0f : 1.0f;
    float _endA = ison ? 1.0f : 0.0f;
    while (_time < _targettime)
    {
      MyLight.intensity = Mathf.Lerp(_startA, _endA, Mathf.Pow(_time / _targettime, 2.0f));
      _time += Time.deltaTime;
      yield return null;
    }
    MyLight.intensity = _endA;
  }
  public void PressDown() { MyEmission.rateOverTime = PressCount; MySpr.sprite = Pressed; }  //�������� PressCount���� ����
  public void PressUp() { MyEmission.rateOverTime = IdleCount;MySpr.sprite = UnPressed; }    //Ű ���� IdleCount�� ����
  public void End()
  {

  }
}
