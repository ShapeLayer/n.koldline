using Infrastructure.Player;
using UnityEngine;

namespace Infrastructure.Camera
{
  [RequireComponent(typeof(UnityEngine.Camera))]
  [RequireComponent(typeof(AudioSource))]
  public partial class MainCameraController : MonoBehaviour
  {
    [Header("References")]
    [SerializeField] private Transform _followingCameraHolder;
    public Transform FollowingCameraHolder => _followingCameraHolder;
    [SerializeField] private Vector3 _distanceOffset = new Vector3(0, 0, 0);
    [SerializeField] private AudioSource _audioSource;
    public AudioSource AudioSource => _audioSource;
    
    private static MainCameraController _instance;
    public static MainCameraController Instance => _instance;

    void Awake()
    {
      if (_instance != null && _instance != this)
      {
        Destroy(gameObject);
        return;
      }
      _instance = this;
      DontDestroyOnLoad(gameObject);

      _audioSource = GetComponent<AudioSource>();
      InitAudioSource();
    }

    void LateUpdate()
    {
      if (_followingCameraHolder == null) return;
      ApplyDistanceOffset();
    }

    public void SetTarget(PlayerController playerController)
    {
      if (playerController == null) return;
      if (playerController.CameraHolderTransform == null) return;
      _followingCameraHolder = playerController.CameraHolderTransform;
    }

    private void ApplyDistanceOffset()
    {
      transform.position = _followingCameraHolder.position + _distanceOffset;
      transform.rotation = _followingCameraHolder.rotation;
    }
  }
}
