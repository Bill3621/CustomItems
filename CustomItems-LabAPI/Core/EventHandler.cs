using LabApi.Events.CustomHandlers;
using CustomItems.API;
using LabApi.Events.Arguments.PlayerEvents;

namespace CustomItems.Core;

internal class EventHandler : CustomEventsHandler
{

    public override void OnServerWaitingForPlayers()
    {
        API.CustomItems.CurrentItems.Clear();
    }

    private bool Check(ushort serial)
    {
        return API.CustomItems.CurrentItems.ContainsKey(serial);
    }

    #region Item Using
    public override void OnPlayerUsingItem(PlayerUsingItemEventArgs ev)
    {
        if (!Check(ev.UsableItem.Serial)) return;
        API.CustomItems.CurrentItems[ev.UsableItem.Serial].OnUsing(ev);
    }
    public override void OnPlayerUsedItem(PlayerUsedItemEventArgs ev)
    {
        if (!Check(ev.UsableItem.Serial)) return;
        API.CustomItems.CurrentItems[ev.UsableItem.Serial].OnUsed(ev);
    }
    #endregion

    #region Item Dropping
    public override void OnPlayerDroppingItem(PlayerDroppingItemEventArgs ev)
    {
        if (!Check(ev.Item.Serial)) return;
        API.CustomItems.CurrentItems[ev.Item.Serial].OnDropping(ev);
    }
    public override void OnPlayerDroppedItem(PlayerDroppedItemEventArgs ev)
    {
        if (!Check(ev.Pickup.Serial)) return;
        API.CustomItems.CurrentItems[ev.Pickup.Serial].OnDropped(ev);
    }
    #endregion


}