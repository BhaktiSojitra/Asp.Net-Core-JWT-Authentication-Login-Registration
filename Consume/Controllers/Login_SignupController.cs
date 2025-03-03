using Consume.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Consume.Controllers
{
    public class Login_SignupController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public Login_SignupController(IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = new HttpClient
            {
                BaseAddress = new System.Uri(_configuration["WebApiBaseUrl"])
            };
        }
        public IActionResult Index()
        {
            return View();
        }

        #region Login
        public IActionResult Login()
        {
            return View();
        }
        #endregion

        #region LoginSave
        public async Task<IActionResult> LoginSave(LoginModel loginModel)
        {
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(loginModel);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response;

                try
                {
                    response = await _httpClient.PostAsync($"{_httpClient.BaseAddress}/User/LoginUser", content);
                    var responseContent = await response.Content.ReadAsStringAsync();

                    Console.WriteLine($"API Response: {response.StatusCode}, Content: {responseContent}");

                    if (response.IsSuccessStatusCode)
                    {
                        var token = JsonConvert.DeserializeObject<JwtResponse>(responseContent);
                        Console.WriteLine("token:- " + token.Token);

                        if (token != null && !string.IsNullOrEmpty(token.Token))
                        {
                            var handler = new JwtSecurityTokenHandler();
                            var jwtToken = handler.ReadJwtToken(token.Token);
                            var role = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

                            HttpContext.Session.SetString("JWToken", token.Token);

                            if (role == "Admin")
                                return RedirectToAction("Admin", "Panel");

                            if (role == "User")
                                return RedirectToAction("User", "Panel");
                        }
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)  // Handle 401 Unauthorized
                    {
                        var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(responseContent);

                        if (errorResponse != null && !string.IsNullOrWhiteSpace(errorResponse.Message))
                        {
                            if (errorResponse.Message.Contains("User does not exist", StringComparison.OrdinalIgnoreCase))
                            {
                                ModelState.AddModelError("UserName", "User does not exist. Please create account.");
                            }
                            else if (errorResponse.Message.Contains("Incorrect password", StringComparison.OrdinalIgnoreCase))
                            {
                                ModelState.AddModelError("Password", "Incorrect password. Please try again.");
                            }
                            else if (errorResponse.Message.Contains("Invalid username or password", StringComparison.OrdinalIgnoreCase))
                            {
                                ViewBag.ErrorMessage = "Invalid UserName or Password";
                            }
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "Invalid credentials. Please check your username and password.");
                        }
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(responseContent);

                        if (errorResponse != null && !string.IsNullOrWhiteSpace(errorResponse.Message))
                        {
                            if (errorResponse.Message.Contains("User does not exist", StringComparison.OrdinalIgnoreCase))
                            {
                                ModelState.AddModelError("UserName", "User does not exist. Please sign up.");
                            }
                            else if (errorResponse.Message.Contains("Incorrect password", StringComparison.OrdinalIgnoreCase))
                            {
                                ModelState.AddModelError("Password", "Incorrect password. Please try again.");
                            }
                            else if (errorResponse.Message.Contains("Invalid username or password", StringComparison.OrdinalIgnoreCase))
                            {
                                ViewBag.ErrorMessage = "Invalid UserName or Password";
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again later.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception: {ex.Message}");
                    ModelState.AddModelError(string.Empty, "An error occurred while communicating with the server.");
                }
            }
            return View("Login", loginModel);
        }
        #endregion

        #region Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("JWToken"); // Remove JWT Token from session
            return RedirectToAction("Login", "Login_Signup"); // Redirect to login page
        }
        #endregion

        #region SignupSave
        public async Task<IActionResult> SignupSave(SignupModel signupModel)
        {
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(signupModel);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response;

                try
                {
                    response = await _httpClient.PostAsync($"{_httpClient.BaseAddress}/User/SignUpUser", content);
                    var responseContent = await response.Content.ReadAsStringAsync();

                    Console.WriteLine($"API Response: {response.StatusCode}, Content: {responseContent}");

                    if (response.IsSuccessStatusCode)
                    {
                        ViewBag.SuccessMessage = "Account created successfully! You can now log in.";
                        ModelState.Clear();
                        return View("Signup", new SignupModel());
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(responseContent);

                        if (errorResponse != null && !string.IsNullOrWhiteSpace(errorResponse.Message))
                        {
                            if (errorResponse.Message.Contains("Username", StringComparison.OrdinalIgnoreCase))
                            {
                                ModelState.AddModelError("UserName", "Username already exists.");
                            }
                            if (errorResponse.Message.Contains("Email", StringComparison.OrdinalIgnoreCase))
                            {
                                ModelState.AddModelError("Email", "Email already exists.");
                            }
                            if (!ModelState.IsValid)
                            {
                                // Ensure all validation errors are shown on the form
                                return View("Signup", signupModel);
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again later.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception: {ex.Message}");
                    ModelState.AddModelError(string.Empty, "An error occurred while communicating with the server.");
                }
            }
            return View("Signup", signupModel);
        }
        #endregion

        #region SignUp
        public IActionResult Signup()
        {
            return View();
        }
        #endregion
    }
}
