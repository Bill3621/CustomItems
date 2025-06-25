using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Features.Wrappers;
using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomItems.API.Example;

public class HealingSyringe : CustomItem
{
    public override string Name => "Healing Syringe";

    public override string Description => "Applies gradual healing to the user.";

    public override ItemType Type => ItemType.Adrenaline;

    public override void OnRegistered() { }

    public override void OnUnregistered() { }


    public override void OnUsing(PlayerUsingItemEventArgs ev)
    {
        const float initialDelay = 1.5f;
        const float duration = 5f;
        const float tickRate = 1f;
        const int healPerTick = 10;

        Timing.RunCoroutine(HealOverTime(ev.Player, initialDelay, duration, tickRate, healPerTick));

        ev.Player.RemoveItem(ev.UsableItem);
    }

    private IEnumerator<float> HealOverTime(Player player, float initialDelay, float duration, float tickRate, int healPerTick)
    {
        yield return Timing.WaitForSeconds(initialDelay);
        float elapsed = 0f;
        while (elapsed < duration)
        {
            if (player.IsAlive)
                player.Heal(healPerTick);
            elapsed += tickRate;
            yield return Timing.WaitForSeconds(tickRate);
        }
    }
}