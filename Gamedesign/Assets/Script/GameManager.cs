using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
  private static GameManager instance;
  public static GameManager Instance { get { return instance; } }
  //  [SerializeField] private Portal MyPortal = null;                //이제 안씀
  [SerializeField] private TutorialManager MyTutorial = null;
  [SerializeField] private RespawnObj OriginRespawn = null;
   private RespawnObj CurrentRespawn = null;
  [SerializeField] private Transform PlayerTransform = null;
  private Player_Move MyPlayerMove = null;
  [SerializeField] private Torch MyTorch = null;
  [SerializeField] private Torch_pivot MyTorchPivot = null;
  [Space(5)]
  [SerializeField] private GameObject WaterDown = null;
  [SerializeField] private GameObject WaterUp = null;
  [SerializeField] private Transform MyPlayer = null;
  [SerializeField] private FIreMask MyMask = null;
  //  [HideInInspector] public Dimension WorldDimension = Dimension.A;
  public StageCollider CurrentSC = null;
  [Space(5)]
  [SerializeField] private HindiData[] RestartHindies = null;
  [SerializeField] private HindiData[] DeadHindies = null;
  [Space(5)]
  [SerializeField] private StageCollider[] AllStages = null;
  public List<Lightobj> AllLights = new List<Lightobj>();
  public void TurnOffLights() { foreach (var lights in AllLights) lights.TurnOff(); }   //모든 불 끄기(카메라 불 빼고)
  [SerializeField] private Transform SkullHolder = null;
  [SerializeField] private float TimeSlowTime = 4.5f;
  [SerializeField] private float TimeRecoveryTime = 1.5f;
  public void SetSC(ref StageCollider newsc)
  {
    newsc.CurrentDimension=CurrentSC.CurrentDimension;
    CurrentSC = newsc;
  }
  private void Awake()
  {
    if (instance == null) instance = this;
    MyPlayerMove=PlayerTransform.GetComponent<Player_Move>();
    CurrentRespawn = OriginRespawn;
  }
  [SerializeField] private MainCamera MyCamera = null;
  private void Update()
  {
//    if (Input.GetKeyDown(KeyCode.Tab)) Spawn();
  }
  private void Start()
  {
   Invoke("StartTutorial",0.1f);
  }
  public void SetNewRespawn(RespawnObj newrespawn) => CurrentRespawn = newrespawn;
  public void Flooded() //게임오버 홍수 시작
  {
    StartCoroutine(recoverytime());
    MyCamera.StartFlood();
 //   foreach (var stages in AllStages) stages.ResetStage();  //스테이지 전체 초기화
    foreach (var lights in AllLights) lights.TurnOff();     //모든 불 끄기(카메라 불 빼고)
  }
  private IEnumerator recoverytime()  //시간 속도 복구
  {
      float _time = 0.0f;
      while (_time < TimeRecoveryTime)
      {
        _time += Time.unscaledDeltaTime;
        Time.timeScale =  _time / TimeRecoveryTime;
        yield return null;
      }
      Time.timeScale = 1.0f;
  }
  public void StartTutorial()       //홍수나고 페이드아웃 다음에 호출되는 튜토리얼 시작 함수
  {
    for(int i=0;i<SkullHolder.childCount;i++)Destroy(SkullHolder.GetChild(i).gameObject);
    foreach (var lights in AllLights) lights.TurnOn();     //모든 불 키기
    CurrentRespawn = OriginRespawn; //리스폰 오브젝트 초기화
    MyTorch.Ignite();               //횃불 불 피우고
    MyTorchPivot.SetTutorial(MyTutorial.TutorialTorchPos);  //횃불 튜토리얼 위치에 위치
    MyCamera.FinishFlood();
    UIManager.Instance.FadeIn(3.0f);  //3.0f초 정도로 페이드인
    MyTutorial.Camera_start();      //카메라 튜토리얼화
  }
  public void FinishTutorial()      //튜토리얼 끝나고 호출
  {
    if (CurrentSC.CurrentDimension != CurrentSC.DefaultDimension) CloseMask(1.0f);
    MyTorchPivot.FinishTutorial();
    Respawn();
  }
  public void Dead_body()  //플레이어 사망 즉시 호출
  {
    if (CurrentSC.CurrentDimension != CurrentSC.DefaultDimension)
    {
      if (CurrentSC.DefaultDimension == Dimension.A) CloseMask(1.0f);
      else OpenMask(1.0f);
    }
    MyTorchPivot.Dead();
    CurrentSC.ResetStage(); //현재 스테이지만 초기화
  }
  public void Dead_soul_0()   //횃불 사망 직후 호출
  {
    MyTorchPivot.Dead();  //횃불 멈추고
    MyTorch.Dead();       //횃불 멈추고
  }
  public void Dead_soul_1() //플레이어 사망 연출 이후 호출
  {
    MyCamera.StartFloodParticle();
    StartCoroutine(slowtime());                              //주위 정지
    int _random = Random.Range(0, DeadHindies.Length);
    UIManager.Instance.OutputHindi(DeadHindies[_random].Sprites, DeadHindies[_random].Length);  //텍스트 출력 시작
  }
  private IEnumerator slowtime()  //시간 정지
  {
    float _time = 0.0f;
    while(_time< TimeSlowTime)
    {
      _time += Time.unscaledDeltaTime;
      if (1 - _time / TimeSlowTime < 0) break;
      Time.timeScale = 1 - _time / TimeSlowTime;
      yield return null;
    }
    Time.timeScale = 0.0f;
  }
  public void Respawn() //플레이어 사망 애니메이션 끝나고 호출
  {
    StartCoroutine(respawn());
  }
  private IEnumerator respawn()
  {
    MyCamera.SetTarget(PlayerTransform, Vector3.zero);

    float _cameramovetime = MyCamera.MoveToResapwn(CurrentRespawn.ObjPos(), 0.0f); ;//카메라가 포탈까지 이동하는 시간

    MyTorchPivot.MoveToRespawn(CurrentRespawn.ObjPos(), _cameramovetime); //토치도 카메라 따라 이동

    MyPlayerMove.Respawn(CurrentRespawn.GetRespawnPos(), CurrentRespawn.ObjPos().x);  //포탈이 열린 후 리스폰 시작

    yield return new WaitForSeconds(_cameramovetime);   //카메라 이동하는 동안 대기

    MyTorch.Ignite();
  }
  public void GetWaterParticle(out Transform waterdowntrans,out ParticleSystem waterdownpar,out Transform wateruptrans,out ParticleSystem wateruppar)
  {
    waterdowntrans = WaterDown.transform;
    waterdownpar = WaterDown.GetComponent<ParticleSystem>();
    wateruptrans = WaterUp.transform;
    wateruppar = WaterUp.GetComponent<ParticleSystem>();
  }
  public void RockPressed()//돌에 깔려서 쿵찍
  {
    MyCamera.RockPressed();
  }
  public void OpenMask(float _size) { MyMask.Open(_size); CurrentSC.CurrentDimension = Dimension.B; }
  public void CloseMask(float _size) { MyMask.Close(_size); CurrentSC.CurrentDimension = Dimension.A; }
  public void PlayRPParticle() => MyCamera.StartRPParticle();
}
