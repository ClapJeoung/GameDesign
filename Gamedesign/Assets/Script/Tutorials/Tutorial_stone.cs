using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_stone : MonoBehaviour
{
  [SerializeField] private int VertexCount = 5;
  private Transform MyTransform = null;
  private Vector2[] BottomVertex = null;
  private Vector2 Velocity = Vector2.zero;
  [SerializeField] private float Gravity = -9.8f;
  [SerializeField] private float ConveyorSpeed = 5.0f;
  private Vector3 Originpos = Vector2.zero;
  private bool IsPlaying = false;
  public void Setup()
  {
    MyTransform = transform;
    Originpos = MyTransform.position;
    BoxCollider2D _mycol = GetComponent<BoxCollider2D>();
    _mycol.size = GetComponent<SpriteRenderer>().size;
    Bounds mybound = _mycol.bounds;
    mybound.Expand(0.005f);
    BottomVertex = new Vector2[VertexCount];
    float _width = mybound.size.x;
    float _widthunit = _width / (VertexCount - 1);
    float _height = mybound.size.y;
    for (int i = 0; i < VertexCount; i++)
    {
      BottomVertex[i] = new Vector2(-_width / 2 + _widthunit * i, -_height / 2);
    }
  }
  private void Start()
  {
    Setup();
  }
  private void VerticalRaycast()
  {
    RaycastHit2D[] _hit;
    RaycastHit2D _targethit = new RaycastHit2D();
    bool _doitagain = false;
    Vector2 _newpos;
    float _distance;
    LayerMask _layermask = LayerMask.NameToLayer("Wall");
    for (int i = 0; i < VertexCount; i++)
    {
      _newpos = (Vector2)MyTransform.position + BottomVertex[i];
      _distance = Mathf.Abs(Velocity.y) * Time.deltaTime;
      _doitagain = false;

      _hit = Physics2D.RaycastAll(_newpos, Vector2.down, _distance, 1 << LayerMask.NameToLayer("Wall"));

      for (int j = _hit.Length - 1; j >= 0; j--)
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
        Tutorial_Wooden _wooden;
        if (_targethit.transform.TryGetComponent<Tutorial_Wooden>(out _wooden) && !_wooden.IsActive) continue; //목재 비활성화면 무시

        Velocity.y = _targethit.distance * -1;

        break;
      }
    }
  }
  private void Update()
  {
    if (!IsPlaying) return; //현재 스테이지가 내 스테이지가 아니라면 물리효과 대기

    Velocity.y = Gravity;

    VerticalRaycast();

    if (Velocity.y <= 0.05f && Velocity.y >= -0.05f) Velocity.y = 0;
    MyTransform.Translate(Velocity * Time.deltaTime);
  }
  public void Active()
  {
    IsPlaying = true;
  }
  public void Deactive()
  {
    Destroy(gameObject);
  }
}
