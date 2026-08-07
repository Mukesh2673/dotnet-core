using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyFirstApi.Data;
using MyFirstApi.Models;
using MyFirstApi.DTOs;

namespace MyFirstApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CustomerController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Customer
        [HttpGet]
        public async Task<IActionResult> GetCustomers()
        {
            var customers = await _context.Customers.ToListAsync();

            return Ok(new
            {
                Message = "Customers retrieved successfully.",
                Data = customers
            });
        }

        // GET: api/Customer/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                return NotFound(new
                {
                    Message = $"Customer with Id {id} not found."
                });
            }

            return Ok(new
            {
                Message = "Customer retrieved successfully.",
                Data = customer
            });
        }

        // POST: api/Customer
        [HttpPost]
        public async Task<IActionResult> PostCustomer(CreateCustomerDto dto)
        {
            var customer = new Customer
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Address = dto.Address,
                Phone = dto.Phone
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetCustomer),
                new { id = customer.Id },
                new
                {
                    Message = "Customer created successfully.",
                    Data = customer
                });
        }


        // PUT: api/Customer/1
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomer(int id, Customer customer)
        {
            if (id != customer.Id)
            {
                return BadRequest(new
                {
                    Message = "Customer Id does not match."
                });
            }

            _context.Entry(customer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
                {
                    return NotFound(new
                    {
                        Message = $"Customer with Id {id} not found."
                    });
                }

                throw;
            }

            return Ok(new
            {
                Message = "Customer updated successfully.",
                Data = customer
            });
        }

        // DELETE: api/Customer/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                return NotFound(new
                {
                    Message = $"Customer with Id {id} not found."
                });
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Customer deleted successfully."
            });
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.Id == id);
        }
    }
}