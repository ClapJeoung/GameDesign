using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
  private Transform PlayerTransform = null; //��ǥ �÷��̾� Ʈ������
  private Transform MyTransform = null;     //ī�޶��� Ʈ������
  [SerializeField] private float CameraSpeed = 1.0f;  //ī�޶� �ӵ�
  [SerializeField] private float MinY = 2.0f; //�ּ� �ٴڰ�
  [SerializeField] private float MaxY = 10.0f;//�ִ� �ٴڰ�
  [SerializeField] private float RespawnMovetime = 1.5f;
  private Vector3 NewPos= Vector3.zero;
  private bool IsDead = false;
  private void Setup()
  {
    MyTransform = transform;
    IsDead = true;
  }
  private void Start()
  {
    Setup();
  }
  private void Update()
  {
    if (IsDead) return;
    NewPos = Vector3.Lerp(MyTransform.position, PlayerTransform.position, Time.deltaTime * CameraSpeed);
    NewPos = new Vector3(NewPos.x, Mathf.Clamp(NewPos.y, MinY, MaxY), -10.0f);
    MyTransform.position = NewPos;
  }
  public void SetPlayer(Transform player)
  {
    PlayerTransform = player;
  }
  public float MoveToPosition(Vector3 newpos,float waittime)
  {
    StartCoroutine(moveto(newpos,waittime));
    return RespawnMovetime + 0.1f;
  }
  private IEnumerator moveto(Vector3 newpos,float waittime)
  {
    IsDead = true;
    float _time = 0.0f;
    Vector3 _originpos = MyTransform.position;
    Vector3 _newpos = newpos + Vector3.back * 10.0f;
    while (_time < RespawnMovetime)
    {
      NewPos = Vector3.Lerp(_originpos, _newpos,Mathf.Sqrt(_time/ RespawnMovetime));
      NewPos = new Vector3(NewPos.x, Mathf.Clamp(NewPos.y, MinY, MaxY), -10.0f);
      MyTransform.position = NewPos;
      _time += Time.deltaTime;
      yield return null;
    }
    yield return new WaitForSeconds(waittime);
    IsDead= false;
  }
}
