/// <summary>
/// 列表项移动工具类（静态类）
/// </summary>
public static class 列表移动器
{
    // ---------- 基础方法 ----------
    #region 基础移动方法

    /// <summary>
    /// 将列表中指定索引位置的项向上移动一位
    /// </summary>
    /// <typeparam name="元素类型">列表元素类型</typeparam>
    /// <param name="目标列表">要操作的列表</param>
    /// <param name="当前索引">要移动的项当前索引位置</param>
    public static void 上移<元素类型>(IList<元素类型> 目标列表, int 当前索引)
    {
        if (当前索引 <= 0 || 当前索引 >= 目标列表.Count) return;
        移动到指定索引(目标列表, 当前索引, 当前索引 - 1);
    }

    /// <summary>
    /// 将列表中指定索引位置的项向下移动一位
    /// </summary>
    /// <typeparam name="元素类型">列表元素类型</typeparam>
    /// <param name="目标列表">要操作的列表</param>
    /// <param name="当前索引">要移动的项当前索引位置</param>
    public static void 下移<元素类型>(IList<元素类型> 目标列表, int 当前索引)
    {
        if (当前索引 < 0 || 当前索引 >= 目标列表.Count - 1) return;
        移动到指定索引(目标列表, 当前索引, 当前索引 + 1);
    }

    /// <summary>
    /// 将项移动到列表中的新位置
    /// </summary>
    /// <typeparam name="元素类型">列表元素类型</typeparam>
    /// <param name="目标列表">要操作的列表</param>
    /// <param name="原索引">要移动的项原始索引</param>
    /// <param name="新索引">要移动的目标索引位置</param>
    public static void 移动到指定索引<元素类型>(
        IList<元素类型> 目标列表,
        int 原索引,
        int 新索引)
    {
        if (原索引 < 0 || 原索引 >= 目标列表.Count)
            throw new ArgumentOutOfRangeException(nameof(原索引), "原始索引超出范围");

        新索引 = Math.Clamp(新索引, 0, 目标列表.Count - 1);
        if (原索引 == 新索引) return;

        元素类型 移动项 = 目标列表[原索引];
        目标列表.RemoveAt(原索引);
        目标列表.Insert(新索引, 移动项);
    }
    #endregion

    // ---------- 新增增强方法 ----------
    #region 增强移动方法

    /// <summary>
    /// 通过元素对象直接上移/下移
    /// </summary>
    /// <typeparam name="元素类型">列表元素类型</typeparam>
    /// <param name="目标列表">要操作的列表</param>
    /// <param name="当前项">要移动的元素对象</param>
    /// <param name="是否上移">true=上移，false=下移</param>
    /// <returns>是否移动成功</returns>
    public static bool 移动元素<元素类型>(
        IList<元素类型> 目标列表,
        元素类型 当前项,
        bool 是否上移 = true)
    {
        int 当前索引 = 目标列表.IndexOf(当前项);
        if (当前索引 == -1) return false;

        if (是否上移)
            上移(目标列表, 当前索引);
        else
            下移(目标列表, 当前索引);

        return true;
    }

    /// <summary>
    /// 将当前项移动到目标项附近（前/后位置）
    /// </summary>
    /// <typeparam name="元素类型">列表元素类型</typeparam>
    /// <param name="目标列表">要操作的列表</param>
    /// <param name="当前项">要移动的项</param>
    /// <param name="目标项">作为位置参考的项</param>
    /// <param name="位置偏移">0=同位置，-1=目标前，1=目标后</param>
    /// <returns>是否移动成功</returns>
    public static bool 移动到目标附近<元素类型>(
        IList<元素类型> 目标列表,
        元素类型 当前项,
        元素类型 目标项,
        int 位置偏移 = 0)
    {
        int 当前索引 = 目标列表.IndexOf(当前项);
        int 目标索引 = 目标列表.IndexOf(目标项);

        if (当前索引 == -1 || 目标索引 == -1)
            return false;

        // 计算新位置（自动处理边界）
        int 新位置 = 目标索引 + 位置偏移;
        新位置 = Math.Clamp(新位置, 0, 目标列表.Count - 1);

        // 防止移动到自身位置导致逻辑错误
        if (当前索引 == 新位置) return true;

        移动到指定索引(目标列表, 当前索引, 新位置);
        return true;
    }

    /// <summary>
    /// 将当前项移动到目标项前一个位置
    /// </summary>
    public static bool 移动到目标前<元素类型>(
        IList<元素类型> 目标列表,
        元素类型 当前项,
        元素类型 目标项)
        => 移动到目标附近(目标列表, 当前项, 目标项, -1);

    /// <summary>
    /// 将当前项移动到目标项后一个位置
    /// </summary>
    public static bool 移动到目标后<元素类型>(
        IList<元素类型> 目标列表,
        元素类型 当前项,
        元素类型 目标项)
        => 移动到目标附近(目标列表, 当前项, 目标项, 1);
    #endregion
}
