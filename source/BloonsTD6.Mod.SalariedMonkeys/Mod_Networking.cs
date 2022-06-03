using BTD_Mod_Helper.Api.Coop;
using NinjaKiwi.NKMulti;

namespace BloonsTD6.Mod.SalariedMonkeys;

/// <summary>
/// This class contains all code specific to the mod's networking functionality.
/// </summary>
public partial class Mod
{
    // Mod Helper devs disabled ActOnMessage unless this is set to false, oof!
    public override bool CheatMod => false;

    // Note: Normally we'd move out handling of different messages to a class per message, with common interface
    // then register them at startup using reflection and call them using foreach.
    // However this is overkill as we don't network much in this mod.
    // Maybe in another mod!
    private const string MSG_SYNCSETTINGS = "SM_SYNC";
    private static NKMultiGameInterface? _nkGi;
    private static ModServerSettings _netplaySettingsBackup = _modSettings;

    public override void OnConnected(NKMultiGameInterface nkGi)
    {
        _nkGi = nkGi;
        _netplaySettingsBackup = _modSettings.Clone();
        if (!nkGi.IsCoOpHost())
            return;

        Log.Always($"Joined Lobby as Host, Sending Settings.");
        nkGi.SendMessageEx((ModServerSettings)_modSettings, null, MSG_SYNCSETTINGS);
    }

    public override void OnConnectFail(NKMultiGameInterface nkGi) => Log.Debug("Failed to Connect");

    public override void OnDisconnected(NKMultiGameInterface nkGi)
    {
        Log.Always($"Disconnected. Restoring settings.");
        _modSettings.Map(_netplaySettingsBackup);
    }

    // Queue of items to be sent, return true on successful send else false. 
    public override bool ActOnMessage(Message message)
    {
        switch (message.Code)
        {
            case MSG_SYNCSETTINGS:
                
                // Let's not trust clients :)
                if (_nkGi.IsCoOpHost()) 
                    return Log.Always(true, $"Is Host. Ignoring sync Code.");

                // Apply settings.
                var data = MessageUtils.ReadMessage<ModServerSettings>(message);
                _modSettings.Map(data);
                return Log.Always(true, $"Applied server settings.");

            default:
                return false;
        }
    }

    public override void OnPeerConnected(NKMultiGameInterface nkGi, int peerId) => HandlePeerConnectedMessage(nkGi, peerId);

    // Returns true on success, false on failure.
    private static bool HandlePeerConnectedMessage(NKMultiGameInterface nkMultiGameInterface, int peerId)
    {
        // Only handle if host.
        if (!_nkGi.IsCoOpHost())
            return Log.Always(true, $"Peer connected but not host, ignoring.");
        
        _nkGi!.SendToPeer(peerId, MessageUtils.CreateMessageEx((ModServerSettings)_modSettings, MSG_SYNCSETTINGS));
        return Log.Always(true, $"Connected and host. Sent settings sync.");
    }
}