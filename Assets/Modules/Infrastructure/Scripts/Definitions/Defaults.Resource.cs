using Unity.VisualScripting;
using UnityEngine;

namespace Infrastructure.Definitions
{
  public static partial class Defaults
  {
    public const string ItemTexturesPath = "Textures/ItemTextures";
    public const string FallbackSpritePath = "fallback";
    static Sprite _fallbackSprite;
    public static Sprite FallbackSprite
    {
      get
      {
        if (_fallbackSprite.IsUnityNull())
          _fallbackSprite = Resources.Load<Sprite>(
            $"{ItemTexturesPath}/{FallbackSpritePath}"
          );
        return _fallbackSprite;
      }
    }
  }
}
