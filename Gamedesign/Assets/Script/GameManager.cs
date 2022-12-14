using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
  private static GameManager instance;
  public static GameManager Instance { get { return instance; } }
  //  [SerializeField] private Portal MyPortal = null;                //???? ?Ⱦ?
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
  public void TurnOffLights() { foreach (var lights in AllLights) lights.TurnOff(); }   //???? ?? ????(ī?޶? ?? ????)
  [SerializeField] private Transform SkullHolder = null;
  [SerializeField] private SouldeathModule SDModule = null;
  public void SetSC(ref StageCollider newsc)
  {
    newsc.CurrentDimension=CurrentSC.CurrentDimension;
    CurrentSC = newsc;
  }
  private void Awake()
  {
    Cursor.visible = false;
    if (instance == null) instance = this;
    MyPlayerMove=PlayerTransform.GetComponent<Player_Move>();
    CurrentRespawn = OriginRespawn;
    Screen.SetResolution(1920, 1080, true);
  }
  public MainCamera MyCamera = null;
  private void Update()
  {
    //    Debug.Log(CurrentSC.name);
    //    if (Input.GetKeyDown(KeyCode.Tab)) Spawn();
    if (Input.GetKeyDown(KeyCode.F12)) Application.Quit();
  }
  private void Start()
  {
   Invoke("StartTutorial",0.1f);
  }
  public void SetNewRespawn(RespawnObj newrespawn) => CurrentRespawn = newrespawn;
  public void Flooded() //???ӿ??? ȫ?? ????
  {
    MyCamera.StartFlooda();
    foreach (var lights in AllLights) lights.TurnOff();     //???? ?? ????(ī?޶? ?? ????)
  }
  public void StartTutorial()       //ȫ?????? ???̵??ƿ? ?????? ȣ???Ǵ? Ʃ?丮?? ???? ?Լ?
  {
    for(int i=0;i<SkullHolder.childCount;i++)Destroy(SkullHolder.GetChild(i).gameObject);
    foreach (var lights in AllLights) lights.TurnOn();     //???? ?? Ű??
    CurrentRespawn = OriginRespawn; //?????? ??????Ʈ ?ʱ?ȭ
    MyTorch.Ignite();               //ȶ?? ?? ?ǿ???
    MyTorchPivot.SetTutorial(MyTutorial.TutorialTorchPos);  //ȶ?? Ʃ?丮?? ??ġ?? ??ġ
    MyCamera.FinishFlood();
    UIManager.Instance.FadeIn(3.0f);  //3.0f?? ?????? ???̵???
    MyTutorial.Camera_start();      //ī?޶? Ʃ?丮??ȭ
  }
  public void FinishTutorial()      //Ʃ?丮?? ?????? ȣ??
  {
    if (CurrentSC.CurrentDimension != CurrentSC.DefaultDimension) CloseMask(transform.position);
    MyTorchPivot.FinishTutorial();
    Respawn();
  }
  public void Dead_body()  //?÷??̾? ???? ???? ?????? ȣ??
  {
    if (CurrentSC.CurrentDimension != CurrentSC.DefaultDimension)
    {
      if (CurrentSC.DefaultDimension == Dimension.A) CloseMask(MyCamera.transform.position);
      else OpenMask(MyCamera.transform.position);
    }
    MyTorchPivot.Dead();
    CurrentSC.ResetStage(); //???? ?????????? ?ʱ?ȭ
  }
  public void RespawnHindi()
  {
    int _random = Random.Range(0, RestartHindies.Length);
    UIManager.Instance.RespawnHindi(RestartHindies[_random].Sprites, RestartHindies[_random].Length);
  }
  public void Dead_soul_0()   //ȶ?? ???? ???? ȣ??
  {
    MyTorchPivot.Dead();  //ȶ?? ???߰?
    MyTorch.Dead();       //ȶ?? ???߰?
  }
  private void _asdf()
  {
    int _random = Random.Range(0, DeadHindies.Length);
    UIManager.Instance.OutputHindi(DeadHindies[_random].Sprites, DeadHindies[_random].Length);  //?ؽ?Ʈ ???? ????
  }
  public void Dead_soul_1() //?÷??̾? ???? ???? ???? ȣ??
  {
    Invoke("Flooded", SDModule.Text_Waittime + SDModule.Text_AppearingTime + 3.0f);
    CurrentSC.StopAllPots();
    Invoke("_asdf", 0.0f);
    MyCamera.StartFloodParticle();
    AudioManager.Instance.PlayClip(13);
  }
  public void Respawn() //?÷??̾? ???? ?ִϸ??̼? ?????? ȣ??
  {
    StartCoroutine(respawn());
  }
  private IEnumerator respawn()
  {
    MyCamera.SetTarget(PlayerTransform, Vector3.zero);

    float _cameramovetime = MyCamera.MoveToResapwn(CurrentRespawn.ObjPos(), 0.0f); ;//ī?޶??? ??Ż???? ?̵??ϴ? ?ð?

    MyTorchPivot.MoveToRespawn(CurrentRespawn.ObjPos(), _cameramovetime); //??ġ?? ī?޶? ???? ?̵?

    MyPlayerMove.Respawn(CurrentRespawn.GetRespawnPos(), CurrentRespawn.ObjPos().x);  //??Ż?? ???? ?? ?????? ????

    yield return new WaitForSeconds(_cameramovetime);   //ī?޶? ?̵??ϴ? ???? ????

    MyTorch.Ignite();
  }
  public void GetWaterParticle(out Transform waterdowntrans,out ParticleSystem waterdownpar,out Transform wateruptrans,out ParticleSystem wateruppar)
  {
    waterdowntrans = WaterDown.transform;
    waterdownpar = WaterDown.GetComponent<ParticleSystem>();
    wateruptrans = WaterUp.transform;
    wateruppar = WaterUp.GetComponent<ParticleSystem>();
  }
  public void RockPressed()//???? ?????? ????
  {
    MyCamera.RockPressed();
  }
  public void OpenMask(Vector2 newpos) { MyMask.Open(newpos); CurrentSC.CurrentDimension = Dimension.B; }   //?Ϲݰ? -> ??ȥ??
  public void CloseMask(Vector2 newpos) { MyMask.Close(newpos); CurrentSC.CurrentDimension = Dimension.A; } //??ȥ?? -> ?Ϲݰ?
  public void PlayRPParticle() => MyCamera.StartRPParticle(); //????????ƼŬ ????(ī?޶? ??ũ??Ʈ???? ȣ??)
}
