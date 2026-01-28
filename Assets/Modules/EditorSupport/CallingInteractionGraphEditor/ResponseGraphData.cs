using System;
using System.Collections.Generic;

namespace EditorSupport.CallingInteractionGraphEditor
{
  [Serializable]
  public sealed class ResponseGraphData
  {
    public List<EditorResponseNode> Nodes = new List<EditorResponseNode>();
  }
}
