using PropertyChanged;
using System.Collections.ObjectModel;

namespace 静默陪伴音乐
{
    [AddINotifyPropertyChangedInterface]
    public class 播放列表项
    {
        public int 排序 { get; set; } = 1; // 用于在播放列表中显示的顺序
        public string? 显示名称 { get; set; }
        public TimeSpan 时长 { get; set; } = TimeSpan.Zero;
        public string? 路径 { get; set; }
        public bool 是否已加载 { get; set; } = false; // 是否已加载音频文件信息
        public bool 是否正在播放 { get; set; } = false; // 是否正在播放此音频

        public override string ToString()
        {
            return $"{排序} - {显示名称}";
        }

        public 播放列表项(string 路径)
        {
            显示名称 = System.IO.Path.GetFileNameWithoutExtension(路径);
            this.路径 = 路径;
            排序 = UI数据.Instance.播放列表.Count + 1;
            UI数据.Instance.更新排序();
        }

    }

    [AddINotifyPropertyChangedInterface]
    public class UI数据
    {
        public static UI数据 Instance { get; set; }
        public UI数据()
        {
            Instance = this;
        }
        public string 应用程序名称 { get; set; } = "静默陪伴音乐";
        public enum 播放状态
        {
            播放中,
            暂停,
            停止
        }




        public 播放状态 当前播放状态 { get; set; } = 播放状态.停止;

        public bool 手动暂停或停止 { get; set; } = false; // 是否手动暂停或停止了播放
        public 播放列表项? 当前歌曲 { get; set; }
        public 播放列表项? 当前选中项 { get; set; }
        public TimeSpan 当前播放时间 { get; set; } = TimeSpan.Zero;
        public TimeSpan 总时长 { get; set; } = TimeSpan.Zero;
        public int 播放进度 { get; set; } = 0; // 0-100，表示播放进度百分比

        public ObservableCollection<播放列表项> 播放列表 { get; set; } = new();
        public int 当前播放索引 { get; set; } = -1; // -1表示没有歌曲在播放
        public enum 播放模式
        {
            顺序播放,
            随机播放,
            单曲循环,
            列表循环
        }
        public 播放模式 当前播放模式 { get; set; } = 播放模式.顺序播放;
        public string 当前播放列表路径 { get; set; } = string.Empty; // 当前播放列表的路径
        public float 当前音量 { get; set; } = 0.5f; // 音量范围0-100
        public bool 静音 { get; set; } = false;
        public string 错误信息 { get; set; } = string.Empty;

        public bool 自动保存播放列表 { get; set; } = true;

        public bool 正在拖拽进度条 { get; set; } = false; // 是否正在拖拽

        public int 无声时间 { get; set; } = 0; // 用于检测音频是否无声的计时器
        public void 更新排序()
        {
            int 排序数 = 1;
            播放列表.ToList().Sort((x, y) => x.排序.CompareTo(y.排序));
            播放列表.ToList().ForEach(x => x.排序 = 排序数++);

        }
    }
}
