using Npgsql;
using System.Text.Json.Serialization;
using HoneyRaesAPI.Models;
using HoneyRaesAPI.Models.DTOs;
using Microsoft.AspNetCore.Http.Json;
var connectionString = "Host=localhost;Port=5433;Username=postgres;Password=C00kies!;Database=HoneyRaes";




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

//////DONT TOUCH THIS////////////////////////////////////////////////////////////////////////////////


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

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


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

/////GET SERVICETICKETS////////////////////////////////////////////////////////////////////////////////////////

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

////GET SERVICE TICKETS BY ID///////////////////////////////////////////////////////////////////////////////////////


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

///////Endpoint to get all employees///////////////////////////////////////////////////////////////////////////////////////

app.MapGet("/employees", () =>
{
    // create an empty list of employees to add to. 
    List<Employee> employees = new List<Employee>();
    //make a connection to the PostgreSQL database using the connection string
    using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
    //open the connection
    connection.Open();
    // create a sql command to send to the database
    using NpgsqlCommand command = connection.CreateCommand();
    command.CommandText = "SELECT * FROM Employee";
    //send the command. 
    using NpgsqlDataReader reader = command.ExecuteReader();
    //read the results of the command row by row
    while (reader.Read()) // reader.Read() returns a boolean, to say whether there is a row or not, it also advances down to that row if it's there. 
    {
        //This code adds a new C# employee object with the data in the current row of the data reader 
        employees.Add(new Employee
        {
            Id = reader.GetInt32(reader.GetOrdinal("Id")), //find what position the Id column is in, then get the integer stored at that position
            Name = reader.GetString(reader.GetOrdinal("Name")),
            Specialty = reader.GetString(reader.GetOrdinal("Specialty"))
        });
    }
    //once all the rows have been read, send the list of employees back to the client as JSON
    return employees;
});

// Endpoint to get an employee by id/////////////////////////////////////////////////////////////////////////////////////

/////////c# METHOD//////////////////////////
// app.MapGet("/employees/{id}", (int id) =>
// {
//     Employee employee = employees.FirstOrDefault(e => e.Id == id);
//     if (employee == null)
//     {
//         return Results.NotFound();
//     }
//     List<ServiceTicket> tickets = serviceTickets.Where(st => st.EmployeeId == id).ToList();
//     return Results.Ok(new EmployeeDTO
//     {
//         Id = employee.Id,
//         Name = employee.Name,
//         Specialty = employee.Specialty,
//         ServiceTickets = tickets.Select(t => new ServiceTicketDTO
//         {
//             Id = t.Id,
//             CustomerId = t.CustomerId,
//             EmployeeId = t.EmployeeId,
//             Description = t.Description,
//             Emergency = t.Emergency,
//             DateCompleted = t.DateCompleted
//         }).ToList()
//     });
// });

// app.MapGet("/employees/{id}", (int id) =>
// {
//     Employee employee = null;
//     using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
//     connection.Open();
//     using NpgsqlCommand command = connection.CreateCommand();
//     command.CommandText = "SELECT * FROM Employee WHERE Id = @id";
//     // use command parameters to add the specific Id we are looking for to the query
//     command.Parameters.AddWithValue("@id", id);
//     using NpgsqlDataReader reader = command.ExecuteReader();
//     // We are only expecting one row back, so we don't need a loop!
//     if (reader.Read())
//     {
//         employee = new Employee
//         {
//             Id = reader.GetInt32(reader.GetOrdinal("Id")),
//             Name = reader.GetString(reader.GetOrdinal("Name")),
//             Specialty = reader.GetString(reader.GetOrdinal("Specialty"))
//         };
//     }
//     return employee;
// });

app.MapGet("/employees/{id}", (int id) =>
{
    Employee employee = null;
    using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
    connection.Open();
    using NpgsqlCommand command = connection.CreateCommand();
    command.CommandText = @"
        SELECT 
            e.Id,
            e.Name, 
            e.Specialty, 
            st.Id AS serviceTicketId, 
            st.CustomerId,
            st.Description,
            st.Emergency,
            st.DateCompleted 
        FROM Employee e
        LEFT JOIN ServiceTicket st ON st.EmployeeId = e.Id
        WHERE e.Id = @id";
    // use command parameters to add the specific Id we are looking for to the query
    command.Parameters.AddWithValue("@id", id);
    using NpgsqlDataReader reader = command.ExecuteReader();
    // We are only expecting one row back, so we don't need a loop!
    while (reader.Read())
    {
        if (employee == null)
        {
            employee = new Employee
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                Specialty = reader.GetString(reader.GetOrdinal("Specialty")),
                ServiceTickets = new List<ServiceTicket>() //empty List to add service tickets to
            };
        }
        // reader.IsDBNull checks if a column in a particular position is null
        if (!reader.IsDBNull(reader.GetOrdinal("serviceTicketId")))
        {
            employee.ServiceTickets.Add(new ServiceTicket
            {
                Id = reader.GetInt32(reader.GetOrdinal("serviceTicketId")),
                CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                //we don't need to get this from the database, we already know it
                EmployeeId = id,
                Description = reader.GetString(reader.GetOrdinal("Description")),
                Emergency = reader.GetBoolean(reader.GetOrdinal("Emergency")),
                // Npgsql can't automatically convert NULL in the database to C# null, so we have to check whether it's null before trying to get it
                DateCompleted = reader.IsDBNull(reader.GetOrdinal("DateCompleted")) ? null : reader.GetDateTime(reader.GetOrdinal("DateCompleted"))
            });
        }
    }
     // Return 404 if the employee is never set (meaning, that reader.Read() immediately returned false because the id did not match an employee)
    // otherwise 200 with the employee data
    return employee == null ? Results.NotFound() : Results.Ok(employee);
});

// Endpoint to get all customers////////////////////////////////////////////////////////////////////////////////////

app.MapGet("/customers", () =>
{
    return customers.Select(c => new CustomerDTO
    {
        Id = c.Id,
        Name = c.Name,
        Address = c.Address
    });
});

// Endpoint to get a customer by id///////////////////////////////////////////////////////////////////////////////////

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
////////CREATE A NEW SERVICE TICKET///////////////////////////////////////////////////////////////////////////////////

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

///////DELETE A SERVICETICKET///////////////////////////////////////////////////////////////////////////////

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

//////////UPDATE A SERVICETICKET///////////////////////////////////////////////////////////////////////////////

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

/////MARK A SERVICE TICKET AS COMPLETE////////////////////////////////////////////////////////////////////////////////


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

///////////////CREATE AN EMPLOYEE//////////////////////////////////////////////////////////////////////////////////

app.MapPost("/employees", (Employee employee) =>
{
    using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
    connection.Open();
    using NpgsqlCommand command = connection.CreateCommand();
    command.CommandText = @"
        INSERT INTO Employee (Name, Specialty)
        VALUES (@name, @specialty)
        RETURNING Id
    ";
    command.Parameters.AddWithValue("@name", employee.Name);
    command.Parameters.AddWithValue("@specialty", employee.Specialty);

    //the database will return the new Id for the employee, add it to the C# object
    employee.Id = (int)command.ExecuteScalar();// ExecuteScalar() returns the value of the first column of the first row.

    return employee;
});
////////UPDATE AN EMPLOYEE//////////////////////////////////////////////////////////////////////////////////////////

app.MapPut("/employees/{id}", (int id, Employee employee) =>
{
    if (id != employee.Id)
    {
        return Results.BadRequest();
    }
    using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
    connection.Open();
    using NpgsqlCommand command = connection.CreateCommand();
    command.CommandText = @"
        UPDATE Employee 
        SET Name = @name,
            Specialty = @specialty
        WHERE Id = @id
    ";
    command.Parameters.AddWithValue("@name", employee.Name);
    command.Parameters.AddWithValue("@specialty", employee.Specialty);
    command.Parameters.AddWithValue("@id", id);

    command.ExecuteNonQuery();//ExecuteNonQuery is used for data changes when you do not need or expect
    //  any data back from the database after the query. 
    // In this case, as long as the query runs correctly, we do not need any information from the database.
    return Results.NoContent();
});


/////DELETE AN EMPLOYEE///////////////////////////////////////////////////////////////////////////////////////

app.MapDelete("/employees/{id}", (int id) =>
{
    using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
    connection.Open();
    using NpgsqlCommand command = connection.CreateCommand();
    command.CommandText = @"
        DELETE FROM Employee WHERE Id=@id
    ";
    command.Parameters.AddWithValue("@id", id);
    command.ExecuteNonQuery();
    return Results.NoContent();
});

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



app.Run();

// record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
// {
//     public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
// }

