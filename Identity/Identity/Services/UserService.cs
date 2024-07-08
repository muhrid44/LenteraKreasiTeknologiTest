
using Dapper;
using Identity.Helper;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Identity.Services
{
    public class UserService : IUserService
    {
        readonly IConfiguration _config;
        readonly DateTime _tokenLifetime = DateTime.Now.AddHours(1);
        private readonly TokenSetting _tokenSettings;

        public UserService(IConfiguration config, IOptions<TokenSetting> tokenSettings)
        {
            _config = config;
            _tokenSettings = tokenSettings.Value;
        }

        public async Task<(string, bool)> Login(LoginModel model)
        {
            using (var db = new NpgsqlConnection(_config.GetConnectionString("connstring")))
            {
                //name verification
                var query = await db.QueryAsync(@"SELECT * FROM ""passwordhashtable"" 
                WHERE ""name"" = @Name;", model);

            if (query.Count() == 0)
            {
                return ("Name is not existed", false);
            }

            //password verification
                var queryPassword = await db.QueryAsync<UserAuthModel>(@"SELECT * FROM ""passwordhashtable"" 
                WHERE ""name"" = @Name;", model);

                if (queryPassword.Count() == 1)
                {
                    var passwordFromDb = queryPassword.FirstOrDefault();

                    bool isPassCorrect = PasswordUtility.VerifyPasswordHash(model.Password, passwordFromDb.PasswordHash, passwordFromDb.PasswordSalt);

                    if (!isPassCorrect)
                    {
                        return ("Password is invalid", false);
                    }
                }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenSettings.Key));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var claims = new List<Claim>
            {
                new (ClaimTypes.Name, model.Name)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = _tokenLifetime,
                Issuer = "http://localhost:5207",
                Audience = "http://localhost:5062",
                SigningCredentials = creds
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            var jwt = tokenHandler.WriteToken(token);

            return (jwt, true);
            }

        }

        public async Task<string> SignUp(UserAuthModel model)
        {
            try
            {
                using (var db = new NpgsqlConnection(_config.GetConnectionString("connstring")))
                {
                    var query = await db.ExecuteAsync(@"INSERT INTO ""passwordhashtable"" 
                (""name"", ""passwordhash"", ""passwordsalt"") 
                VALUES
                (@Name, @PasswordHash, @PasswordSalt);", model);

                    return "Ok";
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
    }
}
