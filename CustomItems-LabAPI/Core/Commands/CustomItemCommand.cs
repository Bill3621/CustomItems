using CommandSystem;
using CustomItems.API;
using InventorySystem.Disarming;
using LabApi.Features.Wrappers;
using RemoteAdmin;
using System;
using System.Linq;

namespace CustomItems.Core.Commands;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
[CommandHandler(typeof(GameConsoleCommandHandler))]
internal class CustomItemCommand : ICommand, IUsageProvider
{
    public string Command => "customItem";

    public string[] Aliases => ["ci"];

    public string Description => "Give a player a specified custom item";

    public string[] Usage => ["itemId", "%player%"];

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!sender.CheckPermission(PlayerPermissions.GivingItems))
        {
            response = "Permission Denied, required: " + PlayerPermissions.GivingItems;
            return false;
        }
        if (arguments.Count == 0)
        {
            response = this.DisplayCommandUsage() + "\n";
            foreach (CustomItem customItem in API.CustomItems.AllItems)
            {
                response += $"Name: {customItem.Name} | Id: {customItem.Id}\n";
            }
            return false;
        }

        if (!ushort.TryParse(arguments.At(0), out var id))
        {
            response = $"Invalid id: {arguments.At(0)}";
            return false;
        }

        CustomItem item = API.CustomItems.GetById(id);
        if (item == null)
        {
            response = $"Cannot find item with id: {arguments.At(0)}";
            return false;
        }

        if (arguments.Count == 1)
        {
            if (sender is PlayerCommandSender playerCommandSender)
            {
                var player = Player.Get(playerCommandSender);

                if (!CheckEligible(player))
                {
                    response = "You cannot receive custom items!";
                    return false;
                }

                API.CustomItems.TryGive(id, player, out _);
                response = $"{item.Name} given to {player.Nickname} ({player.UserId})";
                return true;
            }
            response = "Failed to provide a valid player, please follow the syntax.\n" + this.DisplayCommandUsage();
            return false;
        }

        string identifier = string.Join(" ", arguments.Skip(1));
        switch (identifier)
        {
            case "*":
            case "all":
                var eligiblePlayers = Player.List.Where(CheckEligible).ToList();
                foreach (var ply in eligiblePlayers)
                {
                    API.CustomItems.TryGive(id, ply, out _);
                }
                response = $"{item.Name} given to all players who can receive them ({eligiblePlayers.Count} players)";
                return true;
            default:
                if (Get(identifier) is not { } player)
                {
                    response = $"Unable to find player: {identifier}";
                    return false;
                }
                if (!CheckEligible(player))
                {
                    response = "Player cannot receive custom items!";
                    return false;
                }

                API.CustomItems.TryGive(id, player, out _);
                response = $"{item.Name} given to {player.Nickname} ({player.UserId})";
                return true;
        }
    }

    private bool CheckEligible(Player player) => player.IsAlive && !player.Inventory.IsDisarmed() && (player.Items.Count() < 8);

    // Borrowed from Exiled, but modified to work with LabAPI
    private Player Get(string args)
    {
        if (Player.List.Where(p => p.UserId == args).FirstOrDefault() is Player player)
            return player;
        if (int.TryParse(args, out int id)) return Player.Get(id);

        var lastnameDifference = 3;
        var firstString = args.ToLower();
        Player found = null;

        foreach (Player plr in Player.Dictionary.Values)
        {
            if (!plr.IsOnline || plr.IsHost || plr.Nickname is null) continue;
            if (!plr.Nickname.ToLower().Contains(args.ToLower())) continue;
            var secondString = plr.Nickname;

            var nameDifference = secondString.Length - firstString.Length;
            if (nameDifference < lastnameDifference)
            {
                lastnameDifference = nameDifference;
                found = plr;
            }
        }
        return found;
    }
}