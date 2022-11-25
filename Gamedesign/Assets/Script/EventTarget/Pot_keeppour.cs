using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pot_keeppour : EventTarget
{
  public override void Active()
  {
    WaterFallTrans.localScale = new Vector3(0.5f, 0.0f, 1.0f);
    IsActive = true;
    TopParticle.Play();
    BottomParticle.Play();
  }
  public override void Deactive()
  {
    IsActive = false;
    StartCoroutine(turnoff());
  }
  private Vector2[] RayStartPoint = new Vector2[3];         //광선 검사용 위치 3개
  [SerializeField] private float StartPos = 0.0f;           //물 시작하는 위치
  [SerializeField] private float MaxLength = 0.0f;          //최대 물 길이
  [SerializeField] private Transform WaterFallTrans = null; //물 트랜스폼
  private float SizeOffset = 1.0f / 4.0f;                   //크기 배율 보정(기본적으로 길이 4일듯)
  private StageCollider MySC = null;                        //내가 속한 스테이지 콜라이더
  [SerializeField] private ParticleSystem TopParticle = null; //물 시작점 파티클
  [SerializeField] private ParticleSystem BottomParticle = null;//물 떨어지는 곳 파티클
  private ParticleSystem.ShapeModule BottomShape;
  private bool IsActive = true;
  public void Setup()
  {
    Bounds _bound = transform.GetComponent<BoxCollider2D>().bounds;
    RayStartPoint[0] =transform.position+ new Vector3(-_bound.size.x / 2.0f, StartPos);
    RayStartPoint[1] = transform.position + new Vector3(0.0f, StartPos);
    RayStartPoint[2] = transform.position + new Vector3(+_bound.size.x / 2.0f,  StartPos);
    BottomShape = BottomParticle.shape;
    TopParticle.Play();
    BottomParticle.Play();
    MySC = transform.parent.GetComponent<StageCollider>();
    MySC.SetOrigin(this);
  }
  private void Start()
  {
    Setup();
  }
  private void Update()
  {
    if (GameManager.Instance.CurrentSC != MySC||!IsActive) return;
    UpdateRay();
  }
  private void UpdateRay()
  {
    RaycastHit2D _hit;
    int _layer = 1 << LayerMask.NameToLayer("Player");
    for (int i = 0; i < RayStartPoint.Length; i++)
    {
      _hit = Physics2D.Raycast(RayStartPoint[i], Vector2.down, MaxLength, _layer);
   //   Debug.DrawLine(RayStartPoint[i], RayStartPoint[i] + Vector2.down * MaxLength,Color.red) ;
      if (_hit.transform != null)
      {
        Debug.Log("레후~");
        WaterFallTrans.localScale=new Vector3(0.5f,_hit.distance*SizeOffset+0.1f,1.0f);
        BottomShape.position = new Vector3(0.0f, _hit.point.y - transform.position.y, -1.0f);
        return;
      }
    }
    WaterFallTrans.localScale = new Vector3(0.5f, MaxLength * SizeOffset, 1.0f);
    BottomShape.position =  Vector3.down * (MaxLength- StartPos);
  }
  private IEnumerator turnoff()
  {
    
    float _time = 0.0f, _targettime = 0.4f,_originsize=WaterFallTrans.localScale.y,_targetsize=0.0f;
    while (_time < _targettime)
    {
      WaterFallTrans.localScale = new Vector3( Mathf.Lerp(0.5f, _targetsize, _time / _targettime),_originsize, 1.0f);
      _time += Time.deltaTime; yield return null;
    }
    WaterFallTrans.localScale = new Vector3(0.0f, _originsize, 1.0f);
    TopParticle.Stop();
    BottomParticle.Stop();
  }
  private void OnDrawGizmos()
  {
    Gizmos.color = Color.blue;
    Gizmos.DrawLine((Vector2)transform.position+Vector2.up*StartPos, (Vector2)transform.position + Vector2.up * StartPos + Vector2.down * MaxLength);
  }
}
