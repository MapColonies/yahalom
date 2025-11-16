using System;
using System.Collections.Generic;

namespace com.mapcolonies.yahalom.DataManagement.Workspaces
{
    [Serializable]
    public class WorkspacesState
    {
        public List<WorkspaceEntry> Workspaces = new List<WorkspaceEntry>();
    }
}
