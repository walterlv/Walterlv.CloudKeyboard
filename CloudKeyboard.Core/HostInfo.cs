﻿namespace Walterlv.CloudTyping
{
    public static class HostInfo
    {
#if DEBUG
        // DEBUG 下使用本地调试地址。
        public const string BaseUrl = "https://cloud-keyboard.walterlv.com:4096/api/keyboard";
#else
        // RELEASE 下使用正式环境地址。
        public const string BaseUrl = "https://cloud-keyboard.walterlv.com:4096/api/keyboard";
#endif
    }
}
