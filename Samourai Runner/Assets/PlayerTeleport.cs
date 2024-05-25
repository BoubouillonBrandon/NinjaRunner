using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTeleport : MonoBehaviour
{
    private GameObject currentTeleporter;
    private bool TeleporterPris;

void Update(){

    if ( currentTeleporter != null && TeleporterPris == false)
    {
        transform.position = currentTeleporter.GetComponent<Teleporter>().GetDestination().position;
    }
    
}

private void OnTriggerEnter2D(Collider2D Collider){

    if(Collider.CompareTag("Teleporter")){

        currentTeleporter = Collider.gameObject;
        StartCoroutine(Teleport());
    }

}

private void OnTriggerExit2D(Collider2D Collider){

    if(Collider.CompareTag("Teleporter")){
        if ( Collider.gameObject == currentTeleporter)
        {
            currentTeleporter = null;
        }

        
    }

}

private IEnumerator Teleport()
    {
        
        TeleporterPris = true;
        yield return new WaitForSeconds(2);
        
    }
}
