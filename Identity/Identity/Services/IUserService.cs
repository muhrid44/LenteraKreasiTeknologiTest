namespace Identity.Services
{
    public interface IUserService
    {
        public Task<string> SignUp(UserAuthModel model);
        public Task<(string, bool)> Login(LoginModel model);

    }
}
