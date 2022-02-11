namespace AutoAdminLib.Dto
{
    public class UpdateQueryDto<T> : QueryDto where T : class
    {
        public T Entity { get; set; }
    }
}