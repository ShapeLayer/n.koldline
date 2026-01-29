using UnityEngine;

namespace Infrastructure.Player
{
  /// <summary>
  /// PlayerController의 카메라 및 카메라 홀더 처리 부분 구현
  /// 
  /// PlayerController는 카메라 홀더를 두고, 이를 카메라가 추종하도록 하고 있습니다.
  /// 이것은 플레이어 카메라 홀더에 한 사람만 카메라를 추종하는 것이 아니라,
  /// 여러 명이 한 플레이어의 시점을 공유할 수 있게 할 수 있도록 하기 위함입니다.
  /// 
  /// 이렇게 함으로서, 관전자 모드를 두 가지 방향 (자유 이동, 시점 추종)으로 구현할 수 있습니다.
  /// </summary>
  public partial class PlayerController : MonoBehaviour
  {
    [Header("Camera")]
    [SerializeField] private Transform _cameraHolderTransform;
    public Transform CameraHolderTransform => _cameraHolderTransform;
    [SerializeField] private Vector3 _cameraHolderPositionOffset = new Vector3(0, 1.5f, 0);

    void Awake_Camera()
    {
      InitializeCameraHolder();
    }
    void InitializeCameraHolder()
    {
      GameObject go = new GameObject("CameraHolder");
      go.transform.SetParent(transform);
      go.transform.localPosition = _cameraHolderPositionOffset;
      go.transform.localRotation = Quaternion.identity;
      _cameraHolderTransform = go.transform;
    }

    void Start_Camera()
    {
      if (_camController == null) return;

      if (_camController.FollowingCameraHolder != _cameraHolderTransform)
        _camController.SetTarget(this);
    }
  }
}
