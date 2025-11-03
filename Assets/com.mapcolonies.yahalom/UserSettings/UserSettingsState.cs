using System;

namespace com.mapcolonies.yahalom.UserSettings
{
    [Serializable]
    public class UserSettingsState
    {
        public bool Offline
        {
            get;
            set;
        }

        public BaseMap BaseMap
        {
            get;
            set;
        }

        public Minimap Minimap
        {
            get;
            set;
        }

        public ComponentDisplay ComponentDisplay
        {
            get;
            set;
        }

        public ControlsSettings ControlsSettings
        {
            get;
            set;
        }

        public SpaceAndProcessing SpaceAndProcessing
        {
            get;
            set;
        }

        public SaveSettings SaveSettings
        {
            get;
            set;
        }

        public VRSettings VR
        {
            get;
            set;
        }
    }

    [Serializable]
    public class BaseMap
    {
        public string MapId
        {
            get;
            set;
        }
    }

    [Serializable]
    public class Minimap
    {
        public bool Show
        {
            get;
            set;
        }

        public string MapId
        {
            get;
            set;
        }

        public float MarkerSize
        {
            get;
            set;
        }

        public float Ratio
        {
            get;
            set;
        }
    }

    [Serializable]
    public class ComponentDisplay
    {
        public bool DisplayVisibleLayers
        {
            get;
            set;
        }

        public bool DisplayHiddenLayers
        {
            get;
            set;
        }

        public bool DisplayCompass
        {
            get;
            set;
        }

        public bool DisplayOrientationMap
        {
            get;
            set;
        }

        public bool DisplayLoadedMapsAndUpdateDate
        {
            get;
            set;
        }

        public bool DisplayMarkerOnMap
        {
            get;
            set;
        }
    }

    [Serializable]
    public class ControlsSettings
    {
        public float AxisX
        {
            get;
            set;
        }

        public float AxisY
        {
            get;
            set;
        }

        public bool FlipY
        {
            get;
            set;
        }
    }

    [Serializable]
    public class CoordinateSystem
    {
        public bool GeoDms
        {
            get;
            set;
        }

        public bool GeoDd
        {
            get;
            set;
        }

        public bool Utm
        {
            get;
            set;
        }
    }

    [Serializable]
    public class Measurement
    {
        public bool Imperial
        {
            get;
            set;
        }

        public bool Metric
        {
            get;
            set;
        }
    }

    [Serializable]
    public class SpaceAndProcessing
    {
        public CoordinateSystem CoordinateSystem
        {
            get;
            set;
        }

        public Measurement Measurement
        {
            get;
            set;
        }
    }

    [Serializable]
    public class SaveSettings
    {
        public string SavePath
        {
            get;
            set;
        }

        public bool UseForScreenshots
        {
            get;
            set;
        }

        public int ScreenshotSize
        {
            get;
            set;
        }
    }

    [Serializable]
    public class VRSettings
    {
        public bool CorrectionAffectsPerformance
        {
            get;
            set;
        }

        public bool FlickeringPixelsCorrection
        {
            get;
            set;
        }

        public bool BlurOnJumpToCoordinates
        {
            get;
            set;
        }

        public bool RotationOnJumps
        {
            get;
            set;
        }

        public bool VRSoldierMovementWithTeleportation
        {
            get;
            set;
        }
    }
}
