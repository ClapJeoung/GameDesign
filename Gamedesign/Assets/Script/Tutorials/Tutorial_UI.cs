using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_UI : MonoBehaviour
{
  [SerializeField] private TutorialManager MyManager = null;
  [SerializeField] private SpriteRenderer[] Spr_R = new SpriteRenderer[3];  //좌측 목재 스렌 3개
  [SerializeField] private ParticleSystem Particle_R = null;                //좌측 목재 파티클
  [SerializeField] private SpriteRenderer[] Spr_L = new SpriteRenderer[3];  //우측 목재 스렌 3개
  [SerializeField] private ParticleSystem Particle_L = null;                //우측 목재 파티클
  [SerializeField] private ParticleSystem NextParticle = null;              //목재에서 램프로 넘어가는 파티클
  [SerializeField] private SpriteRenderer LampSpr = null;                   //램프
  [SerializeField] private ParticleSystem LampFinish = null;                //램프 끝
  [SerializeField] private SpriteRenderer[] StageEyes = new SpriteRenderer[3];  //스테이지 단계 표시 3개
  [SerializeField] private Sprite OpenEye = null;                               //눈 연 스프라이트
  [SerializeField] private ParticleSystem[] StageParticles = new ParticleSystem[3]; //스테이지 클리어 파티클 3개
  private int WoodCount = 4;  //남은 목재 개수
  public void Fired()
  {
    switch (WoodCount)
    {
      case 4: //4개 남았을때 불탄다 : 1단계 1/2
        Spr_R[0].enabled = false;
        Spr_R[1].enabled = true;
        Particle_R.Play();
        break;
      case 3: //3개 남았을때 불탄다 : 1단계 2/2
        Spr_L[0].enabled = false;
        Spr_L[1].enabled = true;
        StageEyes[0].sprite = OpenEye;
        StageParticles[0].Play();
        Particle_L.Play();
        MyManager.Set_1();
        break;
      case 2: //2개 남았을때 불탔다 : 2단계 1/2
        Spr_R[0].enabled = true;
        Spr_R[1].enabled = false;
        Particle_R.Play();
        break;
      case 1: //1개 남았을때 불탔다 : 2단계 2/2
        NextParticle.Play();
       Destroy(Spr_R[0].gameObject);
        Destroy(Spr_L[0].gameObject);
       // LampSpr.enabled = true;
        MyManager.Set_2();
        StageEyes[1].sprite = OpenEye;
        StageParticles[1].Play();
        Particle_R.Play();
        GameManager.Instance.CloseMask(new Vector3(-4.0f,4.0f));
        break;
      case 0: //0개 남을때 불탔다 : 3단계 클리어
       // LampSpr.enabled=false;
        LampFinish.Play();
        StageEyes[2].sprite = OpenEye;
        StageParticles[2].Play();
        MyManager.Camera_finish();
        break;
      default: return;
    }
    WoodCount--;
  }
  public void DestroyEyes()
  {
    for(int i = 0; i < StageEyes.Length; i++)
    {
      StageEyes[i].enabled = false;
      StageParticles[i].Play();
    }
    if (Spr_R[0] != null)
    {
      Destroy(Spr_R[0].gameObject);
      Destroy(Spr_L[0].gameObject);
      Particle_R.Play();
      Particle_L.Play();
    }
  }
}
