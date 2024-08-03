using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Agora.Rtc;

#if (UNITY_2018_3_OR_NEWER && UNITY_ANDROID)
using UnityEngine.Android;
#endif


public class joinChannelVideo : MonoBehaviour
{
    // Fill in your app ID
    [SerializeField] private string _appID= "e97337da3d464089b7d9fc90f990ea97";
    // Fill in your channel name
    [SerializeField] private string _channelName = "TuteDude";
    // Fill in a temporary token
    [SerializeField] private string _token = "007eJxTYPDMzYrsSwp4GaW/Nf/I6lW/+KtKik5mRLB/7Iidu6E3/5ICQ6qlubGxeUqicYqJmYmBhWWSeYplWrKlQZqlpUFqoqV585u1aQ2BjAwz276wMjJAIIjPwRBSWpLqUpqSysAAAB6pIrQ=";
    internal VideoSurface LocalView;
    internal VideoSurface RemoteView;
    internal VideoSurface RemoteView2;
    internal IRtcEngine RtcEngine;

    #if (UNITY_2018_3_OR_NEWER && UNITY_ANDROID)
    private ArrayList permissionList = new ArrayList() { Permission.Camera, Permission.Microphone };
    #endif

    private void Start() {
        SetupVideoSDKEngine();
        InitEventHandler();
        SetupUI();
    }

    private void Update() {
        CheckPermissions();
    }

    void OnApplicationQuit() 
    {
        if (RtcEngine != null) {
            Leave();
            // Destroy IRtcEngine
            RtcEngine.Dispose();
            RtcEngine = null;
        }
    }

    private void CheckPermissions() {
        #if (UNITY_2018_3_OR_NEWER && UNITY_ANDROID)
        foreach (string permission in permissionList) {
            if (!Permission.HasUserAuthorizedPermission(permission)) {
                Permission.RequestUserPermission(permission);
            }
        }
        #endif
    }

    private void SetupUI() {
        GameObject go = GameObject.Find("LocalView");
        LocalView = go.AddComponent<VideoSurface>();
        go.transform.Rotate(0.0f, 0.0f, -180.0f);
        go = GameObject.Find("RemoteView");
        RemoteView = go.AddComponent<VideoSurface>();
        go.transform.Rotate(0.0f, 0.0f, -180.0f);
        go = GameObject.Find("RemoteView2");
        RemoteView2 = go.AddComponent<VideoSurface>();
        go.transform.Rotate(0.0f, 0.0f, -180.0f);
        go = GameObject.Find("Leave");
        go.GetComponent<Button>().onClick.AddListener(Leave);
        go = GameObject.Find("Join");
        go.GetComponent<Button>().onClick.AddListener(Join);
    }

    private void SetupVideoSDKEngine() {
        // Create an IRtcEngine instance
        RtcEngine = Agora.Rtc.RtcEngine.CreateAgoraRtcEngine();
        RtcEngineContext context = new RtcEngineContext();
        context.appId = _appID;
        context.channelProfile = CHANNEL_PROFILE_TYPE.CHANNEL_PROFILE_LIVE_BROADCASTING;
        context.audioScenario = AUDIO_SCENARIO_TYPE.AUDIO_SCENARIO_DEFAULT;
        // Initialize the instance
        RtcEngine.Initialize(context);
    }



    // Create a user event handler instance and set it as the engine event handler
    private void InitEventHandler() 
    {
        UserEventHandler handler = new UserEventHandler(this);
        RtcEngine.InitEventHandler(handler);
    }

    public void Join() 
    {
        // Enable the video module
        RtcEngine.EnableVideo();
        // Set channel media options
        ChannelMediaOptions options = new ChannelMediaOptions();
        // Start video rendering
        LocalView.SetEnable(true);
        // Automatically subscribe to all audio streams
        options.autoSubscribeAudio.SetValue(true);
        // Automatically subscribe to all video streams
        options.autoSubscribeVideo.SetValue(true);
        // Set the channel profile to live broadcast
        options.channelProfile.SetValue(CHANNEL_PROFILE_TYPE.CHANNEL_PROFILE_COMMUNICATION);
        //Set the user role as host
        options.clientRoleType.SetValue(CLIENT_ROLE_TYPE.CLIENT_ROLE_BROADCASTER);
        // Join a channel
        RtcEngine.JoinChannel(_token, _channelName, 0, options);
    }

    public void Leave()
    {
        Debug.Log("Leaving _channelName");
        // Leave the channel
        RtcEngine.LeaveChannel();
        // Disable the video module
        RtcEngine.DisableVideo();
        // Stop remote video rendering
        RemoteView.SetEnable(false);
        RemoteView2.SetEnable(false);
        // Stop local video rendering
        LocalView.SetEnable(false);
    }

    // Implement your own EventHandler class by inheriting the IRtcEngineEventHandler interface class implementation
    internal class UserEventHandler : IRtcEngineEventHandler 
    {
        private readonly joinChannelVideo _videoSample;
        internal UserEventHandler(joinChannelVideo videoSample) {
            _videoSample = videoSample;
        }
        

    public class ExtendedVideoSurface : VideoSurface 
    {
        public uint UserId { get; set; } = 0;
    }

    public class VideoSample 
    {
        public ExtendedVideoSurface LocalView { get; set; }
        public ExtendedVideoSurface RemoteView { get; set; }
        public ExtendedVideoSurface RemoteView2 { get; set; }
    }
    public override void OnError(int err, string msg) 
    {
        Debug.LogError($"Error: {err}, Message: {msg}");
    }

    // Triggered when a local user successfully joins the channel
    public override void OnJoinChannelSuccess(RtcConnection connection, int elapsed) 
    {
        Debug.Log("Local user joined the channel successfully");
        _videoSample.LocalView.SetForUser(0, "");
    }

    // When the SDK receives the first frame of a remote video stream and successfully decodes it, the OnUserJoined callback is triggered.
    public override void OnUserJoined(RtcConnection connection, uint uid, int elapsed) {
    // Check if RemoteView1 is not in use
        if (_videoSample.RemoteView.UserId == 0) {
            _videoSample.RemoteView.SetForUser(uid, connection.channelId, VIDEO_SOURCE_TYPE.VIDEO_SOURCE_REMOTE);
            _videoSample.RemoteView.SetEnable(true);
            _videoSample.RemoteView.UserId = (int)uid;
            Debug.Log("First remote user joined");
        }
        // Check if RemoteView2 is not in use
        else if (_videoSample.RemoteView2.UserId == 0) {
            _videoSample.RemoteView2.SetForUser(uid, connection.channelId, VIDEO_SOURCE_TYPE.VIDEO_SOURCE_REMOTE);
            _videoSample.RemoteView2.SetEnable(true);
            _videoSample.RemoteView2.UserId = (int)uid;
            Debug.Log("Second remote user joined");
        } 
        else {
            Debug.LogWarning("No available remote view for the new user");
        }


    }
    }

}






