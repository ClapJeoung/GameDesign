using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainButtonManager : MonoBehaviour
{
    private static MainButtonManager instance;
  public static MainButtonManager Instance {get{return instance;}}
  private void Awake()
  {
    instance = this;
    ButtonIndex = 0;
  }
  private int buttonindex = 0;
  private int ButtonIndex
  {
    get { return buttonindex; }
    set
    {
      MyButtons[buttonindex].UnSelect();
      if (value < 0) buttonindex = MyButtons.Length - 1;
      else if (value == MyButtons.Length) buttonindex = 0;
      else buttonindex = value;
      MyButtons[buttonindex].Select();
    }
  }
  [SerializeField] private MainButtons[] MyButtons = null;
  private void Update()
  {
    if (Input.GetKeyDown(KeyCode.DownArrow)) ButtonIndex--;
    if(Input.GetKeyDown(KeyCode.UpArrow)) ButtonIndex++;
    if (Input.GetKeyDown(KeyCode.Return))
    {
      for(int i=0; i<MyButtons.Length; i++)
      { if (i == ButtonIndex) continue;
        MyButtons[i].UnPressed();
      }
      MyButtons[ButtonIndex].Pressed();
    }
  }
}
