using System;

namespace com.mapcolonies.yahalom.DataManagement.UserSettings
{
    [Serializable]
    public record UserSettingsState
    {
        public bool Offline
        {
            get;
            set;
        } = false;

        public BaseMap BaseMap
        {
            get;
            set;
        } = new BaseMap();

        public Minimap Minimap
        {
            get;
            set;
        } = new Minimap();

        public ComponentDisplay ComponentDisplay
        {
            get;
            set;
        } = new ComponentDisplay();

        public ControlsSettings ControlsSettings
        {
            get;
            set;
        } = new ControlsSettings();

        public SpaceAndProcessing SpaceAndProcessing
        {
            get;
            set;
        } = new SpaceAndProcessing();

        public SaveSettings SaveSettings
        {
            get;
            set;
        } = new SaveSettings();

        public VRSettings VR
        {
            get;
            set;
        } = new VRSettings();
    }

    [Serializable]
    public record BaseMap
    {
        public string MapId
        {
            get;
            set;
        } = string.Empty;
    }

    [Serializable]
    public record Minimap
    {
        public bool Show
        {
            get;
            set;
        } = false;

        public string MapId
        {
            get;
            set;
        } = string.Empty;

        public float MarkerSize
        {
            get;
            set;
        } = 1f;

        public float Ratio
        {
            get;
            set;
        } = 0.5f;
    }

    [Serializable]
    public record ComponentDisplay
    {
        public bool DisplayVisibleLayers
        {
            get;
            set;
        } = false;

        public bool DisplayHiddenLayers
        {
            get;
            set;
        } = false;

        public bool DisplayCompass
        {
            get;
            set;
        } = false;

        public bool DisplayOrientationMap
        {
            get;
            set;
        } = false;

        public bool DisplayLoadedMapsAndUpdateDate
        {
            get;
            set;
        } = false;

        public bool DisplayMarkerOnMap
        {
            get;
            set;
        } = false;
    }

    [Serializable]
    public record ControlsSettings
    {
        public float AxisX
        {
            get;
            set;
        } = 1f;

        public float AxisY
        {
            get;
            set;
        } = 1f;

        public bool FlipY
        {
            get;
            set;
        } = false;
    }

    [Serializable]
    public record CoordinateSystem
    {
        public bool GeoDms
        {
            get;
            set;
        } = false;

        public bool GeoDd
        {
            get;
            set;
        } = false;

        public bool Utm
        {
            get;
            set;
        } = false;
    }

    [Serializable]
    public record Measurement
    {
        public bool Imperial
        {
            get;
            set;
        } = false;

        public bool Metric
        {
            get;
            set;
        } = false;
    }

    [Serializable]
    public record SpaceAndProcessing
    {
        public CoordinateSystem CoordinateSystem
        {
            get;
            set;
        } = new CoordinateSystem();

        public Measurement Measurement
        {
            get;
            set;
        } = new Measurement();
    }

    [Serializable]
    public record SaveSettings
    {
        public string SavePath
        {
            get;
            set;
        } = string.Empty;

        public bool UseForScreenshots
        {
            get;
            set;
        } = false;

        public int ScreenshotSize
        {
            get;
            set;
        } = 1;
    }

    [Serializable]
    public record VRSettings
    {
        public bool CorrectionAffectsPerformance
        {
            get;
            set;
        } = false;

        public bool FlickeringPixelsCorrection
        {
            get;
            set;
        } = false;

        public bool BlurOnJumpToCoordinates
        {
            get;
            set;
        } = false;

        public bool RotationOnJumps
        {
            get;
            set;
        } = false;

        public bool VRSoldierMovementWithTeleportation
        {
            get;
            set;
        } = false;
    }
}
