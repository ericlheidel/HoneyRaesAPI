SELECT * FROM Customer
SELECT * FROM Employee
SELECT * FROM ServiceTicket


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
WHERE e.Id = 2