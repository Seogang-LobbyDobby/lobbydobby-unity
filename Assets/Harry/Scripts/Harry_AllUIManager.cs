using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Harry_AllUIManager : MonoBehaviour
{
    Network_AI na;

    // 스포트라이트로 검색될 기능의 이름과 함수를 담는 딕셔너리
    private Dictionary<string, UnityAction> functions = new Dictionary<string, UnityAction>();
    public GameObject funcFac;

    public static Harry_AllUIManager Instance;

    InputField spotLight;
    Button teleport;
    Button schedule;
    Transform content;

    GameObject Callender;

    public GameObject player;
    public GameObject effectFac;
    public GameObject owlFac;

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
            Destroy(Instance);
            Instance = this;
        }

        spotLight = transform.Find("SpotLight").GetComponent<InputField>();
        spotLight.onValueChanged.AddListener(SpotLight);
        spotLight.gameObject.SetActive(false);
        content = spotLight.transform.Find("Scroll View").transform.Find("Viewport").transform.Find("Content");

        teleport = transform.Find("Teleport").GetComponent<Button>();
        teleport.onClick.AddListener(OnClickTeleport);
        foreach (Transform tr in teleport.transform) { tr.gameObject.SetActive(false); }
        teleport.transform.Find("Club").GetComponent<Button>().onClick.AddListener(Club);
        teleport.transform.Find("MyRoom").GetComponent<Button>().onClick.AddListener(MyRoom);

        schedule = transform.Find("Schedule").GetComponent<Button>();
        schedule.onClick.AddListener(OnClickSchedule);

        // 딕셔너리에 함수 이름과 함수 기능을 추가
        functions.Add("마이룸 이동하기", MyRoom);
        functions.Add("클럽 이동하기", Club);
        functions.Add("일정 확인하기", OnClickSchedule);

        Callender = transform.Find("CallenderUI").gameObject;
        Callender.SetActive(false);

        na = GetComponent<Network_AI>();

        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Return))
        {
            spotLight.gameObject.SetActive(!spotLight.gameObject.activeSelf);
        }
    }

    void SpotLight(string s)
    {
        // 같은 기능의 버튼이 여러개 추가되는것을 막기 위해 매번마다 버튼 다 삭제
        foreach (Transform tr in content)
        {
            Destroy(tr.gameObject);
        }
        bool can = false;
        foreach (var func in functions)
        {
            // 만약 검색한 키워드를 포함하는 함수 이름이 있다면
            if (func.Key.Contains(s) && s.Length > 0)
            {
                // 함수의 기능과 이름을 담아서 버튼으로 생성
                GameObject go = Instantiate(funcFac, content);
                go.GetComponent<Button>().onClick.AddListener(func.Value);
                go.transform.Find("Text").GetComponent<Text>().text = func.Key.ToString();
                can = true;
            }
        }
        if (can == false)
        {

        }
    }

    void OnClickTeleport()
    {
        foreach (Transform tr in teleport.transform)
        {
            tr.gameObject.SetActive(!tr.gameObject.activeSelf);
        }
    }

    void OnClickSchedule()
    {
        Tablet();
        spotLight.gameObject.SetActive(false);
        Callender.SetActive(!Callender.activeSelf);
    }

    public bool isTablet = false;
    void Tablet()
    {
        if (isTablet)
        {
            isTablet = false;
            canMove = true;
        }
        else
        {
            isTablet = true;
            canMove = false;
        }
    }

    void MyRoom()
    {
        StartCoroutine(ChangeToOwl("Harry_MainLobbyDesign"));
    }
    void Club()
    {
        StartCoroutine(ChangeToOwl("Harry_ClubWorld"));
    }

    IEnumerator ChangeToOwl(string sceneName)
    {
        foreach (Transform tr in player.transform)
        {
            tr.gameObject.SetActive(false);
        }
        player.GetComponent<CharacterController>().enabled = false;
        try
        {
            player.GetComponent<Harry_AvatarController>().enabled = false;
        }
        catch (Exception e)
        {
            player.GetComponent<Harry_OwlController>().enabled = false;
        }
        player.GetComponent<Animator>().enabled = false;
        GameObject effect = Instantiate(effectFac);
        effect.transform.position = player.transform.position;

        GameObject owl = Instantiate(owlFac, player.transform);
        owl.GetComponent<Animator>().SetFloat("Status", 2);

        CharacterController cc = owl.GetComponent<CharacterController>();
        float currentTime = 0;
        while (currentTime < 3f)
        {
            cc.Move(Vector3.up * Time.deltaTime * 0.5f);
            yield return null;
            currentTime += Time.deltaTime;
        }

        PhotonNetwork.Disconnect();
        SceneManager.LoadScene(sceneName);
    }
}
