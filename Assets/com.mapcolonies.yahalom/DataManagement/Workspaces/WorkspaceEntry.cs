using System;

namespace com.mapcolonies.yahalom.DataManagement.Workspaces
{
    [Serializable]
    public class WorkspaceEntry
    {
        public string name;
        public string thumbnailPath;
        public DateTime LastModified;
        public DateTime Created;
    }
}
