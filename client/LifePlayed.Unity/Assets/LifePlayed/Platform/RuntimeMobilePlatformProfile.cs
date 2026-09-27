using LifePlayed.Client.Application;

namespace LifePlayed.Client.Platform
{
    public sealed class RuntimeMobilePlatformProfile : IMobilePlatformProfile
    {
        public MobilePlatformKind Platform
        {
            get
            {
#if UNITY_ANDROID
                return MobilePlatformKind.Android;
#elif UNITY_IOS
                return MobilePlatformKind.IOS;
#else
                return MobilePlatformKind.Other;
#endif
            }
        }

        public bool UsesSafeAreaInsets =>
            Platform == MobilePlatformKind.Android ||
            Platform == MobilePlatformKind.IOS;
    }
}
