using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
  [SerializeField] private MainCamera MyCamera = null;
  [SerializeField] private Vector2 TutorialPos = Vector2.zero;  //Ʃ�丮�� �� ī�޶� ��ġ
  public Vector2 TutorialTorchPos = Vector2.zero;
  [SerializeField] private float StartCamearSize = 2.0f;  //Ʃ�丮�� �� ī�޶� Ȯ��� ������
  [SerializeField] private float EndCameraSize = 5.4f;    //Ʃ�丮�� ������ ������ ������
  [SerializeField] private float FadeOutTime = 3.0f;      //Ʃ�丮�� ������ ī�޶� �����Ǵ� �ð�
  [SerializeField] private KeyCode SkipKey = KeyCode.C;   //Ʃ�丮�� ��ŵ ��ư
  private bool IsTutorial = false;

  public void Camera_start()  //ī�޶� Ʃ�丮�� ��ġ�� �̵��ϰ� ������ Ȯ��
  {
    IsTutorial = true;
    MyCamera.Tutorial_start(TutorialPos, StartCamearSize);
  }
  public void Camera_finish() //ī�޶� Ʃ�丮�� ������ ������ ���
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
