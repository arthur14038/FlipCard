using UnityEngine;
using System;
using GoogleMobileAds.Api;

public class AdInterstitial : SingletonMonoBehavior<AdInterstitial> {

	public string unitId;
	private InterstitialAd interstitialAd;

	public void RequestInterstitial(){
		
		if(!string.IsNullOrEmpty(this.unitId)){

			if(this.interstitialAd != null){
				this.interstitialAd.Destroy();
				this.interstitialAd = null;
			}
			
			this.interstitialAd = new InterstitialAd(this.unitId);
			
			AdRequest.Builder _builder = new AdRequest.Builder();
			
			if(Debug.isDebugBuild) _builder.AddTestDevice(AdCommon.DeviceIdForAdmob);
			
			this.interstitialAd.LoadAd(_builder.Build());
		}
	}

	public bool IsLoaded()
	{
		return this.interstitialAd.IsLoaded();
	}

	public void ShowInterstitial(){

		if(this.interstitialAd != null && this.interstitialAd.IsLoaded()){

			this.interstitialAd.Show();
		}
	}

	#region Intersitial Ad Event
	public void RegisterInterstitialLoaded(EventHandler<EventArgs> _handle){
		if(this.interstitialAd == null) return;
		this.interstitialAd.AdLoaded += _handle;
	}
	public void RegisterInterstitialFailedToLoad(EventHandler<AdFailedToLoadEventArgs> _handle){
		if(this.interstitialAd == null) return;
		this.interstitialAd.AdFailedToLoad += _handle;
	}
	public void RegisterInterstitialOpened(EventHandler<EventArgs> _handle){
		if(this.interstitialAd == null) return;
		this.interstitialAd.AdOpened += _handle;
	}
	public void RegisterInterstitialClosing(EventHandler<EventArgs> _handle){
		if(this.interstitialAd == null) return;
		this.interstitialAd.AdClosing += _handle;
	}
	public void RegisterInterstitialClosed(EventHandler<EventArgs> _handle){
		if(this.interstitialAd == null) return;
		this.interstitialAd.AdClosed += _handle;
	}
	public void RegisterInterstitialLeftApplication(EventHandler<EventArgs> _handle){
		if(this.interstitialAd == null) return;
		this.interstitialAd.AdLeftApplication += _handle;
	}
	
	public void CancelInterstitialLoaded(EventHandler<EventArgs> _handle){
		if(this.interstitialAd == null) return;
		this.interstitialAd.AdLoaded -= _handle;
	}
	public void CancelInterstitialFailedToLoad(EventHandler<AdFailedToLoadEventArgs> _handle){
		if(this.interstitialAd == null) return;
		this.interstitialAd.AdFailedToLoad -= _handle;
	}
	public void CancelInterstitialOpened(EventHandler<EventArgs> _handle){
		if(this.interstitialAd == null) return;
		this.interstitialAd.AdOpened -= _handle;
	}
	public void CancelInterstitialClosing(EventHandler<EventArgs> _handle){
		if(this.interstitialAd == null) return;
		this.interstitialAd.AdClosing -= _handle;
	}
	public void CancelInterstitialClosed(EventHandler<EventArgs> _handle){
		if(interstitialAd == null) return;
		interstitialAd.AdClosed -= _handle;
	}
	public void CancelInterstitialLeftApplication(EventHandler<EventArgs> _handle){
		if(interstitialAd == null) return;
		interstitialAd.AdLeftApplication -= _handle;
	}
	#endregion
}
