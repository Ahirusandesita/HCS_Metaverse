
public static class StringExtension
{
    /// <summary>
    /// 文字数降順にバブルソートで並び変える
    /// </summary>
    /// <param name="target"></param>
    public static void SortLength(this string[] target)
    {
        for (int i = 0; i < target.Length - 1; i++)
        {
            for (int j = 0; j < target.Length - (1 + i); j++)
            {
                if (target[j].Length < target[j + 1].Length)
                {
                    string tmp = target[j];
                    target[j] = target[j + 1];
                    target[j + 1] = tmp;
                }
            }
        }
    }
}
