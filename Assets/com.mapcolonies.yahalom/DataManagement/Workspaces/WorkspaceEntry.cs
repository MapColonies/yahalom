using System;

namespace com.mapcolonies.yahalom.DataManagement.Workspaces
{
    [Serializable]
    public class WorkspaceEntry
    {
        public string FileRef
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string ThumbnailPath
        {
            get;
            set;
        }

        public DateTime LastModified
        {
            get;
            set;
        }

        public DateTime Created
        {
            get;
            set;
        }
    }
}
