using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UIManager : MonoBehaviour
{
    private static UIManager instance;
  public static UIManager Instance { get { return instance; } }
  public Image Button_A,Button_D,Button_Right,Button_Left;
  [SerializeField] private Image TextBackground=null;
  [SerializeField] private TextMeshProUGUI MyText = null;
  private void Awake()
  {
    instance = this;
    MyText.ForceMeshUpdate();
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
  private List<int> DeathIndexes=new List<int>();
  private List<int> ReplayIndexes=new List<int>();
  public void Replaytext()
  {

  }
  public void Deathtext()
  {

  }
  private IEnumerator showbackground(bool isdeath)  //백그라운드 투명->불투명, 위치 이동
  {
    int nextindex;
    if (isdeath)
    {
      nextindex = Random.Range(0, Textscripts.DeathCount);
      while (DeathIndexes.Contains(nextindex))
      {
        nextindex = Random.Range(0, Textscripts.DeathCount);
      }
      DeathIndexes.Add(nextindex);
      if (DeathIndexes.Count == Textscripts.DeathCount) DeathIndexes.Clear();
    }
    else
    {
      nextindex = Random.Range(0, Textscripts.ReplayCount);
      while (ReplayIndexes.Contains(nextindex))
      {
        nextindex = Random.Range(0, Textscripts.ReplayCount);
      }
      ReplayIndexes.Add(nextindex);
      if (ReplayIndexes.Count == Textscripts.ReplayCount) ReplayIndexes.Clear();
    }

    yield return null;
  }
}
public static class Textscripts
{
  public static int DeathCount=2 ;
  public static int ReplayCount = 2; 
}
