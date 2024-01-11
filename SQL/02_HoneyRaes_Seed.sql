\c HoneyRaes
-- Insert Customers
INSERT INTO Customer (Name, Address)
VALUES
  ('Customer1', 'Address1'),
  ('Customer2', 'Address2'),
  ('Customer3', 'Address3');

-- Insert Employees
INSERT INTO Employee (Name, Specialty)
VALUES
  ('Employee1', 'Specialty1'),
  ('Employee2', 'Specialty2');

-- Insert ServiceTickets
INSERT INTO ServiceTicket (CustomerId, EmployeeId, Description, Emergency, DateCompleted)
VALUES
  (1, 1, 'Ticket1', true, CURRENT_TIMESTAMP),
  (2, 2, 'Ticket2', false, CURRENT_TIMESTAMP),
  (3, NULL, 'Ticket3', true, NULL),
  (1, NULL, 'Ticket4', false, CURRENT_TIMESTAMP),
  (2, 1, 'Ticket5', true, NULL);
