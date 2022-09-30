using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
  private Transform PlayerTransform = null; //목표 플레이어 트랜스폼
  private Transform MyTransform = null;     //카메라의 트랜스폼
  [SerializeField] private float CameraSpeed = 1.0f;  //카메라 속도
  [SerializeField] private float MinY = 2.0f; //최소 바닥값
  [SerializeField] private float MaxY = 10.0f;//최대 바닥값
  private Vector3 NewPos= Vector3.zero;
  private void Awake()
  {
    MyTransform = transform;
  }

  private void Update()
  {
    if (PlayerTransform == null) return;
    NewPos = Vector3.Lerp(MyTransform.position, PlayerTransform.position, Time.deltaTime * CameraSpeed);
    NewPos = new Vector3(NewPos.x, Mathf.Clamp(NewPos.y, MinY, MaxY), -10.0f);
    MyTransform.position = NewPos;
  }
  public void SetPlayer(Transform player)
  {
    PlayerTransform = player;
  }
  public void MoveToPosition(Vector3 newpos)
  {
    PlayerTransform = null;
    StartCoroutine(moveto(newpos));
  }
  private IEnumerator moveto(Vector3 newpos)
  {
    float _time = 0.0f;
    Vector3 _originpos = MyTransform.position;
    Vector3 _newpos = newpos + Vector3.back * 10.0f;
    float _movetime = 1.0f;
    while (_time < _movetime)
    {
      NewPos = Vector3.Lerp(_originpos, _newpos,Mathf.Sqrt(_time/ _movetime));
      NewPos = new Vector3(NewPos.x, Mathf.Clamp(NewPos.y, MinY, MaxY), -10.0f);
      MyTransform.position = NewPos;
      _time += Time.deltaTime;
      yield return null;
    }
  }
}
