using UnityEngine;
using System;
using System.Collections.Generic;
using GoogleMobileAds.Api;

public class AdBanner : SingletonMonoBehavior<AdBanner> {

	public enum BannerSize{
		
		BANNER,
		MEDIUM_RECTANGLE,
		FULL_BANNER,
		LEADERBOARD,
		SMART_BANNER
	}

	public string unitId;
	public BannerSize size = BannerSize.BANNER;
	public AdPosition position = AdPosition.Top;

	private BannerView bannerView;
	private Dictionary<BannerSize,AdSize> adSize = new Dictionary<BannerSize, AdSize>(){
		{BannerSize.BANNER , AdSize.Banner},
		{BannerSize.MEDIUM_RECTANGLE , AdSize.MediumRectangle},
		{BannerSize.FULL_BANNER , AdSize.IABBanner},
		{BannerSize.LEADERBOARD , AdSize.Leaderboard},
		{BannerSize.SMART_BANNER , AdSize.SmartBanner}
	};

	void Start(){

		this.RequestBanner();
	}

	private void RequestBanner(){

		if(!string.IsNullOrEmpty(this.unitId)){

			this.bannerView = new BannerView(this.unitId , this.adSize[this.size] , this.position);

			AdRequest.Builder _builder = new AdRequest.Builder();
			
			if(Debug.isDebugBuild) _builder.AddTestDevice(AdCommon.DeviceIdForAdmob);
			
			this.bannerView.LoadAd(_builder.Build());
		}
	}
	
	public void ShowBanner(){
		
		if(this.bannerView != null){

			this.bannerView.Show();
		}
	}
	
	public void HideBanner(){
		
		if(this.bannerView != null){

			this.bannerView.Hide();
		}
	}

	#region Banner Event
	public void RegisterBannerLoaded(EventHandler<EventArgs> _handle){
		if(this.bannerView == null) return;
		this.bannerView.AdLoaded += _handle;
	}
	public void RegisterBannerFailedToLoad(EventHandler<AdFailedToLoadEventArgs> _handle){
		if(this.bannerView == null) return;
		this.bannerView.AdFailedToLoad += _handle;
	}
	public void RegisterBannerOpened(EventHandler<EventArgs> _handle){
		if(this.bannerView == null) return;
		this.bannerView.AdOpened += _handle;
	}
	public void RegisterBannerClosing(EventHandler<EventArgs> _handle){
		if(this.bannerView == null) return;
		this.bannerView.AdClosing += _handle;
	}
	public void RegisterBannerClosed(EventHandler<EventArgs> _handle){
		if(this.bannerView == null) return;
		this.bannerView.AdClosed += _handle;
	}
	public void RegisterBannerLeftApplication(EventHandler<EventArgs> _handle){
		if(this.bannerView == null) return;
		this.bannerView.AdLeftApplication += _handle;
	}
	
	public void CancelBannerLoaded(EventHandler<EventArgs> _handle){
		if(this.bannerView == null) return;
		this.bannerView.AdLoaded -= _handle;
	}
	public void CancelBannerFailedToLoad(EventHandler<AdFailedToLoadEventArgs> _handle){
		if(this.bannerView == null) return;
		this.bannerView.AdFailedToLoad -= _handle;
	}
	public void CancelBannerOpened(EventHandler<EventArgs> _handle){
		if(this.bannerView == null) return;
		this.bannerView.AdOpened -= _handle;
	}
	public void CancelBannerClosing(EventHandler<EventArgs> _handle){
		if(this.bannerView == null) return;
		this.bannerView.AdClosing -= _handle;
	}
	public void CancelBannerClosed(EventHandler<EventArgs> _handle){
		if(this.bannerView == null) return;
		this.bannerView.AdClosed -= _handle;
	}
	public void CancelBannerLeftApplication(EventHandler<EventArgs> _handle){
		if(this.bannerView == null) return;
		this.bannerView.AdLeftApplication -= _handle;
	}
	#endregion
}
