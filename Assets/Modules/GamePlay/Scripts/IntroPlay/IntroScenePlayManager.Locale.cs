using UnityEngine;
using Infrastructure.UIDocuments;
using Infrastructure.Config;

namespace GamePlay.IntroPlay
{
  public partial class IntroScenePlayManager : MonoBehaviour
  {
    void Start_Locale()
    {
      if (_localeSelector == null) return;
      _localeSelector.LocaleSelected += OnLocaleSelected;
      _localeSelector.Enable();
    }

    async void OnLocaleSelected(string localeCode)
    {
      await IntroSceneL10nCollections.SetLocaleAsync(localeCode);
      LocaleConfigService.SaveSelectedLocale(localeCode);
      await PlayIntro();
    }
  }
}
