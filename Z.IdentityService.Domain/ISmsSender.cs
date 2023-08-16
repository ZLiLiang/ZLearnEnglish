namespace Z.IdentityService.Domain
{
    public interface ISmsSender
    {
        public Task SendAsync(string phoneNum, params string[] args);
    }
}
