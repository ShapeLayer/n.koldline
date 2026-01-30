using System;

namespace GamePlay.CallingInteraction
{
  [Serializable]
  public struct NodeTransfer
  {
    public NodeTransferCondition Condition;
    public ResponseNode ToNode;
  }
}
