using belajarnetapi.Dto;
using belajarnetapi.Models.Authentication.SignUp;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using mailservice.Services;
using mailservice.Models;
using belajarnetapi.Models.Authentication.Login;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace belajarnetapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController: ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        
        public AuthenticationController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, IEmailService emailService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _emailService = emailService;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUser registerUser)
        {
            ApiResponse<string> response = new ApiResponse<string>();

            try
            {
                var userExists = await _userManager.FindByNameAsync(registerUser.Username);
                if (userExists != null){
                    response.Success = false;
                    response.ErrorMessage = "User already exists!";
                    return StatusCode(403, response);
                }

                IdentityUser user = new IdentityUser()
                {
                    Email = registerUser.Email,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = registerUser.Username
                };

                if ( await _roleManager.RoleExistsAsync("User") ){
                    var result = await _userManager.CreateAsync(user, registerUser.Password);
                    if (!result.Succeeded){
                        response.Success = false;
                        response.ErrorMessage = string.Join(", ", result.Errors.Select(e => e.Description));
                        return StatusCode(400, response);
                    }

                    result = await _userManager.AddToRoleAsync(user, "User");
                    if (!result.Succeeded){
                        response.Success = false;
                        response.ErrorMessage = string.Join(", ", result.Errors.Select(e => e.Description));
                        return StatusCode(400, response);
                    }

                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var confirmationLink = Url.Action("ConfirmEmail", "Authentication", new { token, email = user.Email }, Request.Scheme);
                    var message = new Message(new string[] {user.Email}, "Confirmation email link", confirmationLink);
                    _emailService.Send(message);
                    
                }else{
                    response.Success = false;
                    response.ErrorMessage = "User role doesn't exists!";
                    return StatusCode(500, response);
                }
            }
            catch (Exception e)
            {
                response.Success = false;
                response.ErrorMessage = "Internal server error";
                return StatusCode(500, response);
            }

            response.Success = true;
            response.ErrorMessage = null;
            response.Data = "User created successfully!";
            
            return StatusCode(201, response);
        }


        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string email, string token)
        {
            ApiResponse<string> response = new ApiResponse<string>();

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token))
            {
                response.Success = false;
                response.ErrorMessage = $"User Id or Token cannot be empty";
                return StatusCode(400, response);
            }

            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    response.Success = false;
                    response.ErrorMessage = $"User with Email {email} not found";
                    return StatusCode(404, response);
                }

                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (!result.Succeeded)
                {
                    response.Success = false;
                    response.ErrorMessage = $"Error confirming email for user with Email {email}";
                    return StatusCode(500, response);
                }
            }
            catch (Exception e)
            {
                response.Success = false;
                response.Data = e.Message;
                response.ErrorMessage = $"Error confirming email for user with Email {email}";
                return StatusCode(500, response);
            }

            response.Success = true;
            response.ErrorMessage = null;
            response.Data = "Email confirmed successfully!";
            
            return StatusCode(200, response);
        }


        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginUser loginUser)
        {
            ApiResponse<string> response = new ApiResponse<string>();

            try 
            {
                var user = await _userManager.FindByNameAsync(loginUser.Username);
                if (user != null && await _userManager.CheckPasswordAsync(user, loginUser.Password) && await _userManager.IsEmailConfirmedAsync(user))
                {
                    var authClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    };

                    var userRoles = await _userManager.GetRolesAsync(user);

                    foreach (var userRole in userRoles)
                    {
                        authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                    }

                    var jwtToken = GetToken(authClaims);
                    var expirationDate = jwtToken.ValidTo;

                    response.Success = true;
                    response.ErrorMessage = null;
                    response.Data = new JwtSecurityTokenHandler().WriteToken(jwtToken);
                    return StatusCode(200, response);
                }else if (user == null | !await _userManager.CheckPasswordAsync(user, loginUser.Password)){
                    response.Success = false;
                    response.ErrorMessage = "Invalid username or password";
                    return StatusCode(401, response);
                }else{
                    response.Success = false;
                    response.ErrorMessage = "Email not confirmed yet";
                    return StatusCode(401, response);
                }
            }
            catch (Exception e)
            {
                response.Success = false;
                response.Data = e.Message;
                response.ErrorMessage = "Internal server error";
                return StatusCode(500, response);
            }
        }

        private JwtSecurityToken GetToken(List<Claim> authClaims){
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(1),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }
    }
}