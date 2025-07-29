using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using 辅助类;
using static 静默陪伴音乐.UI数据;

namespace 静默陪伴音乐
{
    public partial class 音乐播放器
    {

        #region 文件菜单
        private void 保存播放列表_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog save = new() { Filter = "播放列表 (*.json)|*.json" };
            if (save.ShowDialog() == true)
            {
                File.WriteAllText(save.FileName, JsonSerializer.Serialize(数据.播放列表.ToArray()));
            }
        }

        private void 打开播放列表_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog open = new() { Filter = "播放列表 (*.json)|*.json" };
            if (open.ShowDialog() == true)
            {
                数据.播放列表.Clear();
                var list = JsonSerializer.Deserialize<ObservableCollection<播放列表项>>(File.ReadAllText(open.FileName));
                foreach (var item in list)
                    if (!string.IsNullOrWhiteSpace(item.路径) && 音频控制器.文件支持(item.路径))
                    { 数据.播放列表.Add(item); }
            }
        }


        private void 打开单个音频_Click(object sender, RoutedEventArgs e)
        {
            var 选择文件 = new Microsoft.Win32.OpenFileDialog();
            选择文件.Filter = "音频文件|*.mp3;*.wav;*.flac;*.ogg";
            if (选择文件.ShowDialog() == true)
            {
                if (!音频控制器.文件支持(选择文件.FileName))
                {
                    MessageBox.Show("不支持的音频格式", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                播放列表项 新项 = new 播放列表项(选择文件.FileName);
                播放音频(新项);
                数据.播放列表.Add(新项);
            }
        }

        private void 关闭按钮_Click(object sender, RoutedEventArgs e) => Environment.Exit(0);
        #endregion

        #region 控制菜单


        private void 播放或暂停按钮_Click(object sender, RoutedEventArgs e)
        {

            音频处理器.播放或暂停();
            if (数据.当前播放状态 == 播放状态.播放中)
            {
                数据.手动暂停或停止 = true;
            }
            else if (数据.当前播放状态 == 播放状态.暂停)
            {
                数据.手动暂停或停止 = false;
            }
        }


        private void 停止按钮_Click(object sender, RoutedEventArgs e)
        {
            数据.手动暂停或停止 = true;
            音频处理器.停止();
        }

        private void 上一曲_Click(object sender, RoutedEventArgs e)
        {
            if (数据.当前歌曲 == null) { return; }
            if (数据.播放列表.Count == 0) { return; }
            var 当前曲目数 = 数据.播放列表.ToList().IndexOf(数据.当前歌曲);

            if (当前曲目数 == -1) { 当前曲目数 = 0; }
            当前曲目数--;
            当前曲目数 = Math.Clamp(当前曲目数, 0, 数据.播放列表.Count - 1);
            播放音频(数据.播放列表[当前曲目数]);
        }

        private void 下一曲_Click(object sender, RoutedEventArgs e)
        {
            if (数据.当前歌曲 == null) { return; }
            if (数据.播放列表.Count == 0) { return; }
            var 当前曲目数 = 数据.播放列表.ToList().IndexOf(数据.当前歌曲);

            if (当前曲目数 == -1) { 当前曲目数 = 0; }
            当前曲目数++;
            if (当前曲目数 > 数据.播放列表.Count - 1)
            {
                当前曲目数 = 0;
            }
            当前曲目数 = Math.Clamp(当前曲目数, 0, 数据.播放列表.Count - 1);
            播放音频(数据.播放列表[当前曲目数]);

        }

        private void 播放模式_单曲循环_Click(object sender, RoutedEventArgs e) => 数据.当前播放模式 = UI数据.播放模式.单曲循环;
        private void 播放模式_顺序播放_Click(object sender, RoutedEventArgs e) => 数据.当前播放模式 = UI数据.播放模式.顺序播放;
        private void 播放模式_列表循环_Click(object sender, RoutedEventArgs e) => 数据.当前播放模式 = UI数据.播放模式.列表循环;

        private void 播放模式_随机播放_Click(object sender, RoutedEventArgs e) => 数据.当前播放模式 = UI数据.播放模式.随机播放;

        private void 随机播放_Click(object sender, RoutedEventArgs e)
        {

            var 随机数 = new Random().Next(0, 数据.播放列表.Count);
            播放音频(数据.播放列表[随机数]);

        }
        private void 增大音量(object sender, RoutedEventArgs e)
        {
            if (音频处理器 == null) { return; }
            if (音频处理器.播放设备 == null) { return; }
            数据.当前音量 += 0.1f;
            数据.当前音量 = Math.Clamp(数据.当前音量, 0, 1);
            音频处理器.播放设备.Volume = 数据.当前音量;
        }

        private void 减小音量(object sender, RoutedEventArgs e)
        {
            if (音频处理器 == null) { return; }
            if (音频处理器.播放设备 == null) { return; }
            数据.当前音量 -= 0.1f;
            数据.当前音量 = Math.Clamp(数据.当前音量, 0, 1);
            音频处理器.播放设备.Volume = 数据.当前音量;

        }

        #endregion

        #region 侦听菜单
        private void 自动启停_Click(object sender, RoutedEventArgs e) => 设置.自动启停 = !设置.自动启停;

        private void 立即重播(object sender, RoutedEventArgs e)
        {
            设置.重播间隔 = 0;
        }

        private void 等待2秒后重播(object sender, RoutedEventArgs e)
        {
            设置.重播间隔 = 2;
        }

        private void 等待4秒后重播(object sender, RoutedEventArgs e)
        {
            设置.重播间隔 = 4;
        }

        #endregion

        #region 播放列表菜单
        private void 播放列表_清空_Click(object sender, RoutedEventArgs e) => 数据.播放列表.Clear();


        private void 移除歌曲_Click(object sender, RoutedEventArgs e)
        {
            if (播放列表.SelectedItem == null) { return; }
            数据.播放列表.Remove((播放列表项)播放列表.SelectedItem);
        }

        private void 上移_Click(object sender, RoutedEventArgs e)
        {
            if (播放列表.SelectedItem == null) { return; }
            var 当前选中 = (播放列表项)播放列表.SelectedItem;
            列表移动器.上移(数据.播放列表, 数据.播放列表.IndexOf(当前选中));
            播放列表.SelectedValue = 当前选中;
            数据.更新排序();
        }

        private void 下移_Click(object sender, RoutedEventArgs e)
        {
            if (播放列表.SelectedItem == null) { return; }
            var 当前选中 = (播放列表项)播放列表.SelectedItem;
            列表移动器.下移(数据.播放列表, 数据.播放列表.IndexOf(当前选中));
            播放列表.SelectedValue = 当前选中;
            数据.更新排序();
        }


        private void 保存为默认_Click(object sender, RoutedEventArgs e) =>
            File.WriteAllText(设置.默认播放列表路径, JsonSerializer.Serialize(数据.播放列表.ToArray()));


        #endregion

        #region UI界面
        private void 播放列表_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] 拖入的所有文件 = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (var 文件 in 拖入的所有文件)
                {
                    if (音频控制器.文件支持(文件))
                        数据.播放列表.Add(new 播放列表项(文件));
                }
            }
        }



        private void 播放列表_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var 列表 = (ListBox)sender;
            var 选中项 = (播放列表项)列表.SelectedItem;
            数据.当前选中项 = 选中项;
        }


        private void 播放列表_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var 列表 = (ListBox)sender;
            var 选中项 = (播放列表项)列表.SelectedItem;
            if (选中项 == null) { return; }
            播放音频(选中项);
        }


        private void 打开文件位置(object sender, RoutedEventArgs e)
        {
            if (播放列表.SelectedItem == null) { return; }
            var 当前选中 = (播放列表项)播放列表.SelectedItem;
            if (string.IsNullOrWhiteSpace(当前选中.路径)) { return; }
            文件定位器.定位文件(当前选中.路径);
        }




        private void 修改音量(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (音频处理器 == null) { return; }
            if (音频处理器.播放设备 == null) { return; }
            音频处理器.播放设备.Volume = 数据.当前音量;
        }
        #endregion
    }
}
