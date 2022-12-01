using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TutorialManager : MonoBehaviour
{
  [SerializeField] private MainCamera MyCamera = null;
  [SerializeField] private Vector2 TutorialCameraPos = Vector2.zero;  //튜토리얼 시 카메라 위치
  public Vector2 TutorialTorchPos = Vector2.zero;
  [SerializeField] private float StartCamearSize = 2.0f;  //튜토리얼 시 카메라 확대된 사이즈
  [SerializeField] private float EndCameraSize = 5.4f;    //튜토리얼 끝나고 복구될 사이즈
  [HideInInspector] public float TutorialRatio = 0.0f;
  [SerializeField] private float FadeOutTime = 3.0f;      //튜토리얼 끝나고 카메라 복구되는 시간
  [SerializeField] private Tutorial_keycode Key_A = null;
  [SerializeField] private Tutorial_keycode Key_D = null;
  [SerializeField] private Light2D Tutorial_Light = null; //튜토리얼용 배경 라이트
  [SerializeField] private float Tutorial_Light_intensity = 0.0f; //튜토리얼용 배경 라이트 밝기
  [SerializeField] private KeyCode SkipKey = KeyCode.C;   //튜토리얼 스킵 버튼
  private bool IsTutorial = false;
  [HideInInspector] public Dimension TutorialDimension = Dimension.A;
  [SerializeField] private SpriteRenderer SkipSpr = null;
  [SerializeField] private Tutorial_UI MyUI = null;
  [SerializeField] private Tutorial_Wooden[] Woodens = new Tutorial_Wooden[2];  //튜토리얼 목재들
  [SerializeField] private Tutorial_stone Stone = null;                         //튜토리얼 돌
  [SerializeField] private Tutorial_lamp_dimension LampDimension = null;        //튜토리얼 차원 램프
  [SerializeField] private Tutorial_lamp_event LampEvent = null;                //튜토리얼 이벤트 램프
  public void Fired() => MyUI.Fired();
  public void Set_0() //1번 튜토리얼 세팅
  {
    if (!IsTutorial) return;
    Woodens[0].Active();
    Woodens[1].Active();
  }
  public void Set_1() //2번 튜토리얼 세팅
  {
    if (!IsTutorial) return;
    LampDimension.Active();
  }
  public void SetStone() => Stone.Active();

  public void Set_2() //3번 튜토리얼 세팅
  {
    if (!IsTutorial) return;
    Woodens[0].DeActive();
    Woodens[1].DeActive();
  //  LampDimension.DeActive();
    LampEvent.Active();
  }
  public void Camera_start()  //카메라가 튜토리얼 위치로 이동하고 사이즈 확대      튜토리얼 스타트
  {
    IsTutorial = true;
    MyCamera.Tutorial_start(TutorialCameraPos, StartCamearSize);
    StartCoroutine(settinglight(Tutorial_Light_intensity));
    StartCoroutine(settingspr(true));
    Key_A.StartTutorial();
    Key_D.StartTutorial();
    Set_0();
  }
  public void Camera_finish() //카메라가 튜토리얼 끝내고 사이즈 축소
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
  private IEnumerator settinglight(float targetint) //불조정
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
