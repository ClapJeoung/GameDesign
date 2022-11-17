using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flood : EventTarget
{
  private Vector2 OriginPos = Vector2.zero;
  [SerializeField] private Vector2 TargetPos = Vector2.zero;
  [SerializeField] private float FillingTime = 10.0f;
  [SerializeField] private float DrainingSpeed = 2.0f;
  private float Progress = 0.0f;
  private bool IsDone = false;
  private Transform MyTransform = null;
  [SerializeField] private float[] SpringPos = new float[3];
  [SerializeField] private BoxCollider2D MyCol = null;
  [SerializeField] private ParticleSystem[] WaveParticle = null;
  public void Setup()
  {
    MyTransform = transform;
    OriginPos = MyTransform.position;
  }
  private void Start()
  {
    Setup();
  }
  public override void Active()
  {
  //  if (IsDone) return;
    StopAllCoroutines();
    StartCoroutine(startfilling());
    StartCoroutine(updatewave());
  }
  public override void Deactive()
  {
  //  if (IsDone) return;
    StopAllCoroutines();
    StartCoroutine(startdraining());
  }
  private IEnumerator startfilling()
  {
    WaveParticle[0].Play();
    WaveParticle[1].Play();
    WaveParticle[2].Play();
    while (Progress < FillingTime)
    {
      MyTransform.position = Vector2.Lerp(OriginPos, TargetPos, Progress / FillingTime);
      Progress += Time.deltaTime;
      yield return null;
    }
    IsDone = true;
  }
  private IEnumerator startdraining()
  {
    WaveParticle[0].Stop();
    WaveParticle[1].Stop();
    WaveParticle[2].Stop();
    while (Progress > 0)
    {
      MyTransform.position = Vector2.Lerp(OriginPos, TargetPos, Progress / FillingTime);
      Progress -= Time.deltaTime * DrainingSpeed;
      yield return null;
    }
  }
  private IEnumerator updatewave()
  {
    float _currenty=  MyCol.bounds.size.y / 2.0f-0.2f;
    Vector2[] _newpos = new Vector2[3];
    ParticleSystem.ShapeModule[] _shapes = new ParticleSystem.ShapeModule[3];
    for(int i = 0; i < 3; i++)
    {
      _newpos[i]=new Vector2(SpringPos[i],_currenty);
      _shapes[i] = WaveParticle[i].shape;
    }
    while (true)
    {
      for (int i = 0; i < 3; i++)
      {
        _shapes[i].position = MyTransform.position + (Vector3)_newpos[i]+Vector3.back*0.1f;
        yield return null;
      }
      yield return null;
    }
  }
  private void OnTriggerEnter2D(Collider2D collision)
  {
    if (collision.CompareTag("Torch"))
    {
      StopAllCoroutines();
      StartCoroutine(startdraining());
    }
  }
  private void OnDrawGizmos()
  {
    Gizmos.color = Color.blue;
    float _y =  MyCol.bounds.size.y/2.0f;
    for(int i = 0; i < SpringPos.Length; i++)
    {
      Gizmos.DrawSphere((Vector2)transform.position + new Vector2(SpringPos[i], _y),1.0f);
    }
  }
}
