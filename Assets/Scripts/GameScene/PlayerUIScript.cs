using TMPro;
using Unity.Collections;
using UnityEngine;

public class PlayerUIScript : MonoBehaviour
{
    public string typeHost = "HOST";
    public string typeContestant = "CONTESTANT";
    public string formatOther = "<b>{0}</b>";
    public string formatYou = "<b>{0}</b> <size=80%>(you)</size>";

    public TMP_Text textName;
    public TMP_InputField fieldName;
    public TMP_Text textType;
    private PlayerScript previousPlayer;

    public GameObject panelNotReady;
    public GameObject panelReady;

    private string newName;
    private float newNameLastTime;
    private const float newNameThrottle = 0.25f;

    private void OnDestroy()
    {
        if (previousPlayer != null)
        {
            previousPlayer.playerName.OnValueChanged -= OnPlayerNameChanged;
            previousPlayer.playerIsReady.OnValueChanged -= OnPlayerIsReadyChanged;
        }
    }

    private void Update()
    {
        if (newName != null && previousPlayer)
        {
            if (newNameLastTime + newNameThrottle > Time.time)
            {
                previousPlayer.SetPlayerNameServerRpc(newName);
                newNameLastTime = Time.time;
                newName = null;
            }
        }
    }

    public void SetPlayer(PlayerScript player)
    {
        if (previousPlayer != null)
        {
            previousPlayer.playerName.OnValueChanged -= OnPlayerNameChanged;
            previousPlayer.playerIsReady.OnValueChanged -= OnPlayerIsReadyChanged;
        }

        player.playerName.OnValueChanged += OnPlayerNameChanged;
        player.playerIsReady.OnValueChanged += OnPlayerIsReadyChanged;

        fieldName.text =
        textName.text = player.playerName.Value.ToString();
        fieldName.gameObject.SetActive(player.IsOwner);
        textName.gameObject.SetActive(!player.IsOwner);

        var type = player.IsOwnedByServer ? typeHost : typeContestant;
        var format = player.IsLocalPlayer ? formatYou : formatOther;
        textType.text = string.Format(format, type);
        previousPlayer = player;
    }

    private void OnPlayerNameChanged(FixedString128Bytes previousValue, FixedString128Bytes newValue)
    {
        if (fieldName.isFocused)
        {
            return;
        }
        fieldName.text =
        textName.text = newValue.Value;
    }

    private void OnPlayerIsReadyChanged(bool previousValue, bool newValue)
    {
        panelReady.SetActive(newValue);
        panelNotReady.SetActive(!newValue);
    }

    public void OnNameFieldInputChanged()
    {
        if (!previousPlayer
            || previousPlayer.playerName.Value == fieldName.text
            || !previousPlayer.IsOwner)
        {
            return;
        }
        if (newName != null || Time.time - newNameLastTime < newNameThrottle)
        {
            newName = fieldName.text;
            return;
        }
        previousPlayer.SetPlayerNameServerRpc(fieldName.text);
        newNameLastTime = Time.time;
    }
}
