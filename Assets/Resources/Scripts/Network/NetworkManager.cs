using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        //base.OnConnectedToMaster();
        Debug.Log("Ŭ���̾�Ʈ�� �����Ϳ� �����");
        // �����Ϳ� ����Ǹ� �κ� ���� ��û
        PhotonNetwork.JoinLobby();
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

}
