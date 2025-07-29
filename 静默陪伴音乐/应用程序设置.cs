using PropertyChanged;
namespace 静默陪伴音乐
{
    [AddINotifyPropertyChangedInterface]
    public class 应用程序设置
    {

        public bool 自动启停 { get; set; } = true;
        public int 检测间隔 { get; set; } = 1;
        public int 重播间隔 { get; set; } = 5;
        public string 默认播放列表路径 { get; set; } = "默认播放列表.json";

    }
}
