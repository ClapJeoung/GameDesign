using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;
  public static UIManager Instance { get { return instance; } }
  public Image Button_A,Button_D,Button_Right,Button_Left;
  private void Awake()
  {
    instance = this;
  }

  private void Update()
  {
    if (Input.GetKeyDown(KeyCode.A)) Button_A.color = Color.green;
    else if (Input.GetKeyUp(KeyCode.A)) Button_A.color = Color.white;
    if (Input.GetKeyDown(KeyCode.D)) Button_D.color = Color.green;
    else if(Input.GetKeyUp(KeyCode.D)) Button_D.color = Color.white;
    if (Input.GetKeyDown(KeyCode.RightArrow)) Button_Right.color = Color.green;
    else if(Input.GetKeyUp(KeyCode.RightArrow))Button_Right.color = Color.white;
    if (Input.GetKeyDown(KeyCode.LeftArrow)) Button_Left.color = Color.green;
    else if (Input.GetKeyUp(KeyCode.LeftArrow)) Button_Left.color = Color.white;
  }
}
