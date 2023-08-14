namespace Z.EventBus
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class EventNameAttribute : Attribute
    {
        public string Name { get; set; }
        public EventNameAttribute(string name)
        {
            this.Name = name;
        }
    }
}
