using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
  private static GameManager instance;
  public static GameManager Instance { get { return instance; } }
  //  [SerializeField] private Portal MyPortal = null;                //이제 안씀
  [SerializeField] private RespawnObj MyRespawn = null;
  [SerializeField] private Transform PlayerTransform = null;
  private Player_Move MyPlayerMove = null;
  [SerializeField] private Torch MyTorch = null;
  [SerializeField] private Torch_pivot MyTorchPivot = null;
  [Space(5)]
  [SerializeField] private GameObject WaterDown = null;
  [SerializeField] private GameObject WaterUp = null;
  private Transform MyPlayer = null;
  [SerializeField] private FIreMask MyMask = null;
  [HideInInspector] public Dimension WorldDimension = Dimension.A;
  private void Awake()
  {
    if (instance == null) instance = this;
    MyPlayerMove=PlayerTransform.GetComponent<Player_Move>();
  }
  [SerializeField] private MainCamera MyCamera = null;
  private void Update()
  {
    if (Input.GetKeyDown(KeyCode.Tab)) Spawn();
  }
  public void SetNewPlayer(Transform player)
  {
    MyPlayer = player;
    MyCamera.SetPlayer(player);
  }
  public void SetNewRespawn(RespawnObj newrespawn) => MyRespawn = newrespawn;
  public void Dead()  //플레이어 사망 즉시 호출
  {
    MyTorchPivot.Dead();
    MyTorch.Dead();
  }
  public void Spawn()   //게임 최초 시작
  {
    StartCoroutine(spawn());
  }
  private IEnumerator spawn()
  {
    bool isleft = PlayerTransform.position.x > MyRespawn.ObjPos().x;
    float _cameramovetime = MyCamera.MoveToPosition(MyRespawn.ObjPos(),0.0f);;//카메라가 포탈까지 이동하는 시간

    MyTorchPivot.MoveToRespawn(MyRespawn.ObjPos(), _cameramovetime ); //토치도 카메라 따라 이동
  
    MyPlayerMove.Respawn(MyRespawn.GetRespawnPos(isleft), MyRespawn.ObjPos().x, isleft);  //포탈이 열린 후 리스폰 시작
   
    yield return new WaitForSeconds(_cameramovetime);   //카메라 이동하는 동안 대기
                                                                                         
    //  MyPortal.Open(NewestLamp.transform.position + Vector3.up * 1.0f);

    //  yield return new WaitForSeconds(MyPortal.OpenningTime);  //포탈이 다 열릴때까지 대기


    //  yield return new WaitForSeconds(MyPortal.RespawnTime);    //플레이어가 재생성되는 동안 대기

    MyTorch.Ignite();
  //  MyPortal.Close();
  }
  public void Respawn() //플레이어 사망 애니메이션 끝나고 호출
  {
    StartCoroutine(respawn());
  }
  private IEnumerator respawn()
  {
    bool isleft = PlayerTransform.position.x > MyRespawn.ObjPos().x;
    float _cameramovetime = MyCamera.MoveToPosition(MyRespawn.ObjPos(), 0.0f); ;//카메라가 포탈까지 이동하는 시간

    MyTorchPivot.MoveToRespawn(MyRespawn.ObjPos(), _cameramovetime); //토치도 카메라 따라 이동

    MyPlayerMove.Respawn(MyRespawn.GetRespawnPos(isleft), MyRespawn.ObjPos().x, isleft);  //포탈이 열린 후 리스폰 시작

    yield return new WaitForSeconds(_cameramovetime);   //카메라 이동하는 동안 대기

    MyTorch.Ignite();

    /*
      float _cameramovetime = MyCamera.MoveToPosition(NewestLamp.transform.position,MyPortal.OpenningTime+MyPortal.RespawnTime);//카메라가 포탈까지 이동하는 시간

      MyTorchPivot.MoveToRespawn(NewestLamp.transform.position + Vector3.up * 1.0f, _cameramovetime+MyPortal.OpenningTime+MyPortal.RespawnTime); //토치도 카메라 따라 이동
      yield return new WaitForSeconds(_cameramovetime);   //카메라 이동하는 동안 대기

      MyPortal.Open(NewestLamp.transform.position + Vector3.up * 1.0f);

      yield return new WaitForSeconds(MyPortal.OpenningTime);  //포탈이 다 열릴때까지 대기

      MyPlayerMove.Respawn(NewestLamp.transform.position + Vector3.up * 1.0f, MyPortal.RespawnTime);  //포탈이 열린 후 리스폰 시작

      yield return new WaitForSeconds(MyPortal.RespawnTime);    //플레이어가 재생성되는 동안 대기

      MyTorch.Ignite();
      MyPortal.Close();   */
  }
  public void GetWaterParticle(out Transform waterdowntrans,out ParticleSystem waterdownpar,out Transform wateruptrans,out ParticleSystem wateruppar)
  {
    waterdowntrans = WaterDown.transform;
    waterdownpar = WaterDown.GetComponent<ParticleSystem>();
    wateruptrans = WaterUp.transform;
    wateruppar = WaterUp.GetComponent<ParticleSystem>();
  }

  public void OpenMask(Vector2 newpos) { MyMask.Open(newpos); WorldDimension = Dimension.B; }
  public void CloseMask(Vector2 newpos) { MyMask.Close(newpos); WorldDimension = Dimension.A; }
}
