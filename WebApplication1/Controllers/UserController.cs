using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<User> _hasher;

        public UserController(ILogger<UserController> logger,ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
            _hasher = new PasswordHasher<User>();
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> Get(int Id)
        {
            var user = await _context.Users.FindAsync(Id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> Post(User user)
        {
            if (user == null)
            {
                return BadRequest();
            }

            if (user.Password != null && user.Password != "")
            {
                user.Password = _hasher.HashPassword(user, user.Password);
            }

            user.CreateDate = DateTime.UtcNow;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return Ok(user);
        }

        [HttpPut("{Id}")]
        public async Task<IActionResult> Put(int Id, User user)
        {
            if (user == null)
            {
                return BadRequest();
            }

            var userCheck = await _context.Users.FindAsync(Id);

            if (userCheck == null)
            {
                return NotFound();
            }

            userCheck.FullName = user.FullName;
            userCheck.Email = user.Email;
            userCheck.Status = user.Status;

            if (user.Password != null && user.Password != "")
            {
                userCheck.Password = _hasher.HashPassword(user, user.Password);

                //userCheck.Password = user.Password; //no secure method (alternative)
            }

            _context.Users.Update(userCheck);
               
            //for result
            user.Id = Id;
            user.Password = userCheck.Password;

            await _context.SaveChangesAsync();
            return Ok(user);
        }

        [HttpDelete("{Id}")]
        public async Task<IActionResult> Delete(int Id)
        {
            var user = await _context.Users.FindAsync(Id);

            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
