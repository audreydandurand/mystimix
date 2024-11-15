using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Joueur : MonoBehaviour
{
    public GameObject ampoule01;
    public GameObject ampoule02;
    public GameObject ampoule03;
    public GameObject ampoule04;

    public void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Lanterne01")
        {
            ampoule01.SetActive(true);
            ampoule02.SetActive(true);
        }
    }
    
    public void OnTriggerExit(Collider other)
    {
        if(other.tag == "Lanterne01")
        {
            ampoule01.SetActive(false);
            ampoule02.SetActive(false);
        }
    }
}
