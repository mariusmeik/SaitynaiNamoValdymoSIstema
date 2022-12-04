
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SaitynaiNamoValdymoSIstema.DataDB;
using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using XSystem.Security.Cryptography;

namespace SaitynaiNamoValdymoSIstema.Services
{
    public class SignInManager
    {
        private readonly ILogger<SignInManager> _logger;
        private readonly SaitynaiNamoValdymoSistemaDBContext _ctx;
        private readonly JWTAuthService _JwtAuthService;
        private readonly JwtTokenConfig _jwtTokenConfig;
        XSystem.Security.Cryptography.MD5CryptoServiceProvider md5 = new XSystem.Security.Cryptography.MD5CryptoServiceProvider();

        public SignInManager(ILogger<SignInManager> logger,
                             JWTAuthService JWTAuthService,
                             JwtTokenConfig jwtTokenConfig,
                             SaitynaiNamoValdymoSistemaDBContext ctx)
        {
            _logger = logger;
            _ctx = ctx;
            _JwtAuthService = JWTAuthService;
            _jwtTokenConfig = jwtTokenConfig;
        }

        public async System.Threading.Tasks.Task<SignInResult> SignIn(string userName, string password)
        {
            _logger.LogInformation($"Validating user [{userName}]", userName);

            SignInResult result = new SignInResult();

            if (string.IsNullOrWhiteSpace(userName)) return result;
            if (string.IsNullOrWhiteSpace(password)) return result;

            CreatePasswordHash(password, out byte[] passwordHash);

            //var user = _ctx.People.FromSqlRaw($"SELECT * FROM Person WHERE Name = \'{userName}\' AND Password = \'{password}\';" ).ToList().FirstOrDefault();
            List<Person> users = _ctx.People.FromSqlRaw($"SELECT * FROM Person WHERE Name = \'{userName}\';").ToList();
            var user = users.FirstOrDefault(x=>VerifyPasswordHash(password, x.Password));

            if (user != null)
            {

                var claims = BuildClaims(user);
                result.User = user;
                result.AccessToken = _JwtAuthService.BuildToken(claims);
                result.RefreshToken = _JwtAuthService.BuildRefreshToken();
                RefreshToken rfr = new RefreshToken()
                {
                    PersonId = user.Id,
                    Token = result.RefreshToken,
                    IssuedAt = DateTime.Now,
                    ExpiresAt = DateTime.Now.AddMinutes(_jwtTokenConfig.RefreshTokenExpiration)
                };
                _ctx.RefreshTokens.Add(rfr);
                _ctx.SaveChanges();

                result.Success = true;
            };

            return result;
        }

        public async System.Threading.Tasks.Task<SignInResult> RefreshToken(string AccessToken, string RefreshToken)
        {

            ClaimsPrincipal claimsPrincipal = _JwtAuthService.GetPrincipalFromToken(AccessToken);
            SignInResult result = new SignInResult();

            if (claimsPrincipal == null) return result;

            string id = claimsPrincipal.Claims.First(c => c.Type == "id").Value;
            var user = await _ctx.People.FindAsync(Convert.ToInt32(id));

            if (user == null) return result;

            //var token = await _ctx.RefreshTokens
            //        .Where(f => f.PersonId == user.Id
            //                && f.Token == RefreshToken
            //                && f.ExpiresAt >= DateTime.Now)
            //        .FirstOrDefaultAsync();
            var token = _ctx.RefreshTokens.FromSqlRaw($"SELECT * FROM RefreshToken WHERE PersonId = \'{user.Id}\' AND Token = \'{RefreshToken}\' AND ExpiresAt >= \'{DateTime.Now}\';").ToList().FirstOrDefault();

            if (token == null) return result;

            var claims = BuildClaims(user);

            result.User = user;
            result.AccessToken = _JwtAuthService.BuildToken(claims);
            result.RefreshToken = _JwtAuthService.BuildRefreshToken();

            _ctx.RefreshTokens.Remove(token);
            _ctx.RefreshTokens.Add(new RefreshToken { PersonId = user.Id, Token = result.RefreshToken, IssuedAt = DateTime.Now, ExpiresAt = DateTime.Now.AddMinutes(_jwtTokenConfig.RefreshTokenExpiration) });
            _ctx.SaveChanges();

            result.Success = true;

            return result;
        }

        private Claim[] BuildClaims(Person user)
        {
            //User is Valid
            Claim[] claims = new[]
            {
                new Claim("id",user.Id.ToString()),
                new Claim(ClaimTypes.Name,user.LastName),
                new Claim(ClaimTypes.Role,user.Role)
                //Add Custom Claims here
            };

            return claims;
        }
        private void CreatePasswordHash(string password, out byte[] passwordHash)
        {
            using (var hmac = new HMACSHA512())
            {
                hmac.Key= Encoding.ASCII.GetBytes("123"); 
                //passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash)
        {
            var passwordSalt = Encoding.ASCII.GetBytes("123");
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

    }

    public class SignInResult
    {
        public bool Success { get; set; }
        public Person User { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }

        public SignInResult()
        {
            Success = false;
        }
    }
}
