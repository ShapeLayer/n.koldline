using System;

namespace Infrastructure.Commons
{
  [Serializable]
  public struct TimeStruct
  {
    public int Hours { get; set; }
    public int Minutes { get; set; }
    public int Seconds { get; set; }
    public int Milliseconds { get; set; }
  }
}
