using FullStackAPi.Data;
using FullStackAPi.Helper;
using FullStackAPi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.Text.RegularExpressions;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace FullStackAPi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController: Controller
    {
        private readonly FullStackDbContext _fullStackDbContext;
      
        private readonly ILogger _logger;
        public UserController(FullStackDbContext fullStackDbContext, ILoggerFactory loggerFactory)
        {
            _fullStackDbContext = fullStackDbContext;
            _logger = loggerFactory.CreateLogger("Information");
        }

        [HttpPost("Authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] User user)
        {
            if (user == null) { return BadRequest(); }
            var userexist = await _fullStackDbContext.Users.FirstOrDefaultAsync(
                x=>x.UserName==user.UserName);
           if (userexist == null) { return NotFound(new { Message ="User Not Found"}); }
           if(!PasswordHasher.VerifyPassword(user.Password!, userexist.Password!))
                return BadRequest(new { Message = "password is incorrect" });
            userexist.Token = CreateJwtToken(userexist);
            return Ok(new { Token= userexist.Token,Message = "Login Success!" });
        }

       

        [HttpDelete]
        [Route("deleteuser/{id:Guid}")]
        public async Task<IActionResult> DeleteUser([FromRoute] Guid id)
        {
            var deleteduser = await _fullStackDbContext.Users.FindAsync(id);
            if (deleteduser == null) { return NotFound(); }
            _fullStackDbContext.Users.Remove(deleteduser);
            await _fullStackDbContext.SaveChangesAsync();
            return Ok(deleteduser);
        }
        [HttpPost("Register")]
        public async Task<IActionResult> RegisterUser([FromBody] User user)
        {
            user.Id = Guid.NewGuid();
            if (user == null) { return NotFound(); }
            if(await CheckUsernameExistAsync(user.UserName!)) { 
               return BadRequest(new { Message = "Username Already exist!" }); }
            if (await CheckEmailExistAsync(user.Email!))
            {
                return BadRequest(new { Message = "Email Already exist!"});
            }
            var pass = CheckPasswordStrength(user.Password!);
            if (!string.IsNullOrEmpty(pass))
            {
                return BadRequest(new { Message = pass.ToString()}); 
            }
                
            user.CreatedAt = DateTime.Now.ToString();
            user.ModifiedAt = DateTime.Now.ToString();
            if(user.Password != null)
            {
                user.Password = PasswordHasher.HashPassword(user.Password);
                if(user.Role == null) { user.Role = "User"; }
               
                user.Token = "";
                await _fullStackDbContext.Users.AddAsync(user);
                await _fullStackDbContext.SaveChangesAsync();

                return Ok(new { Message = "User Registerd!" });
            }
            return BadRequest(new { Message = "User Not Registerd!" });
        }
        //[Authorize(Roles ="admin")]
        [HttpGet, Route("Users")]
        public async Task<ActionResult<User>> GetAllUsers()
        {
            var users = await _fullStackDbContext.Users.ToListAsync();
            return Ok(users);
        }

        [HttpGet, Route("user/{id:Guid}")]
        public async Task<ActionResult<User>> GetUser([FromRoute] Guid id)
        {

            var user = await _fullStackDbContext.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user == null) { return NotFound(); }
            return Ok(user);
        }
        private async Task<bool> CheckUsernameExistAsync(string username)
        {
            return await _fullStackDbContext.Users.AnyAsync(u => u.UserName == username);
        }

        private async Task<bool> CheckEmailExistAsync(string email)
        {
            return await _fullStackDbContext.Users.AnyAsync(u => u.Email == email);
        }
        private string CheckPasswordStrength(string password)
        {
            StringBuilder sb = new();

            if (password.Length < 8) { sb.Append("Minimum password length should be 8" + Environment.NewLine); }

            if (!(Regex.IsMatch(password, "[a-z]") && Regex.IsMatch(password, "[A-Z]") && Regex.IsMatch(password, "[0-9]")))
            {
                sb.Append("Password should be alphanumeric" + Environment.NewLine);
            }

            if ((!Regex.IsMatch(password, "[<,>,@,!,#,$,%,^,&,*,(,),_,+,\\[,\\],{,},?,:,;,|,',\\,.,/,~,`,-,=]"))) { sb.Append("Password should contain any special character" + Environment.NewLine); }

            return sb.ToString();
        }
        private string CreateJwtToken(User userObj)
        {
            JwtSecurityTokenHandler jwtTokenHandler = new();
            var key = Encoding.ASCII.GetBytes("veryverysecret.....");
            var identity = new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.Role, userObj.Role!),
                new Claim(ClaimTypes.Name, $"{userObj.UserName}")
            });

            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                SigningCredentials = credentials,
                Expires = DateTime.UtcNow.AddHours(1)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);

            return jwtTokenHandler.WriteToken(token);
        }

        [HttpPut]
        [Route("updateuser/{id:Guid}")]  
        public async Task<IActionResult> UpdateUser([FromRoute] Guid id, User updateuserRequested)
        {
            var user = await _fullStackDbContext.Users.FindAsync(id);
            if (user == null) { return NotFound(); }
            user.FirstName = updateuserRequested.FirstName;
            user.LastName = updateuserRequested.LastName;
            user.Email = updateuserRequested.Email;
            user.Role = updateuserRequested.Role;
            user.UserName = updateuserRequested.UserName;
            user.Address = updateuserRequested.Address;
            user.ModifiedAt= DateTime.Now.ToString();
            await _fullStackDbContext.SaveChangesAsync();
            _logger.LogInformation( "Update User with {id} at:{}", id, user.ModifiedAt);
            return Ok(user);
        }

    }
}
