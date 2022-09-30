using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
  private static GameManager instance;
  public static GameManager Instance { get { return instance; } }
  [SerializeField] private Intr_Lamp NewestLamp = null;
  [SerializeField] private Portal MyPortal = null;
  [SerializeField] private GameObject PlayerPrefab = null;
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
  public void SetNewLamp(Intr_Lamp newlamp) => NewestLamp = newlamp;
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
}
