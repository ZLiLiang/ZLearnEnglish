using Z.DomainCommons.Models;

namespace Z.Listening.Domain.Entities
{
    public record Category : AggregateRootEntity, IAggregateRoot
    {
        private Category() { }

        /// <summary>
        /// 在所有Category中的显示序号，越小越靠前
        /// </summary>
        public int SequenceNumber { get; private set; }

        /// <summary>
        /// 名称
        /// </summary>
        public MultilingualString Name { get; private set; }

        /// <summary>
        /// 封面图片。现在一般都不会直接把图片保存到数据库中（Blob），而是只是保存图片的路径。
        /// </summary>
        public Uri CoverUrl { get; private set; }

        /// <summary>
        /// 创建实体实例
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sequenceNumber"></param>
        /// <param name="name"></param>
        /// <param name="coverUrl"></param>
        /// <returns></returns>
        public static Category Create(Guid id, int sequenceNumber, MultilingualString name, Uri coverUrl)
        {
            Category category = new();
            category.Id = id;
            category.SequenceNumber = sequenceNumber;
            category.Name = name;
            category.CoverUrl = coverUrl;
            //category.AddDomainEvent(new CategoryCreatedEventArgs { NewObj = category });
            return category;
        }

        /// <summary>
        /// 改变序号
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Category ChangeSequenceNumber(int value)
        {
            this.SequenceNumber = value;
            return this;
        }

        /// <summary>
        /// 改变名字
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Category ChangeName(MultilingualString value)
        {
            this.Name = value;
            return this;
        }

        /// <summary>
        /// 改变覆盖url
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Category ChangeCoverUrl(Uri value)
        {
            //todo: 做项目的时候，不管这个事件是否有被用到，都尽量publish。
            this.CoverUrl = value;
            return this;
        }
    }
}
