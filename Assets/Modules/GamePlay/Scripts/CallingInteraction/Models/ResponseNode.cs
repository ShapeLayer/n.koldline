using System;

namespace GamePlay.CallingInteraction
{
  [Serializable]
  public struct ResponseNode
  {
    public string Id;
    public string Description;
    public bool SuppressNavigationHint;
    public string CustomContentId;
    public string ContentL10NKey;
    public string AudioL10NKey;
    public bool IsContentFormattingRequired;
    public NodeTransfer[] Transfers;
  }
}
