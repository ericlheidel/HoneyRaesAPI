using HoneyRaesAPI.Models;
using Npgsql;
var connectionString = "Host=localhost;Port=5432;Username=postgres;Password=yardstick;Database=HoneyRaes";

List<Customer> customers = new List<Customer>
{
    new Customer()
    {
        Id = 1,
        Name = "Charlie Kelly",
        Address = "111 Broadway Philadelphia, PA 99999",
    },
    new Customer()
    {
        Id = 2,
        Name = "Dennis Reynolds",
        Address = "222 Broadway Philadelphia, PA 99999",
    },
    new Customer()
    {
        Id = 3,
        Name = "Mac McDonald",
        Address = "333 Broadway Philadelphia, PA 99999",
    }
};

List<Employee> employees = new List<Employee>
{
    new Employee()
    {
        Id = 1,
        Name = "Jerry Seinfeld",
        Specialty = "Computers",
    },
    new Employee()
    {
        Id = 2,
        Name = "Comso Kramer",
        Specialty = "Phones & Tablets",
    }
};

List<ServiceTicket> serviceTickets = new List<ServiceTicket>
{
    new ServiceTicket()
    {
        Id = 1,
        CustomerId = 1,
        EmployeeId = null,
        Description = "iPhone broken",
        Emergency = false,
        DateCompleted = null
    },
    new ServiceTicket()
    {
        Id = 2,
        CustomerId = 2,
        EmployeeId = 2,
        Description = "MacBook broken",
        Emergency = false,
        DateCompleted = new DateTime(2022,06,06)
    },
    new ServiceTicket()
    {
        Id = 3,
        CustomerId = 3,
        EmployeeId = 1,
        Description = "Zune broken",
        Emergency = false,
        DateCompleted = new DateTime(2022,07,07)
    },
    new ServiceTicket()
    {
        Id = 4,
        CustomerId = 3,
        EmployeeId = null,
        Description = "iPad broken",
        Emergency = true,
        DateCompleted = null
    },
    new ServiceTicket()
    {
        Id = 5,
        CustomerId = 2,
        EmployeeId = null,
        Description = "AppleTV broken",
        Emergency = true,
        DateCompleted = null
    },
};

var builder = WebApplication.CreateBuilder(args);

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

//++   /\\\\\\\\\\\\   /\\\\\\\\\\\\\\\   /\\\\\\\\\\\\\\\
//++  /\\\//////////   \/\\\///////////   \///////\\\/////
//++  /\\\              \/\\\                    \/\\\
//++  \/\\\    /\\\\\\\  \/\\\\\\\\\\\            \/\\\
//++   \/\\\   \/////\\\  \/\\\///////             \/\\\
//++    \/\\\       \/\\\  \/\\\                    \/\\\
//++     \/\\\       \/\\\  \/\\\                    \/\\\
//++      \//\\\\\\\\\\\\/   \/\\\\\\\\\\\\\\\        \/\\\
//++        \////////////     \///////////////         \///

app.MapGet("/customers", () =>
{
    return customers.Select(c => new CustomerDTO
    {
        Id = c.Id,
        Name = c.Name,
        Address = c.Address
    });
});

app.MapGet("/customers/{id}", (int id) =>
{
    Customer customer = customers.FirstOrDefault(c => c.Id == id);

    List<ServiceTicket> tickets = serviceTickets.Where(st => st.CustomerId == id).ToList();

    if (customer == null)
    {
        return Results.NotFound();
    }

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
        }
        ).ToList()
    });
});

// app.MapGet("/employees", () =>
// {
//     return employees.Select(e => new EmployeeDTO
//     {
//         Id = e.Id,
//         Name = e.Name,
//         Specialty = e.Specialty
//     });
// });

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

app.MapGet("/employees/{id}", (int id) =>
{
    Employee employee = employees.FirstOrDefault(c => c.Id == id);

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

app.MapGet("/servicetickets", () =>
{
    List<ServiceTicketDTO> ticketsToReturnDTO = new List<ServiceTicketDTO>();

    foreach (ServiceTicket ticket in serviceTickets)
    {
        ServiceTicketDTO ticketToReturnDTO = new ServiceTicketDTO
        {
            Id = ticket.Id,
            CustomerId = ticket.CustomerId,
            Customer = customers
            .Where(c => c.Id == ticket.CustomerId).Select(c => new CustomerDTO
            {
                Id = c.Id,
                Name = c.Name,
                Address = c.Address

            }).FirstOrDefault(),
            EmployeeId = ticket.EmployeeId,
            Employee = employees
            .Where(e => e.Id == ticket.EmployeeId).Select(e => new EmployeeDTO
            {
                Id = e.Id,
                Name = e.Name,
                Specialty = e.Specialty
            }).FirstOrDefault(),
            Description = ticket.Description,
            Emergency = ticket.Emergency,
            DateCompleted = ticket.DateCompleted
        };

        ticketsToReturnDTO.Add(ticketToReturnDTO);
    }

    return Results.Ok(ticketsToReturnDTO);
});

app.MapGet("/servicetickets/{id}", (int id) =>
{
    ServiceTicket serviceTicket = serviceTickets.FirstOrDefault(st => st.Id == id);

    if (serviceTicket == null)
    {
        return Results.NotFound();
    }

    Customer customer = customers.FirstOrDefault(c => c.Id == serviceTicket.CustomerId);

    Employee employee = employees.FirstOrDefault(e => e.Id == serviceTicket.EmployeeId);

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

app.MapGet("/servicetickets/emergencies", () =>
{
    List<ServiceTicketDTO> emergencyTicketsDTO = new List<ServiceTicketDTO>();

    List<ServiceTicket> emergencyTickets = serviceTickets
        .Where(st => st.DateCompleted == null && st.Emergency == true).ToList();

    foreach (ServiceTicket emergencyTicket in emergencyTickets)
    {
        ServiceTicketDTO emergencyTicketDTO = new ServiceTicketDTO
        {
            Id = emergencyTicket.Id,
            CustomerId = emergencyTicket.CustomerId,
            Customer = customers
            .Where(c => c.Id == emergencyTicket.CustomerId).Select(c => new CustomerDTO
            {
                Id = c.Id,
                Name = c.Name,
                Address = c.Address

            }).FirstOrDefault(),
            EmployeeId = emergencyTicket.EmployeeId,
            Employee = employees
            .Where(e => e.Id == emergencyTicket.EmployeeId).Select(e => new EmployeeDTO
            {
                Id = e.Id,
                Name = e.Name,
                Specialty = e.Specialty
            }).FirstOrDefault(),
            Description = emergencyTicket.Description,
            Emergency = emergencyTicket.Emergency,
            DateCompleted = emergencyTicket.DateCompleted
        };

        emergencyTicketsDTO.Add(emergencyTicketDTO);
    }

    return Results.Ok(emergencyTicketsDTO);
});

app.MapGet("/servicetickets/unassigned", () =>
{
    List<ServiceTicketDTO> unassignedTicketsDTO = new List<ServiceTicketDTO>();

    List<ServiceTicket> unassignedTickets = serviceTickets
    .Where(st => st.EmployeeId == null).ToList();

    foreach (ServiceTicket unassignedTicket in unassignedTickets)
    {
        ServiceTicketDTO unassignedTicketDTO = new ServiceTicketDTO
        {
            Id = unassignedTicket.Id,
            CustomerId = unassignedTicket.CustomerId,
            Customer = customers
            .Where(c => c.Id == unassignedTicket.CustomerId).Select(c => new CustomerDTO
            {
                Id = c.Id,
                Name = c.Name,
                Address = c.Address

            }).FirstOrDefault(),
            EmployeeId = unassignedTicket.EmployeeId,
            Employee = employees
            .Where(e => e.Id == unassignedTicket.EmployeeId).Select(e => new EmployeeDTO
            {
                Id = e.Id,
                Name = e.Name,
                Specialty = e.Specialty
            }).FirstOrDefault(),
            Description = unassignedTicket.Description,
            Emergency = unassignedTicket.Emergency,
            DateCompleted = unassignedTicket.DateCompleted
        };

        unassignedTicketsDTO.Add(unassignedTicketDTO);
    }

    return Results.Ok(unassignedTicketsDTO);
});

//++ /\\\\\\\\\\\\\        /\\\\\         /\\\\\\\\\\\    /\\\\\\\\\\\\\\\
//++ \/\\\/////////\\\    /\\\///\\\     /\\\/////////\\\ \///////\\\/////
//++  \/\\\       \/\\\  /\\\/  \///\\\  \//\\\      \///        \/\\\
//++   \/\\\\\\\\\\\\\/  /\\\      \//\\\  \////\\\               \/\\\
//++    \/\\\/////////   \/\\\       \/\\\     \////\\\            \/\\\
//++     \/\\\            \//\\\      /\\\         \////\\\         \/\\\
//++      \/\\\             \///\\\  /\\\    /\\\      \//\\\        \/\\\
//++       \/\\\               \///\\\\\/    \///\\\\\\\\\\\/         \/\\\
//++        \///                  \/////        \///////////           \///

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

app.MapPost("/servicetickets/{id}/complete", (int id) =>
{
    ServiceTicket ticketToComplete = serviceTickets.FirstOrDefault(st => st.Id == id);

    ticketToComplete.DateCompleted = DateTime.Today;
});

//++  /\\\\\\\\\\\\      /\\\\\\\\\\\\\\\   /\\\
//++  \/\\\////////\\\   \/\\\///////////   \/\\\
//++   \/\\\      \//\\\  \/\\\              \/\\\
//++    \/\\\       \/\\\  \/\\\\\\\\\\\      \/\\\
//++     \/\\\       \/\\\  \/\\\///////       \/\\\
//++      \/\\\       \/\\\  \/\\\              \/\\\
//++       \/\\\       /\\\   \/\\\              \/\\\
//++        \/\\\\\\\\\\\\/    \/\\\\\\\\\\\\\\\  \/\\\\\\\\\\\\\\\
//++         \////////////      \///////////////   \///////////////

app.MapDelete("/servicetickets/{id}", (int id) =>
{
    ServiceTicket serviceTicket = serviceTickets.FirstOrDefault(st => st.Id == id);

    if (serviceTicket == null)
    {
        return Results.NotFound();
    }

    serviceTickets.Remove(serviceTicket);

    return Results.NoContent();
});

//++  /\\\\\\\\\\\\\     /\\\        /\\\   /\\\\\\\\\\\\\\\
//++  \/\\\/////////\\\  \/\\\       \/\\\  \///////\\\/////
//++   \/\\\       \/\\\  \/\\\       \/\\\        \/\\\
//++    \/\\\\\\\\\\\\\/   \/\\\       \/\\\        \/\\\
//++     \/\\\/////////     \/\\\       \/\\\        \/\\\
//++      \/\\\              \/\\\       \/\\\        \/\\\
//++       \/\\\              \//\\\      /\\\         \/\\\
//++        \/\\\               \///\\\\\\\\\/          \/\\\
//++         \///                  \/////////            \///

app.MapPut("/servicetickets/{id}", (int id, ServiceTicket serviceTicket) =>
{
    ServiceTicket ticketToUpdate = serviceTickets.FirstOrDefault(st => st.Id == id);

    if (ticketToUpdate == null)
    {
        return Results.NotFound();
    }

    if (id != ticketToUpdate.Id)
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

app.Run();
