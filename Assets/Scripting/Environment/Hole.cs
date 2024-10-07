using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hole : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private Interactor interact;
    [SerializeField] private Raycaster raycaster;

    private void Start()
    {
        player = GameObject.Find("Diggy (Player)").GetComponent<PlayerController>();
        interact = GameObject.Find("Interactor").GetComponent<Interactor>();
        raycaster = GameObject.Find("Raycaster").GetComponent<Raycaster>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Push Block")
        {
            StartCoroutine(FillHole(other));
        }
    }

    private IEnumerator FillHole(Collider rock)
    {
        yield return new WaitForSeconds(0.5f);
        player.isGrabbingRock = false;
        raycaster.isFilling = true;

        if(interact.grabbedObject != null)
        {
            interact.grabbedObject.transform.parent = null;
        }

        interact.grabbedObject = null;
        //change texture to filled hole
        Destroy(rock.gameObject);
        this.transform.parent.gameObject.SetActive(false);
        raycaster.isFilling = false;
    }
}
