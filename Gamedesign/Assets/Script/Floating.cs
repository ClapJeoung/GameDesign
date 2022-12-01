using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floating : MonoBehaviour
{
  [SerializeField] private float Range = 1.0f;
  [SerializeField] private float Speed = 180.0f;

  private void Start()
  {
    StartCoroutine(floating());
  }
  private IEnumerator floating()
  {
    float _time = 0.0f, _targettime = 1.0f;
    Transform MyTrans=transform;
    Vector3 _origin = MyTrans.position, _new = Vector3.zero; ;
    while (true)
    {
      _new.y=Range*Mathf.Sin(_time*Speed*Mathf.Deg2Rad);
      MyTrans.position = _origin + _new;
      _time += Time.deltaTime * Speed;
      yield return null;
    }
  }
  private void OnDrawGizmos()
  {
    Gizmos.color = Color.red;
    Gizmos.DrawLine(transform.position + Vector3.up * Range, transform.position + Vector3.down * Range);
  }
}
