using CustomItems.Core;
using InventorySystem.Items;
using LabApi.Features.Wrappers;
using Mirror;
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
        if (pickup == null) return false;
        CurrentItems.Add(pickup.Serial, (CustomItem)Activator.CreateInstance(item.GetType()));
        NetworkServer.Spawn(pickup.GameObject);
        Log.Debug($"Spawned item '{item.Name}' at {position} with ID {id}.");
        return true;
    }

    public static bool TryGive(ushort id, Player player, out Item item)
    {
        if (!_itemsById.TryGetValue(id, out CustomItem cItem))
        {
            item = null;
            return false;
        }
        item = player.AddItem(cItem.Type, ItemAddReason.Undefined);
        if (item == null) return false;
        CurrentItems.Add(item.Serial, (CustomItem)Activator.CreateInstance(cItem.GetType()));
        Log.Debug($"Gave item '{cItem.Name}' to '{player.Nickname}' with ID {id}.");
        return true;
    }
    #endregion

    internal static ushort GetNextId() => _nextId++;
}