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
        return new VirtualCurrencyPack[] {Tier3MoniPack, Tier4MoniPack, Tier5MoniPack, Tier11MoniPack, Tier23MoniPack, Tier45MoniPack};
    }

    public VirtualGood[] GetGoods()
    {
        return new VirtualGood[] { CardBack_000, CardBack_001, CardBack_002,
			CardFace_000, CardFace_001, CardFace_002,
			Theme_00, Theme_01, Theme_02 };
    }

    public int GetVersion()
    {
        return 1;
    }

    #region const strings
    public const string MONI_ITEM_ID = "currency_moni";
	
	public const string MONI_PACK_TIER3_PRODUCT_ID = "150_pack";
	public const string MONI_PACK_TIER3_ITEM_ID = "150moni_pack";

	public const string MONI_PACK_TIER4_PRODUCT_ID = "200_pack";
	public const string MONI_PACK_TIER4_ITEM_ID = "200moni_pack";

	public const string MONI_PACK_TIER5_PRODUCT_ID = "250_pack";
	public const string MONI_PACK_TIER5_ITEM_ID = "250moni_pack";

	public const string MONI_PACK_TIER11_PRODUCT_ID = "650_pack";
	public const string MONI_PACK_TIER11_ITEM_ID = "650moni_pack";

	public const string MONI_PACK_TIER23_PRODUCT_ID = "1600_pack";
	public const string MONI_PACK_TIER23_ITEM_ID = "1600moni_pack";

	public const string MONI_PACK_TIER45_PRODUCT_ID = "3250_pack";
	public const string MONI_PACK_TIER45_ITEM_ID = "3250moni_pack";

	public const string CARD_BACK_000_ITEM_ID = "CardBack_000";
	public const string CARD_BACK_001_ITEM_ID = "CardBack_001";
	public const string CARD_BACK_002_ITEM_ID = "CardBack_002";

	public const string CARD_FACE_000_ITEM_ID = "CardFace_000";
	public const string CARD_FACE_001_ITEM_ID = "CardFace_001";
	public const string CARD_FACE_002_ITEM_ID = "CardFace_002";

	public const string THEME_00_ITEM_ID = "Theme_00";
	public const string THEME_01_ITEM_ID = "Theme_01";
	public const string THEME_02_ITEM_ID = "Theme_02";
	#endregion

	#region Virtual Currency
	public static VirtualCurrency Moni = new VirtualCurrency("Moni", "Basic Currency", MONI_ITEM_ID);
    #endregion

    #region Virtual Currency Packs
    public static VirtualCurrencyPack Tier3MoniPack = new VirtualCurrencyPack("150 Moni", "150 Moni in a pack", MONI_PACK_TIER3_ITEM_ID, 150, MONI_ITEM_ID, new PurchaseWithMarket(MONI_PACK_TIER3_PRODUCT_ID, 2.99));

	public static VirtualCurrencyPack Tier4MoniPack = new VirtualCurrencyPack("200 Moni", "200 Moni in a pack", MONI_PACK_TIER4_ITEM_ID, 200, MONI_ITEM_ID, new PurchaseWithMarket(MONI_PACK_TIER4_PRODUCT_ID, 3.99));

	public static VirtualCurrencyPack Tier5MoniPack = new VirtualCurrencyPack("250 Moni", "250 Moni in a pack", MONI_PACK_TIER5_ITEM_ID, 250, MONI_ITEM_ID, new PurchaseWithMarket(MONI_PACK_TIER5_PRODUCT_ID, 4.99));
	
	public static VirtualCurrencyPack Tier11MoniPack = new VirtualCurrencyPack("650 Moni", "650 Moni in a pack", MONI_PACK_TIER11_ITEM_ID, 650, MONI_ITEM_ID, new PurchaseWithMarket(MONI_PACK_TIER11_PRODUCT_ID, 10.99));
	
	public static VirtualCurrencyPack Tier23MoniPack = new VirtualCurrencyPack("1600 Moni", "1600 Moni in a pack", MONI_PACK_TIER23_ITEM_ID, 1600, MONI_ITEM_ID, new PurchaseWithMarket(MONI_PACK_TIER23_PRODUCT_ID, 22.99));

	public static VirtualCurrencyPack Tier45MoniPack = new VirtualCurrencyPack("3250 Moni", "3250 Moni in a pack", MONI_PACK_TIER45_ITEM_ID, 3250, MONI_ITEM_ID, new PurchaseWithMarket(MONI_PACK_TIER45_PRODUCT_ID, 44.99));
	#endregion

	#region Virtual Goods
	public static VirtualGood CardBack_000 = new EquippableVG(EquippableVG.EquippingModel.CATEGORY, "Default Card Back", "Default Card Back", CARD_BACK_000_ITEM_ID, new PurchaseWithVirtualItem(MONI_ITEM_ID, 0));
	public static VirtualGood CardBack_001 = new EquippableVG(EquippableVG.EquippingModel.CATEGORY, "Card Back 001", "Default Card Back", CARD_BACK_001_ITEM_ID, new PurchaseWithVirtualItem(MONI_ITEM_ID, 150));
	public static VirtualGood CardBack_002 = new EquippableVG(EquippableVG.EquippingModel.CATEGORY, "Card Back 002", "Default Card Back", CARD_BACK_002_ITEM_ID, new PurchaseWithVirtualItem(MONI_ITEM_ID, 150));

	public static VirtualGood CardFace_000 = new EquippableVG(EquippableVG.EquippingModel.CATEGORY, "Default Card Face", "Default Card Face", CARD_FACE_000_ITEM_ID, new PurchaseWithVirtualItem(MONI_ITEM_ID, 0));
	public static VirtualGood CardFace_001 = new EquippableVG(EquippableVG.EquippingModel.CATEGORY, "Default Card Face", "Default Card Face", CARD_FACE_001_ITEM_ID, new PurchaseWithVirtualItem(MONI_ITEM_ID, 150));
	public static VirtualGood CardFace_002 = new EquippableVG(EquippableVG.EquippingModel.CATEGORY, "Default Card Face", "Default Card Face", CARD_FACE_002_ITEM_ID, new PurchaseWithVirtualItem(MONI_ITEM_ID, 150));

	public static VirtualGood Theme_00 = new EquippableVG(EquippableVG.EquippingModel.CATEGORY, "Default Theme", "Default Theme", THEME_00_ITEM_ID, new PurchaseWithVirtualItem(MONI_ITEM_ID, 0));
	public static VirtualGood Theme_01 = new EquippableVG(EquippableVG.EquippingModel.CATEGORY, "Default Theme", "Default Theme", THEME_01_ITEM_ID, new PurchaseWithVirtualItem(MONI_ITEM_ID, 150));
	public static VirtualGood Theme_02 = new EquippableVG(EquippableVG.EquippingModel.CATEGORY, "Default Theme", "Default Theme", THEME_02_ITEM_ID, new PurchaseWithVirtualItem(MONI_ITEM_ID, 150));
	#endregion

	#region Virtual Categories
	public static VirtualCategory CardBackCategory = new VirtualCategory(
        "Card Back", new List<string>(new string[] { CARD_BACK_000_ITEM_ID, CARD_BACK_001_ITEM_ID, CARD_BACK_002_ITEM_ID }));
    
    public static VirtualCategory CardFaceCategory = new VirtualCategory(
        "Card Face", new List<string>(new string[] { CARD_FACE_000_ITEM_ID, CARD_FACE_001_ITEM_ID, CARD_FACE_002_ITEM_ID }));

    public static VirtualCategory ThemeCategory = new VirtualCategory(
        "Theme", new List<string>(new string[] { THEME_00_ITEM_ID, THEME_01_ITEM_ID, THEME_02_ITEM_ID }));
    #endregion
}
