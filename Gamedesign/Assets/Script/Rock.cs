using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour
{
  [SerializeField] private BoxCollider2D MyCol = null;
  [SerializeField] private int VertexCount = 5;
  private Transform MyTransform = null;
  private Vector2[] BottomVertex = null;
  private Vector2 Velocity = Vector2.zero;
  [SerializeField] private float Gravity = -9.8f;
  private int Conveyor = 0;
  [SerializeField] private float ConveyorSpeed = 5.0f;
  public void Setup()
  {
    MyTransform = transform;
    Bounds mybound=MyCol.bounds;
    mybound.Expand(0.01f);
    BottomVertex = new Vector2[VertexCount];
    float _width = mybound.size.x;
    float _widthunit=_width / (VertexCount - 1);
    float _height = mybound.size.y;
    for(int i = 0; i < VertexCount; i++)
    {
      BottomVertex[i] = new Vector2(- _width/2 + _widthunit * i, -_height / 2);
    }
  }
  private void Start()
  {
    Setup();
  }
  private void VerticalRaycast()
  {
    RaycastHit2D[] _hit;
    RaycastHit2D _targethit=new RaycastHit2D();
    bool _doitagain = false;
    Vector2 _newpos;
    float _distance;
    LayerMask _layermask = LayerMask.NameToLayer("Wall");
    for (int i = 0; i < VertexCount; i++)
    {
      _newpos = (Vector2)MyTransform.position + BottomVertex[i];
      _distance =Mathf.Abs( Velocity.y) * Time.deltaTime;
      _doitagain = false;

      _hit = Physics2D.RaycastAll(_newpos, Vector2.down,_distance, 1 << LayerMask.NameToLayer("Wall"));
   //   Debug.DrawRay(_newpos, Vector3.down, Color.red, _distance);

      if (_hit.Length == 0) continue;

      string _name = "걸린거 : ";
      for(int j = 0; j < _hit.Length; j++)
      {
        _name += _hit[j].transform.name + " ";
      }
 //     Debug.Log(_name);
      for(int j=_hit.Length-1; j>=0; j--)
      {
        _targethit = _hit[j]; //_hit의 맨 뒤부터(제일 먼저 걸린 것부터) _targethit에 대입
        if (_targethit.transform == transform)  //_targethit에 있는게 자신일 경우
        {
          if (_hit.Length == 1) _doitagain = true;  //그리고 유일한 요소가 자신일 경우라면 넘어가기
          else continue;  //아직 더 검사할 요소가 있다면 다음으로 넘어가기
        }
        break;  //자신이 아닐 경우 그대로 진행
      }
      if (_doitagain) continue;

      if (_targethit.transform != null)
      {
        Wooden iswooden;
        if (_targethit.transform.TryGetComponent<Wooden>(out iswooden) && !iswooden.IsActive) continue; //목재 비활성화면 무시

        Velocity.y = _targethit.distance * -1;
   //     Debug.DrawRay((Vector2)MyTransform.position + BottomVertex[i], Vector3.down, Color.green,_targethit.distance);
        //    Debug.Log(_hit.transform.tag);

        if (_targethit.transform.CompareTag("Breakable"))
          _targethit.transform.GetComponent<Breakable>().Pressed(); //밟아서 부숴지는 이벤트 실행
        else if (_targethit.transform.CompareTag("Conveyor_R")) Conveyor = 1;
        else if (_targethit.transform.CompareTag("Conveyor_L")) Conveyor = -1;
        break;
      }
    }
  }
  private void Update()
  {
    Velocity.y = Gravity;

    VerticalRaycast();

    if (Velocity.y <= 0.05f && Velocity.y >= -0.05f) Velocity.y = 0;
    MyTransform.Translate((Velocity + Vector2.right * ConveyorSpeed * Conveyor) * Time.deltaTime);
  }
}
