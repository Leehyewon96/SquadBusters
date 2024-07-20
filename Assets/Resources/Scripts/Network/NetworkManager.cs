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
        stateText.SetText("���� ��...");
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

        SceneManager.LoadScene(SceneLocalize.lobbyScene);
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
}
