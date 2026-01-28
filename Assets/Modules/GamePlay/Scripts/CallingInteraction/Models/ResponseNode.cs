using System;

namespace GamePlay.CallingInteraction
{
  [Serializable]
  public struct ResponseNode
  {
    public string Id;
    public string Content;
    public NodeTransfer[] Transfers;
  }
}
