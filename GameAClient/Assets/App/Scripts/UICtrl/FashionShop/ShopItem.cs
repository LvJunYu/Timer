using System;
using System.Collections;
using System.Collections.Generic;
using GameA.Game;
using SoyEngine.Proto;
using UnityEngine;

public class    ShopItem
{

    public EAvatarPart _avatarType;
    // 序号
    public int Id;
	// 名称
	public string Name;
	// 介绍
	public string Description;
	// 性别
	public int Sex;
	// 能装备角色
	public int Character;
	// 金币价格1天
	public int PriceGoldDay;
	// 金币价格7天
	public int PriceGoldWeek;
	// 金币价格30天
	public int PriceGoldMonth;
	// 金币价格永久
	public int PriceGoldPermanent;
	// 钻石价格一天
	public int PriceDiamondDay;
	// 钻石价格一周
	public int PriceDiamondWeek;
	// 钻石价格一月
	public int PriceDiamondMonth;
	// 钻石价格永久
	public int PriceDiamondPermanent;
	// 大贴图
	public string BigTexture;
	// 小贴图
	public string SmallTexture;
	// 预览图
	public string PreviewTexture;
	// 所在皮肤id
	public int SkinId;

	public ShopItem(Table_HeadParts headParts)
	{
	    this._avatarType = EAvatarPart.AP_Head;
        this.Id = headParts.Id;
        this.Name = headParts.Name;
		this.Description = headParts.Description;
		this.Sex = headParts.Sex;
		this.Character = headParts.Character;
		this.PriceGoldDay = headParts.PriceGoldDay;
		this.PriceGoldMonth = headParts.PriceGoldMonth;
		this.PriceGoldPermanent = headParts.PriceGoldPermanent;
		this.PriceDiamondDay = headParts.PriceDiamondDay;
		this.PriceDiamondWeek = headParts.PriceDiamondWeek;
		this.PriceDiamondMonth = headParts.PriceDiamondMonth;
		this.PriceDiamondPermanent = headParts.PriceDiamondPermanent;
		this.BigTexture = headParts.BigTexture;
		this.SmallTexture = headParts.SmallTexture;
		this.PreviewTexture = headParts.PreviewTexture;
		this.SkinId = headParts.SkinId;

	}
	public ShopItem(Table_UpperBodyParts UpperBodyParts)
	{
		this._avatarType = EAvatarPart.AP_Upper;
        this.Id = UpperBodyParts.Id;
        this.Name = UpperBodyParts.Name;
		this.Description = UpperBodyParts.Description;
		this.Sex = UpperBodyParts.Sex;
		this.Character = UpperBodyParts.Character;
		this.PriceGoldDay = UpperBodyParts.PriceGoldDay;
		this.PriceGoldMonth = UpperBodyParts.PriceGoldMonth;
		this.PriceGoldPermanent = UpperBodyParts.PriceGoldPermanent;
		this.PriceDiamondDay = UpperBodyParts.PriceDiamondDay;
		this.PriceDiamondWeek = UpperBodyParts.PriceDiamondWeek;
		this.PriceDiamondMonth = UpperBodyParts.PriceDiamondMonth;
		this.PriceDiamondPermanent = UpperBodyParts.PriceDiamondPermanent;
		this.BigTexture = UpperBodyParts.BigTexture;
		this.SmallTexture = UpperBodyParts.SmallTexture;
		this.PreviewTexture = UpperBodyParts.PreviewTexture;
		this.SkinId = UpperBodyParts.SkinId;


	}
	public ShopItem(Table_LowerBodyParts LowerBodyParts)
	{
		//this._lowerBodyParts = LowerBodyParts;
		this._avatarType = EAvatarPart.AP_Lower;
	    this.Id = LowerBodyParts.Id;
        this.Name = LowerBodyParts.Name;
		this.Description = LowerBodyParts.Description;
		this.Sex = LowerBodyParts.Sex;
		this.Character = LowerBodyParts.Character;
		this.PriceGoldDay = LowerBodyParts.PriceGoldDay;
		this.PriceGoldMonth = LowerBodyParts.PriceGoldMonth;
		this.PriceGoldPermanent = LowerBodyParts.PriceGoldPermanent;
		this.PriceDiamondDay = LowerBodyParts.PriceDiamondDay;
		this.PriceDiamondWeek = LowerBodyParts.PriceDiamondWeek;
		this.PriceDiamondMonth = LowerBodyParts.PriceDiamondMonth;
		this.PriceDiamondPermanent = LowerBodyParts.PriceDiamondPermanent;
		this.BigTexture = LowerBodyParts.BigTexture;
		this.SmallTexture = LowerBodyParts.SmallTexture;
		this.PreviewTexture = LowerBodyParts.PreviewTexture;
		this.SkinId = LowerBodyParts.SkinId;



	}
	public ShopItem(Table_AppendageParts AppendageParts)
	{
		//this._appendageParts = AppendageParts;
		this._avatarType = EAvatarPart.AP_Appendage;
        this.Id = AppendageParts.Id;
        this.Name = AppendageParts.Name;
		this.Description = AppendageParts.Description;
		this.Sex = AppendageParts.Sex;
		this.Character = AppendageParts.Character;
		this.PriceGoldDay = AppendageParts.PriceGoldDay;
		this.PriceGoldMonth = AppendageParts.PriceGoldMonth;
		this.PriceGoldPermanent = AppendageParts.PriceGoldPermanent;
		this.PriceDiamondDay = AppendageParts.PriceDiamondDay;
		this.PriceDiamondWeek = AppendageParts.PriceDiamondWeek;
		this.PriceDiamondMonth = AppendageParts.PriceDiamondMonth;
		this.PriceDiamondPermanent = AppendageParts.PriceDiamondPermanent;
		this.BigTexture = AppendageParts.BigTexture;
		this.SmallTexture = AppendageParts.SmallTexture;
		this.PreviewTexture = AppendageParts.PreviewTexture;
		this.SkinId = AppendageParts.SkinId;



	}



}

//public enum AvatarPart
//{
//	AP_None,
//	AP_Head,
//	AP_Upper,
//	AP_Lower,
//	AP_Appendage,
//}
