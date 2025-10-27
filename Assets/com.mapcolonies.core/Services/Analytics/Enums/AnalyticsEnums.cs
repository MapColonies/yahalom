namespace com.mapcolonies.core.Services.Analytics.Enums
{
    [System.Serializable]
    public enum AnalyticsMessageTypes
    {
        AppStarted,
        AppExited,
        UserDetails,
        UserMachineSpec,
        UserDevices,
        DeviceConnected,
        DeviceDisconnected,
        GameModeStarted,
        GameModeEnded,
        IdleTimeStarted,
        IdleTimeEnded,
        LayerUseStarted,
        LayerUserEnded,
        MultiplayerStarted,
        MultiplayerEnded,
        Location,
        Error,
        GeneralInfo,
        Warning,
        ConsumptionStatus,
        ApplicationData
    }

    [System.Serializable]
    public enum UserInputDevice
    {
        None,
        Joystick,
        VR,
        Keyboard,
        Mouse
    }
}
