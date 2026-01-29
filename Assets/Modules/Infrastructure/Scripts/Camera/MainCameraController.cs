using Infrastructure.Player;
using UnityEngine;

namespace Infrastructure.Camera
{
  [RequireComponent(typeof(UnityEngine.Camera))]  
  public class MainCameraController : MonoBehaviour
  {
    [Header("References")]
    [SerializeField] private Transform _followingCameraHolder;
    public Transform FollowingCameraHolder => _followingCameraHolder;
    [SerializeField] private Vector3 _distanceOffset = new Vector3(0, 0, 0);
    
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
