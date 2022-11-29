using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Souldead")]
public class SouldeathModule : ScriptableObject
{
  public float RisingTime = 0.8f;         //플레이어 공중으로
  public float RisingDistance = 1.5f;          //떠오르는 거리
  public float ShakeWaittime = 0.3f;      //횃불 꺼진 직후 진동까지 대기시간
  public float Text_Waittime = 1.0f;      //횃불 꺼진 직후 텍스트까지 대기시간
  public float Text_AppearingTime = 2.5f; //텍스트 전체 출력까지 걸리는 시간
  public float Text_ShakeCount = 5.0f;    //텍스트 초당 진동 횟수
  public float Text_ShakeDeg = 3.0f;      //텍스트 진동 범위
                                          //  public float ParticleWaitTime = 0.25f;   //플레이어 떠오르고 파티클 시작까지 걸리는 시간
  public float Text_DestroyWaittime = 3.0f;//텍스트 다 나오고 지울 때까지 걸리는 시간
  public float Particle_minspeed = 8.5f, Particle_maxspeed = 25.0f;
  public float Particle_mincount = 5.0f, Particle_maxcount = 80.0f;
  public float Gain_mincount = 10.0f, Gain_maxcount = 40.0f;
  public float Flood_originpos = -12.0f;//홍수 시작 위치
  public float Flood_targetpos = 0.5f;  //홍수 종료 위치
  public float Flood_time = 6.0f;       //카메라가 다 잠기는 시간
  public float Flood_shakedeg = 0.25f;   //카메라 진동 정도
  public int Flood_shakecount = 15;      //초당 카메라 진동 횟수
  public float Flood_angle = -8;     //카메라 회전 크기
  public float Flood_FadeOutTime = 7.5f;  //카메라 페이드아웃하는 시간
}
