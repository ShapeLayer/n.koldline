using System;

namespace Infrastructure.Exceptions
{
  public class InitializingNotSucceed : Exception
  {
    public InitializingNotSucceed() {}
    public InitializingNotSucceed(string message) : base(message) {}
    public InitializingNotSucceed(string message, Exception inner) : base(message, inner) {}
  }
}
