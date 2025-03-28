CREATE TABLE Clientes (
    id INT IDENTITY(1,1) PRIMARY KEY,
    nombre NVARCHAR(100) NOT NULL,
    correo NVARCHAR(100) UNIQUE NOT NULL,
    telefono NVARCHAR(15) NULL
);

CREATE TABLE Ventas (
    id INT IDENTITY(1,1) PRIMARY KEY,
    id_cliente INT NOT NULL,
    servicio NVARCHAR(100) NOT NULL,
    detalle NVARCHAR(255) NULL,
    monto DECIMAL(10,2) NOT NULL,
    fecha DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (id_cliente) REFERENCES Clientes(id)
);

CREATE TABLE Proveedores (
    id INT IDENTITY(1,1) PRIMARY KEY,
    nombre NVARCHAR(100) NOT NULL,
    contacto NVARCHAR(100) NOT NULL,
    correo NVARCHAR(100) UNIQUE NOT NULL,
    numero NVARCHAR(15) NOT NULL
);

CREATE TABLE Productos (
    id INT IDENTITY(1,1) PRIMARY KEY,
    nombre NVARCHAR(100) NOT NULL,
    stock INT NOT NULL,
    id_proveedor INT NOT NULL,
    categoria NVARCHAR(100) NOT NULL,
    FOREIGN KEY (id_proveedor) REFERENCES Proveedores(id)
);
CREATE TABLE Profesionales (
    id INT IDENTITY(1,1) PRIMARY KEY,
    nombre NVARCHAR(100) NOT NULL,
    especialidad NVARCHAR(100) NOT NULL,
    correo NVARCHAR(100) UNIQUE NOT NULL,
    numero NVARCHAR(15) NOT NULL
);

CREATE TABLE Citas (
    id INT IDENTITY(1,1) PRIMARY KEY,
    id_cliente INT NOT NULL,
    fecha DATE NOT NULL,
    hora TIME NOT NULL,
    servicio NVARCHAR(100) NOT NULL,
    id_profesional INT NOT NULL,
    FOREIGN KEY (id_cliente) REFERENCES Clientes(id),
    FOREIGN KEY (id_profesional) REFERENCES Profesionales(id)
);

CREATE TABLE Recepcionistas(
    id INT IDENTITY(1,1) PRIMARY KEY,
    nombre NVARCHAR(100) NOT NULL,
    correo NVARCHAR(100) UNIQUE NOT NULL,
    numero NVARCHAR(15) NOT NULL
);