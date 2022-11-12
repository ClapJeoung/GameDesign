using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEvent : MonoBehaviour
{
  public enum EventType { Lerp,Offset}
  public EventType CameraEventType=EventType.Lerp;
  [Space(10)]
  [SerializeField] private Vector3 Lerp_targetpos = Vector3.zero;
  [SerializeField] private float Lerp_ratio = 0.5f;
  [Space(5)]
  [SerializeField] private Vector2 Offset_position= Vector2.zero;
  [Space(5)]
  [SerializeField] private float CameraSizeRatio = 1.0f;
  [SerializeField] private float CameraSizeTime = 1.0f;

  private void OnTriggerEnter2D(Collider2D collision)
  {
    if (!collision.CompareTag("Player")) return;
    if (CameraEventType == EventType.Lerp) GameManager.Instance.MyCamera.SetSpecialCamera(Lerp_targetpos, Lerp_ratio, CameraSizeRatio, CameraSizeTime);
    else GameManager.Instance.MyCamera.SetSpecialCamera(Offset_position, CameraSizeRatio, CameraSizeTime);
  }
  private void OnTriggerExit2D(Collider2D collision)
  {
    if (!collision.CompareTag("Player")) return;
    GameManager.Instance.MyCamera.ResetCamera(); 
  }

  private void OnDrawGizmos()
  {
    if (CameraEventType != EventType.Lerp) return;
    Gizmos.color = Color.blue;
    Gizmos.DrawSphere(Lerp_targetpos, 0.5f);
  }

}
