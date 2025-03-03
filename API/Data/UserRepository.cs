using API.Models;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace API.Data
{
    public class UserRepository
    {
        private readonly string _connectionString;
        private readonly IConfiguration _configuration;
        public UserRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = configuration.GetConnectionString("ConnectionString");
        }

        #region SelectAll
        public IEnumerable<UserModel> SelectAll()
        {
            var users = new List<UserModel>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_User_SelectAll", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    users.Add(new UserModel
                    {
                        UserID = Convert.ToInt32(reader["UserID"]),
                        UserName = reader["UserName"].ToString(),
                        Password = reader["Password"].ToString(),
                        Email = reader["Email"].ToString(),
                        PhoneNumber = reader["PhoneNumber"].ToString(),
                        Address = reader["Address"].ToString(),
                        Role = reader["Role"].ToString()
                    });
                }
                return users;
            }
        }
        #endregion

        #region Signup
        public bool Signup(UserModel userModel, out string errorMessage)
        {
            errorMessage = string.Empty;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_User_SignUp", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@UserName", userModel.UserName);
                cmd.Parameters.AddWithValue("@Password", userModel.Password);
                cmd.Parameters.AddWithValue("@Email", userModel.Email);
                cmd.Parameters.AddWithValue("@PhoneNumber", userModel.PhoneNumber);
                cmd.Parameters.AddWithValue("@Address", userModel.Address);

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    return true;
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 50000) // Custom RAISERROR
                    {
                        errorMessage = ex.Message;
                    }
                    else
                    {
                        errorMessage = "An error occurred while inserting the user.";
                    }
                    return false;
                }
            }
        }
        #endregion

        #region Login
        public string Login(LoginModel loginModel, out string errorMessage)
        {
            errorMessage = string.Empty;
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand("PR_User_Login", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserName", loginModel.UserName);
                    cmd.Parameters.AddWithValue("@Password", loginModel.Password);
                    cmd.Parameters.AddWithValue("@Role", loginModel.Role);  

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read()) // If login is successful
                        {
                            var user = new UserModel
                            {
                                UserID = Convert.ToInt32(reader["UserID"]),
                                UserName = reader["UserName"].ToString(),
                                Role = reader["Role"].ToString()
                            };

                            return GenerateJwtToken(user);
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                errorMessage = ex.Message; // Capture error message from stored procedure
                return null;
            }
            catch (Exception ex)
            {
                errorMessage = "An unexpected error occurred. Please try again.";
                return null;
            }

            errorMessage = "Invalid username or password.";
            return null;
        }

        private string GenerateJwtToken(UserModel user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("UserID", user.UserID.ToString()) // Ensure UserID is included
            };

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddDays(7), // Token valid for 7 days
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        #endregion
    }
}
