package com.lolofinil.androidplugin;

import android.os.Bundle;
import android.util.Log;
import android.view.View;
import android.view.ViewGroup;

import com.apowo.gsdk.GSDKJNI;
import com.apowo.gsdk.PlatformLib.util.GlobalExceptionHandler;
import com.apowo.platformWrap.Platform;
import com.unity3d.player.UnityPlayerNativeActivity;

public class AndroidPluginActivity extends UnityPlayerNativeActivity {
	public static String TAG = AndroidPluginActivity.class.getSimpleName();

	@Override
	 protected void onCreate(Bundle savedInstanceState) {
		 super.onCreate(savedInstanceState);
		 Log.d(TAG, "onCreate called!");
		 
		 Log.d(TAG, "GSDK initialize began");
		 Platform platfrom = Platform.Instance();
		 ViewGroup viewGroup = (ViewGroup)findViewById(android.R.id.content);
		 View view = viewGroup.getChildAt(0);
		 platfrom.Initialize(this, view);
		 GSDKJNI.Initialize(platfrom);
		 Log.d(TAG, "GSDK initialize ended");
		 
		 GlobalExceptionHandler.RegisterGlobalExceptionHandler();
	 }
}
