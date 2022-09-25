using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour
{
  private bool IsActive = false;  //활성화(부셔지는중)인지
  [SerializeField] private ParticleSystem DustParticle = null;  //먼지 파티클
  [SerializeField] private ParticleSystem DestroyParticle = null;//파괴 파티클
  [SerializeField] private ParticleSystem RestoreParticle = null;//수복 파티클
  [SerializeField] private SpriteRenderer MySpr = null; //스프라이트랜더러
  private Color MyAlpha = Color.white;                  //투명값
  [SerializeField] private float WaitTime = 1.0f;  //밟은 후 파괴까지 걸리는 시간
  [SerializeField] private float DestroyTime = 0.5f;//파괴하는데 걸리는ㅅ ㅣ간
  [SerializeField] private float RestoreTIme = 5.0f;  //파괴 후 수복까지 대기하는 시간

  public void Pressed() //밟혔을때
  {
    if (IsActive) return;

    DustParticle.Play();
    IsActive = true;
    StartCoroutine(DestroyCoroutine());
  }
  private IEnumerator DestroyCoroutine()
  {
    yield return new WaitForSeconds(WaitTime); //DestroyTime만큼 대기

    DustParticle.Stop();  //먼지 파티클 종료
    gameObject.layer = LayerMask.NameToLayer("Default");//기본 레이어로 전환
    DestroyParticle.Play(); //파괴 파티클 실행
    float _time = 0.0f;
    while (_time < DestroyTime)
    {
      MyAlpha.a = 1 - _time / DestroyTime;  //알파값이 1~0로
      _time += Time.deltaTime;
      MySpr.color = MyAlpha;
      yield return null;
    }
    MyAlpha.a = 0;
    MySpr.color = MyAlpha;
    //여기까지 왔으면 다 박살난것

    yield return new WaitForSeconds(RestoreTIme); //수복 타임만큼 대기

    RestoreParticle.Play(); //수복 파티클 실행
    _time = 0.0f;
    while (_time < DestroyTime)
    {
      MyAlpha.a = _time / DestroyTime;  //알파값이 0~1로
      _time += Time.deltaTime;
      MySpr.color = MyAlpha;
      yield return null;
    }
    gameObject.layer = LayerMask.NameToLayer("Breakable");  //레이어 복구로 다시 발판 역할 가능하게

    IsActive = false; //다시 밟혀서 부숴질 수 있도록 변수 초기화
  }
}
