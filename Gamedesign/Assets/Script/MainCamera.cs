using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
  private Transform PlayerTransform = null; //목표 플레이어 트랜스폼
  private Transform MyTransform = null;     //카메라의 트랜스폼
  [SerializeField] private float CameraSpeed = 1.0f;  //카메라 속도
  [SerializeField] private float MinY = 2.0f; //최소 바닥값
  [SerializeField] private float MaxY = 10.0f;//최대 바닥값
  [SerializeField] private float RespawnMovetime = 1.5f;
  private Vector3 NewPos= Vector3.zero;
  private bool IsDead = false;
  [SerializeField] private ParticleSystem RP_start = null;
  private ParticleSystem.EmissionModule RP_start_emission;
  [SerializeField] private int MinParticle = 50;
  [SerializeField] private int MaxParticle = 180;
  [SerializeField] private ParticleSystem RP_end = null;
  private void Setup()
  {
    MyTransform = transform;
    IsDead = true;
    RP_start_emission = RP_start.emission;
  }
  private void Start()
  {
    Setup();
  }
  private void Update()
  {
    if (IsDead) return;
    NewPos = Vector3.Lerp(MyTransform.position, PlayerTransform.position, Time.deltaTime * CameraSpeed);
    NewPos = new Vector3(NewPos.x, Mathf.Clamp(NewPos.y, MinY, MaxY), -10.0f);
    MyTransform.position = NewPos;
  }
  public void SetPlayer(Transform player)
  {
    PlayerTransform = player;
  }
  public float MoveToPosition(Vector3 newpos,float waittime)
  {
    StartCoroutine(moveto(newpos,waittime));
    return RespawnMovetime + 0.1f;
  }
  private IEnumerator moveto(Vector3 newpos,float waittime)
  {
    IsDead = true;
    float _time = 0.0f;
    Vector3 _originpos = MyTransform.position;
    Vector3 _newpos = newpos + Vector3.back * 10.0f;
    float _particlerate = MinParticle;
   // RP_start_emission.rateOverTime = _particlerate;
   // RP_start.Play();
    while (_time < RespawnMovetime)
    {
      NewPos = Vector3.Lerp(_originpos, _newpos,Mathf.Pow(_time/ RespawnMovetime,2.5f));
      NewPos = new Vector3(NewPos.x, Mathf.Clamp(NewPos.y, MinY, MaxY), -10.0f);
      MyTransform.position = NewPos;
      _particlerate = Mathf.Lerp(MinParticle, MaxParticle, Mathf.Pow((_time / RespawnMovetime), 2.0f));
      RP_start_emission.rateOverTime=_particlerate;
      _time += Time.deltaTime;
      yield return null;
    }
    yield return new WaitForSeconds(waittime);
  //  RP_start.Stop();
  //  RP_end.Play();
    IsDead = false;
  }
}
