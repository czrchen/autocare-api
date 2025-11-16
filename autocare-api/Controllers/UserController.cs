using Microsoft.AspNetCore.Mvc;
using autocare_api.Data;
using autocare_api.Models;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly AppDbContext _db;

    public UserController(AppDbContext db)
    {
        _db = db;
    }

    // GET: api/user
    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_db.Users.ToList());
    }

    // POST: api/user
    [HttpPost]
    public IActionResult Create(User user)
    {
        _db.Users.Add(user);
        _db.SaveChanges();
        return Ok(user);
    }

    // GET: api/user/{id}
    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var user = _db.Users.Find(id);
        if (user == null) return NotFound();
        return Ok(user);
    }

    // PUT: api/user/{id}
    [HttpPut("{id}")]
    public IActionResult Update(int id, User updated)
    {
        var user = _db.Users.Find(id);
        if (user == null) return NotFound();

        user.FullName = updated.FullName;
        user.Email = updated.Email;
        _db.SaveChanges();

        return Ok(user);
    }

    // DELETE: api/user/{id}
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var user = _db.Users.Find(id);
        if (user == null) return NotFound();

        _db.Users.Remove(user);
        _db.SaveChanges();

        return Ok(new { message = "User deleted" });
    }
}
