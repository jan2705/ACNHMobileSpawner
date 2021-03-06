﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class OffsetHelper
{
    // some helpers
    public const ulong PlayerSize = 0x74420;
    public const ulong PlayerOtherStartPadding = 0x35E40;

    // player other (Personal Offsets 13)
    public const ulong InventoryOffset = 0xABC25840; // player 0 (A)
    private const ulong playerOtherStart = InventoryOffset - 0x10; // helps to get other values, unused 

    public const ulong WalletAddress = InventoryOffset + 0xB8;
    public const ulong MilesAddress = InventoryOffset - 0x25590;
    public const ulong BankAddress = InventoryOffset + 0x3517C;

    // main player offsets functions
    private static ulong getPlayerStart(ulong invOffset) => invOffset - 0x10 - PlayerOtherStartPadding + 0x110;
    public static ulong getPlayerIdAddress(ulong invOffset) => getPlayerStart(invOffset) + 0xAFA8;
    public static ulong getPlayerProfileMainAddress(ulong invOffset) => getPlayerStart(invOffset) + 0x116A0;


    // main save offsets
    public const ulong TurnipAddress = 0xAA9CB2D8;
    public const ulong VillagerAddress = TurnipAddress - 0x41D390 + 0x10;
    public const ulong VillagerHouseAddress = VillagerAddress + 0x40E228; // TODO
    public const ulong VillagerHouseBufferDiff = 0xB057B0; // TODO

    public const ulong FieldItemStart = VillagerAddress - 0x10 + 0x20CC0C;
}
