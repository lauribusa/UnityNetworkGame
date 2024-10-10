﻿using Assets.Features.Entities;
using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerItemLoader : NetworkBehaviour
{
    public event Action<Item> ItemLoadingRequested;

    public bool IsReadyToloadItem { get; set; }

    private PlayerNetworkClient _playerNetworkClient;
    private Valve _valve;

    private void Awake() => _playerNetworkClient = GetComponent<PlayerNetworkClient>();

    private void Update()
    {
        if (!IsLocalPlayer) return;

        RequestToLoadItem();
        TurnValve();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsLocalPlayer) return;
        if (!other.TryGetComponent(out Valve valve)) return;

        _valve = valve;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsLocalPlayer) return;
        if (!other.TryGetComponent(out Valve valve)) return;

        _valve = null;
    }

    private void RequestToLoadItem()
    {
        if (!Input.GetKeyDown(KeyCode.L)) return;
        if (!IsReadyToloadItem) return;

        Item carriedItem = _playerNetworkClient.carriedItem;
        if (!carriedItem) return;

        _playerNetworkClient.PerformPutDownRpc();
        ItemLoadingRequested?.Invoke(carriedItem);
    }

    private void TurnValve()
    {
        if (!_valve) return;

        if (Input.GetKey(KeyCode.C)) _valve.TurnValve();
        if (Input.GetKey(KeyCode.V)) _valve.TurnValveReverse();
    }
}