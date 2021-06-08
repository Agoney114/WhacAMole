using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using TMPro;

public class GameController : MonoBehaviour
{  

    public static GameController instance;
    public GameObject mainMenu, inGameUI,endScreen,recordPanel;
    public GameObject[] powerUp1;
    public GameObject[] powerUp2;

    public Transform molesParent;
    public Transform pos;
    private MoleBehaviour[] moles;

    public bool playing = false;

    public float gameDuration = 0f;
    public float timePlayed = 60;

    int points = 0;
    public float clicks = 0;
    public float successClicks = 0;
    public float failedClicks = 0;
    int recordScore = 0;


    string highScoreKey = "HighScore";

    Vector3 posicionInical;

    public TextMeshProUGUI timeText, recordText, pointsText, infoPointsText, inforRecordText, infoSuccesClicks, infoFailedClicks;

    void Awake()
    {
        if (GameController.instance == null)
        {
            ConfigureInstance();
        }
        else
        {
            Destroy(this);
        }
        recordText.text = PlayerPrefs.GetInt(highScoreKey, 0).ToString();

    }

    void ConfigureInstance()
    {
        //Configura acceso a moles
        moles = new MoleBehaviour[molesParent.childCount];

        for (int i = 0; i < molesParent.childCount; i++)
        {
            moles[i] = molesParent.GetChild(i).GetComponent<MoleBehaviour>();
        }

        //Inicia los puntos
        points = 0;
        clicks = 0;
        failedClicks = 0;

        //Activa la UI inicial
        inGameUI.SetActive(false);
        mainMenu.SetActive(true);
        endScreen.SetActive(false);
        recordPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //se muestra los puntos y el record
        recordText.text = PlayerPrefs.GetInt(highScoreKey, 0).ToString();
        pointsText.text = "puntos: " + points;
        if (playing == true)
        {
            timePlayed -= Time.deltaTime;     
            timeText.text = "Tiempo: " + Mathf.Floor(timePlayed);
            if (timePlayed <= gameDuration)
            {

                ShowEndScreen();
                playing = false;
                for (int i = 0; i < moles.Length; i++)
                {
                    moles[i].StopMole();
                }

                
            }
            else
            {
                CheckClicks();
            }
            //se llama a la funcion que almacena el record en el update para que este actualizado siempre
            SaveRecord();
        }
    }


    void ShowEndScreen()
    {
        endScreen.SetActive(true);
        infoPointsText.text = pointsText.text;
        inforRecordText.text = "record : " + recordText.text;
        infoSuccesClicks.text = "% punteria : " + ((successClicks / clicks) * 100) + " %";
        infoFailedClicks.text = "fallos : " + failedClicks;

        bool isRecord = false;
        //si hay nuevo record mostrar el panel recordPanel
        recordPanel.SetActive(isRecord);
    }

    /// <summary>
    /// Function called from End Screen when players hits Retry button
    /// </summary>
    public void Retry()
    {
        //Reinicia información del juego
        ResetGame();
        //Cambia las pantallas
        inGameUI.SetActive(true);
        mainMenu.SetActive(false);
        endScreen.SetActive(false);
        //Activa juego
        playing = true;

        //Reinicia moles
        for (int i = 0; i < moles.Length; i++)
        {
            moles[i].ResetMole();
        }
    }

    /// <summary>
    /// Restarts all info game
    /// </summary>
    void ResetGame()
    {
        for (int i = 0; i < moles.Length; i++)
        {
            moles[i].StopMole();
        }

        timePlayed = 60;
        points = 0;
        clicks = 0;
        failedClicks = 0;
    }

    public void EnterMainScreen()
    {
        //Reinicia información del juego
        ResetGame();
        //Cambia las pantallas
        inGameUI.SetActive(false);
        mainMenu.SetActive(true);
        endScreen.SetActive(false);
        recordPanel.SetActive(false);

    }

    /// <summary>
    /// Used to check if players hits or not the moles/powerups
    /// </summary>
    public void CheckClicks()
    {
        if ((Input.touchCount >= 1 && Input.GetTouch(0).phase == TouchPhase.Ended) || (Input.GetMouseButtonUp(0)))
        {
            //recuento de todos los clicks de la partida
            clicks += 1;
            Vector3 pos = Input.mousePosition;
            if (Application.platform == RuntimePlatform.Android)
            {
                pos = Input.GetTouch(0).position;
            }

            Ray rayo = Camera.main.ScreenPointToRay(pos);
            RaycastHit hitInfo;
            if (Physics.Raycast(rayo, out hitInfo))
            {
                if (hitInfo.collider.tag.Equals("Mole"))
                {
                    MoleBehaviour mole = hitInfo.collider.GetComponent<MoleBehaviour>();
                    if (mole != null)
                    {
                        mole.OnHitMole();
                        //recuento de los puntos y de los clicks dados con exito
                        points += 100;
                        successClicks += 1;
                    }
                }
                if (hitInfo.collider.tag.Equals("Tag"))
                {
                    //recuento de los cliks fallados
                    failedClicks += 1;
                }
            }
        }
    }

    public void OnGameStart()
    {
        mainMenu.SetActive(false);
        inGameUI.SetActive(true);
        points = 0;
        for (int i = 0; i < moles.Length; i++)
        {
            moles[i].ResetMole(moles[i].initTimeMin, moles[i].initTimeMax);
        }
        playing = true;
    }

    /// <summary>
    /// Funcion para entrar en pausa, pone playing en false y muestra la pantalla de pausa.
    /// </summary>
    

    //sistema de guardado de record con playerprefs
    public void SaveRecord()
    {
        if (points > PlayerPrefs.GetInt(highScoreKey, 0))
        {
            PlayerPrefs.SetInt(highScoreKey, points);
            PlayerPrefs.Save();
            recordText.text = recordScore.ToString();
        }
    }
    //comienzo de los power ups
    public void PowerUp1()
    {
        if (playing == true)
        {
            Vector3 newPos = posicionInical;
            
            int n = Random.Range(0, powerUp2.Length);    
            LeanTween.move(Instantiate(powerUp2[n], pos.position, powerUp2[n].transform.rotation), newPos, 3.5f);
           
        }
    }
    public void PowerUp2()
    {
        if (playing == true)
        {
            Vector3 newPos = posicionInical;

            int i = Random.Range(0, powerUp1.Length);
            LeanTween.move(Instantiate(powerUp1[i], pos.position, powerUp1[i].transform.rotation), newPos, 3.5f);

        }
    }

}
