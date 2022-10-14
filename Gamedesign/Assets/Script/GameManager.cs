using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
  private static GameManager instance;
  public static GameManager Instance { get { return instance; } }
  //  [SerializeField] private Portal MyPortal = null;                //���� �Ⱦ�
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
  public void Dead()  //�÷��̾� ��� ��� ȣ��
  {
    MyTorchPivot.Dead();
    MyTorch.Dead();
  }
  public void Spawn()   //���� ���� ����
  {
    StartCoroutine(spawn());
  }
  private IEnumerator spawn()
  {
    bool isleft = PlayerTransform.position.x > MyRespawn.ObjPos().x;
    float _cameramovetime = MyCamera.MoveToPosition(MyRespawn.ObjPos(),0.0f);;//ī�޶� ��Ż���� �̵��ϴ� �ð�

    MyTorchPivot.MoveToRespawn(MyRespawn.ObjPos(), _cameramovetime ); //��ġ�� ī�޶� ���� �̵�
  
    MyPlayerMove.Respawn(MyRespawn.GetRespawnPos(isleft), MyRespawn.ObjPos().x, isleft);  //��Ż�� ���� �� ������ ����
   
    yield return new WaitForSeconds(_cameramovetime);   //ī�޶� �̵��ϴ� ���� ���
                                                                                         
    //  MyPortal.Open(NewestLamp.transform.position + Vector3.up * 1.0f);

    //  yield return new WaitForSeconds(MyPortal.OpenningTime);  //��Ż�� �� ���������� ���


    //  yield return new WaitForSeconds(MyPortal.RespawnTime);    //�÷��̾ ������Ǵ� ���� ���

    MyTorch.Ignite();
  //  MyPortal.Close();
  }
  public void Respawn() //�÷��̾� ��� �ִϸ��̼� ������ ȣ��
  {
    StartCoroutine(respawn());
  }
  private IEnumerator respawn()
  {
    bool isleft = PlayerTransform.position.x > MyRespawn.ObjPos().x;
    float _cameramovetime = MyCamera.MoveToPosition(MyRespawn.ObjPos(), 0.0f); ;//ī�޶� ��Ż���� �̵��ϴ� �ð�

    MyTorchPivot.MoveToRespawn(MyRespawn.ObjPos(), _cameramovetime); //��ġ�� ī�޶� ���� �̵�

    MyPlayerMove.Respawn(MyRespawn.GetRespawnPos(isleft), MyRespawn.ObjPos().x, isleft);  //��Ż�� ���� �� ������ ����

    yield return new WaitForSeconds(_cameramovetime);   //ī�޶� �̵��ϴ� ���� ���

    MyTorch.Ignite();

    /*
      float _cameramovetime = MyCamera.MoveToPosition(NewestLamp.transform.position,MyPortal.OpenningTime+MyPortal.RespawnTime);//ī�޶� ��Ż���� �̵��ϴ� �ð�

      MyTorchPivot.MoveToRespawn(NewestLamp.transform.position + Vector3.up * 1.0f, _cameramovetime+MyPortal.OpenningTime+MyPortal.RespawnTime); //��ġ�� ī�޶� ���� �̵�
      yield return new WaitForSeconds(_cameramovetime);   //ī�޶� �̵��ϴ� ���� ���

      MyPortal.Open(NewestLamp.transform.position + Vector3.up * 1.0f);

      yield return new WaitForSeconds(MyPortal.OpenningTime);  //��Ż�� �� ���������� ���

      MyPlayerMove.Respawn(NewestLamp.transform.position + Vector3.up * 1.0f, MyPortal.RespawnTime);  //��Ż�� ���� �� ������ ����

      yield return new WaitForSeconds(MyPortal.RespawnTime);    //�÷��̾ ������Ǵ� ���� ���

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
