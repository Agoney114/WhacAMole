using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using TMPro;

public class GameController : MonoBehaviour
{  

    //Este commit es los mismo que el de el ejercico 2 pero se me olvidó mandar el 2 antes



    public static GameController instance;
    public GameObject mainMenu, inGameUI,endScreen,recordPanel;

    public Transform molesParent;
    private MoleBehaviour[] moles;

    public bool playing = false;

    public float gameDuration = 0f;
    public float timePlayed = 60;

    int points = 0;
    public float clicks = 0;
    public float successClicks = 0;
    public float failedClicks = 0;
    int recordScore = 0;

    public TMP_InputField nameField;
    string playerName;
    string highScoreKey = "HighScore";

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
        recordText.text = PlayerPrefs.GetInt(highScoreKey, 0).ToString();
        pointsText.text = "puntos: " + points;
        if (playing == true)
        {
            timePlayed -= Time.deltaTime;
            timeText.text = "Tiempo: " + Mathf.Floor(timePlayed) + "/60";
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
        //Guardar record si es necesario

        //Acceso al texto escrito
        playerName = nameField.text;
        Debug.Log("Record de " + playerName);

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
                        points += 100;
                        successClicks += 1;
                    }
                }
                if (hitInfo.collider.tag.Equals("Tag"))
                {
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
    public void SaveRecord()
    {
        if (points > PlayerPrefs.GetInt(highScoreKey, 0))
        {
            PlayerPrefs.SetInt(highScoreKey, points);
            PlayerPrefs.Save();
            recordText.text = recordScore.ToString();
        }
    }
}
