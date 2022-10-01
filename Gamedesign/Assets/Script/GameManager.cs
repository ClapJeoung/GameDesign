using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
  private static GameManager instance;
  public static GameManager Instance { get { return instance; } }
  [SerializeField] private Lamp_save NewestLamp = null;
  [SerializeField] private Portal MyPortal = null;
  [SerializeField] private GameObject PlayerPrefab = null;
  [Space(5)]
  [SerializeField] private GameObject WaterDown = null;
  [SerializeField] private GameObject WaterUp = null;
  private Transform MyPlayer = null;
  private void Awake()
  {
    if (instance == null) instance = this;
  }
  [SerializeField] private MainCamera MyCamera = null;

  public void SetNewPlayer(Transform player)
  {
    MyPlayer = player;
    MyCamera.SetPlayer(player);
  }
  public void SetNewLamp(Lamp_save newlamp) => NewestLamp = newlamp;
  public void Dead()  //플레이어 사망 애니메이션 전부 끝나고 호출
  {
    StartCoroutine(respawn());
  }
  private IEnumerator respawn()
  {
    MyCamera.MoveToPosition(NewestLamp.transform.position);
    yield return new WaitForSeconds(MyPortal.Open(NewestLamp.transform.position+Vector3.up*1.0f)+1.0f);
    Instantiate(PlayerPrefab, NewestLamp.transform.position + Vector3.up * 1.0f, Quaternion.identity);
    MyPortal.Close();
  }
  public void GetWaterParticle(out Transform waterdowntrans,out ParticleSystem waterdownpar,out Transform wateruptrans,out ParticleSystem wateruppar)
  {
    waterdowntrans = WaterDown.transform;
    waterdownpar = WaterDown.GetComponent<ParticleSystem>();
    wateruptrans = WaterUp.transform;
    wateruppar = WaterUp.GetComponent<ParticleSystem>();
  }
}
