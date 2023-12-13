using System.Text.Json.Serialization;
using HoneyRaesAPI.Models;
using HoneyRaesAPI.Models.DTOs;
using Microsoft.AspNetCore.Http.Json;




List<Customer> customers = new List<Customer>
{
            new Customer
            {
               Id = 1,
               Name = "Customer1",
               Address = "Address1"
            },

            new Customer
            {
               Id = 2,
               Name = "Customer2",
               Address = "Address2"
            },

            new Customer
            {
               Id = 3,
               Name = "Customer3",
               Address = "Address3"
            }
 };

List<Employee> employees = new List<Employee>
{
            new Employee
            {
                Id = 1,
                Name = "Employee1",
                Specialty = "Specialty1"

            },

            new Employee
            {
                Id = 2,
                Name = "Employee2",
                Specialty = "Specialty2"
            }
};

List<ServiceTicket> serviceTickets = new List<ServiceTicket>

{
            new ServiceTicket
            {
                Id = 1,
                CustomerId = 1,
                EmployeeId = 1,
                Description = "Ticket1",
                Emergency = true,
                DateCompleted = DateTime.Now
            },

            new ServiceTicket
            {
                Id = 2,
                CustomerId = 2,
                EmployeeId = 2,
                Description = "Ticket2",
                Emergency = false,
                DateCompleted = DateTime.Now
            },

            new ServiceTicket
            {
                Id = 3,
                CustomerId = 3,
                EmployeeId = 0,
                Description = "Ticket3",
                Emergency = true,
                DateCompleted = DateTime.MinValue
            },

            new ServiceTicket
            {
                Id = 4,
                CustomerId = 1,
                EmployeeId = 0,
                Description = "Ticket4",
                Emergency = false,
                DateCompleted = DateTime.Now
            },

            new ServiceTicket
            {
                Id = 5,
                CustomerId = 2,
                EmployeeId = 1,
                Description = "Ticket5",
                Emergency = true,
                DateCompleted = DateTime.MinValue
            }




};

// List<HoneyRaesAPI.Models.Customer> customers = new List<HoneyRaesAPI.Models.Customer> {};
// List<HoneyRaesAPI.Models.Employee> employees = new List<HoneyRaesAPI.Models.Employee> {};
// List<HoneyRaesAPI.Models.ServiceTicket> serviceTickets = new List<HoneyRaesAPI.Models.ServiceTicket> {};



var builder = WebApplication.CreateBuilder(args);

// Set the JSON serializer options
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// var summaries = new[]
// {
//     "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
// };

// app.MapGet("/weatherforecast", () =>
// {
//     var forecast = Enumerable.Range(1, 5).Select(index =>
//         new WeatherForecast
//         (
//             DateTime.Now.AddDays(index),
//             Random.Shared.Next(-20, 55),
//             summaries[Random.Shared.Next(summaries.Length)]
//         ))
//         .ToArray();
//     return forecast;
// })
// .WithName("GetWeatherForecast");

// app.MapGet("/hello", () =>
// {
//     return "hello";
// });

app.MapGet("/servicetickets", () =>
{
    return serviceTickets.Select(t => new ServiceTicketDTO
    {
        Id = t.Id,
        CustomerId = t.CustomerId,
        EmployeeId = t.EmployeeId,
        Description = t.Description,
        Emergency = t.Emergency,
        DateCompleted = t.DateCompleted
    });
});

app.MapGet("/servicetickets/{id}", (int id) =>
{
    ServiceTicket serviceTicket = serviceTickets.FirstOrDefault(st => st.Id == id);

    if (serviceTicket == null)
    {
        return Results.NotFound();
    }

    Employee employee = employees.FirstOrDefault(e => e.Id == serviceTicket.EmployeeId);

    Customer customer = customers.FirstOrDefault(c => c.Id == serviceTicket.CustomerId);

    return Results.Ok(new ServiceTicketDTO
    {
        Id = serviceTicket.Id,
        CustomerId = serviceTicket.CustomerId,
        Customer = customer == null ? null : new CustomerDTO
        {
            Id = customer.Id,
            Name = customer.Name,
            Address = customer.Address
        },
        EmployeeId = serviceTicket.EmployeeId,
        Employee = employee == null ? null : new EmployeeDTO
        {
            Id = employee.Id,
            Name = employee.Name,
            Specialty = employee.Specialty
        },
        Description = serviceTicket.Description,
        Emergency = serviceTicket.Emergency,
        DateCompleted = serviceTicket.DateCompleted
    });
});

// Endpoint to get all employees
app.MapGet("/employees", () =>
{
    return employees.Select(e => new EmployeeDTO
    {
        Id = e.Id,
        Name = e.Name,
        Specialty = e.Specialty
    });
});

// Endpoint to get an employee by id
app.MapGet("/employees/{id}", (int id) =>
{
    Employee employee = employees.FirstOrDefault(e => e.Id == id);
    if (employee == null)
    {
        return Results.NotFound();
    }
    List<ServiceTicket> tickets = serviceTickets.Where(st => st.EmployeeId == id).ToList();
    return Results.Ok(new EmployeeDTO
    {
        Id = employee.Id,
        Name = employee.Name,
        Specialty = employee.Specialty,
        ServiceTickets = tickets.Select(t => new ServiceTicketDTO
        {
            Id = t.Id,
            CustomerId = t.CustomerId,
            EmployeeId = t.EmployeeId,
            Description = t.Description,
            Emergency = t.Emergency,
            DateCompleted = t.DateCompleted
        }).ToList()
    });
});
// Endpoint to get all customers
app.MapGet("/customers", () =>
{
    return customers.Select(c => new CustomerDTO
    {
        Id = c.Id,
        Name = c.Name,
        Address = c.Address
    });
});

// Endpoint to get a customer by id
app.MapGet("/customers/{id}", (int id) =>
{
    Customer customer = customers.FirstOrDefault(cust => cust.Id == id);

    if (customer == null)
    {
        return Results.NotFound();
    }
List<ServiceTicket> tickets = serviceTickets.Where(st => st.CustomerId == id).ToList(); 
    return Results.Ok(new CustomerDTO
    {
        Id = customer.Id,
        Name = customer.Name,
        Address = customer.Address,
        ServiceTickets = tickets.Select(t => new ServiceTicketDTO
        {
            Id = t.Id,
            CustomerId = t.CustomerId,
            EmployeeId = t.EmployeeId,
            Description = t.Description,
            Emergency = t.Emergency,
            DateCompleted = t.DateCompleted
        }).ToList()
    });
});

app.MapPost("/servicetickets", (ServiceTicket serviceTicket) =>
{

    // Get the customer data to check that the customerid for the service ticket is valid
    Customer customer = customers.FirstOrDefault(c => c.Id == serviceTicket.CustomerId);

    // if the client did not provide a valid customer id, this is a bad request
    if (customer == null)
    {
        return Results.BadRequest();
    }

    // creates a new id (SQL will do this for us like JSON Server did!)
    serviceTicket.Id = serviceTickets.Max(st => st.Id) + 1;
    serviceTickets.Add(serviceTicket);

    // Created returns a 201 status code with a link in the headers to where the new resource can be accessed
    return Results.Created($"/servicetickets/{serviceTicket.Id}", new ServiceTicketDTO
    {
        Id = serviceTicket.Id,
        CustomerId = serviceTicket.CustomerId,
        Customer = new CustomerDTO
        {
            Id = customer.Id,
            Name = customer.Name,
            Address = customer.Address
        },
        Description = serviceTicket.Description,
        Emergency = serviceTicket.Emergency
    });

});

app.MapDelete("/servicetickets/{id}", (int id) =>
{
   // Assuming serviceTickets is a List<ServiceTicket>
    ServiceTicket serviceTicketToRemove = serviceTickets.FirstOrDefault(st => st.Id == id);

    if (serviceTicketToRemove != null)
    {
        serviceTickets.Remove(serviceTicketToRemove);
        // Perform additional logic (e.g., delete from the database) if needed
        // ...


        return Results.NoContent();
    }
    else
    {
        return Results.NotFound();
    }

});

app.MapPut("/servicetickets/{id}", (int id, ServiceTicket serviceTicket) =>
{
    ServiceTicket ticketToUpdate = serviceTickets.FirstOrDefault(st => st.Id == id);

    if (ticketToUpdate == null)
    {
        return Results.NotFound();
    }
    if (id != serviceTicket.Id)
    {
        return Results.BadRequest();
    }

    ticketToUpdate.CustomerId = serviceTicket.CustomerId;
    ticketToUpdate.EmployeeId = serviceTicket.EmployeeId;
    ticketToUpdate.Description = serviceTicket.Description;
    ticketToUpdate.Emergency = serviceTicket.Emergency;
    ticketToUpdate.DateCompleted = serviceTicket.DateCompleted;

    return Results.NoContent();
});

app.MapPost("/servicetickets/{id}/complete", (int id) =>
{
    ServiceTicket ticketToComplete = serviceTickets.FirstOrDefault(st => st.Id == id);

    if (ticketToComplete != null)
    {
        ticketToComplete.DateCompleted = DateTime.Today;


        return Results.NoContent();
    }
    else
    {
        return Results.NotFound();
    }


});



app.Run();

// record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
// {
//     public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
// }

