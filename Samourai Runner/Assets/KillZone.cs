using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillZone : MonoBehaviour
{
  public Canvas Canvas;
  
  
  void Start(){
        Canvas.enabled = false;
    }
  private void OnTriggerEnter2D(Collider2D Collider){


    if(Collider.CompareTag("Player")){
        Canvas.enabled = true;
    
    }

}
}
