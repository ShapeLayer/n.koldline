using System;

namespace GamePlay.CallingInteraction
{
  [Serializable]
  public enum NodeTransferConditionType
  {
    None,
    Always,
    SinglePressed,
    Custom,

    // When Number Sequence, detecting number button pressed would be ignored.
    NumberSequence,
  }
}
