namespace Identity
{
    public class UserAuthModel
    {
        public string? Name { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
    }
}
