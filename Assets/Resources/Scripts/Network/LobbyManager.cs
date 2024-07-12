using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private Button btnStart = null;
    [SerializeField] private GameObject loadingUI = null;
    private TextMeshProUGUI loadingText = null;

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        btnStart.onClick.AddListener(delegate { PhotonNetwork.JoinRandomRoom(); });
        loadingUI.SetActive(false);
        loadingText = loadingUI.GetComponentInChildren<TextMeshProUGUI>(true);
    }

    public void CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsVisible = true;
        roomOptions.MaxPlayers = 2;

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
            loadingText.SetText("���� �߰�!");
        }
        loadingUI.SetActive(true);

    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            loadingText.SetText("���� �߰�!");
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
}
