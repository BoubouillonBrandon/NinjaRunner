using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ennemicontroller : MonoBehaviour
{
    public Animation anim;
    public bool mort;
   void Update(){

    if (mort == true){

        Mourir();
    }


   }

   public void Anim(){

    anim = GetComponent<Animation>();

    



   }
   public void Mourir()
    {
       Destroy(gameObject);
    }
}
