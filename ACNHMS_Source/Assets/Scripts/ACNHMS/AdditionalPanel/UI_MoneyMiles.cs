﻿using UnityEngine;
using NHSE.Core;
using UnityEngine.UI;
using System;

public class UI_MoneyMiles : IUI_Additional
{
    public const int ENCRYPTIONSIZE = 0x8;

    public static string MoneyValueAddress = OffsetHelper.BankAddress.ToString("X"); // ABA86BC4
    public static string MilesAddress = OffsetHelper.MilesAddress.ToString("X"); // has miles current then miles total after it :) ABA2DD28
    public static string WalletAddress = OffsetHelper.WalletAddress.ToString("X"); // has storage a bit after it in ram ABA52760 
    public static uint CurrentMoneyAddress { get { return StringUtil.GetHexValue(MoneyValueAddress); } }
    public static uint CurrentMilesAddress { get { return StringUtil.GetHexValue(MilesAddress); } }
    public static uint CurrentWalletAddress { get { return StringUtil.GetHexValue(WalletAddress); } }

    public InputField BankInput, PouchInput, MilesInput, MilesTotalInput;
    public InputField MoneyAddressInput, PouchAddressInput, MilesAddressInput;

    private MoneyMilesUtility currentUtil;

    // Start is called before the first frame update
    void Start()
    {
        currentUtil = new MoneyMilesUtility();

        //money + miles
        BankInput.onValueChanged.AddListener(delegate { currentUtil.Bank.Value = Convert.ToUInt32(BankInput.text); });
        PouchInput.onValueChanged.AddListener(delegate { currentUtil.Pouch.Value = Convert.ToUInt32(PouchInput.text); });
        MilesInput.onValueChanged.AddListener(delegate { currentUtil.MilesNow.Value = Convert.ToUInt32(MilesInput.text); });
        MilesTotalInput.onValueChanged.AddListener(delegate { currentUtil.MilesTotal.Value = Convert.ToUInt32(MilesTotalInput.text); });

        //ram offsets
        MoneyAddressInput.text = MoneyValueAddress;
        PouchAddressInput.text = WalletAddress;
        MilesAddressInput.text = MilesAddress;

        MoneyAddressInput.onValueChanged.AddListener(delegate { MoneyValueAddress = MoneyAddressInput.text; });
        PouchAddressInput.onValueChanged.AddListener(delegate { WalletAddress = PouchAddressInput.text; });
        MilesAddressInput.onValueChanged.AddListener(delegate { MilesAddress = MilesAddressInput.text; });
    }

    public void GetMoneyValues()
    {
        try
        {
            byte[] bytes;

            //money
            bytes = CurrentConnection.ReadBytes(CurrentMoneyAddress, ENCRYPTIONSIZE);
            currentUtil.LoadBank(bytes);

            //miles
            bytes = CurrentConnection.ReadBytes(CurrentMilesAddress, ENCRYPTIONSIZE * 2);
            currentUtil.LoadMilesNow(bytes); currentUtil.LoadMilesForever(bytes);

            //wallet
            bytes = CurrentConnection.ReadBytes(CurrentWalletAddress, ENCRYPTIONSIZE);
            currentUtil.LoadPouch(bytes);

            moneyToUI();
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
#if PLATFORM_ANDROID
            AndroidUSBUtils.CurrentInstance.DebugToast(e.Message);
#endif
        }
    }

    public void SetMoneyValues()
    {
        try
        {
            byte[] bytes;

            //money
            bytes = new byte[ENCRYPTIONSIZE];
            currentUtil.Bank.Write(bytes, 0);
            CurrentConnection.WriteBytes(bytes, CurrentMoneyAddress);

            //miles
            bytes = new byte[ENCRYPTIONSIZE * 2];
            currentUtil.MilesNow.Write(bytes, 0); currentUtil.MilesNow.Write(bytes, ENCRYPTIONSIZE);
            CurrentConnection.WriteBytes(bytes, CurrentMilesAddress);

            //wallet
            bytes = new byte[ENCRYPTIONSIZE];
            currentUtil.Pouch.Write(bytes, 0);
            CurrentConnection.WriteBytes(bytes, CurrentWalletAddress);

            if (UI_ACItemGrid.LastInstanceOfItemGrid != null)
                UI_ACItemGrid.LastInstanceOfItemGrid.PlayHappyParticles();
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
#if PLATFORM_ANDROID
            AndroidUSBUtils.CurrentInstance.DebugToast(e.Message);
#endif
        }
    }

    void moneyToUI()
    {
        BankInput.text = currentUtil.Bank.Value.ToString();
        PouchInput.text = currentUtil.Pouch.Value.ToString();
        MilesInput.text = currentUtil.MilesNow.Value.ToString();
        MilesTotalInput.text = currentUtil.MilesTotal.Value.ToString();
    }

}

public class MoneyMilesUtility
{
    public EncryptedInt32 Bank, Pouch, MilesNow, MilesTotal;

    public MoneyMilesUtility() { }

    public void LoadBank(byte[] bytes) => Bank = EncryptedInt32.ReadVerify(bytes, 0);
    public void LoadPouch(byte[] bytes) => Pouch = EncryptedInt32.ReadVerify(bytes, 0);
    public void LoadMilesNow(byte[] bytes) => MilesNow = EncryptedInt32.ReadVerify(bytes, 0);
    public void LoadMilesForever(byte[] bytes) => MilesTotal = EncryptedInt32.ReadVerify(bytes, UI_MoneyMiles.ENCRYPTIONSIZE);
}
