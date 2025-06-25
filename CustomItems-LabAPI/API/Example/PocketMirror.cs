using CustomPlayerEffects;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using MapGeneration;
using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomItems.API.Example;

public class PocketMirror : CustomItem
{
    public override string Name => "Pocket Mirror";

    public override string Description => "When used, teleport to the Pocket Dimension. If already in the pocket dimension, get out without any cost.";

    public override ItemType Type => ItemType.Coin;

    public override void OnRegistered()
    {
        PlayerEvents.FlippedCoin += OnFlippedCoin;
    }

    public override void OnUnregistered()
    {
        PlayerEvents.FlippedCoin -= OnFlippedCoin;
    }

    private void OnFlippedCoin(PlayerFlippedCoinEventArgs ev)
    {
        if (PocketDimension.IsPlayerInside(ev.Player))
        {
            Timing.CallDelayed(1.5f, () =>
            {
                if (!ev.Player.IsAlive) return;
                if (!PocketDimension.IsPlayerInside(ev.Player)) return;
                if (!ev.Player.Items.Any(item => item is not null && item.Serial == ev.CoinItem.Serial)) return;
                PocketDimension.ForceExit(ev.Player);
                ev.Player.RemoveItem(ev.CoinItem);
            });
            return;
        }
        Timing.CallDelayed(1.5f, () =>
        {
            if (!ev.Player.IsAlive) return;
            if (PocketDimension.IsPlayerInside(ev.Player)) return;
            if (!ev.Player.Items.Any(item => item is not null && item.Serial == ev.CoinItem.Serial)) return;
            PocketDimension.ForceInside(ev.Player);
        });
    }
}