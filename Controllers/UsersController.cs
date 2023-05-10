using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace VKTask
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UsersDbContext _context;
        private readonly int recordsOnPage = 5;

        public UsersController(UsersDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers(int pageNumber = 1)
        {
            return await _context.User.Where(t => t.Page == pageNumber).Include(u => u.UserGroup).Include(t => t.UserState).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return user;
        }

        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            if (await _context.User.Where(u => u.Login == user.Login).CountAsync() > 0)
            {
                return BadRequest("Login already exists!");
            }
            var group = await _context.UserGroup.FindAsync(user.UserGroupId);
            var stateActive = await _context.UserState.Where(t => t.Code == "Active").FirstAsync();
            if (group == null || (group.Code == "Admin" && _context.User.Where(t => t.UserGroup == group).Count() > 0))
            {
                return BadRequest("Wrong group parametr!");
            }
            user.UserGroup = group;
            user.UserState = stateActive;
            user.UserStateId = stateActive.Id;
            if (_context.User.Any())
            {
                var lastUser = await _context.User.OrderBy(t => t.Page).LastAsync();
                int pageNumber = await _context.User.Where(t => t.Page == lastUser.Page).CountAsync();
                if (pageNumber == recordsOnPage)
                    user.Page = lastUser.Page + 1;
                else
                    user.Page = lastUser.Page;
            }
            else
                user.Page = 1;
            _context.User.Add(user);
            try 
            {
                await _context.SaveChangesAsync();
            }
            catch(DbUpdateException)
            {
                return BadRequest("Wrong params!");
            }
            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.User.FindAsync(id);
            var state = await _context.UserState.Where(t => t.Code == "Blocked").FirstAsync();
            if (user == null || state == null)
            {
                return NotFound();
            }
            user.UserGroupId = state.Id;
            user.UserState = state;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UnblockUser(int id)
        {
            var user = await _context.User.FindAsync(id);
            var state = await _context.UserState.Where(t => t.Code == "Active").FirstAsync();
            if (user == null || state == null)
            {
                return NotFound();
            }
            user.UserGroupId = state.Id;
            user.UserState = state;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
