create database SistemaInventario
go

use SistemaInventario

CREATE TABLE Usuarios (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    NombreUsuario NVARCHAR(50) NOT NULL UNIQUE,
    Contrasena NVARCHAR(255) NOT NULL,
    ROL NVARCHAR(50) NOT NULL,
    FechaCreacion DATETIME DEFAULT GETDATE(),
 
);
INSERT INTO Usuarios (NombreUsuario, Contrasena, ROL, FechaCreacion)
VALUES 
(
    'admin',                              -- Nombre de usuario
   'admin123',
   	'administrador',-- Contraseña encriptada con SHA2_256                
    GETDATE()                            -- Fecha de creación actual
);

CREATE TABLE Proveedores (
    ID_Proveedor INT IDENTITY(1,1) PRIMARY KEY,             -- Identificador único del proveedor
    Nombre_Proveedor VARCHAR(100),            -- Nombre del proveedor
    Contacto VARCHAR(100),                   -- Nombre de la persona de contacto
    Telefono VARCHAR(15),                    -- Número de teléfono
    Correo_Electronico VARCHAR(100),         -- Correo electrónico
    Direccion VARCHAR(200),                  -- Dirección física del proveedor
    Productos_Servicios VARCHAR(200),        -- Productos o servicios que ofrece el proveedor
    Fecha_Registro DATETIME DEFAULT GETDATE(),                    -- Fecha en que se registró el proveedor
    Condiciones_Pago VARCHAR(100),           -- Condiciones de pago acordadas
    Estado VARCHAR(50)                       -- Estado del proveedor (Activo/Inactivo)
);

INSERT INTO Proveedores (Nombre_Proveedor, Contacto, Telefono, Correo_Electronico, Direccion, Productos_Servicios, Estado)
VALUES
('Televendores', 'Juan Pérez', '809-555-1234', 'juan@proveedora.com', 'Av. 27 de Febrero', 'Televisores de todo tipo', 'Activo'),
('AireAcomodados', 'María García', '809-555-5678', 'maria@proveedorb.com', 'Calle 123', 'Aire Acondicionados', 'Activo');
CREATE TABLE Productos (
    ID INT IDENTITY(1,1) PRIMARY KEY,         -- Identificador único
    Nombre NVARCHAR(100) NOT NULL,            -- Nombre del producto
    Marca NVARCHAR(100) NOT NULL,             -- Marca del producto
    Modelo NVARCHAR(100) NOT NULL,            -- Modelo del producto
    Descripcion NVARCHAR(MAX),                -- Descripción del producto
    Precio DECIMAL(10, 2) NOT NULL,           -- Precio del producto con 2 decimales
    Stock INT NOT NULL DEFAULT 0,             -- Cantidad disponible
    FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(), -- Fecha de creación
    Proveedor INT,                            -- ID_Proveedor como clave foránea
    CONSTRAINT FK_Productos_Proveedores       -- Clave foránea a la tabla Proveedores
    FOREIGN KEY (Proveedor) REFERENCES Proveedores(ID_Proveedor)
);



INSERT INTO Productos (Nombre, Marca, Modelo, Descripcion, Precio, Stock, Proveedor)
VALUES 
('Refrigerador', 'Samsung', 'RF28T5001SR', 'Refrigerador de 28 pies cúbicos con tecnología inverter', 1200.99, 5, 
    (SELECT ID_Proveedor FROM Proveedores WHERE Nombre_Proveedor = 'Televendores')),
    
('Microondas', 'LG', 'MS2535GIS', 'Microondas con grill y 25 litros de capacidad', 150.50, 15, 
    (SELECT ID_Proveedor FROM Proveedores WHERE Nombre_Proveedor = 'AireAcomodados')),

('Lavadora', 'Whirlpool', 'WFW5605MC', 'Lavadora automática de carga frontal', 850.75, 8, 
    (SELECT ID_Proveedor FROM Proveedores WHERE Nombre_Proveedor = 'Televendores')),

('Aspiradora', 'Dyson', 'V11 Absolute', 'Aspiradora inalámbrica con batería de larga duración', 450.00, 20, 
    (SELECT ID_Proveedor FROM Proveedores WHERE Nombre_Proveedor = 'AireAcomodados'));

	CREATE TABLE Clientes (
    ID_Cliente INT IDENTITY(1,1) PRIMARY KEY,         -- Identificador único del cliente
    Nombre NVARCHAR(100) NOT NULL,                   -- Nombre del cliente
    Apellido NVARCHAR(100) NOT NULL,                 -- Apellido del cliente
    Correo_Electronico NVARCHAR(100),                -- Correo electrónico del cliente
    Telefono NVARCHAR(15),                           -- Número de teléfono del cliente
    Direccion NVARCHAR(200),                         -- Dirección física del cliente
    Fecha_Registro DATETIME DEFAULT GETDATE(),       -- Fecha en que se registró el cliente
    Estado NVARCHAR(50) DEFAULT 'Activo'             -- Estado del cliente (Activo/Inactivo)
);
INSERT INTO Clientes (Nombre, Apellido, Correo_Electronico, Telefono, Direccion, Estado)
VALUES
('Carlos', 'Martínez', 'carlos.martinez@example.com', '809-555-1234', 'Calle Primera #45, Santo Domingo', 'Activo'),
('Ana', 'Gómez', 'ana.gomez@example.com', '809-555-5678', 'Av. Las Palmas, Santiago', 'Activo'),
('Luis', 'Pérez', 'luis.perez@example.com', '809-555-9012', 'Calle Central, La Vega', 'Inactivo'),
('María', 'Santos', 'maria.santos@example.com', '809-555-3456', 'Av. Duarte, San Cristóbal', 'Activo');

CREATE TABLE Ventas (
    ID_Venta INT PRIMARY KEY IDENTITY(1,1), -- Identificador único para la venta
    ID_Cliente INT NOT NULL,               -- Relación con la tabla de Clientes
    ID_Producto INT NOT NULL,              -- Relación con la tabla de Productos
    Cantidad INT NOT NULL,                 -- Cantidad de productos vendidos
    Precio_Unitario DECIMAL(10, 2) NOT NULL, -- Precio unitario del producto
    Total DECIMAL(10, 2) NOT NULL,         -- Total (Cantidad * Precio_Unitario)
    Fecha_Venta DATETIME NOT NULL DEFAULT GETDATE(), -- Fecha de la venta
    Estado NVARCHAR(50) NOT NULL DEFAULT 'Completada', -- Estado de la venta (e.g., Completada, Cancelada)
	ID_Pedido INT NOT NULL, 
    -- Claves foráneas
    CONSTRAINT FK_Ventas_Clientes FOREIGN KEY (ID_Cliente) REFERENCES Clientes(ID_Cliente),
    CONSTRAINT FK_Ventas_Productos FOREIGN KEY (ID_Producto) REFERENCES Productos(ID)
);
INSERT INTO Ventas (ID_Cliente, ID_Producto, Cantidad, Precio_Unitario, Total, ID_Pedido)
VALUES (1, 1, 2, 150.00, 300.00, 1);

SELECT ID_Pedido from Ventas
UPDATE Productos
SET Proveedor = 
    (SELECT ID_Proveedor 
     FROM Proveedores 
     WHERE Nombre_Proveedor = 'Televendores')
WHERE Nombre = 'Refrigerador' AND Marca = 'Samsung';

UPDATE Productos
SET Proveedor = 
    (SELECT ID_Proveedor 
     FROM Proveedores 
     WHERE Nombre_Proveedor = 'AireAcondicionados')
WHERE Nombre = 'Microondas' AND Marca = 'LG';

UPDATE Productos
SET Proveedor = 
    (SELECT ID_Proveedor 
     FROM Proveedores 
     WHERE Nombre_Proveedor = 'Televendores')
WHERE Nombre = 'Lavadora' AND Marca = 'Whirlpool';

UPDATE Productos
SET Proveedor = 
    (SELECT ID_Proveedor 
     FROM Proveedores 
     WHERE Nombre_Proveedor = 'AireAcondicionados')
WHERE Nombre = 'Aspiradora' AND Marca = 'Dyson';


DROP TABLE Usuarios;
DROP TABLE Proveedores;
DROP TABLE Productos;
DROP TABLE Clientes;
DROP TABLE Ventas;
select * from ventas;
SELECT COUNT(*) AS TotalUsuarios FROM Usuarios;
select * from Usuarios;
SELECT Id, NombreUsuario, FechaCreacion, Rol, Activo FROM Usuarios;
SELECT ID_Proveedor, Nombre_Proveedor, Contacto, Telefono, Correo_Electronico, Direccion, Productos_Servicios, Fecha_Registro, Estado FROM Proveedores;
SELECT ID, Nombre, Marca, Modelo, Descripcion, Precio, Stock, FechaCreacion, Proveedor FROM Productos;
SELECT Nombre_Proveedor FROM Proveedores WHERE ID_Proveedor = 1

SELECT Stock FROM Productos WHERE ID = 1;

UPDATE Productos
SET Nombre = @Nombre,
    Marca = @Marca,
    Modelo = @Modelo,
    Descripcion = @Descripcion,
    Precio = @Precio,
    Stock = @Stock,
    Proveedor = @Proveedor
WHERE ID = @ID;

SELECT p.ID, p.Nombre, p.Marca, p.Modelo, p.Descripcion, p.Precio, p.Stock, 
                            pr.Nombre_Proveedor AS Proveedor
                     FROM Productos p
                     INNER JOIN Proveedores pr ON 