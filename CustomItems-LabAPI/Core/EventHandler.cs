using LabApi.Events.CustomHandlers;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Features.Wrappers;

namespace CustomItems.Core;

internal class EventHandler : CustomEventsHandler
{

    public override void OnServerWaitingForPlayers()
    {
        API.CustomItems.CurrentItems.Clear();

        if (!CustomItemsPlugin.Instance.Config.TestItemSpawning) return;
        if (API.CustomItems.AllItems.Count == 0)
        {
            Log.Error("No custom items registered, cannot spawn test items.");
            return;
        }

        foreach (var room in Room.List)
        {
            if (room.IsDestroyed) continue;
            API.CustomItems.TrySpawn(0, API.CustomItems.GetRandomPositionInRoom(room), out var pickup);
        }
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

    #region Item Picking Up
    public override void OnPlayerPickingUpItem(PlayerPickingUpItemEventArgs ev)
    {
        if (!Check(ev.Pickup.Serial)) return;
        API.CustomItems.CurrentItems[ev.Pickup.Serial].OnPickingUp(ev);
    }
    public override void OnPlayerPickedUpItem(PlayerPickedUpItemEventArgs ev)
    {
        if (!Check(ev.Item.Serial)) return;
        var customItem = API.CustomItems.CurrentItems[ev.Item.Serial];
        customItem.OnPickedUp(ev);

        if (!customItem.ShowItemHints || !customItem.ShowPickupHints) return;
        ev.Player.SendHint($"You have picked up {customItem.Name}\n{customItem.Description}");
    }
    #endregion

    #region Item Selection
    public override void OnPlayerChangingItem(PlayerChangingItemEventArgs ev)
    {

        if (ev.OldItem != null && !ev.OldItem.IsDestroyed && ev.OldItem.GameObject != null && Check(ev.OldItem.Serial))
        {
            API.CustomItems.CurrentItems[ev.OldItem.Serial].OnUnselecting(ev);
            if (!ev.IsAllowed) return;
        }
        if (ev.NewItem == null || ev.NewItem.IsDestroyed || ev.NewItem.GameObject == null || !Check(ev.NewItem.Serial)) return;
        API.CustomItems.CurrentItems[ev.NewItem.Serial].OnSelecting(ev);
    }
    public override void OnPlayerChangedItem(PlayerChangedItemEventArgs ev)
    {
        if (ev.OldItem != null && !ev.OldItem.IsDestroyed && Check(ev.OldItem.Serial))
        {
            API.CustomItems.CurrentItems[ev.OldItem.Serial].OnUnselected(ev);
        }
        if (ev.NewItem == null || ev.NewItem.IsDestroyed || !Check(ev.NewItem.Serial)) return;
        var customItem = API.CustomItems.CurrentItems[ev.NewItem.Serial];
        customItem.OnSelected(ev);
        if (!customItem.ShowItemHints || !customItem.ShowSelectedHints) return;
        ev.Player.SendHint($"You have selected {customItem.Name}\n{customItem.Description}");
    }
    #endregion
}