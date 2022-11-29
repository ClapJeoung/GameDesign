using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Souldead")]
public class SouldeathModule : ScriptableObject
{
  public float RisingTime = 0.8f;         //�÷��̾� ��������
  public float RisingDistance = 1.5f;          //�������� �Ÿ�
  public float ShakeWaittime = 0.3f;      //ȶ�� ���� ���� �������� ���ð�
  public float Text_Waittime = 1.0f;      //ȶ�� ���� ���� �ؽ�Ʈ���� ���ð�
  public float Text_AppearingTime = 2.5f; //�ؽ�Ʈ ��ü ��±��� �ɸ��� �ð�
  public float Text_ShakeCount = 5.0f;    //�ؽ�Ʈ �ʴ� ���� Ƚ��
  public float Text_ShakeDeg = 3.0f;      //�ؽ�Ʈ ���� ����
                                          //  public float ParticleWaitTime = 0.25f;   //�÷��̾� �������� ��ƼŬ ���۱��� �ɸ��� �ð�
  public float Text_DestroyWaittime = 3.0f;//�ؽ�Ʈ �� ������ ���� ������ �ɸ��� �ð�
  public float Particle_minspeed = 8.5f, Particle_maxspeed = 25.0f;
  public float Particle_mincount = 5.0f, Particle_maxcount = 80.0f;
  public float Gain_mincount = 10.0f, Gain_maxcount = 40.0f;
  public float Flood_originpos = -12.0f;//ȫ�� ���� ��ġ
  public float Flood_targetpos = 0.5f;  //ȫ�� ���� ��ġ
  public float Flood_time = 6.0f;       //ī�޶� �� ���� �ð�
  public float Flood_shakedeg = 0.25f;   //ī�޶� ���� ����
  public int Flood_shakecount = 15;      //�ʴ� ī�޶� ���� Ƚ��
  public float Flood_angle = -8;     //ī�޶� ȸ�� ũ��
  public float Flood_FadeOutTime = 7.5f;  //ī�޶� ���̵�ƿ��ϴ� �ð�
}
