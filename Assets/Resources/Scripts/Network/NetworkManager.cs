using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject loading = null;
    [SerializeField] private TextMeshProUGUI stateText = null;
    [SerializeField] private TMP_InputField idInputField = null;
    [SerializeField] private Button btnLogin = null;
    [SerializeField] private Button btnStart = null;
    [SerializeField] private GameObject lobbyLoadingUI = null;
    [SerializeField] private TextMeshProUGUI lobbyLoadingText = null;

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        loading.SetActive(true);
        stateText.SetText("���� ��...");
        btnLogin.onClick.AddListener(RequestJoinLobby);
        btnLogin.interactable = false;
        idInputField.interactable = false;

        btnStart.gameObject.SetActive(false);
        lobbyLoadingUI.SetActive(false);
        btnStart.onClick.AddListener(OnClickBtnStart);
    }

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Ŭ���̾�Ʈ�� �����Ϳ� �����");
        StartCoroutine(CoConnectedToMaster());
    }

    private IEnumerator CoConnectedToMaster()
    {
        stateText.SetText("���� ����!");
        loading.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        stateText.SetText("���̵� �Է��ϼ���.");
        btnLogin.interactable = true;
        idInputField.interactable = true;
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        print(System.Reflection.MethodBase.GetCurrentMethod().Name);

        //SceneManager.LoadScene(SceneLocalize.lobbyScene);
        stateText.SetText($"{idInputField.text}��! �ݰ�����!");
        idInputField.gameObject.SetActive(false);
        btnLogin.gameObject.SetActive(false);
        btnStart.gameObject.SetActive(true);
        Debug.Log("�κ� ���� �Ϸ�");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        Debug.LogWarningFormat("�������� ������ ������. ���� : {0}", cause);
    }

    private void RequestJoinLobby()
    {
        StartCoroutine(CoRequestJoinLobby());
    }
    
    private IEnumerator CoRequestJoinLobby()
    {
        btnLogin.interactable = false;
        idInputField.interactable = false;
        stateText.SetText("�α��� ��...");
        loading.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        stateText.SetText("�α��� ����!");
        loading.SetActive(false);
        GameManager.Instance.userName = idInputField.text;
        yield return new WaitForSeconds(1.5f);
        PhotonNetwork.JoinLobby();
    }

    #region InLobby
    public void CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsVisible = true;
        roomOptions.MaxPlayers = 3;

        PhotonNetwork.CreateRoom(null, roomOptions);
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        Debug.Log("����� �Ϸ�");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        Debug.Log($"����� ����, {returnCode}, {message}");
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("�� ���� �Ϸ�");
        //SceneManager.LoadScene(SceneLocalize.gameScene);
        //GameManager.Instance.isConnect = true;
        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            lobbyLoadingText.SetText("���� �߰�!");
        }
        //lobbyLoadingUI.SetActive(true);

    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            lobbyLoadingText.SetText("���� �߰�!");
            if (PhotonNetwork.IsMasterClient)
            {
                StartCoroutine(CoStartGame());
            }
        }
    }

    private IEnumerator CoStartGame()
    {
        yield return new WaitForSecondsRealtime(2f);
        PhotonNetwork.LoadLevel(SceneLocalize.gameScene);
    }


    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);
        Debug.Log("������ ����. ���ο� �� ����");
        CreateRoom();
    }

    public void OnClickBtnStart()
    {
        btnStart.interactable = false;
        lobbyLoadingText.SetText("�ε���...");
        lobbyLoadingUI.SetActive(true);
        PhotonNetwork.JoinRandomRoom();
    }
    #endregion
}
