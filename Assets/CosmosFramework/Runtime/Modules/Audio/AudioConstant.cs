﻿namespace Cosmos.Audio
{
    /// <summary>
    /// 声音模块常量；
    /// </summary>
    public class AudioConstant
    {
        public const bool PalyOnAwake = true;
        public const bool Loop = false;
        public const int Priority = 128;
        public const float Volume = 1;
        public const float Pitch = 1;
        public const float StereoPan = 0;
        public const float SpatialBlend = 0;
        public const float ReverbZoneMix = 1;
        public const float DopplerLevel = 1;
        public const int Spread = 0;
        public const float MaxDistance = 500;

        /// <summary>
        /// 检查播放间隔，5秒；
        /// </summary>
        public const int CheckPlayingIntervalSecond = 5;

        /// <summary>
        /// 声音常量
        /// </summary>
        public const string PREFIX = "SND-";

    }
}
