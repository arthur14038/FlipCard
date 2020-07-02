using Soomla.Store;
using System;
using System.Collections.Generic;

public class FlipCardStoreAsset
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
        return new VirtualGood[] { CardBack_000, CardBack_001, CardBack_002, CardBack_003,
			CardBack_004, CardBack_005, CardBack_006, CardBack_007, CardBack_008,
			CardFace_000, CardFace_001, CardFace_002, CardFace_003, CardFace_004,
			CardFace_005, CardFace_006, CardFace_007, CardFace_008, Theme_00,
			Theme_01, Theme_02, Theme_03, Theme_04, Theme_05, Theme_06,
			Theme_07, Theme_08
		};
    }

    public int GetVersion()
    {
        return 3;
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
	public const string CARD_BACK_003_ITEM_ID = "CardBack_003";
	public const string CARD_BACK_004_ITEM_ID = "CardBack_004";
	public const string CARD_BACK_005_ITEM_ID = "CardBack_005";
	public const string CARD_BACK_006_ITEM_ID = "CardBack_006";
	public const string CARD_BACK_007_ITEM_ID = "CardBack_007";
	public const string CARD_BACK_008_ITEM_ID = "CardBack_008";

	public const string CARD_FACE_000_ITEM_ID = "CardFace_000";
	public const string CARD_FACE_001_ITEM_ID = "CardFace_001";
	public const string CARD_FACE_002_ITEM_ID = "CardFace_002";
	public const string CARD_FACE_003_ITEM_ID = "CardFace_003";
	public const string CARD_FACE_004_ITEM_ID = "CardFace_004";
	public const string CARD_FACE_005_ITEM_ID = "CardFace_005";
	public const string CARD_FACE_006_ITEM_ID = "CardFace_006";
	public const string CARD_FACE_007_ITEM_ID = "CardFace_007";
	public const string CARD_FACE_008_ITEM_ID = "CardFace_008";

	public const string THEME_00_ITEM_ID = "Theme_00";
	public const string THEME_01_ITEM_ID = "Theme_01";
	public const string THEME_02_ITEM_ID = "Theme_02";
	public const string THEME_03_ITEM_ID = "Theme_03";
	public const string THEME_04_ITEM_ID = "Theme_04";
	public const string THEME_05_ITEM_ID = "Theme_05";
	public const string THEME_06_ITEM_ID = "Theme_06";
	public const string THEME_07_ITEM_ID = "Theme_07";
	public const string THEME_08_ITEM_ID = "Theme_08";
	#endregion

	#region Virtual Currency
	public static VirtualCurrency Moni = new VirtualCurrency("Moni", "Basic Currency", MONI_ITEM_ID);
    #endregion

    #region Virtual Currency Packs
    public static VirtualCurrencyPack Tier3MoniPack = new VirtualCurrencyPack("150 Moni", "150 Moni in a pack", MONI_PACK_TIER3_ITEM_ID, 150, MONI_ITEM_ID, new PurchaseWithMarket(MONI_PACK_TIER3_PRODUCT_ID, 90));

	public static VirtualCurrencyPack Tier4MoniPack = new VirtualCurrencyPack("200 Moni", "200 Moni in a pack", MONI_PACK_TIER4_ITEM_ID, 200, MONI_ITEM_ID, new PurchaseWithMarket(MONI_PACK_TIER4_PRODUCT_ID, 120));

	public static VirtualCurrencyPack Tier5MoniPack = new VirtualCurrencyPack("250 Moni", "250 Moni in a pack", MONI_PACK_TIER5_ITEM_ID, 250, MONI_ITEM_ID, new PurchaseWithMarket(MONI_PACK_TIER5_PRODUCT_ID, 150));
	
	public static VirtualCurrencyPack Tier11MoniPack = new VirtualCurrencyPack("650 Moni", "650 Moni in a pack", MONI_PACK_TIER11_ITEM_ID, 650, MONI_ITEM_ID, new PurchaseWithMarket(MONI_PACK_TIER11_PRODUCT_ID, 330));
	
	public static VirtualCurrencyPack Tier23MoniPack = new VirtualCurrencyPack("1600 Moni", "1600 Moni in a pack", MONI_PACK_TIER23_ITEM_ID, 1600, MONI_ITEM_ID, new PurchaseWithMarket(MONI_PACK_TIER23_PRODUCT_ID, 690));

	public static VirtualCurrencyPack Tier45MoniPack = new VirtualCurrencyPack("3250 Moni", "3250 Moni in a pack", MONI_PACK_TIER45_ITEM_ID, 3250, MONI_ITEM_ID, new PurchaseWithMarket(MONI_PACK_TIER45_PRODUCT_ID, 1350));
	#endregion

	#region Virtual Goods
	public static VirtualGood CardBack_000 = new EquippableVG(EquippableVG.EquippingModel.CATEGORY, "Default Card Back", "Default Card Back", CARD_BACK_000_ITEM_ID, new PurchaseWithVirtualItem(MONI_ITEM_ID, 0));
	public static VirtualGood CardBack_001 = new EquippableVG(EquippableVG.EquippingModel.CATEGORY, "Card Back 001", "Card Back 001", CARD_BACK_001_ITEM_ID, new PurchaseWithVirtualItem(MONI_ITEM_ID, 150));
	public static VirtualGood CardBack_002 = new EquippableVG(EquippableVG.EquippingModel.CATEGORY, "Card Back 002", "Card Back 002", CARD_BACK_002_ITEM_ID, new PurchaseWithVirtualItem(MONI_ITEM_ID, 150));
	public static VirtualGood CardBack_003 = new EquippableVG(EquippableVG.EquippingModel.CATEGORY, "Card Back 003", "Card Back 003", CARD_BACK_003_ITEM_ID, new PurchaseWithVirtualItem(MONI_ITEM_ID, 150));
	public static VirtualGood CardBack_004 = new EquippableVG(EquippableVG.EquippingModel.CATEGORY, "Card Back 004", "Card Back 004", CARD_BACK_004_ITEM_ID, new PurchaseWithVirtualItem(MONI_ITEM_ID, 150));
	public static VirtualGood CardBack_005 = new EquippableVG(EquippableVG.EquippingModel.CATEGORY, "Card Back 005", "Card Back 005", CARD_BACK_005_ITEM_ID, new PurchaseWithVirtualItem(MONI_ITEM_ID, 150));
	public static VirtualGood CardBack_006 = new EquippableVG(EquippableVG.EquippingModel.CATEGORY, "Card Back 006", "Card Back 006", CARD_BACK_006_ITEM_ID, new PurchaseWithVirtualItem(MONI_ITEM_ID, 150));
	public static VirtualGood CardBack_007 = new EquippableVG(EquippableVG.EquippingModel.CATEGORY, "Card Back 007", "Card Back 007", CARD_BACK_007_ITEM_ID, new PurchaseWithVirtualItem(MONI_ITEM_ID, 150));
	public static VirtualGood CardBack_008 = new EquippableVG(EquippableVG.EquippingModel.CATEGORY, "Card Back 008", "Card Back 008", CARD_BACK_008_ITEM_ID, new PurchaseWithVirtualItem(MONI_ITEM_ID, 150));

	public static VirtualGood CardFace_000 = new EquippableVG(EquippableVG.EquippingModel.CATEGORY, "Default Card Face", "Default Card Face", CARD_FACE_000_ITEM_ID, new PurchaseWithVirtualItem(MONI_ITEM_ID, 0));
	public static VirtualGood CardFace_001 = new EquippableVG(EquippableVG.EquippingModel.CATEGORY, "Card Face 001", "Card Face 001", CARD_FACE_001_ITEM_ID, new PurchaseWithVirtualItem(MONI_ITEM_ID, 150));
	public static VirtualGood CardFace_002 = new EquippableVG(EquippableVG.EquippingModel.CATEGORY, "Card Face 002", "Card Face 002", CARD_FACE_002_ITEM_ID, new PurchaseWithVirtualItem(MONI_ITEM_ID, 150));
	public static VirtualGood CardFace_003 = new EquippableVG(EquippableVG.EquippingModel.CATEGORY, "Card Face 003", "Card Face 003", CARD_FACE_003_ITEM_ID, new PurchaseWithVirtualItem(MONI_ITEM_ID, 150));
	public static VirtualGood CardFace_004 = new EquippableVG(EquippableVG.EquippingModel.CATEGORY, "Card Face 004", "Card Face 004", CARD_FACE_004_ITEM_ID, new PurchaseWithVirtualItem(MONI_ITEM_ID, 150));
	public static VirtualGood CardFace_005 = new EquippableVG(EquippableVG.EquippingModel.CATEGORY, "Card Face 005", "Card Face 005", CARD_FACE_005_ITEM_ID, new PurchaseWithVirtualItem(MONI_ITEM_ID, 150));
	public static VirtualGood CardFace_006 = new EquippableVG(EquippableVG.EquippingModel.CATEGORY, "Card Face 006", "Card Face 006", CARD_FACE_006_ITEM_ID, new PurchaseWithVirtualItem(MONI_ITEM_ID, 150));
	public static VirtualGood CardFace_007 = new EquippableVG(EquippableVG.EquippingModel.CATEGORY, "Card Face 007", "Card Face 007", CARD_FACE_007_ITEM_ID, new PurchaseWithVirtualItem(MONI_ITEM_ID, 150));
	public static VirtualGood CardFace_008 = new EquippableVG(EquippableVG.EquippingModel.CATEGORY, "Card Face 008", "Card Face 008", CARD_FACE_008_ITEM_ID, new PurchaseWithVirtualItem(MONI_ITEM_ID, 150));

	public static VirtualGood Theme_00 = new EquippableVG(EquippableVG.EquippingModel.CATEGORY, "北國(夜)", "Default Theme", THEME_00_ITEM_ID, new PurchaseWithVirtualItem(MONI_ITEM_ID, 0));
	public static VirtualGood Theme_01 = new EquippableVG(EquippableVG.EquippingModel.CATEGORY, "北國(夏)", "Theme 001", THEME_01_ITEM_ID, new PurchaseWithVirtualItem(MONI_ITEM_ID, 200));
	public static VirtualGood Theme_02 = new EquippableVG(EquippableVG.EquippingModel.CATEGORY, "三角塔(日)", "Theme 002", THEME_02_ITEM_ID, new PurchaseWithVirtualItem(MONI_ITEM_ID, 150));
	public static VirtualGood Theme_03 = new EquippableVG(EquippableVG.EquippingModel.CATEGORY, "三角塔(夜)", "Theme 003", THEME_03_ITEM_ID, new PurchaseWithVirtualItem(MONI_ITEM_ID, 500));
	public static VirtualGood Theme_04 = new EquippableVG(EquippableVG.EquippingModel.CATEGORY, "美麗的陷阱", "Theme 004", THEME_04_ITEM_ID, new PurchaseWithVirtualItem(MONI_ITEM_ID, 250));
	public static VirtualGood Theme_05 = new EquippableVG(EquippableVG.EquippingModel.CATEGORY, "宇宙", "Theme 005", THEME_05_ITEM_ID, new PurchaseWithVirtualItem(MONI_ITEM_ID, 1250));
	public static VirtualGood Theme_06 = new EquippableVG(EquippableVG.EquippingModel.CATEGORY, "小廟", "Theme 006", THEME_06_ITEM_ID, new PurchaseWithVirtualItem(MONI_ITEM_ID, 850));
	public static VirtualGood Theme_07 = new EquippableVG(EquippableVG.EquippingModel.CATEGORY, "三瓣花", "Theme 007", THEME_07_ITEM_ID, new PurchaseWithVirtualItem(MONI_ITEM_ID, 150));
	public static VirtualGood Theme_08 = new EquippableVG(EquippableVG.EquippingModel.CATEGORY, "白蓮花", "Theme 008", THEME_08_ITEM_ID, new PurchaseWithVirtualItem(MONI_ITEM_ID, 200));
	#endregion

	#region Virtual Categories
	public static VirtualCategory CardBackCategory = new VirtualCategory(
        "Card Back", new List<string>(new string[] { CARD_BACK_000_ITEM_ID, CARD_BACK_001_ITEM_ID, CARD_BACK_002_ITEM_ID,
		CARD_BACK_003_ITEM_ID, CARD_BACK_004_ITEM_ID, CARD_BACK_005_ITEM_ID, CARD_BACK_006_ITEM_ID,
		CARD_BACK_007_ITEM_ID, CARD_BACK_008_ITEM_ID}));
    
    public static VirtualCategory CardFaceCategory = new VirtualCategory(
        "Card Face", new List<string>(new string[] { CARD_FACE_000_ITEM_ID, CARD_FACE_001_ITEM_ID, CARD_FACE_002_ITEM_ID,
		CARD_FACE_003_ITEM_ID, CARD_FACE_004_ITEM_ID, CARD_FACE_005_ITEM_ID, CARD_FACE_006_ITEM_ID,
		CARD_FACE_007_ITEM_ID, CARD_FACE_008_ITEM_ID}));

    public static VirtualCategory ThemeCategory = new VirtualCategory(
        "Theme", new List<string>(new string[] { THEME_00_ITEM_ID, THEME_01_ITEM_ID, THEME_02_ITEM_ID,
		THEME_03_ITEM_ID, THEME_04_ITEM_ID, THEME_05_ITEM_ID, THEME_06_ITEM_ID, THEME_07_ITEM_ID, THEME_08_ITEM_ID}));
    #endregion
}
