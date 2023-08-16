using Z.FileService.Domain;
using Z.FileService.Domain.Entities;

namespace Z.FileService.Infrastructure
{
    public class FSRepository : IFSRepository
    {
        private readonly FSDbContext dbContext;

        public FSRepository(FSDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        /// <summary>
        /// 查找文件（拥有具体业务逻辑）
        /// </summary>
        /// <param name="fileSize"></param>
        /// <param name="sha256Hash"></param>
        /// <returns></returns>
        public Task<UploadedItem?> FindFileAsync(long fileSize, string sha256Hash)
        {
            return dbContext.UploadItems.FirstOrDefaultAsync(u => u.FileSizeInBytes == fileSize && u.FileSHA256Hash == sha256Hash);
        }
    }
}
