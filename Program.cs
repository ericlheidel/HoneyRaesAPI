using HoneyRaesAPI.Models;

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
        EmployeeId = 1,
        Description = "iPhone broken",
        Emergency = true,
        DateCompleted = new DateTime(2022,05,05)
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
        // EmployeeId = ,
        Description = "iPad broken",
        Emergency = true,
        // DateCompleted = new DateTime()
    },
    new ServiceTicket()
    {
        Id = 5,
        CustomerId = 2,
        // EmployeeId = ,
        Description = "AppleTV broken",
        Emergency = true,
        // DateCompleted = new DateTime()
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

    if (customer == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(new CustomerDTO
    {
        Id = customer.Id,
        Name = customer.Name,
        Address = customer.Address
    });
});

app.MapGet("/employees", () =>
{
    return employees.Select(e => new EmployeeDTO
    {
        Id = e.Id,
        Name = e.Name,
        Specialty = e.Specialty
    });
});

app.MapGet("/employees/{id}", (int id) =>
{
    Employee employee = employees.FirstOrDefault(c => c.Id == id);

    if (employee == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(new EmployeeDTO
    {
        Id = employee.Id,
        Name = employee.Name,
        Specialty = employee.Specialty
    });
});

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

    return Results.Ok(new ServiceTicketDTO
    {
        Id = serviceTicket.Id,
        CustomerId = serviceTicket.CustomerId,
        EmployeeId = serviceTicket.EmployeeId,
        Description = serviceTicket.Description,
        Emergency = serviceTicket.Emergency,
        DateCompleted = serviceTicket.DateCompleted
    });
});

app.Run();
