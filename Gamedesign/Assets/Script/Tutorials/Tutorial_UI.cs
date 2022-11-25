using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_UI : MonoBehaviour
{
  [SerializeField] private TutorialManager MyManager = null;
  [SerializeField] private SpriteRenderer[] Spr_R = new SpriteRenderer[3];  //���� ���� ���� 3��
  [SerializeField] private ParticleSystem Particle_R = null;                //���� ���� ��ƼŬ
  [SerializeField] private SpriteRenderer[] Spr_L = new SpriteRenderer[3];  //���� ���� ���� 3��
  [SerializeField] private ParticleSystem Particle_L = null;                //���� ���� ��ƼŬ
  [SerializeField] private ParticleSystem NextParticle = null;              //���翡�� ������ �Ѿ�� ��ƼŬ
  [SerializeField] private SpriteRenderer LampSpr = null;                   //����
  [SerializeField] private ParticleSystem LampFinish = null;                //���� ��
  [SerializeField] private SpriteRenderer[] StageEyes = new SpriteRenderer[3];  //�������� �ܰ� ǥ�� 3��
  [SerializeField] private Sprite OpenEye = null;                               //�� �� ��������Ʈ
  [SerializeField] private ParticleSystem[] StageParticles = new ParticleSystem[3]; //�������� Ŭ���� ��ƼŬ 3��
  private int WoodCount = 4;  //���� ���� ����
  public void Fired()
  {
    switch (WoodCount)
    {
      case 4: //4�� �������� ��ź�� : 1�ܰ� 1/2
        Spr_R[0].enabled = false;
        Spr_R[1].enabled = true;
        Particle_R.Play();
        break;
      case 3: //3�� �������� ��ź�� : 1�ܰ� 2/2
        Spr_L[0].enabled = false;
        Spr_L[1].enabled = true;
        StageEyes[0].sprite = OpenEye;
        StageParticles[0].Play();
        Particle_L.Play();
        MyManager.Set_1();
        break;
      case 2: //2�� �������� ������ : 2�ܰ� 1/2
        Spr_R[0].enabled = true;
        Spr_R[1].enabled = false;
        Particle_R.Play();
        break;
      case 1: //1�� �������� ������ : 2�ܰ� 2/2
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
      case 0: //0�� ������ ������ : 3�ܰ� Ŭ����
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
