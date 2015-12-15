using Soomla.Store;
using System;
using System.Collections.Generic;

public class FlipCardStoreAsset : IStoreAssets
{
    public VirtualCategory[] GetCategories()
    {
        return new VirtualCategory[] { CardBackCategory, CardFaceCategory, ThemeCategory };
    }

    public VirtualCurrency[] GetCurrencies()
    {
        return new VirtualCurrency[] { Moni };
    }

    public VirtualCurrencyPack[] GetCurrencyPacks()
    {
        return new VirtualCurrencyPack[] { Tier1MoniPack };
    }

    public VirtualGood[] GetGoods()
    {
        return new VirtualGood[] { CardBack_000, CardFace_000, Theme_00 };
    }

    public int GetVersion()
    {
        return 0;
    }

    #region const strings
    public const string MONI_ITEM_ID = "currency_moni";

    public const string MONI_PACK_TIER1_PRODUCT_ID = "600_pack";
    public const string MONI_PACK_TIER1_ITEM_ID = "600moni_pack";

    public const string CARD_BACK_000_ITEM_ID = "CardBack_000";

    public const string CARD_FACE_000_ITEM_ID = "CardFace_000";

    public const string THEME_00_ITEM_ID = "Theme_00";
    #endregion

    #region Virtual Currency
    public static VirtualCurrency Moni = new VirtualCurrency("Moni", "Basic Currency", MONI_ITEM_ID);
    #endregion

    #region Virtual Currency Packs
    public static VirtualCurrencyPack Tier1MoniPack = new VirtualCurrencyPack("600 Moni", "600 Moni in a pack", MONI_PACK_TIER1_ITEM_ID, 600, MONI_ITEM_ID, new PurchaseWithMarket(MONI_PACK_TIER1_PRODUCT_ID, 0.99));
    #endregion

    #region Virtual Goods
    public static VirtualGood CardBack_000 = new EquippableVG(EquippableVG.EquippingModel.CATEGORY, "Default Card Back", "Default Card Back", CARD_BACK_000_ITEM_ID, new PurchaseWithVirtualItem(MONI_ITEM_ID, 0));

    public static VirtualGood CardFace_000 = new EquippableVG(EquippableVG.EquippingModel.CATEGORY, "Default Card Face", "Default Card Face", CARD_FACE_000_ITEM_ID, new PurchaseWithVirtualItem(MONI_ITEM_ID, 0));

    public static VirtualGood Theme_00 = new EquippableVG(EquippableVG.EquippingModel.CATEGORY, "Default Theme", "Default Theme", THEME_00_ITEM_ID, new PurchaseWithVirtualItem(MONI_ITEM_ID, 0));
    #endregion

    #region Virtual Categories
    public static VirtualCategory CardBackCategory = new VirtualCategory(
        "Card Back", new List<string>(new string[] { CARD_BACK_000_ITEM_ID }));
    
    public static VirtualCategory CardFaceCategory = new VirtualCategory(
        "Card Face", new List<string>(new string[] { CARD_FACE_000_ITEM_ID }));

    public static VirtualCategory ThemeCategory = new VirtualCategory(
        "Theme", new List<string>(new string[] { THEME_00_ITEM_ID }));
    #endregion
}
