using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace WingApi
{

    public sealed class ClaimsPrincipalFactory
    {
        /// <summary>
        /// Create a <see cref="ClaimsPrincipal"/> using the claims passed
        /// in. After this call returns the <see cref="ClaimsIdentity.IsAuthenticated"/> flag will be
        /// set to <see cref="true"/> indicating that the authentication has been successful.
        /// </summary>
        /// <param name="claims">The claims with which to initialise the <see cref="ClaimsPrincipal"/></param>
        /// <param name="authenticationType">This could be any string. Defaults to "Password"</param>
        /// <param name="roleType">This could be any string. Defaults to "Recipient"</param>
        /// <returns>An instance of <see cref="ClaimsPrincipal"/> with the claims embedded</returns>
        public static ClaimsPrincipal CreatePrincipal(
            IEnumerable<Claim> claims,
            string authenticationType = null,
            string roleType = null)
        {
            var claimsPrincipal = new ClaimsPrincipal();
            claimsPrincipal.AddIdentity(new ClaimsIdentity(claims,
                                                           string.IsNullOrWhiteSpace(authenticationType) ? "Password" : authenticationType,
                                                           ClaimTypes.Name,
                                                           string.IsNullOrWhiteSpace(roleType) ? "Recipient" : roleType));

            return claimsPrincipal;
        }
    }

    public sealed class TokenOptions
    {
        
        public TokenOptions(string issuer,
                            string audience,
                            string rawSigningKey,
                            int tokenExpiryInMinutes = 5)
        {
            if (string.IsNullOrWhiteSpace(audience))
            {
                throw new ArgumentNullException(
                    $"{nameof(Audience)} is mandatory in order to generate a JWT!");
            }

            if (string.IsNullOrWhiteSpace(issuer))
            {
                throw new ArgumentNullException(
                    $"{nameof(Issuer)} is mandatory in order to generate a JWT!");
            }

            Audience = audience;
            Issuer = issuer;
            SigningKey = new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes(rawSigningKey)) ??
                throw new ArgumentNullException(
                    $"{nameof(SigningKey)} is mandatory in order to generate a JWT!");

            TokenExpiryInMinutes = tokenExpiryInMinutes;
        }

        public SecurityKey SigningKey { get; }

        public string Issuer { get; }

        public string Audience { get; }

        public int TokenExpiryInMinutes { get; }
    }

    public struct TokenConstants
    {
        public const string TokenName = "jwt";
    }

    public interface IJwtTokenGenerator
    {
        TokenWithClaimsPrincipal GenerateAccessTokenWithClaimsPrincipal(string userName, int UserId, int CustomerId,
          IEnumerable<Claim> userClaims);

        string GenerateAccessToken(string userName, int UserId, int CustomerId,
          IEnumerable<Claim> userClaims);
    }
    public class TokenWithClaimsPrincipal
    {
        public string AccessToken { get; internal set; }
        public ClaimsPrincipal ClaimsPrincipal { get; internal set; }
        public AuthenticationProperties AuthProperties { get; internal set; }
    }
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly TokenOptions tokenOptions;
        public JwtTokenGenerator(TokenOptions tokenOptions)
        {
            this.tokenOptions = tokenOptions ??
              throw new ArgumentNullException(
                  $"An instance of valid {nameof(TokenOptions)} must be passed in order to generate a JWT!");
        }

       
        public string GenerateAccessToken(string userName, int UserId, int CustomerId, IEnumerable<Claim> userClaims)
        {
            var expiration = TimeSpan.FromMinutes(tokenOptions.TokenExpiryInMinutes);
            var jwt = new JwtSecurityToken(issuer: tokenOptions.Issuer,
                                           audience: tokenOptions.Audience,
                                           claims: MergeUserClaimsWithDefaultClaims(userName, userClaims),
                                           notBefore: DateTime.UtcNow,
                                           expires: DateTime.UtcNow.Add(expiration),
                                           signingCredentials: new SigningCredentials(
                                               tokenOptions.SigningKey,
                                               SecurityAlgorithms.HmacSha256));

            var accessToken = new JwtSecurityTokenHandler().WriteToken(jwt);

            return accessToken;
        }
               

        public TokenWithClaimsPrincipal GenerateAccessTokenWithClaimsPrincipal(string userName, int UserId, int CustomerId, IEnumerable<Claim> userClaims)
        {
            var userClaimList = userClaims.ToList();
            var accessToken = GenerateAccessToken(userName, UserId,CustomerId, userClaimList);

            return new TokenWithClaimsPrincipal()
            {
                AccessToken = accessToken,
                ClaimsPrincipal = ClaimsPrincipalFactory.CreatePrincipal(
                    MergeUserClaimsWithDefaultClaims(userName, userClaimList)),
                AuthProperties = CreateAuthProperties(accessToken)
            };
        }

        private static AuthenticationProperties CreateAuthProperties(string accessToken)
        {
            var authProps = new AuthenticationProperties();
            authProps.StoreTokens(
                new[]
                {
                    new AuthenticationToken()
                    {
                        Name = TokenConstants.TokenName,
                        Value = accessToken
                    }
                });

            return authProps;
        }

        private static IEnumerable<Claim> MergeUserClaimsWithDefaultClaims(string userName,
            IEnumerable<Claim> userClaims)
        {
            var claims = new List<Claim>(userClaims)
            {
                new Claim(ClaimTypes.Name, userName),
                new Claim(JwtRegisteredClaimNames.Sub, userName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat,
                    DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                    ClaimValueTypes.Integer64)
            };

            return claims;
        }
    }
}
