using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject loading = null;
    [SerializeField] private TextMeshProUGUI stateText = null;
    [SerializeField] private TMP_InputField idInputField = null;
    [SerializeField] private Button btnLogin = null;

    private void Awake()
    {
        loading.SetActive(true);
        stateText.SetText("연결 중...");
        btnLogin.onClick.AddListener(RequestJoinLobby);
        btnLogin.interactable = false;
        idInputField.interactable = false;
    }

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("클라이언트가 마스터에 연결됨");
        StartCoroutine(CoConnectedToMaster());
    }

    private IEnumerator CoConnectedToMaster()
    {
        stateText.SetText("연결 성공!");
        loading.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        stateText.SetText("아이디를 입력하세요.");
        btnLogin.interactable = true;
        idInputField.interactable = true;
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        print(System.Reflection.MethodBase.GetCurrentMethod().Name);

        SceneManager.LoadScene(SceneLocalize.lobbyScene);
        Debug.Log("로비 입장 완료");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        Debug.LogWarningFormat("서버와의 연결이 끊어짐. 사유 : {0}", cause);
    }

    private void RequestJoinLobby()
    {
        StartCoroutine(CoRequestJoinLobby());
    }
    
    private IEnumerator CoRequestJoinLobby()
    {
        btnLogin.interactable = false;
        idInputField.interactable = false;
        stateText.SetText("로그인 중...");
        loading.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        stateText.SetText("로그인 성공!");
        loading.SetActive(false);
        GameManager.Instance.userName = idInputField.text;
        yield return new WaitForSeconds(1.5f);
        PhotonNetwork.JoinLobby();
    }
}
