using System;

namespace GamePlay.CallingInteraction
{
  [Serializable]
  public struct NodeTransferCondition
  {
    public NodeTransferConditionType Condition;
    public string CustomConditionId;
    public TelephoneButtonType[] Value;
  }
}
