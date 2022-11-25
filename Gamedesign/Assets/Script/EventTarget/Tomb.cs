using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class Tomb : EventTarget
{
  [SerializeField] private MainCamera MyCamera = null;
  [SerializeField] private float CameraMoveTime = 2.0f;
  private float CameraShakeTime = 0.0f;
  [SerializeField] private Player_Move MyPlayer = null;
  [SerializeField] private Transform LightTransform = null;
  [SerializeField] private ParticleSystem PlayerParticle = null;
  [SerializeField] private Torch_pivot MyTorch = null;
  [SerializeField] private ParticleSystem TorchParticle = null;
  [SerializeField] private Vector2 TorchPos = Vector2.zero;
  [SerializeField] private float TorchMoveTime = 3.0f;
  [SerializeField] private float TorchFireTime = 3.0f;
  [SerializeField] private float PlayerAboveTime = 3.0f;  //플레이어가 부양할때까지 걸리는 시간
  [SerializeField] private Vector2 PlayerAbovePos = Vector2.zero; //플레이어가 부양하는 위치
  [SerializeField] private float PlayerFireTime = 10.0f;  //플레이어가 불타는 시간
  [SerializeField] private int PlayerParticleCount = 180; //플레이어 불 파티클 최대 개수
  [SerializeField] private float LightingOnTime_y = 1.5f;  //가운데 빛이 끝까지 커지는 시간(위로)
  [SerializeField] private float LightingOnSize_y = 0.0f;
  [SerializeField] private float LightingOnTime_x = 10.0f;  //가운데 빛이 끝까지 커지는 시간(좌우로)
  [SerializeField] private float LightSizeupSpeed = 0.0f;
  [SerializeField] private float LightSpeedupTime = 7.0f;
  [SerializeField] private Vector2 EndPos = Vector2.zero;   //카메라가 이동되는 곳
  [SerializeField] private Light2D WorldLight = null;
  [SerializeField] private float LightUpSpeed = 4.0f;
  [SerializeField] private float SpaceWaitTime = 0.5f;    //연출 중간중간 대기시간
  private float CameraLight_origin = 0.0f;
  public override void Active()
  {
    CameraShakeTime = LightingOnTime_x + LightingOnTime_y - CameraMoveTime;
    StartCoroutine(actioncoroutine());
  }
  private IEnumerator actioncoroutine()
  {
    MyPlayer.EndingEngage();
    MyCamera.Ending_move(EndPos, CameraMoveTime);
    MyTorch.MoveToTomb(TorchPos, TorchMoveTime);
    yield return new WaitForSeconds(TorchMoveTime + SpaceWaitTime);
    ParticleSystem.ShapeModule _torchparticleshape = TorchParticle.shape;
    _torchparticleshape.position = MyTorch.transform.position+Vector3.back*2.0f;
    TorchParticle.Play();
    MyTorch.End();
    yield return new WaitForSeconds(SpaceWaitTime);
    MyPlayer.Ending(PlayerAbovePos, PlayerAboveTime);
    yield return new WaitForSeconds(PlayerAboveTime );
    StartCoroutine(playerparticle());
    yield return new WaitForSeconds(SpaceWaitTime);
    MyCamera.Ending_shake(CameraShakeTime);
    StartCoroutine(lightsize());
    StartCoroutine(lightup());
    yield return new WaitForSeconds(SpaceWaitTime);
  }
  private IEnumerator playerparticle()  //플레이어 불타는 파티클 위치 지정하고 실행
  {
    Transform _playertrans = MyPlayer.transform;
    float _time = 0.0f;
    ParticleSystem.ShapeModule _shape= PlayerParticle.shape;
    ParticleSystem.EmissionModule _emission= PlayerParticle.emission;
    _emission.rateOverTime = 0.0f;
    PlayerParticle.Play();
    while (_time < PlayerFireTime)
    {
      _shape.position = _playertrans.position+Vector3.back;
      _emission.rateOverTime =Mathf.Lerp(0.0f, PlayerParticleCount,(_time / PlayerFireTime));
      _time += Time.deltaTime;
      yield return null;
    }
  }
  private IEnumerator lightsize() //빛 크기 설정
  {
    Vector3 _minsize = LightTransform.localScale;
    float _time = 0.0f;
    GameManager.Instance.TurnOffLights();
    while (_time < LightingOnTime_y)  //LightingOnTime_y 동안 빛 y 축으로 늘리고
    {
      LightTransform.localScale = _minsize+ Vector3.up * (_time / LightingOnTime_y) * LightingOnSize_y+Vector3.forward;
      _time += Time.deltaTime;
      yield return null;
    }
 //    LightTransform.GetComponent<SpriteRenderer>().sortingOrder = 1;
    Vector3 _originsize = _minsize+ new Vector3(0.0f, LightingOnSize_y,1.0f);
    _time = 0.0f;
    float _offset = 0.05f;
    while (_time<15.0f) //이후 15초 동안 쭉 좌우로 늘림
    {
      LightTransform.localScale +=_offset* Vector3.one * Time.deltaTime * LightSizeupSpeed;
      _time += Time.deltaTime;
      if (_time > LightSpeedupTime)
      {
        _offset = 5.0f; 
        LightTransform.Translate(Vector3.down * Time.deltaTime * _offset * LightSizeupSpeed);
      }
      yield return null;
    }
    //어느정도 시간 흘렀으면 페이드아웃
    UIManager.Instance.FadeOut(4.0f,false);
    yield return new WaitForSeconds(4.5f);
    SceneManager.LoadScene(0);
  }
  private IEnumerator lightup() //카메라 라이트도 쭉 증가
  {
    CameraLight_origin = WorldLight.intensity;  //카메라 빛 기본값
    while (WorldLight.intensity<10.0f)
    {
      WorldLight.intensity += Time.deltaTime * LightUpSpeed;
      WorldLight.pointLightOuterRadius += Time.deltaTime * LightUpSpeed;
      yield return null;
    }
  }
}
