using HealthITDatim.Configeration;
using HealthITDatim.Models.DTOs.Requests;
using HealthITDatim.Models.DTOs.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HealthITDatim.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthManagementController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtConfig _jwtConfig;

        public AuthManagementController(UserManager<IdentityUser> userManager,IOptionsMonitor<JwtConfig> optionsMonitor)
        {
            _jwtConfig = optionsMonitor.CurrentValue;
            _userManager = userManager;
        }
        [HttpPost("Register")]
       public async Task<IActionResult> Register([FromBody] UserRegistrationDto user)
        {
            if (ModelState.IsValid)
            {
               var existingUser = await _userManager.FindByEmailAsync(user.Email);
                if(existingUser != null)
                {
                    return BadRequest(new RegistrationResponses()
                    {
                        Errors = new List<string>()
                        {
                            "Email already in user!"
                        },
                        Success = false
                    });
                }
                var newuser = new IdentityUser()
                {
                    Email = user.Email,
                    UserName = user.Username
                };
                var isCreated= await _userManager.CreateAsync(newuser,user.Password);
                if (isCreated.Succeeded)
                {
                    var usrToken = GenerateToken(newuser);

                    return Ok(new RegistrationResponses()
                    {
                        Success = true,
                        Token = usrToken
                    });
                }
                else
                {
                    return BadRequest(new RegistrationResponses()
                    {
                        Errors=isCreated.Errors.Select(x=>x.Description).ToList(),
                        Success = false
                    });
                }
            }

            return BadRequest(new RegistrationResponses()
            {
               Errors=new List<string>()
               {
                   "Invalid Action"
               },
               Success=false
            });
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest user)
        {
            if (ModelState.IsValid)
            {
                var existingUser=await _userManager.FindByEmailAsync(user.Email);
                if(existingUser == null)
                {

                    return BadRequest(new RegistrationResponses()
                    {
                        Errors = new List<string>()
                {
                    "Invalid Login Request!",
                },
                        Success = false
                    });
                }
                var loginsFound = await _userManager.CheckPasswordAsync(existingUser, user.Password);

                if (!loginsFound)
                {
                    return BadRequest(new RegistrationResponses()
                    {
                        Errors = new List<string>()
                {
                    "Login Failed!",
                },
                        Success = false
                    });
                }

                var userToken= GenerateToken(existingUser);
                return Ok(new RegistrationResponses()
                {
                    Success=true,
                    Token=userToken
                });


            }


            return BadRequest(new RegistrationResponses()
            {
                Errors = new List<string>()
                {
                    "Invalid Login Request!",
                },
                Success = false
            });

        }

        private string GenerateToken(IdentityUser user)
        {
            var JwtTokenHandler = new JwtSecurityTokenHandler();

            var key=Encoding.ASCII.GetBytes(_jwtConfig.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Id",user.Id),
                    new Claim(JwtRegisteredClaimNames.Email,user.Email),
                    new Claim(JwtRegisteredClaimNames.Sub,user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                }),
                Expires=DateTime.UtcNow.AddHours(12),
                SigningCredentials= new SigningCredentials(new SymmetricSecurityKey(key),SecurityAlgorithms.HmacSha256Signature)
            };
            var mytoken = JwtTokenHandler.CreateToken(tokenDescriptor);
            var jwtToken=JwtTokenHandler.WriteToken(mytoken);

            return jwtToken;
        }
    }
}
