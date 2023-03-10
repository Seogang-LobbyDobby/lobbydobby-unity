using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Harry_SquareManager : MonoBehaviour
{
    public static Harry_SquareManager Instance;

    // 채팅 관련 함수는 AvatarController에 있음 (Photon 관련 이슈)
    public InputField chatInput;
    Button emotion;

    public GameObject player;

    GameObject canvas;
    GameObject decoCam;

    bool canMove = true;
    public bool CanMove
    {
        get { return canMove; }
    }

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance == null) 
            Instance = this;
        else
        {
            Destroy(Instance.gameObject);
            Instance = this;
        }

        canvas = GameObject.Find("Canvas");
        emotion = GameObject.Find("Emotion").GetComponent<Button>();
        emotion.transform.Find("Motions").gameObject.SetActive(false);
        emotion.onClick.AddListener(OnClickEmotion);

        decoCam = GameObject.Find("Deco Camera");
        if (decoCam)
            decoCam.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        canvas.SetActive(!Harry_AllUIManager.Instance.isTablet);
    }

    void OnClickEmotion()
    {
        emotion.transform.Find("Motions").gameObject.SetActive(!emotion.transform.Find("Motions").gameObject.activeSelf);
    }

    public void OnClickMotions(string s)
    {
        if (player)
        {
            player.GetComponent<Harry_AvatarController>().EMOTE(s);
        }
    }

    public void OnClickDeco()
    {
        if (Camera.main != null)
        {
            if (decoCam.activeSelf)
            {
                decoCam.GetComponent<Harry_DecoCam>().cc.EndInter();
                canMove = true;
                decoCam.SetActive(false);
            }
            else
            {
                decoCam.SetActive(true);
                canMove = false;
                decoCam.GetComponent<Harry_DecoCam>().cc = Camera.main.transform.parent.GetComponent<Harry_CamController>();
            }
        }
    }
}
