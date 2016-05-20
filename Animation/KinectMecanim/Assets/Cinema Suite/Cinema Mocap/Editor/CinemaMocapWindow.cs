using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;

/// <summary>
/// The main window for Cinema Mocap.
/// </summary>
public class CinemaMocapWindow : EditorWindow
{
    #region UI

    private ZigEditorDepthViewer depthViewer = new ZigEditorDepthViewer();
    private ZigEditorImageViewer imageViewer = new ZigEditorImageViewer();
    //private ZigEditorMixedViewer mixedViewer = new ZigEditorMixedViewer();
    private ZigEditorUsersRadar usersRadar = new ZigEditorUsersRadar();

    private ResolutionData workingResolution;

    private const int UI_SPACING = 10;

    private float desiredTilt = 0f;
    private ZigResolution kinectCaptureResolution = ZigResolution.QVGA_320_x_240;
    private NUIViewerType viewerType = NUIViewerType.DepthViewer;
    //private RecordingStart recordingStart = RecordingStart.ButtonPress;
    private bool viewNodes = true;
    private bool isAdvancedExposed = false;

    private Texture2D mocapLogo;
    private Texture2D radarLogo;
    private Texture2D recordingImage;

    GUIContent[] delays = { new GUIContent("0 Seconds"), new GUIContent("3 Seconds"), new GUIContent("5 Seconds"), new GUIContent("10 Seconds") };
    private int delaySelection = 0;
    
    #endregion

    #region Zigfu & Recording

    private bool isNUIOn = false;

    private ZigInputType inputType = ZigInputType.KinectSDK;
    private ZigInputSettings settings = new ZigInputSettings();
    private NUIHumanoidAnimation captureData = null;

    private RecordingState captureState = RecordingState.NotRecording;
    private SmoothingOptions smoothingOptions = SmoothingOptions.Light;
    private TransformationType transformationType = TransformationType.TransRotLoc;

    private const string SOURCE_FILE = "Assets/Cinema Suite/Cinema Mocap/Resources/Cinema_Mocap_Humanoid.dae";
    private const string SOURCE_FILE_MATRIX = "Assets/Cinema Suite/Cinema Mocap/Resources/Cinema_Mocap_Humanoid_Matrix.dae";
    private const string FILE_DESTINATION = "Assets/Cinema Suite/Cinema Mocap/Animations//{0}.dae";

    private Stopwatch stopwatch = new Stopwatch();
    private int delaySeconds = 0;

    private GameObject cinema_mocap_humanoid_prefab;
    private GameObject cinema_mocap_humanoid_instance;
    private NUIInputToRigMapper inputMapper;
    private ColladaRigData rigData;
    #endregion

    #region Language
    private const string TITLE = "Cinema Mocap";
    private const string MENU_ITEM = "Window/Cinema Suite/Cinema Mocap";
    private const string NAME_DUPLICATE_ERROR_MSG = "{0}.dae exists. Saving as {1}.dae...";
    private const string DEVICE_TRACKING = "Device Tracking";
    private const string ON = "ON";
    private const string OFF = "OFF";
    private const string VIEWER = "Viewer";

    private GUIContent KINECT_RESOLUTION = new GUIContent("Kinect Resolution");
    private GUIContent SMOOTHING_OPTIONS = new GUIContent("Smoothing Options");
    private GUIContent DEVICE_TILT = new GUIContent("Device Tilt");
    private GUIContent JOINT_VIEW = new GUIContent("Joint View","Toggle joint markers on the display.");
    private string fileName = "Animation";
    #endregion

    #region Enums

    /// <summary>
    /// Various methods for starting a recording
    /// </summary>
    private enum RecordingStart
    {
        ButtonPress,
        tPose,
        HoverButton
    }

    /// <summary>
    /// Options for preset Kinect hardware smoothing
    /// </summary>
    private enum SmoothingOptions
    {
        None,
        Light,
        Moderate,
        Heavy
    }
    
    /// <summary>
    /// States for capturing motion
    /// </summary>
    private enum RecordingState
    {
        NotRecording,
        PreRecording,
        Recording
    }

    /// <summary>
    /// COLLADA encoding types (Advanced)
    /// </summary>
    private enum TransformationType
    {
        TransRotLoc,
        Matrix
    }

    #endregion

    /// <summary>
    /// Sets the window title and minimum pane size
    /// </summary>
    public void Awake()
    {
        base.title = TITLE;
        this.minSize = new Vector2(700f, 500f);

        mocapLogo = Resources.Load("Cinema Mocap") as Texture2D;
        if (mocapLogo == null)
        {
            UnityEngine.Debug.LogWarning("Cinema Mocap image missing from Resources folder.");
        }
        radarLogo = Resources.Load("Cinema Radar") as Texture2D;
        if (radarLogo == null)
        {
            UnityEngine.Debug.LogWarning("Cinema Radar image missing from Resources folder.");
        }
        recordingImage = Resources.Load("Recording") as Texture2D;
        if (recordingImage == null)
        {
            UnityEngine.Debug.LogWarning("Recording Image missing from Resources folder.");
        }

        cinema_mocap_humanoid_prefab = Resources.Load("Player") as GameObject;
        if (cinema_mocap_humanoid_prefab == null)
        {
            UnityEngine.Debug.LogError("Cinema_Mocap_Humanoid.dae is missing from the Resources folder. This item is required for the system.");
        }
        rigData = ColladaUtility.ReadRigData(SOURCE_FILE);
        inputMapper = new NUIInputToRigMapper(rigData);
        
        workingResolution = ResolutionData.FromZigResolution(kinectCaptureResolution);
	}

    /// <summary>
    /// Update the logic for the window.
    /// </summary>
    protected void Update()
    {
        if (isNUIOn && ZigEditorInput.Instance.ReaderInited)
        {
            // Update Device
            ZigEditorInput.Instance.Update();

            // Get the tracked user
            ZigTrackedUser user = null;
            foreach (KeyValuePair<int, ZigTrackedUser> trackedUser in ZigEditorInput.Instance.TrackedUsers)
            {
                user = trackedUser.Value;
            }

            // Update viewers
            if (viewerType == NUIViewerType.DepthViewer)
            {
                depthViewer.Update(ZigEditorInput.Depth, user, viewNodes);
            }
            else if (viewerType == NUIViewerType.ImageViewer)
            {
                imageViewer.Update(ZigEditorInput.Image, kinectCaptureResolution, user, viewNodes);
            }
            //else if (viewerType == NUIViewerType.DepthAndImageViewer)
            //{
            //    mixedViewer.Update(ZigEditorInput.Image, ZigEditorInput.Depth, kinectCaptureResolution);
            //}
            
            // Check if in pre-recording state
            if (captureState == RecordingState.PreRecording)
            {
                long timeLeft = (delaySeconds * 1000) - stopwatch.ElapsedMilliseconds;
                if (timeLeft <= 0)
                {
                    // Begin recording data
                    BeginRecording();
                }
            }
            
            // Add a keyframe if in recording state
            if (user != null && captureData != null && user.SkeletonTracked && captureState == RecordingState.Recording)
            {
                captureData.AddKeyframe(user.Skeleton, stopwatch.ElapsedMilliseconds);
            }

            // Pose the preview model
            if (inputMapper != null && cinema_mocap_humanoid_instance != null && user != null && user.SkeletonTracked)
            {
                RealtimeHumanoidPosing poser = cinema_mocap_humanoid_instance.GetComponent<RealtimeHumanoidPosing>();
                poser.SetRotations(inputMapper.GetRotations(user.Skeleton));
                poser.SetWorldPosition(inputMapper.GetHipPosition(user.Skeleton));
            }
            
            Repaint();
        }
    }

    /// <summary>
    /// Draw the Window's contents
    /// </summary>
	protected void OnGUI()
    {
        if (workingResolution == null)
        {
            workingResolution = ResolutionData.FromZigResolution(kinectCaptureResolution);
        }
        float aspectRatio = workingResolution.Width / workingResolution.Height;
        float textureWidth = (base.position.width - (UI_SPACING * 3)) / 2;
        float textureHeight = textureWidth * aspectRatio;
        float newDesiredTilt = desiredTilt;

        if (isNUIOn && ZigEditorInput.Instance.ReaderInited)
        {
            if (viewerType == NUIViewerType.DepthViewer)
            {
                GUI.DrawTexture(new Rect(UI_SPACING, UI_SPACING, textureWidth, textureHeight), depthViewer.Texture);
            }
            else if (viewerType == NUIViewerType.ImageViewer)
            {
                GUI.DrawTexture(new Rect(UI_SPACING, UI_SPACING, textureWidth, textureHeight), imageViewer.Texture);
            }
            //else if (viewerType == NUIViewerType.DepthAndImageViewer)
            //{
            //    GUI.DrawTexture(new Rect(UI_SPACING, UI_SPACING, textureWidth, textureHeight), mixedViewer.Texture);
            //}
            if (captureState == RecordingState.PreRecording)
            {
                int timeLeft = (int)(((delaySeconds * 1000) - stopwatch.ElapsedMilliseconds)/1000) + 1;
                GUIStyle countdownFont = new GUIStyle(EditorStyles.label);
                countdownFont.fontSize = (int)(textureHeight/6);
                countdownFont.normal.textColor = Color.white;
                Vector2 size = countdownFont.CalcSize(new GUIContent(timeLeft.ToString()));
                GUI.Label(new Rect(textureWidth - (size.x), textureHeight - size.y, size.x, size.y), timeLeft.ToString(), countdownFont);
            }
            if (captureState == RecordingState.Recording)
            {
                GUI.DrawTexture(new Rect((textureWidth) - recordingImage.width, (UI_SPACING + textureHeight) - recordingImage.height, recordingImage.width, recordingImage.height), recordingImage);
            }
            GUI.DrawTexture(new Rect((UI_SPACING * 2) + textureWidth, UI_SPACING, textureWidth, textureHeight), radarLogo);
            usersRadar.Render(ZigEditorInput.Instance, new Rect((UI_SPACING * 2) + textureWidth, UI_SPACING, textureWidth, textureHeight));
        }
        else if (mocapLogo != null && workingResolution != null)
        {
            // Draw place holders
            GUI.DrawTexture(new Rect(UI_SPACING, UI_SPACING, textureWidth, textureHeight), mocapLogo);
            GUI.DrawTexture(new Rect((UI_SPACING * 2) + textureWidth, UI_SPACING, textureWidth, textureHeight), radarLogo);
        }

        float panelHeight = base.position.height - textureHeight - (UI_SPACING * 3);
        if (panelHeight > 5)
        {
            GUILayout.BeginArea(new Rect(UI_SPACING, textureHeight + (UI_SPACING * 2), textureWidth, panelHeight), string.Empty, "box");
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel(new GUIContent(DEVICE_TRACKING));
                bool toggleOn = false;
                Color temp = GUI.color;
                if (isNUIOn)
                {
                    GUI.color = Color.green;
                    toggleOn = GUILayout.Toggle(isNUIOn, ON, EditorStyles.miniButton);
                }
                else
                {
                    GUI.color = Color.red;
                    toggleOn = GUILayout.Toggle(isNUIOn, OFF, EditorStyles.miniButton);
                }
                GUI.color = temp;
                EditorGUILayout.EndHorizontal();

                if (toggleOn && !isNUIOn)
                {
                    turnOnNUIDevice();
                }
                else if (!toggleOn && isNUIOn)
                {
                    turnOffNUIDevice();
                }
                
                viewerType = (NUIViewerType)EditorGUILayout.EnumPopup(new GUIContent(VIEWER), viewerType);
                
                List<GUIContent> resolutions = new List<GUIContent>();
                foreach(string name in Enum.GetNames(typeof(ZigResolution)))
                {
                    resolutions.Add(new GUIContent(name.Replace('_',' ')));
                }
                kinectCaptureResolution = (ZigResolution)EditorGUILayout.Popup(new GUIContent(KINECT_RESOLUTION), (int)kinectCaptureResolution, resolutions.ToArray());
                
                SmoothingOptions newSmoothingOptions = (SmoothingOptions)EditorGUILayout.EnumPopup(new GUIContent(SMOOTHING_OPTIONS), smoothingOptions);
                if (newSmoothingOptions != smoothingOptions)
                {
                    smoothingOptions = newSmoothingOptions;
                    if (smoothingOptions == SmoothingOptions.None)
                    {
                        settings.KinectSDKSpecific.SmoothingParameters.SetNoSmoothing();
                    }
                    else if(smoothingOptions == SmoothingOptions.Light)
                    {
                        settings.KinectSDKSpecific.SmoothingParameters.SetLightSmoothing();
                    }
                    else if (smoothingOptions == SmoothingOptions.Moderate)
                    {
                        settings.KinectSDKSpecific.SmoothingParameters.SetModerateSmoothing();
                    }
                    else if (smoothingOptions == SmoothingOptions.Heavy)
                    {
                        settings.KinectSDKSpecific.SmoothingParameters.Smoothing = 0.6f;
                        settings.KinectSDKSpecific.SmoothingParameters.Correction = 0.4f;
                        settings.KinectSDKSpecific.SmoothingParameters.Prediction = 0.6f;
                        settings.KinectSDKSpecific.SmoothingParameters.JitterRadius = 0.15f;
                        settings.KinectSDKSpecific.SmoothingParameters.MaxDeviationRadius = 0.10f;
                        //settings.KinectSDKSpecific.SmoothingParameters.SetHeavySmoothing();
                    }
                    ZigEditorInput.Instance.SetSmoothingParameters((smoothingOptions != SmoothingOptions.None), settings.KinectSDKSpecific.SmoothingParameters);
                }
                viewNodes = EditorGUILayout.Toggle(JOINT_VIEW, viewNodes);
                
                EditorGUI.BeginDisabledGroup(!isNUIOn);
                newDesiredTilt = EditorGUILayout.IntSlider(new GUIContent(DEVICE_TILT), (int)desiredTilt, -27, 27);
                EditorGUI.EndDisabledGroup();

                workingResolution = ResolutionData.FromZigResolution(kinectCaptureResolution);
            }
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect((UI_SPACING * 2) + textureWidth, textureHeight + (UI_SPACING * 2), textureWidth, panelHeight), String.Empty, "box");
            {
                fileName = EditorGUILayout.TextField(new GUIContent("Filename"), fileName);

                /*
                isFilterFoldout = EditorGUILayout.Foldout(isFilterFoldout, new GUIContent("Filters"));
                if (isFilterFoldout)
                {
                    EditorGUI.indentLevel = 1;
                    EditorGUILayout.Toggle(new GUIContent("Smooth"), false);
                    EditorGUILayout.Toggle(new GUIContent("Mirror"), false);
                    EditorGUILayout.Toggle(new GUIContent("Correct Sensor Tilt"), false);
                    EditorGUILayout.Toggle(new GUIContent("Correct Sensor Offset"), false);
                    EditorGUI.indentLevel = 0;
                }

                GUILayout.Space(UI_SPACING);*/
                //recordingStart = (RecordingStart)EditorGUILayout.EnumPopup(new GUIContent("Start On"), recordingStart);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel(new GUIContent("Model Preview"));
                if (GUILayout.Button("Generate"))
                {
                    createModelPreview();
                }
                EditorGUILayout.EndHorizontal();

                delaySelection = EditorGUILayout.Popup(new GUIContent("Start Delay"), delaySelection, delays);

                isAdvancedExposed = EditorGUILayout.Foldout(isAdvancedExposed, new GUIContent("Advanced"));
                if (isAdvancedExposed)
                {
                    transformationType = (TransformationType)EditorGUILayout.EnumPopup(new GUIContent("Transformation Type"), transformationType);
                }

                GUILayout.Space(UI_SPACING);
                EditorGUI.BeginDisabledGroup(!isNUIOn);
                if (GUILayout.Button(captureState == RecordingState.NotRecording ? new GUIContent("Record") : new GUIContent("Stop")))
                {
                    if (captureState == RecordingState.NotRecording)
                    {
                        BeginPreRecording();
                    }
                    else
                    {
                        StopRecording();
                    }
                }
                EditorGUI.EndDisabledGroup();
            }
            GUILayout.EndArea();
        }
        GUI.skin = null;

        if (newDesiredTilt != desiredTilt)
        {
            desiredTilt = newDesiredTilt;
            NuiWrapper.NuiCameraElevationSetAngle((long)desiredTilt);
        }
    }

    /// <summary>
    /// Perform cleanup on window close
    /// </summary>
    protected void OnDestroy()
    {
        if (cinema_mocap_humanoid_instance != null)
        {
            UnityEngine.Object.DestroyImmediate(cinema_mocap_humanoid_instance);
        }
        Resources.UnloadUnusedAssets();
    }

    /// <summary>
    /// Load up the Zigfu driver and turn on the NUI device.
    /// </summary>
    private void turnOnNUIDevice()
    {
        settings.UpdateImage = true;
      
        ZigEditorInput.InputType = inputType;
        ZigEditorInput.Settings = settings;

        ZigEditorInput.Instance.Init();
        isNUIOn = ZigEditorInput.Instance.ReaderInited;
       
        depthViewer = new ZigEditorDepthViewer();
        imageViewer = new ZigEditorImageViewer();
        //mixedViewer = new ZigEditorMixedViewer();
        usersRadar = new ZigEditorUsersRadar();
        usersRadar.Start();

        long tilt = (long)desiredTilt;
        NuiWrapper.NuiCameraElevationGetAngle(out tilt);
        desiredTilt = (float)tilt;
    }
    
    /// <summary>
    /// Destroy the zig editor input.
    /// </summary>
    private void turnOffNUIDevice()
    {
        ZigEditorInput.Instance.ShutdownReader();
        isNUIOn = ZigEditorInput.Instance.ReaderInited;

        if (captureState != RecordingState.NotRecording)
        {
            StopRecording();
        }

        if (cinema_mocap_humanoid_instance != null && inputMapper != null)
        {
            RealtimeHumanoidPosing poser = cinema_mocap_humanoid_instance.GetComponent<RealtimeHumanoidPosing>();
            poser.SetRotations(inputMapper.GetBaseRotations());            
        }
    }

    /// <summary>
    /// Initialize the phase before recording begins. Allowing for countdowns, prep, etc.
    /// </summary>
    private void BeginPreRecording()
    {
        captureState = RecordingState.PreRecording;

        delaySeconds = int.Parse(delays[delaySelection].text.Split(' ')[0]);
        stopwatch.Reset();
        stopwatch.Start();
    }

    /// <summary>
    /// Begin capturing the motion from the user.
    /// </summary>
    private void BeginRecording()
    {
        captureState = RecordingState.Recording;
        captureData = new NUIHumanoidAnimation();

        stopwatch.Stop();
        stopwatch.Reset();
		stopwatch.Start();
    }
    
    /// <summary>
    /// Once capturing is complete, write out the animation file.
    /// </summary>
    private void StopRecording()
    {
        // Change to stopped state
        stopwatch.Stop();
        captureState = RecordingState.NotRecording;

        // Check if there is capture data
        if (captureData == null)
        {
            UnityEngine.Debug.LogWarning("No capture data was found.");
            return;
        }

        // Reload the rig data and mapper if necessary
        if (inputMapper == null)
        {
            rigData = ColladaUtility.ReadRigData(SOURCE_FILE);
            inputMapper = new NUIInputToRigMapper(rigData);
        }

        // Map captured data to Collada data
        ColladaAnimationData data = inputMapper.GetColladaAnimation(captureData);

        // Check filename
        string appendedFileName = string.Format("MoCapHumanoid@{0}", fileName);
        string newFileName = appendedFileName;
        if (System.IO.File.Exists(string.Format(FILE_DESTINATION, appendedFileName)))
        {
            newFileName = getNewFilename(appendedFileName);
            UnityEngine.Debug.LogWarning(string.Format(NAME_DUPLICATE_ERROR_MSG, appendedFileName, newFileName));
        }

        // Save
        if (transformationType == TransformationType.Matrix)
        {
            ColladaUtility.SaveAnimationData(data, SOURCE_FILE_MATRIX, string.Format(FILE_DESTINATION, newFileName), true);
        }
        else
        {
            ColladaUtility.SaveAnimationData(data, SOURCE_FILE, string.Format(FILE_DESTINATION, newFileName), false);
        }
        
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// Create a prefab of the mocap humanoid and have the model refect the NUI user's movement.
    /// </summary>
    private void createModelPreview()
    {
        if (cinema_mocap_humanoid_instance == null)
        {
            cinema_mocap_humanoid_instance = PrefabUtility.InstantiatePrefab(cinema_mocap_humanoid_prefab) as GameObject;
            cinema_mocap_humanoid_instance.AddComponent<RealtimeHumanoidPosing>();
        }
    }

    /// <summary>
    /// If there is a name conflict. Iterate until we find a new name.
    /// </summary>
    /// <param name="fileName">The original filename</param>
    /// <returns>The new filename.</returns>
    private string getNewFilename(string fileName)
    {
        int i = 1;
        while (System.IO.File.Exists(string.Format("Assets/Cinema Suite/Cinema Mocap/Animations//{0}{1}.dae", fileName,i)))
        {
            i++;
        }
        return string.Format("{0}{1}", fileName, i);
    }

    /// <summary>
    /// Show the Cinema Mocap Window
    /// </summary>
    [MenuItem(MENU_ITEM)]
    private static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(CinemaMocapWindow));
    }
}
