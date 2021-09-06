using API.Data;
using API.DTO;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace API.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase {
        private readonly DBContext _context;
        private readonly ITokenService _tokenService;

        public AccountsController(DBContext context, ITokenService tokenService) {
            _context = context;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterCredentials registerCredentials) {
            if (await IsUserExists(registerCredentials.UserName)) return BadRequest("Username already exists");
            
            using var hmac = new HMACSHA512();

            var user = new User() {
                Name = registerCredentials.Name,
                UserName = registerCredentials.UserName,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerCredentials.Password)),
                PasswordSalt = hmac.Key
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UserDto {
                UserName = registerCredentials.UserName,
                Token = _tokenService.GenerateToken(user)
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginCredentials loginCredentials) {
            if (!await IsUserExists(loginCredentials.UserName)) return Unauthorized("Invalid Username");

            var user = await _context.Users.FirstOrDefaultAsync(user => user.UserName == loginCredentials.UserName);

            using var hmac = new HMACSHA512(user.PasswordSalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginCredentials.Password));

            for (int hashIndex = 0; hashIndex < computedHash.Length; hashIndex++)
                if (computedHash[hashIndex] != user.PasswordHash[hashIndex]) return Unauthorized("Invalid Password");

            return new UserDto {
                UserName = loginCredentials.UserName,
                Token = _tokenService.GenerateToken(user)
            };
        }

        private async Task<bool> IsUserExists(string username) {
            return await _context.Users.AnyAsync(user => user.UserName == username);
        }
    }
}
