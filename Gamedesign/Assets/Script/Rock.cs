using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour
{
  [SerializeField] private int VertexCount = 5;
  private Transform MyTransform = null;
  private Vector2[] BottomVertex = null;
  private Vector2 Velocity = Vector2.zero;
  [SerializeField] private float Gravity = -9.8f;
  private int Conveyor = 0;
  [SerializeField] private float ConveyorSpeed = 5.0f;
  private StageCollider MySC = null;
  private Vector2 Originpos = Vector2.zero;
  private bool IsPlaying = false;
  private float ResetTime = 3.0f;
  public void Setup()
  {
    MySC = transform.parent.GetComponent<StageCollider>();
    MySC.SetOrigin(this);
    MyTransform = transform;
    Originpos = MyTransform.position;
    BoxCollider2D _mycol = GetComponent<BoxCollider2D>();
    _mycol.size = GetComponent<SpriteRenderer>().size;
    Bounds mybound= _mycol.bounds;
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

      string _name = "�ɸ��� : ";
      for(int j = 0; j < _hit.Length; j++)
      {
        _name += _hit[j].transform.name + " ";
      }
 //     Debug.Log(_name);
      for(int j=_hit.Length-1; j>=0; j--)
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
        Wooden iswooden;
        if (_targethit.transform.TryGetComponent<Wooden>(out iswooden) && !iswooden.IsActive) continue; //���� ��Ȱ��ȭ�� ����

        Velocity.y = _targethit.distance * -1;
   //     Debug.DrawRay((Vector2)MyTransform.position + BottomVertex[i], Vector3.down, Color.green,_targethit.distance);
        //    Debug.Log(_hit.transform.tag);

        if (_targethit.transform.CompareTag("Breakable"))
          _targethit.transform.GetComponent<Breakable>().Pressed(); //��Ƽ� �ν����� �̺�Ʈ ����
        else if (_targethit.transform.CompareTag("Conveyor_R")) Conveyor = 1;
        else if (_targethit.transform.CompareTag("Conveyor_L")) Conveyor = -1;
        break;
      }
    }
  }
  private void Update()
  {
    if (GameManager.Instance.CurrentSC != MySC) return; //���� ���������� �� ���������� �ƴ϶�� ���

    Velocity.y = Gravity;

    VerticalRaycast();

    if (Velocity.y <= 0.05f && Velocity.y >= -0.05f) Velocity.y = 0;
    MyTransform.Translate((Velocity + Vector2.right * ConveyorSpeed * Conveyor) * Time.deltaTime);
  }
  public void Resetpos() => StartCoroutine(resetpos());
  private IEnumerator resetpos()
  {
    IsPlaying = false;
    float _time = 0.0f;
    Vector2 _currentpos = MyTransform.position;
    while (_time < ResetTime)
    {
      MyTransform.position=Vector2.Lerp(_currentpos,Originpos,Mathf.Sqrt(_time/ResetTime));
      _time += Time.deltaTime;
      yield return null;
    }
    IsPlaying = true;
  }
}
