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
    public AudioSource tapisAudioSource;
    public AudioSource livreAudioSource;
    public AudioSource craieAudioSource;

    // Son de marche
    public GameObject marcherAudioObject; // Empty object avec le tag "marcher"
    private AudioSource marcheAudioSource; // AudioSource attachée à marcherAudioObject
    private Vector3 dernierePosition;
    private bool estEnMouvement = false;

    void Start()
    {
        // Récupère l'AudioSource depuis l'objet "marcher"
        if (marcherAudioObject != null && marcherAudioObject.CompareTag("marcher"))
        {
            marcheAudioSource = marcherAudioObject.GetComponent<AudioSource>();
        }
        else
        {
            Debug.LogError("L'objet marcherAudioObject est manquant ou le tag n'est pas 'marcher'.");
        }

        dernierePosition = transform.position;
    }

    void Update()
    {
        // Vérifier si le joueur se déplace
        bool joueurBouge = (transform.position - dernierePosition).sqrMagnitude > 0.001f;

        if (joueurBouge && !estEnMouvement)
        {
            estEnMouvement = true;
            if (marcheAudioSource != null && !marcheAudioSource.isPlaying)
            {
                marcheAudioSource.Play(); // Jouer le son de marche
            }
        }
        else if (!joueurBouge && estEnMouvement)
        {
            estEnMouvement = false;
            if (marcheAudioSource != null && marcheAudioSource.isPlaying)
            {
                marcheAudioSource.Pause(); // Mettre le son en pause
            }
        }

        dernierePosition = transform.position;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Lanterne01")
        {
            ampoule01.SetActive(true);
            ampoule02.SetActive(true);
        }

        if (other.tag == "Lanterne02")
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

        if (other.tag == "Tapis")
        {
            if (tapisAudioSource != null) 
            {
                tapisAudioSource.Play();
            }
        }
        
        if (other.tag == "Table")
        {
            if (livreAudioSource != null) 
            {
                livreAudioSource.Play();
            }
        }

                if (other.tag == "Board")
        {
            if (craieAudioSource != null) 
            {
                craieAudioSource.Play();
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.tag == "Lanterne01")
        {
            ampoule01.SetActive(false);
            ampoule02.SetActive(false);
        }

        if (other.tag == "Lanterne02")
        {
            ampoule03.SetActive(false);
            ampoule04.SetActive(false);
        }

        if (other.tag == "Tapis")
        {
            if (tapisAudioSource != null) 
            {
                tapisAudioSource.Stop();
            }
        }

        if (other.tag == "Table")
        {
            if (livreAudioSource != null) 
            {
                livreAudioSource.Stop();
            }
        }

                if (other.tag == "Board")
        {
            if (craieAudioSource != null) 
            {
                craieAudioSource.Stop();
            }
        }
    }
}
