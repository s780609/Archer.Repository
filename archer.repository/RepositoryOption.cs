namespace Archer.Repository
{
    public enum RepositoryOption
    {
        /// <summary>
        /// 使Update方法可以更新欄位為Null。注意塞進來的Model是否有多餘的Null欄位。
        /// </summary>
        EnableUpdateNull = 1
    }
}
