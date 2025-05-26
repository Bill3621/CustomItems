using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Features.Wrappers;

namespace CustomItems.API;

public abstract class CustomItem
{
    public ushort Id { get; internal set; }

    public abstract string Name { get; }
    public abstract string Description { get; }
    public abstract ItemType Type { get; }
    public virtual float Weight => 1.0f;

    public virtual bool ShowItemHints => true;
    public virtual bool ShowPickupHints => true;
    public virtual bool ShowSelectedHints => true;

    public abstract void OnRegistered();
    public abstract void OnUnregistered();

    public bool Check(ushort serial) => CustomItems.CurrentItems.ContainsKey(serial) && CustomItems.CurrentItems[serial].Id == Id;
#nullable enable
    public bool Check(Pickup? pickup) => pickup is not null && Check(pickup.Serial);
    public bool Check(Item? item) => item is not null && Check(item.Serial);
    public bool Check(Player? player) => player is not null && Check(player.CurrentItem);
#nullable disable
    public virtual void OnUsing(PlayerUsingItemEventArgs ev) { }
    public virtual void OnUsed(PlayerUsedItemEventArgs ev) { }
    public virtual void OnDropping(PlayerDroppingItemEventArgs ev) { }
    public virtual void OnDropped(PlayerDroppedItemEventArgs ev) { }
    public virtual void OnPickingUp(PlayerPickingUpItemEventArgs ev) { }
    public virtual void OnPickedUp(PlayerPickedUpItemEventArgs ev) { }
    public virtual void OnSelecting(PlayerChangingItemEventArgs ev) { }
    public virtual void OnUnselecting(PlayerChangingItemEventArgs ev) { }
    public virtual void OnSelected(PlayerChangedItemEventArgs ev) { }
    public virtual void OnUnselected(PlayerChangedItemEventArgs ev) { }

}