using CommandSystem;
using LabApi.Features.Wrappers;
using RemoteAdmin;
using System;
using UnityEngine;

namespace CustomItems.Core.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    internal class TPPos : ICommand
    {
        public string Command => "tppos";

        public string[] Aliases => ["tpp"];

        public string Description => "Teleport to a specified position";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission(PlayerPermissions.Noclip))
            {
                response = "Permission Denied, required: " + PlayerPermissions.Noclip;
                return false;
            }

            if (sender is PlayerCommandSender p)
            {
                Player player = Player.Get(p);

                if (arguments.Count < 3)
                {
                    response = "Usage: tppos <x> <y> <z>";
                    return false;
                }

                if (!float.TryParse(arguments.At(0).Replace(",", ""), out float x) ||
                    !float.TryParse(arguments.At(1).Replace(",", ""), out float y) ||
                    !float.TryParse(arguments.At(2).Replace(",", ""), out float z))
                {
                    response = "Invalid coordinates.";
                    return false;
                }

                Vector3 position = new Vector3(x, y, z);
                player.Position = position;

                response = "Teleported.";
                return true;
            }

            response = "Not a player.";
            return false;
        }
    }
}
