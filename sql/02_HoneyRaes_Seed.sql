\c HoneyRaes;

INSERT INTO Customer (Name, Address)
VALUES ('Charlie Kelly', '111 Broadway Philadelphia, PA 99999');


INSERT INTO Customer (Name, Address)
VALUES ('Dennis Reynolds', '222 Broadway Philadelphia, PA 99999');

INSERT INTO Customer (Name, Address)
VALUES ('Mac McDonald', '333 Broadway Philadelphia, PA 99999');

INSERT INTO Employee (Name, Specialty)
VALUES ('Jerry Seinfeld', 'Computers');

INSERT INTO Employee (Name, Specialty)
VALUES ('Cosmo Kramer', 'Phones & Tablets');

INSERT INTO Employee (Name, Specialty)
VALUES ('George Costanza', 'Zunes');

INSERT INTO ServiceTicket (CustomerId, EmployeeId, Description, Emergency, DateCompleted)
VALUES (1, 2, 'iPhone broken', 'false', NULL);

INSERT INTO ServiceTicket (CustomerId, EmployeeId, Description, Emergency, DateCompleted) 
VALUES (2, 2, 'MacBook broken', 'false', '2022-06-06');

INSERT INTO ServiceTicket (CustomerId, EmployeeId, Description, Emergency, DateCompleted) 
VALUES (3, 1, 'Zune broken', 'false', '2022-07-07');

INSERT INTO ServiceTicket (CustomerId, EmployeeId, Description, Emergency, DateCompleted) 
VALUES (3, NULL, 'iPad broken', 'true', NULL);

INSERT INTO ServiceTicket (CustomerId, EmployeeId, Description, Emergency, DateCompleted) 
VALUES (2, NULL, 'AppleTV broken', 'true', NULL);
