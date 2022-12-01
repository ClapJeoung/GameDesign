using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TutorialManager : MonoBehaviour
{
  [SerializeField] private MainCamera MyCamera = null;
  [SerializeField] private Vector2 TutorialCameraPos = Vector2.zero;  //Ʃ�丮�� �� ī�޶� ��ġ
  public Vector2 TutorialTorchPos = Vector2.zero;
  [SerializeField] private float StartCamearSize = 2.0f;  //Ʃ�丮�� �� ī�޶� Ȯ��� ������
  [SerializeField] private float EndCameraSize = 5.4f;    //Ʃ�丮�� ������ ������ ������
  [HideInInspector] public float TutorialRatio = 0.0f;
  [SerializeField] private float FadeOutTime = 3.0f;      //Ʃ�丮�� ������ ī�޶� �����Ǵ� �ð�
  [SerializeField] private Tutorial_keycode Key_A = null;
  [SerializeField] private Tutorial_keycode Key_D = null;
  [SerializeField] private Light2D Tutorial_Light = null; //Ʃ�丮��� ��� ����Ʈ
  [SerializeField] private float Tutorial_Light_intensity = 0.0f; //Ʃ�丮��� ��� ����Ʈ ���
  [SerializeField] private KeyCode SkipKey = KeyCode.C;   //Ʃ�丮�� ��ŵ ��ư
  private bool IsTutorial = false;
  [HideInInspector] public Dimension TutorialDimension = Dimension.A;
  [SerializeField] private SpriteRenderer SkipSpr = null;
  [SerializeField] private Tutorial_UI MyUI = null;
  [SerializeField] private Tutorial_Wooden[] Woodens = new Tutorial_Wooden[2];  //Ʃ�丮�� �����
  [SerializeField] private Tutorial_stone Stone = null;                         //Ʃ�丮�� ��
  [SerializeField] private Tutorial_lamp_dimension LampDimension = null;        //Ʃ�丮�� ���� ����
  [SerializeField] private Tutorial_lamp_event LampEvent = null;                //Ʃ�丮�� �̺�Ʈ ����
  public void Fired() => MyUI.Fired();
  public void Set_0() //1�� Ʃ�丮�� ����
  {
    if (!IsTutorial) return;
    Woodens[0].Active();
    Woodens[1].Active();
  }
  public void Set_1() //2�� Ʃ�丮�� ����
  {
    if (!IsTutorial) return;
    LampDimension.Active();
  }
  public void SetStone() => Stone.Active();

  public void Set_2() //3�� Ʃ�丮�� ����
  {
    if (!IsTutorial) return;
    Woodens[0].DeActive();
    Woodens[1].DeActive();
  //  LampDimension.DeActive();
    LampEvent.Active();
  }
  public void Camera_start()  //ī�޶� Ʃ�丮�� ��ġ�� �̵��ϰ� ������ Ȯ��      Ʃ�丮�� ��ŸƮ
  {
    IsTutorial = true;
    MyCamera.Tutorial_start(TutorialCameraPos, StartCamearSize);
    StartCoroutine(settinglight(Tutorial_Light_intensity));
    StartCoroutine(settingspr(true));
    Key_A.StartTutorial();
    Key_D.StartTutorial();
    Set_0();
  }
  public void Camera_finish() //ī�޶� Ʃ�丮�� ������ ������ ���
  {
   if(Woodens[0]!=null) Woodens[0].DeActive();
    if (Woodens[1] != null) Woodens[1].DeActive();
   if(LampDimension!=null) LampDimension.DeActive();
   if(LampEvent!=null) LampEvent.DeActive();
    Stone.Deactive();
    IsTutorial = false;
    Invoke("asdf", 1.5f);
  }
  private void asdf()
  {
    GameManager.Instance.FinishTutorial();
    MyUI.DestroyEyes();
    MyCamera.Tutorial_finish(EndCameraSize, FadeOutTime);
    StartCoroutine(settinglight(0.0f));
    StartCoroutine(settingspr(false));
    Key_A.EndTutorial();
    Key_D.EndTutorial();
  }
  private IEnumerator settinglight(float targetint) //������
  {
    float _targettime = 0.5f;
    float _time = 0.0f;
    float _originint = Tutorial_Light.intensity;
    while (_time < _targettime)
    {
      Tutorial_Light.intensity = Mathf.Lerp(_originint, targetint, _time / _targettime);
      _time += Time.deltaTime;
      yield return null;
    }
    Tutorial_Light.intensity = targetint;
  }
  private IEnumerator settingspr(bool isin)
  {
    float _targettime = 0.2f;
    float _time = 0.0f;
    float _startA = isin ? 0.0f : 1.0f;
    float _endA = isin ? 1.0f : 0.0f;
    Color _currentcolor = Color.white;
    while (_time < _targettime)
    {
      _currentcolor.a = Mathf.Lerp(_startA, _endA, Mathf.Pow(_time / _targettime, 2.0f));
      SkipSpr.color = _currentcolor;
      _time += Time.deltaTime;
      yield return null;
    }
    _currentcolor.a = _endA;
    SkipSpr.color = _currentcolor;
  }
  private void Start()
  {
    IsTutorial = true;
    TutorialRatio = StartCamearSize / EndCameraSize;
    if (SkipManager.Instance.isfirst) SkipSpr.enabled = false;
  }
  private void Update()
  {
    if ( IsTutorial) {
      if (Input.GetKeyDown(SkipKey)&&SkipManager.Instance.isfirst == false) {  Camera_finish();  }
      if(Input.GetKeyDown(KeyCode.LeftArrow))Key_A.PressDown();
      if(Input.GetKeyDown(KeyCode.RightArrow))Key_D.PressDown();
      if(Input.GetKeyUp(KeyCode.LeftArrow))Key_A.PressUp();
      if(Input.GetKeyUp(KeyCode.RightArrow))Key_D.PressUp();
    }
  }
}
