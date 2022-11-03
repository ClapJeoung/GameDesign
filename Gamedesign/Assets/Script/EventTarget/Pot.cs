using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pot : EventTarget
{
  [SerializeField] private WaterDrop[] MyWaters = null; //자식으로 가지고 있는 물들
  [SerializeField] private Vector3 FallPostion = Vector3.zero;  //물이 떨어질 위치
  [SerializeField] private ParticleSystem OpenParticle = null;  //물 떨어뜨릴때 파티클
  [SerializeField] private bool AllwaysActive = false;          //영구적으로 활성화된건지
  [SerializeField] private float FirstWaitTime = 0.0f;          //영구적으로 활성화됐을때 초기 대기시간
  private float PouringTime = 2.0f;                             //물 붓는 시간
  [SerializeField] private float WaitTime = 4.0f;               //물 붓고 다음 붓기까지 대기하는 시간
  public int MyId = 0;
  private IEnumerator pouring()
  {
    yield return new WaitForSeconds(FirstWaitTime);
    while (true)
    {
      OpenParticle.Play();
      for(int i=0; i < MyWaters.Length; i++)
      {
        MyWaters[i].Fall(FallPostion+Vector3.back);
        yield return new WaitForSeconds(PouringTime / MyWaters.Length);
      }
      OpenParticle.Stop();
      yield return new WaitForSeconds(WaitTime);
    }
  }
  public override void  Active()
  {
    StartCoroutine(pouring());
  }
  public override void Deactive()
  {
    OpenParticle.Stop();
    StopAllCoroutines();
  }
  private void Start()
  {
    if (AllwaysActive) StartCoroutine(pouring());
  }
}
