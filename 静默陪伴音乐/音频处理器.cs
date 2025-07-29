using NAudio.Wave;
using static 静默陪伴音乐.UI数据;

namespace 静默陪伴音乐
{
    public class 音频处理器
    {
        public NAudio.Wave.WaveOutEvent? 播放设备 { get; set; } = new();
        public NAudio.Wave.IWaveProvider? 当前音频适配器 { get; set; } = null;
        public WaveStream? 音频流 { get; set; } = null;

        public event Action? 等待下一曲;

        public bool 载入音频文件(string 文件路径)
        {
            if (播放设备 == null) { 播放设备 = new NAudio.Wave.WaveOutEvent(); }
            string ext = System.IO.Path.GetExtension(文件路径).ToLower();
            if (ext == ".mp3" || ext == ".wav" || ext == ".flac")
            {
                // 使用NAudio加载常见音频格式
                var 常用音频服务器 = new NAudio.Wave.AudioFileReader(文件路径);
                当前音频适配器 = 常用音频服务器;
                音频流 = 常用音频服务器;


            }
            else if (ext == ".ogg")
            {
                var 无损音频读取器 = new NAudio.Vorbis.VorbisWaveReader(文件路径);
                当前音频适配器 = 无损音频读取器;
                音频流 = 无损音频读取器;
            }
            else
            {
                return false; // 不支持的音频格式
            }
            播放设备.Init(当前音频适配器);
            播放设备.Play();
            播放设备.PlaybackStopped += 播放设备_PlaybackStopped;
            return true;
        }

        private void 播放设备_PlaybackStopped(object? sender, StoppedEventArgs e)
        {
            if (音频流 == null) return;
            if (播放设备 == null) return;
            // 判断是否为自然播放结束
            bool 是播放完毕 = (double)音频流.Position / (double)音频流.Length > 0.9;

            if (是播放完毕)
            {
                等待下一曲?.Invoke();
            }
            // 否则为手动停止或异常，不触发“等待下一曲”
        }

        public async void 播放()
        {
            if (当前音频适配器 != null && 播放设备 != null)
            {
                播放设备.Play();
                await 音量渐变(0f, UI数据.Instance.当前音量, 800);
                UI数据.Instance.当前播放状态 = 播放状态.播放中;
            }
        }

        public void 重播()
        {
            if (音频流 == null) { return; }
            if (播放设备 == null) { return; }
            音频流.Position = 0;
            播放设备.Play();
        }

        public async void 暂停()
        {
            if (当前音频适配器 != null && 播放设备 != null)
            {
                await 音量渐变(UI数据.Instance.当前音量, 0f, 800); // 800ms 渐出
                播放设备.Pause();
                UI数据.Instance.当前播放状态 = 播放状态.暂停;
            }
        }

        public void 播放或暂停()
        {
            if (播放设备 == null) { return; }
            if (播放设备.PlaybackState == PlaybackState.Playing) { 暂停(); }
            else
            {
                if (当前音频适配器 != null && 播放设备 != null)
                {
                    播放();
                }
            }
        }

        public void 停止()
        {
            if (当前音频适配器 != null && 播放设备 != null)
            {
                播放设备.Stop();
                UI数据.Instance.当前播放状态 = 播放状态.停止;
            }
        }
        public TimeSpan 获取当前播放时间()
        {
            if (音频流 != null)
            {
                return 音频流.CurrentTime;
            }
            return TimeSpan.Zero;
        }

        public TimeSpan 获取总时长()
        {
            if (音频流 != null)
            {
                return 音频流.TotalTime;
            }
            return TimeSpan.Zero;
        }

        public int 获取播放进度()
        {
            if (音频流 != null && 音频流.TotalTime > TimeSpan.Zero)
            {
                return (int)(音频流.CurrentTime.TotalSeconds / 音频流.TotalTime.TotalSeconds * 100);
            }
            return 0;
        }



        public void 设置音量(float 音量)
        {
            if (播放设备 != null)
            {
                播放设备.Volume = 音量;
            }
        }

        public void 对齐到用户设置音量()
        {
            if (播放设备 != null)
            {
                播放设备.Volume = UI数据.Instance.当前音量;
            }
        }

        public void 释放资源()
        {
            if (播放设备 != null) { 播放设备.Dispose(); }
            if (音频流 != null) { 音频流.Dispose(); }
            播放设备 = null;
            当前音频适配器 = null;
            音频流 = null;
        }

        public bool 是否正在播放()
        {
            return 播放设备?.PlaybackState == PlaybackState.Playing;
        }

        public void 设置播放位置(TimeSpan 时间)
        {
            if (音频流 != null)
            {
                音频流.CurrentTime = 时间;
            }
        }



        private CancellationTokenSource? 淡化音量令牌;

        private async Task 音量渐变(float 起始音量, float 目标音量, int 持续时间毫秒)
        {
            淡化音量令牌?.Cancel(); // 取消之前的渐变任务
            淡化音量令牌 = new CancellationTokenSource();
            var token = 淡化音量令牌.Token;

            int 步数 = 20;
            float 步进音量 = (目标音量 - 起始音量) / 步数;
            int 步进时间 = 持续时间毫秒 / 步数;

            try
            {
                for (int i = 0; i < 步数; i++)
                {
                    if (token.IsCancellationRequested) break;

                    float 当前音量 = 起始音量 + 步进音量 * i;
                    设置音量(Math.Clamp(当前音量, 0, 1));
                    await Task.Delay(步进时间, token);
                }

                设置音量(目标音量); // 最后确保精确设定
            }
            catch (TaskCanceledException)
            {
                // 被取消时忽略
            }
        }


    }
}
