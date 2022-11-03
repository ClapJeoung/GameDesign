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
        _targethit = _hit[j]; //_hit�� �� �ں���(���� ���� �ɸ� �ͺ���) _targethit�� ����
        if (_targethit.transform == transform)  //_targethit�� �ִ°� �ڽ��� ���
        {
          if (_hit.Length == 1) _doitagain = true;  //�׸��� ������ ��Ұ� �ڽ��� ����� �Ѿ��
          else continue;  //���� �� �˻��� ��Ұ� �ִٸ� �������� �Ѿ��
        }
        break;  //�ڽ��� �ƴ� ��� �״�� ����
      }
      if (_doitagain) continue;

      if (_targethit.transform != null)
      {
        Tutorial_Wooden _wooden;
        if (_targethit.transform.TryGetComponent<Tutorial_Wooden>(out _wooden) && !_wooden.IsActive) continue; //���� ��Ȱ��ȭ�� ����

        Velocity.y = _targethit.distance * -1;

        break;
      }
    }
  }
  private void Update()
  {
    if (!IsPlaying) return; //���� ���������� �� ���������� �ƴ϶�� ����ȿ�� ���

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
