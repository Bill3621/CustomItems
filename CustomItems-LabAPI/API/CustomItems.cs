using CustomItems.Core;
using InventorySystem.Items;
using LabApi.Features.Wrappers;
using Mirror;
using PlayerRoles.FirstPersonControl;
using RelativePositioning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace CustomItems.API;

public static class CustomItems
{
    private static ushort _nextId = 0;
    private static readonly Dictionary<ushort, CustomItem> _itemsById = [];
    private static readonly Dictionary<string, CustomItem> _itemsByName = [];
    public static List<CustomItem> AllItems => [.. _itemsById.Values];

    // ItemSerial - CustomItem
    public static readonly Dictionary<ushort, CustomItem> CurrentItems = [];

    #region Register Functions
    public static void Register(CustomItem item)
    {
        if (_itemsByName.ContainsKey(item.Name))
            throw new InvalidOperationException($"Item '{item.Name}' already registered.");

        item.Id = GetNextId();
        _itemsById[item.Id] = item;
        _itemsByName[item.Name] = item;

        item.OnRegistered();

        Log.Debug($"Registered item '{item.Name}' with ID {item.Id} and type {item.Type}.");
    }

    public static List<CustomItem> RegisterAll()
    {
        var itemTypes = Assembly.GetCallingAssembly().GetTypes().Where(t => !t.IsAbstract && typeof(CustomItem).IsAssignableFrom(t));
        var createdItems = new List<CustomItem>();

        foreach (var type in itemTypes)
        {
            if (Activator.CreateInstance(type) is not CustomItem item)
                continue;

            try
            {
                Register(item);
                createdItems.Add(item);
            }
            catch (Exception ex)
            {
                Log.Warn($"Failed to register item '{item.Name}': {ex.Message}");
            }
        }

        Log.Debug($"Registered {createdItems.Count} items from assembly '{Assembly.GetCallingAssembly().GetName().Name}'.");

        return createdItems;
    }
    #endregion

    #region Unregister Functions
    public static void Unregister(CustomItem item)
    {
        if (!_itemsByName.ContainsKey(item.Name))
            throw new InvalidOperationException($"Item '{item.Name}' not registered.");
        _itemsById.Remove(item.Id);
        _itemsByName.Remove(item.Name);

        foreach (var serial in CurrentItems.Where(kvp => kvp.Value == item).Select(kvp => kvp.Key).ToList())
        {
            CurrentItems.Remove(serial);
        }

        item.OnUnregistered();

        Log.Debug($"Unregistered item '{item.Name}' with ID {item.Id}.");
    }

    public static List<CustomItem> UnregisterAll()
    {
        var callingAssembly = Assembly.GetCallingAssembly();

        var itemsToRemove = _itemsByName.Values
            .Where(item => item.GetType().Assembly == callingAssembly)
            .ToList();

        foreach (var item in itemsToRemove) Unregister(item);

        Log.Debug($"Unregistered {itemsToRemove.Count} items from assembly '{callingAssembly.GetName().Name}'.");

        return itemsToRemove;
    }
    #endregion

    #region Get By
    public static ushort GetIdByName(string name)
    {
        if (_itemsByName.TryGetValue(name, out CustomItem item))
        {
            return item.Id;
        }
        throw new KeyNotFoundException($"Item with name '{name}' not found.");
    }

    public static CustomItem GetById(ushort id)
    {
        return _itemsById.TryGetValue(id, out CustomItem item) ? item : null;
    }
    #endregion

    #region Spawn / Give
    public static bool TrySpawn(ushort id, Vector3 position, out Pickup pickup)
    {
        if (!_itemsById.TryGetValue(id, out CustomItem item))
        {
            pickup = null;
            return false;
        }

        pickup = Pickup.Create(item.Type, position);
        pickup.Weight = item.Weight;
        if (pickup == null) return false;
        CurrentItems.Add(pickup.Serial, (CustomItem)Activator.CreateInstance(item.GetType()));
        NetworkServer.Spawn(pickup.GameObject);
        Log.Debug($"Spawned item '{item.Name}' ({id}) at {position} with serial {pickup.Serial}.");
        return true;
    }

    public static bool TryGive(ushort id, Player player, out Item item, ItemAddReason addReason = ItemAddReason.Undefined)
    {
        if (!_itemsById.TryGetValue(id, out CustomItem cItem))
        {
            item = null;
            return false;
        }
        item = player.AddItem(cItem.Type, addReason);
        if (item == null) return false;
        CurrentItems.Add(item.Serial, (CustomItem)Activator.CreateInstance(cItem.GetType()));
        Log.Debug($"Gave item '{cItem.Name}' ({id}) to '{player.Nickname}' with serial {item.Serial}.");
        return true;
    }
    #endregion

    #region Positioning
    internal static readonly List<Vector3> rayDirections = [Vector3.left, Vector3.right, Vector3.forward, Vector3.back];
    /// <summary>
    /// Gets a random position in the specified room. It may rarely return a position behind a wall which is unreachable by players.
    /// </summary>
    /// <param name="room"></param>
    /// <returns></returns>
    public static Vector3 GetRandomPositionInRoom(Room room)
    {
        if (room.IsDestroyed) return Vector3.zero;
        var roomCenter = room.Base.WorldspaceBounds.center;
        int attempts = 0;
        while (attempts++ <= 100)
        {
            if (!TryGetRoofPosition(GetRandomPointInBounds(room.Base.WorldspaceBounds), out var roofPos)) continue;
            var targetPosition = new RelativePosition(roofPos).Position;

            bool isInsideObject = Physics.CheckSphere(targetPosition, 0.1f, FpcStateProcessor.Mask);
            if (isInsideObject) continue;

            var directionToCenter = (roomCenter - targetPosition).normalized;
            var distance = (roomCenter - targetPosition).magnitude;

            // Check raycasts
            bool hasRayMissed = false;

            foreach (var dir in rayDirections)
            {
                if (Physics.Raycast(targetPosition, dir, out RaycastHit hit, 30f, FpcStateProcessor.Mask))
                {
                    var reverseDir = -dir;
                    var hitPoint = hit.point;

                    if (!Physics.Raycast(hitPoint + reverseDir * 0.1f, reverseDir, out RaycastHit reverseHit, 30f, FpcStateProcessor.Mask))
                    {
                        hasRayMissed = true;
                        break;
                    }

                    if (hit.collider != reverseHit.collider)
                    {
                        hasRayMissed = true;
                        break;
                    }
                }
                else
                {
                    hasRayMissed = true;
                    break;
                }
            }

            if (!hasRayMissed)
            {
                // Raycast to middle of room and back
                var hitA = Physics.RaycastAll(targetPosition, directionToCenter, distance, FpcStateProcessor.Mask);
                var hitB = Physics.RaycastAll(roomCenter, -directionToCenter, distance, FpcStateProcessor.Mask);

                if (hitA.Length != hitB.Length)
                {
                    hasRayMissed = true;
                }
            }

            if (!hasRayMissed)
                return targetPosition;
        }
        // Backup:
        if (!TryGetRoofPosition(room.Base.WorldspaceBounds.center, out var backupRoofPos))
            return Vector3.zero;
        return new RelativePosition(backupRoofPos).Position;
    }

    public static Vector3 GetRandomPointInBounds(Bounds bounds)
    {
        Vector3 randomOffset = UnityEngine.Random.insideUnitSphere;
        randomOffset = new Vector3(
            randomOffset.x * bounds.extents.x,
            randomOffset.y * bounds.extents.y,
            randomOffset.z * bounds.extents.z
        );

        return bounds.center + randomOffset;
    }

    public static Vector3 GetRandomPosition()
    {
        var rooms = Room.List.Where(room => !room.IsDestroyed).ToList();
        var randomRoom = rooms[UnityEngine.Random.Range(0, rooms.Count)];
        if (!TryGetRoofPosition(randomRoom.Position, out var roofPos))
            return Vector3.zero;
        return new RelativePosition(roofPos).Position;
    }

    internal static bool TryGetRoofPosition(Vector3 point, out Vector3 result)
    {
        if (Physics.Raycast(point, Vector3.up, out var hitInfo, 30f, FpcStateProcessor.Mask))
        {
            result = hitInfo.point + Vector3.down * 0.3f;
            return true;
        }
        result = Vector3.zero;
        return false;
    }
    #endregion

    internal static ushort GetNextId() => _nextId++;
}