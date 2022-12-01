using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SDEffect : MonoBehaviour
{
  public ParticleSystem[] Particles_Idle = null;  //유지 파티클
  public ParticleSystem[] Particle_Delete = null; //제거 파티클
  public Light2D MyLight = null;
  [SerializeField] private float MaxLight = 1.0f;
  [SerializeField] private Transform LightTrans = null;
  [SerializeField] private SouldeathModule SDM = null;
  [SerializeField] private Transform BubbltTrans = null;
  [SerializeField] private SpriteRenderer BubbleSpr = null;
  public int Count = 0;
  public void SetPosition(Vector2[] _newpos, Vector2[] _scale,Vector2 _lightscale,Vector2 _lightpos)
  {
    Count = _newpos.Length;
    ParticleSystem.ShapeModule _shape;
    Vector3 _camerapos=Camera.main.transform.position+Vector3.forward*10.0f;
    for(int i = 0; i < Particles_Idle.Length; i++) 
    {
      if (i < Count)
      {
        Debug.Log($"{_newpos[i]} -> {Camera.main.ScreenToWorldPoint(_newpos[i])})");
        _shape = Particle_Delete[i].shape;
      //  _shape.position = Camera.main.ScreenToWorldPoint(_newpos[i])+Vector3.forward*5.0f+Vector3.up* SDM.RisingDistance;
      Particle_Delete[i].transform.localPosition= Camera.main.ScreenToWorldPoint(_newpos[i]) + Vector3.forward * 5.0f -_camerapos;
        _shape.scale = _scale[i]/100.0f;
        _shape = Particles_Idle[i].shape;
        Particles_Idle[i].transform.localPosition = Camera.main.ScreenToWorldPoint(_newpos[i]) + Vector3.forward * 5.0f - _camerapos;
        //  _shape.position = Camera.main.ScreenToWorldPoint(_newpos[i]) + Vector3.forward * 5.0f + Vector3.up * SDM.RisingDistance;
        _shape.scale = _scale[i]/100.0f;
      }
      LightTrans.position = Camera.main.ScreenToWorldPoint(_lightpos) + Vector3.forward * 9.0f;
      LightTrans.localScale = _lightscale * 1.3f / 100.0f;
      BubbltTrans.position = Camera.main.ScreenToWorldPoint(_lightpos) +Vector3.forward*8.0f;
      BubbltTrans.localScale =new Vector3(_lightscale.x*1.35f,_lightscale.y*1.3f)/100.0f;
    }
  }
  public void TurnOn(int num)
  {
    StartCoroutine(turn(num,MaxLight,0.5f));
    Particles_Idle[num].Play();
  }
  public void TurnOffAll()
  {
    for (int i = 0; i < Count; i++)
    {
      StartCoroutine(turn(i,0.0f,0.0f));
      Particles_Idle[i].Stop();
      Particle_Delete[i].Play();
    }
  }
  public void TurnOffBasic()
  {
    for (int i = 0; i < Count; i++)
    {
      StartCoroutine(turn(i, 0.0f,0.0f));
      Particles_Idle[i].Stop();
    }
  }
  private IEnumerator turn(int num,float _targetint,float _targetA)
  {
    float _time = 0.0f, _targettime = 0.3f;
    float _originint = MyLight.intensity;
    float _originA = BubbleSpr.color.a;
    Color _color = Color.white;
    _color.a = _originA;
    while(_time<_targettime)
    {
      MyLight.intensity = Mathf.Lerp(_originint, _targetint, _time / _targettime);
      _color.a=Mathf.Lerp(_originA,_targetA, _time / _targettime);
      BubbleSpr.color = _color;
      _time += Time.deltaTime; yield return null;
    }
    _color.a = _targetA;
    BubbleSpr.color = _color;
    MyLight.intensity = _targetint;
  }
}
