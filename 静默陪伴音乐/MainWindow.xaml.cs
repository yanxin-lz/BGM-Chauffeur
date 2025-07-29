using PropertyChanged;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using static 静默陪伴音乐.UI数据;

namespace 静默陪伴音乐
{
    [AddINotifyPropertyChangedInterface]
    public partial class 音乐播放器 : Window
    {

        public UI数据 数据 { get; set; } = new();
        public 应用程序设置 设置 { get; set; } = new();
        public 音频处理器 音频处理器 { get; set; } = new();

        public 音乐播放器()
        {
            InitializeComponent();
            DataContext = this;
            事件绑定();
            读取默认播放列表();
        }

        private void 读取默认播放列表()
        {
            if (File.Exists(设置.默认播放列表路径))
            {
                var list = JsonSerializer.Deserialize<ObservableCollection<播放列表项>>(File.ReadAllText(设置.默认播放列表路径));
                if (list != null)
                {
                    foreach (var item in list)
                    {
                        if (!string.IsNullOrWhiteSpace(item.路径) && 音频控制器.文件支持(item.路径)) 数据.播放列表.Add(item);
                    }

                }

            }
        }



        private void 事件绑定()
        {
            播放列表.Drop += 播放列表_Drop;
            播放列表.PreviewDragOver += (s, e) => e.Handled = true;


            播放进度.AddHandler(Slider.PreviewMouseDownEvent, new RoutedEventHandler((s, e) => 数据.正在拖拽进度条 = true), true);
            播放进度.AddHandler(Slider.PreviewMouseUpEvent, new RoutedEventHandler((s, e) =>
            {

                音频处理器.设置播放位置(播放进度.Value / 100 * 音频处理器.获取总时长());
                数据.正在拖拽进度条 = false;
            }), true);

            var 音频检测计时器 = new DispatcherTimer { Interval = TimeSpan.FromSeconds(设置.检测间隔) };

            音频检测计时器.Tick += (s, e) =>
            {
                if (数据.当前歌曲 == null) { return; }

                var 有音频播放 = 音频控制器.检查是否有其他程序在播放音频();
                if (有音频播放) { 数据.无声时间 = 0; 音频处理器.暂停(); }
                else if (!数据.手动暂停或停止)
                {

                    if (!设置.自动启停) { return; }
                    else { if (数据.当前播放状态 == 播放状态.播放中) { return; } 数据.无声时间++; }
                    if (数据.无声时间 >= 设置.重播间隔) { 数据.无声时间 = 0; 音频处理器.播放(); return; }

                }
                else if (数据.手动暂停或停止)
                {
                    数据.无声时间 = 0; return;
                }
            };
            var UI更新 = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            UI更新.Tick += (s, e) =>
            {
                数据.当前播放时间 = 音频处理器.获取当前播放时间();
                数据.总时长 = 音频处理器.获取总时长();
                if (!数据.正在拖拽进度条)
                {
                    数据.播放进度 = (int)((double)数据.当前播放时间.TotalSeconds / (double)数据.总时长.TotalSeconds * 100);
                }

                if (数据.当前歌曲 != null)
                {
                    Title = $"{数据.当前播放状态} | {数据.当前播放时间.ToString(@"mm\:ss")}/{数据.总时长.ToString(@"mm\:ss")}";
                }

            };
            音频检测计时器.Start(); UI更新.Start();
            音频处理器.等待下一曲 += 音频处理器_等待下一曲;
        }

        private void 音频处理器_等待下一曲()
        {
            if (数据.手动暂停或停止) { return; }
            else { 数据.手动暂停或停止 = false; }
            if (数据.当前播放模式 == 播放模式.顺序播放 || 数据.当前播放模式 == 播放模式.列表循环) { 下一曲_Click(new object(), null); }
            if (数据.当前播放模式 == 播放模式.单曲循环) { 音频处理器.重播(); }
            if (数据.当前播放模式 == 播放模式.随机播放) { 随机播放_Click(new object(), null); }
        }

        private void 播放音频(播放列表项 项)
        {

            if (!string.IsNullOrWhiteSpace(项.路径))
            {
                数据.播放列表.ToList().ForEach(x => x.是否正在播放 = false);
                项.是否正在播放 = true;
                音频处理器.停止();
                音频处理器.载入音频文件(项.路径);
                数据.当前歌曲 = 项;
                数据.当前播放状态 = 播放状态.播放中;
                if (音频处理器 == null) { return; }
                if (音频处理器.播放设备 == null) { return; }
                音频处理器.播放设备.Volume = 数据.当前音量;
            }
        }


    }

}
