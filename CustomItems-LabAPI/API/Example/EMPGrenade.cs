using CustomItems.Core;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using MEC;
using Mirror;

namespace CustomItems.API.Example;

public class EMPGrenade : CustomItem
{
    public override string Name => "EMP Grenade";

    public override string Description => "Locks doors and disables lights in current room.";

    public override ItemType Type => ItemType.GrenadeHE;

    public override void OnRegistered()
    {
        ServerEvents.ProjectileExploding += OnExplosion;
    }

    public override void OnUnregistered()
    {
        ServerEvents.ProjectileExploding -= OnExplosion;
    }

    private void OnExplosion(ProjectileExplodingEventArgs ev)
    {
        if (!Check(ev.TimedGrenade)) return;
        ev.IsAllowed = false;
        Log.Debug($"EMP Grenade exploded at {ev.TimedGrenade.Position} in room {ev.TimedGrenade.Room.Name}.");

        Room room = ev.TimedGrenade.Room;

        room.LightController.FlickerLights(10);

        foreach (var door in room.Doors)
        {
            if (door.IsLocked) continue;
            door.IsLocked = true;

            Timing.CallDelayed(10f, () =>
            {
                door.IsLocked = false;
            });
        }

        NetworkServer.Destroy(ev.TimedGrenade.GameObject);
    }
}