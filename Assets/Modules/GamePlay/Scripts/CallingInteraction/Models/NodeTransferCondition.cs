using System;
using System.Collections.Generic;

namespace GamePlay.CallingInteraction
{
  [Serializable]
  public struct NodeTransferCondition
  {
    public NodeTransferConditionType Condition;
    public int[] Value;
  }
}
