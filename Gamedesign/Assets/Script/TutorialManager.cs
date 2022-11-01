using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
  [SerializeField] private MainCamera MyCamera = null;
  [SerializeField] private Vector2 TutorialPos = Vector2.zero;  //튜토리얼 시 카메라 위치
  public Vector2 TutorialTorchPos = Vector2.zero;
  [SerializeField] private float StartCamearSize = 2.0f;  //튜토리얼 시 카메라 확대된 사이즈
  [SerializeField] private float EndCameraSize = 5.4f;    //튜토리얼 끝나고 복구될 사이즈
  [SerializeField] private float FadeOutTime = 3.0f;      //튜토리얼 끝나고 카메라 복구되는 시간
  [SerializeField] private KeyCode SkipKey = KeyCode.C;   //튜토리얼 스킵 버튼
  private bool IsTutorial = false;

  public void Camera_start()  //카메라가 튜토리얼 위치로 이동하고 사이즈 확대
  {
    IsTutorial = true;
    MyCamera.Tutorial_start(TutorialPos, StartCamearSize);
  }
  public void Camera_finish() //카메라가 튜토리얼 끝내고 사이즈 축소
  {
    MyCamera.Tutorial_finish(EndCameraSize, FadeOutTime);
    IsTutorial = false;
  }
  private void Start()
  {
    IsTutorial = true;
  }
  private void Update()
  {
    if (Input.GetKeyDown(SkipKey) && IsTutorial) { Camera_finish();GameManager.Instance.FinishTutorial(); }
  }
}
