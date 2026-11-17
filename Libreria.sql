DROP DATABASE Libreria;
CREATE DATABASE Libreria;
USE Libreria;

-- Tablas del negocio
CREATE TABLE Categoria(
    id_categoria INT PRIMARY KEY IDENTITY(1,1),
    nombreCategoria VARCHAR(80) NOT NULL CHECK(nombreCategoria NOT LIKE '%[^A-Za-záéíóúÁÉÍÓÚÑñ ]%'),
    descripcion VARCHAR(100),
    fecha_creacion DATE NOT NULL
);

CREATE TABLE Autor(
    id_autor INT PRIMARY KEY IDENTITY(1,1),  
    nombre   VARCHAR(50) NOT NULL CHECK(nombre NOT LIKE '%[^A-Za-záéíóúÁÉÍÓÚÑñ  ''- ]%'),
    apellido VARCHAR(80) NOT NULL CHECK(apellido NOT LIKE '%[^A-Za-záéíóúÁÉÍÓÚÑñ ''- ]%'),
    nacionalidad VARCHAR(50) CHECK(nacionalidad NOT LIKE '%[^A-Za-záéíóúÁÉÍÓÚÑñ ]%'),
    fecha_nacimiento DATE NOT NULL
);

CREATE TABLE Editorial(
    id_editorial INT PRIMARY KEY IDENTITY(1,1),
    nombre VARCHAR(100) NOT NULL,
    pais VARCHAR(50) NOT NULL CHECK(pais NOT LIKE '%[^A-Za-záéíóúÁÉÍÓÚÑñ ]%'),
    telefono VARCHAR(10) NOT NULL CHECK(telefono NOT LIKE '%[^0-9]%' AND LEN(telefono) = 10),
    correo VARCHAR(100) NOT NULL UNIQUE CHECK(correo LIKE '%_@_%._%') 
);

CREATE TABLE Proveedor(
    id_proveedor INT PRIMARY KEY IDENTITY(1,1),
    nombreProveedor VARCHAR(100) NOT NULL,
    telefono VARCHAR(10) NOT NULL CHECK(telefono NOT LIKE '%[^0-9]%' AND LEN(telefono) = 10),
    correo VARCHAR(100) NOT NULL UNIQUE CHECK(correo LIKE '%_@_%._%'),
    direccion VARCHAR(150) NOT NULL,
    estado BIT DEFAULT 1,
    fecha_registro DATE NOT NULL
);

CREATE TABLE Libro(
    id_libro INT PRIMARY KEY IDENTITY(1,1),
    titulo VARCHAR(100) NOT NULL,
    fecha_publicacion DATE,
    precio_compra DECIMAL(10,2) NOT NULL DEFAULT 0 CHECK(precio_compra >= 0),
    precio_venta DECIMAL(10,2) NOT NULL CHECK(precio_venta >= 0),
    descripcion VARCHAR(100),
    stock INT NOT NULL DEFAULT 0 CHECK(stock >= 0), 
    stock_minimo INT NOT NULL DEFAULT 10 CHECK(stock_minimo >= 0),
    idCategoria INT NOT NULL FOREIGN KEY REFERENCES Categoria(id_categoria),
    idAutor INT NOT NULL FOREIGN KEY REFERENCES Autor(id_autor),
    idEditorial INT NOT NULL FOREIGN KEY REFERENCES Editorial(id_editorial),
    idProveedor INT NOT NULL FOREIGN KEY REFERENCES Proveedor(id_proveedor),
    estado BIT DEFAULT 1,
    fecha_registro DATE NOT NULL
);

CREATE TABLE Empleado(
    id_empleado INT PRIMARY KEY IDENTITY(1,1),
    nombreEmpleado VARCHAR(100) NOT NULL CHECK(nombreEmpleado NOT LIKE '%[^A-Za-záéíóúÁÉÍÓÚÑñ ]%'),
    puesto VARCHAR(50) NOT NULL CHECK(puesto NOT LIKE '%[^A-Za-záéíóúÁÉÍÓÚÑñ ]%'),
    salario DECIMAL(10,2) NOT NULL CHECK(salario >= 0),
    fecha_ingreso DATE NOT NULL
);

CREATE TABLE Cliente(
    id_cliente INT PRIMARY KEY IDENTITY(1,1),
    documento VARCHAR(50) NOT NULL,
    nombreCliente VARCHAR(100) NOT NULL CHECK(nombreCliente NOT LIKE '%[^A-Za-záéíóúÁÉÍÓÚÑñ ]%'),
	fechaNacimiento DATE NOT NULL,
    correo VARCHAR(100) NOT NULL UNIQUE CHECK(correo LIKE '%_@_%._%'),
    telefono VARCHAR(10) NOT NULL CHECK (telefono NOT LIKE '%[^0-9]%' AND LEN(telefono) = 10),
    direccion VARCHAR(150) NOT NULL,
    estado BIT DEFAULT 1,
    fecha_registro DATE NOT NULL
);

-- Tablas de seguridad y usuarios
CREATE TABLE Rol(
    id_rol INT PRIMARY KEY IDENTITY(1,1),
    descripcion VARCHAR(50),
    fecha_registro DATETIME DEFAULT GETDATE() -- utiliza la fecha del sistema por DEFAULT
);

CREATE TABLE Permiso(
    id_permiso INT PRIMARY KEY IDENTITY(1,1),
    id_rol INT REFERENCES ROL(id_rol),
    nombre_menu VARCHAR(100),
    fecha_registro  DATETIME DEFAULT GETDATE()
);

CREATE TABLE Usuario(
    id_usuario INT PRIMARY KEY IDENTITY(1,1),
    idEmpleado INT FOREIGN KEY REFERENCES Empleado(id_empleado),
	idRol INT FOREIGN KEY REFERENCES Rol(id_rol), --> Saber si es Administrador, Cajero, Almacenero, etc., y ocultar/mostrar menús según el rol.
    documento VARCHAR(50),
    nombre_completo VARCHAR(100),
    correo VARCHAR(100) NOT NULL UNIQUE,
    clave VARCHAR(100) NOT NULL,
    estado BIT DEFAULT 1,
    fecha_registro  DATETIME DEFAULT GETDATE()
);

-- Tablas de compras y abastecimiento

CREATE TABLE Compra(
    id_compra INT PRIMARY KEY IDENTITY(1,1),
    idUsuario INT REFERENCES Usuario(id_usuario),
	idProveedor INT REFERENCES Proveedor(id_proveedor),
    tipo_documento VARCHAR(50),
    numero_documento VARCHAR(50),
    monto_total DECIMAL(10,2) DEFAULT 0,
    fecha_registro DATE NOT NULL DEFAULT GETDATE()
);

CREATE TABLE DetalleCompra(
    id_detalle_compra INT PRIMARY KEY IDENTITY(1,1),
    idCompra INT NOT NULL FOREIGN KEY REFERENCES Compra(id_compra), 
    idLibro INT NOT NULL FOREIGN KEY REFERENCES Libro(id_libro),
    precio_compra DECIMAL(10,2) DEFAULT 0,
    precio_venta DECIMAL(10,2) DEFAULT 0,
    cantidad INT NOT NULL CHECK(cantidad > 0),
    monto_total AS (cantidad * precio_compra) PERSISTED,
    fecha_registro DATE NOT NULL DEFAULT GETDATE()
);

CREATE TABLE Caja(
    id_caja INT PRIMARY KEY IDENTITY(1,1),
    idEmpleado INT NOT NULL FOREIGN KEY REFERENCES Empleado(id_empleado),
    fecha_apertura DATETIME NOT NULL DEFAULT GETDATE(),
    fecha_cierre DATETIME,
    monto_inicial DECIMAL(10,2) NOT NULL DEFAULT 0,
    monto_final DECIMAL(10,2),
    estado VARCHAR(20) NOT NULL DEFAULT 'Abierta' 
        CHECK(estado IN('Abierta','Cerrada'))
);

CREATE TABLE MovimientoCaja (
    id_movimiento INT PRIMARY KEY IDENTITY(1,1),
    id_caja INT FOREIGN KEY REFERENCES Caja(id_caja),
    tipo VARCHAR(20) CHECK(tipo IN ('Ingreso', 'Egreso')),
    monto DECIMAL(10,2) NOT NULL CHECK(monto > 0),
    descripcion VARCHAR(200),
    fecha_movimiento DATETIME DEFAULT GETDATE()
);

CREATE TABLE Pedido(
    id_caja INT NULL FOREIGN KEY REFERENCES Caja(id_caja), -- Para ver las ventas en cada turno
    id_pedido INT PRIMARY KEY IDENTITY(1,1),
    idCliente INT NOT NULL FOREIGN KEY REFERENCES Cliente(id_cliente),
    idUsuario INT NOT NULL FOREIGN KEY REFERENCES Usuario(id_usuario),
    tipo_documento VARCHAR(50),
    numero_documento VARCHAR(50),
    documento_cliente VARCHAR(50),
    nombre_cliente VARCHAR(100),
    monto_pago DECIMAL(10,2),
    monto_cambio DECIMAL(10,2),
    monto_total DECIMAL(10,2),
    metodo_pago VARCHAR(30) CHECK (metodo_pago IN ('Efectivo', 'Tarjeta', 'Transferencia', 'Yape/Plin')),
    fecha_pedido DATE NOT NULL DEFAULT GETDATE(),
    estado VARCHAR(20) NOT NULL DEFAULT 'Pendiente' 
        CHECK(estado IN('Pendiente', 'Pagado', 'Enviado', 'Cancelado'))
);

CREATE TABLE DetallePedido(
    id_detalle INT PRIMARY KEY IDENTITY(1,1),
    idPedido INT NOT NULL FOREIGN KEY REFERENCES Pedido(id_pedido),
    idLibro INT NOT NULL FOREIGN KEY REFERENCES Libro(id_libro),
    cantidad INT NOT NULL CHECK(cantidad > 0),
    precio_unitario DECIMAL(10,2) NOT NULL CHECK(precio_unitario >= 0),
    descuento_aplicado DECIMAL(10,2) DEFAULT 0 CHECK(descuento_aplicado >= 0),
    subtotal AS (cantidad * precio_unitario - descuento_aplicado) PERSISTED,
    fecha_registro DATE NOT NULL DEFAULT GETDATE()
);

-- Tablas de operaciones adicionales y negocio

CREATE TABLE Negocio (
    id_negocio INT PRIMARY KEY IDENTITY(1,1),
    nombre VARCHAR(100),
    documento VARCHAR(50), -- RUC / RFC
    direccion VARCHAR(150),
    logo VARBINARY(MAX) -- Para guardar la imagen del logo
);

CREATE TABLE Resena(
    id_resena INT PRIMARY KEY IDENTITY(1,1),
    idCliente INT NOT NULL FOREIGN KEY REFERENCES Cliente(id_cliente),
    idLibro INT NOT NULL FOREIGN KEY REFERENCES Libro(id_libro),
    calificacion INT NOT NULL CHECK(calificacion BETWEEN 1 AND 5),
    comentario VARCHAR(200),
    fecha_resena DATE NOT NULL DEFAULT GETDATE()
);

CREATE TABLE Promocion(
    id_promocion INT PRIMARY KEY IDENTITY(1,1),
    idLibro INT FOREIGN KEY REFERENCES Libro(id_libro),
    idCategoria INT FOREIGN KEY REFERENCES Categoria(id_categoria),
    descripcion VARCHAR(150) NOT NULL,
    descuento DECIMAL(5, 2) NOT NULL CHECK(descuento > 0 AND descuento <= 100),
    fecha_inicio DATE NOT NULL,
    fecha_fin DATE NOT NULL, 
    activo BIT NOT NULL DEFAULT 0,
    CHECK (
        (idLibro IS NOT NULL AND idCategoria IS NULL)
    OR
        (idLibro IS NULL AND idCategoria IS NOT NULL) 
    ),
    CHECK (fecha_fin >= fecha_inicio) -- CHECK a nivel de tabla aquí
);

--> Roles de los empledos----------------------------------------------------------------------------------------------------------------------
INSERT INTO Rol(descripcion) VALUES('Administrador'), ('Cajero'), ('Almacenero');

INSERT INTO Empleado(nombreEmpleado , puesto, salario, fecha_ingreso) VALUES
('Carlos Ramirez', 'Gerente General', 8500, '2023-01-15'),
('Angelica Hernandez', 'Cajera', 1500, '2024-02-02'),
('Nolan Grayson', 'Almacenero', 1000, '2024-01-01');

INSERT INTO Usuario(idEmpleado, idRol, documento, nombre_completo, correo, clave, estado) VALUES
(1, 1, '12345678', 'Carlos Ramirez', 'admin@libreria.com', '12345678', 1),
(2, 2, '52653244', 'Angelica Hernandez', 'cajita@libreria.com', '52653244', 1),
(3, 3, '72738738', 'Nolan Grayson', 'almacen@libreria.com', '72738738', 1);

SELECT -- > Revision
u.id_usuario,
u.nombre_completo,
u.correo,
u.clave,
r.descripcion AS rol
FROM Usuario u
LEFT JOIN Rol r ON u.idRol = r.id_rol;

--> Información de libros -------------------------------------------------------------------------------------------------------------------------
INSERT INTO Categoria (nombreCategoria, descripcion, fecha_creacion) VALUES
('Novela',     'Libros de ficción narrativa', GETDATE()),
('Ciencia',    'Divulgación científica',      GETDATE()),
('Tecnología', 'Programación y sistemas',     GETDATE()),
('Poesía', 'Verso, ritmo y emoción',          GETDATE()),
('Teatro', 'Obras para ser representadas',    GETDATE()),
('Historia', 'Estudio del pasado humano',     GETDATE()),
('Fantacía', 'Mundos imaginarios y futuros',     GETDATE()),
('Infantil',   'Libros para niños',           GETDATE());

INSERT INTO Autor (nombre, apellido, nacionalidad, fecha_nacimiento) VALUES
('Gabriel', 'Garcia Marquez', 'Colombiana',     '1927-03-06'),
('Isaac',   'Asimov',         'Rusa',           '1920-01-02'),
('Robert',  'Martin',         'Estadounidense', '1952-12-05'),
('Antoine', 'Saint Exupery',  'Francesa',       '1900-06-29');

INSERT INTO Editorial (nombre, pais, telefono, correo) VALUES
('Sudamericana',  'Argentina',      '1145678901', 'contacto@sudamericana.com'),
('Debolsillo',    'Espana',         '3491234567', 'info@debolsillo.es'),
('Prentice Hall', 'Estados Unidos', '1234567890', 'ventas@prentice.com'),
('Salamandra',    'Espana',         '3498765432', 'hola@salamandra.es');

INSERT INTO Proveedor (nombreProveedor, telefono, correo, direccion, estado, fecha_registro) VALUES
('Distribuidora Central', '5551234567', 'ventas@distcentral.com', 'Av. Reforma 123',   1, GETDATE()),
('Libros del Mundo',      '5559876543', 'contacto@librosmundo.com', 'Calle 5 # 45-67', 1, GETDATE()); --> Listo xdxd

--> insertar libros
INSERT INTO Libro (titulo, fecha_publicacion, precio_compra, precio_venta, descripcion, stock, idCategoria, idAutor, idEditorial, idProveedor, estado, fecha_registro) VALUES
('Cien Anos de Soledad',              '1967-05-30', 150.00, 250.00, 'Realismo magico',  15, 1, 1, 1, 1, 1, GETDATE()),
('El Amor en los Tiempos del Colera', '1985-01-01', 132.00, 220.00, 'Novela romantica', 10, 1, 1, 1, 1, 1, GETDATE()),
('Fundacion',                         '1951-06-01', 108.00, 180.00, 'Ciencia ficcion',  20, 2, 2, 2, 1, 1, GETDATE()),
('Yo, Robot',                         '1950-12-02', 120.00, 200.00, 'Robots e IA',      12, 2, 2, 2, 1, 1, GETDATE()),
('Clean Code',                        '2008-08-01', 270.00, 450.00, 'Buenas practicas',  8, 3, 3, 3, 2, 1, GETDATE()),
('El Principito',                     '1943-04-06',  90.00, 150.00, 'Clasico infantil', 25, 4, 4, 4, 2, 1, GETDATE());


-- Trigers ------------------------------------------------------------------------------------------------------------------------------------------
-- 1. TRIGGER PARA VENTAS (Descuenta el stock automáticamente)
CREATE TRIGGER TR_DescontarStock_Venta
ON DetallePedido
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE l
    SET l.stock = l.stock - i.cantidad
    FROM Libro l
    INNER JOIN inserted i ON l.id_libro = i.idLibro;
END;

-- 2. TRIGGER PARA COMPRAS (Aumenta el stock y actualiza precios automáticamente)
CREATE TRIGGER TR_ActualizarStockCompra
ON DetalleCompra
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE l
    SET l.stock = l.stock + i.cantidad,
        l.precio_compra = i.precio_compra,
        l.precio_venta = i.precio_venta
    FROM Libro l
    INNER JOIN inserted i ON l.id_libro = i.idLibro;
END;


-- Verificaciones -----------------------------------------------------
SELECT '=== ROLES ===' AS Info;
SELECT id_rol, descripcion FROM Rol;

SELECT '=== USUARIOS ===' AS Info;
SELECT u.id_usuario, u.nombre_completo, u.correo, r.descripcion AS rol
FROM Usuario u
LEFT JOIN Rol r ON u.idRol = r.id_rol;

SELECT '=== LIBROS ===' AS Info;
SELECT id_libro, titulo, precio_compra, precio_venta, stock, stock_minimo FROM Libro;

SELECT '=== ESTRUCTURA DE DetalleCompra ===' AS Info;
SELECT COLUMN_NAME, DATA_TYPE 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'DetalleCompra';


select * From Caja;
select * From Empleado;
select * From DetallePedido;
select * From Libro;