using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Joueur : MonoBehaviour
{
    public GameObject ampoule01;
    public GameObject ampoule02;
    public GameObject ampoule03;
    public GameObject ampoule04;
    public GameObject chandelier01;
    public GameObject chandelier02;
    public GameObject chandelier03;
    public GameObject chandelier04;
    public GameObject lanternedeuxieme01;
    public GameObject lanternedeuxieme02;
    public GameObject lanternedeuxieme03;
    public GameObject lanternedeuxieme04;
    public GameObject lampcustom3em_etage01;
    public GameObject lampcustom3em_etage02;
    public GameObject lampcustom2em_etage01;
    public GameObject lampcustom2em_etage02;
    public GameObject lampcustom1em_etage01;
    public GameObject lampcustom1em_etage02;
    public GameObject lampcustom1em_etage03;
    public GameObject lampcustom1em_etage04;
    public GameObject roomlighting;




    public void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Lanterne01")
        {

            ampoule01.SetActive(true);
            ampoule02.SetActive(true);

        }

        if(other.tag == "Lanterne02")
        {
            ampoule03.SetActive(true);
            ampoule04.SetActive(true);
            chandelier01.SetActive(true);
            chandelier02.SetActive(true);
            chandelier03.SetActive(true);
            chandelier04.SetActive(true);
            lanternedeuxieme01.SetActive(true);
            lanternedeuxieme02.SetActive(true);
            lanternedeuxieme03.SetActive(true);
            lanternedeuxieme04.SetActive(true);
            lampcustom3em_etage01.SetActive(true);
            lampcustom3em_etage02.SetActive(true);
            lampcustom2em_etage01.SetActive(true);
            lampcustom2em_etage02.SetActive(true);
            lampcustom1em_etage01.SetActive(true);
            lampcustom1em_etage02.SetActive(true);
            lampcustom1em_etage03.SetActive(true);
            lampcustom1em_etage04.SetActive(true);
            roomlighting.SetActive(true);
            
        }
    }
    
    public void OnTriggerExit(Collider other)
    {
        if(other.tag == "Lanterne01")
        {
            ampoule01.SetActive(false);
            ampoule02.SetActive(false);

        }

        if(other.tag == "Lanterne02")
        {
            ampoule03.SetActive(false);
            ampoule04.SetActive(false);
        }
    }

   
}
