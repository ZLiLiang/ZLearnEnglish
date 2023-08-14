using Microsoft.EntityFrameworkCore;

namespace Z.ASPNETCore
{
    /// <summary>
    /// 特性，构造函数验证传入的参数与DbContext是否相关，不相关则抛出错误
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Module, AllowMultiple = false, Inherited = true)]
    public class UnitOfWorkAttribute : Attribute
    {
        public Type[] DbContextTypes { get; set; }
        public UnitOfWorkAttribute(params Type[] dbContextTypes)
        {
            this.DbContextTypes = dbContextTypes;
            foreach (var type in dbContextTypes)
            {
                if (!typeof(DbContext).IsAssignableFrom(type))
                {
                    throw new ArgumentException($"{type} must inherit from DbContext");
                }
            }
        }
    }
}
