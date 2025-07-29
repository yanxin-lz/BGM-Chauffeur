using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;

namespace 静默陪伴音乐
{
    public static class 音频控制器
    {
        public static bool 检查是否有其他程序在播放音频()
        {
            using var enumerator = new MMDeviceEnumerator();
            var device = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            var sessions = device.AudioSessionManager.Sessions;

            bool 有外部音频 = false;
            for (int i = 0; i < sessions.Count; i++)
            {
                var session = sessions[i];
                if (session.IsSystemSoundsSession) continue;
                if (session.State != AudioSessionState.AudioSessionStateActive) continue;
                try
                {
                    var sessionProcess = session.GetProcessID != 0 ? System.Diagnostics.Process.GetProcessById((int)session.GetProcessID)?.ProcessName : "";
                    //除自身外，检查其他进程是否在播放音频
                    if (!string.IsNullOrEmpty(sessionProcess) && !sessionProcess.Equals(System.Diagnostics.Process.GetCurrentProcess().ProcessName, StringComparison.OrdinalIgnoreCase))
                    {
                        有外部音频 = true;
                        break;
                    }
                }
                catch { }
            }

            return 有外部音频;
        }

        public static bool 文件支持(string path)
        {
            string ext = System.IO.Path.GetExtension(path).ToLower();
            return ext == ".mp3" || ext == ".wav" || ext == ".flac" || ext == ".ogg" || ext == ".ape";
        }
    }
}
