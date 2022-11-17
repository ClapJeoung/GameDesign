using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skull : MonoBehaviour
{
  [SerializeField] private Sprite[] Sprites = null;
   public void Setup(Vector2 currentpos,int rot)
  {
    GetComponent<SpriteRenderer>().sprite=Sprites[Random.Range(0,Sprites.Length)];
    Vector2 _newpos = new Vector2(currentpos.x , Mathf.Round(currentpos.y * 2.0f) / 2.0f);
    transform.position = _newpos;
//    Debug.Log($"{currentpos} -> {_newpos}");
    //180,90,0,-90
    float _z = 180.0f - 90.0f * rot;
    transform.eulerAngles = Vector3.forward * rot;
  }
}
