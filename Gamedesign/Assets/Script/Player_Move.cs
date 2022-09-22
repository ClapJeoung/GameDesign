using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Move : MonoBehaviour
{
  private Vector2 Velocity = Vector2.zero;            //현재 속도
  [SerializeField] private float AccelDegree = -3.0f; //이동 입력시 가속도
  [SerializeField] private float GrvtDegree = -9.8f;  //중력가속도
  [SerializeField] private float AccelResist = -3.0f; //멈춤 가속도
  private Vector2 Accel = Vector2.zero;               //현재 가속도
  [SerializeField] private float JumpPower = 8.0f;    //점프 입력시 바뀌는 속도
  [SerializeField] private float MaxXSpeed = 5.0f;    //최대 X 속도(절댓값)
  [SerializeField] private float MaxYSpeed = 10.0f;   //최대 Y 속도(절댓값)
  private BoxCollider2D Col;                          //내 콜라이더
  private Bounds MyBound;                             //내 바운드
  [SerializeField] private int VertexCount = 5;       //충돌 검사할 점 개수
  private Vector2[] Vertex_top, Vertex_bottom,Vertex_right,Vertex_left;//충돌 검사할 점 위치
  private Transform MyTransform;                      //내 트랜스폼
  private bool Jumpable = true;                       //지금 점프 가능한지


  private void Awake()
  {
    Setup();
  }
  public void Setup()
  {
    MyTransform = transform;
    Col = GetComponent<BoxCollider2D>();
    MyBound = Col.bounds;
    Vertex_top = new Vector2[VertexCount]; Vertex_bottom = new Vector2[VertexCount];
    Vertex_right = new Vector2[VertexCount]; Vertex_left = new Vector2[VertexCount]; //버텍스 개수 설정

    float _width = MyBound.size.x;
    float _height = MyBound.size.y;
    float _size_width = _width / VertexCount;
    float _size_height = _height / VertexCount;                                 //바운드 사이즈, 단위 설정

    for (int i = 0; i < VertexCount; i++)
    {
      Vertex_top[i] = new Vector2(-_width / 2 + _size_width * i, _height / 2);
      Vertex_bottom[i] = new Vector2(-_width / 2 + _size_width * i, -_height / 2);
      Vertex_right[i] = new Vector2(_width / 2, -_height / 2 + _size_height * VertexCount);
      Vertex_left[i] = new Vector2(-_width / 2, -_height / 2 + _size_height * VertexCount);
    }
  }
  private void UpdateMove()
  {
    Accel = new Vector2(0, GrvtDegree);
    if (Input.GetKey(KeyCode.D)) Accel.x = AccelDegree;                      //좌측 버튼 : 가속도가 +
    else if (Input.GetKey(KeyCode.A)) Accel.x = -AccelDegree;                 //우측 버튼 : 가속도가 -
    else Accel.x = (Velocity.x != 0 ? Mathf.Sign(Velocity.x) : 0) * AccelResist;//아무것도 안 누름 : 가속도가 속도 반대로


    if (Input.GetKeyDown(KeyCode.Space) && Jumpable) { Velocity.y = JumpPower; Jumpable = false; } //스페이스바는 점프



    Velocity += Accel * Time.deltaTime;

    Velocity = new Vector2(Mathf.Clamp(Velocity.x,-MaxXSpeed,MaxXSpeed), Mathf.Clamp(Velocity.y,-MaxYSpeed,MaxYSpeed));

    RaycastVertical();
    RaycastHorizontal();

    MyTransform.Translate(Velocity * Time.deltaTime);
  }
  private void RaycastVertical()  //위아래 검사
  {
    Vector2[] _pos = Velocity.y > 0 ? Vertex_top : Vertex_bottom; //선이 시작되는 위치(바운드 기준)
    Vector2 _dir = Velocity.y > 0?Vector2.up : Vector2.down;      //선이 발사되는 위치
    Vector3 _newpos = Vector3.zero;                               //선이 시작되는 위치(플레이어 기준)
    int _layermask;          //레이어마스크(int)
    float _distance = Velocity.y * Time.deltaTime;                //선이 발사되는 거리
    RaycastHit2D _hit;                                            //선이 발사되고 닿은 곳의 정보

    for (int i = 0; i < VertexCount; i++)
    {
      _layermask = 1 << LayerMask.NameToLayer("Wall");  //벽 검사
      _newpos = MyTransform.position + (Vector3)_pos[i];
      _hit = Physics2D.Raycast(_newpos, _dir, _distance, _layermask) ;
      if (_hit.transform != null)
      {
        if (Velocity.y < 0) Jumpable = true;
        Velocity.y = _hit.distance;
        break;
      }
      if (Velocity.y > 0) continue;
      _layermask = 1 << LayerMask.NameToLayer("Upper");  //윗블록 검사
      _hit = Physics2D.Raycast(_newpos, _dir, _distance, _layermask);
      if (_hit.transform != null)
      {
        if (Velocity.y < 0) Jumpable = true;
        Velocity.y = _hit.distance;
        break;
      }
    }
  }
  private void RaycastHorizontal()//좌우 검사
  {
    Vector2[] _pos = Velocity.x > 0 ? Vertex_right : Vertex_left; //선이 시작되는 위치(바운드 기준)
    Vector2 _dir = Velocity.x > 0 ? Vector2.right : Vector2.left;      //선이 발사되는 위치
    Vector3 _newpos = Vector3.zero;                               //선이 시작되는 위치(플레이어 기준)
    int _layermask = 1 << LayerMask.NameToLayer("Wall");          //레이어마스크(int)
    float _distance = Velocity.x * Time.deltaTime;                //선이 발사되는 거리
    RaycastHit2D _hit;                                            //선이 발사되고 닿은 곳의 정보

    for (int i = 0; i < VertexCount; i++)
    {
      _newpos = MyTransform.position + (Vector3)_pos[i];
      _hit = Physics2D.Raycast(_newpos, _dir, _distance, _layermask);
      if (_hit.transform != null)
      {
        Velocity.x = _hit.distance;
        break;
      }
    }
  }
  private void Update()
  {
    UpdateMove();
  }
}
