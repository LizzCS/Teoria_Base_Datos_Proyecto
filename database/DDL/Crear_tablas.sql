USE proyecto_tbd;

-- Crear tablas

CREATE TABLE usuario (
  usuario_id INT PRIMARY KEY IDENTITY (1,1),
  nombre VARCHAR(100) NOT NULL,
  apellido VARCHAR(100) NOT NULL,
  correo_electronico VARCHAR(100) NOT NULL UNIQUE,
  contrasena VARCHAR(255) NOT NULL,
  fecha_registro DATETIME NOT NULL DEFAULT GETDATE(),
  salario_mensual DECIMAL(10, 2) NOT NULL CHECK (salario_mensual >= 0),
  estado_usuario  BIT NOT NULL DEFAULT 1
  );

CREATE TABLE presupuesto (
  presupuesto_id INT PRIMARY KEY IDENTITY (1,1),
  usuario_id INT NOT NULL,
  FOREIGN KEY (usuario_id) REFERENCES usuario(usuario_id),
  nombre_descriptivo VARCHAR(255) NOT NULL,
  anio_inicio INT NOT NULL,
  mes_inicio INT NOT NULL CHECK (mes_inicio BETWEEN 1 AND 12),
  anio_fin INT NOT NULL,
  mes_fin INT NOT NULL CHECK (mes_fin BETWEEN 1 AND 12),
  total_ingresos DECIMAL(15, 2) NOT NULL,
  total_gastos DECIMAL(15, 2) NOT NULL,
  total_ahorros DECIMAL(15, 2) NOT NULL,
  fecha_creacion DATETIME NOT NULL DEFAULT GETDATE(),
  estado_presupuesto VARCHAR(20) NOT NULL CHECK (estado_presupuesto IN ('activo', 'cerrado', 'borrador')),
  CHECK (
        anio_fin > anio_inicio OR
        (anio_fin = anio_inicio AND mes_fin >= mes_inicio)
    )
  );

CREATE TABLE categoria (
  id_categoria INT PRIMARY KEY IDENTITY (1,1),
  nombre VARCHAR(100) NOT NULL,
  descripcion VARCHAR(255) NULL,
  tipo_categoria VARCHAR(20) NOT NULL CHECK (tipo_categoria IN ('ingreso', 'gasto', 'ahorro'))
  );

CREATE TABLE subcategoria(
  id_subcategoria INT PRIMARY KEY IDENTITY (1,1),
  id_categoria INT NOT NULL,
  FOREIGN KEY (id_categoria) REFERENCES categoria(id_categoria),
  nombre VARCHAR(100) NOT NULL,
  descripcion VARCHAR(255) NULL,
  es_activa BIT NOT NULL DEFAULT 1,
  es_sub_defecto BIT NOT NULL DEFAULT 1
);

CREATE TABLE presupuesto_detalle (
  id_detalle INT PRIMARY KEY IDENTITY (1,1),
  id_presupuesto INT NOT NULL,
  FOREIGN KEY (id_presupuesto) REFERENCES presupuesto(presupuesto_id),
  id_subcategoria INT NOT NULL,
  FOREIGN KEY (id_subcategoria) REFERENCES subcategoria(id_subcategoria),
  monto_mensual NUMERIC(12,2) NOT NULL CHECK (monto_mensual >= 0),
  observaciones VARCHAR(255) NULL
);

CREATE TABLE obligacion_fija (
    id_obligacion INT PRIMARY KEY IDENTITY (1,1),
    id_subcategoria INT NOT NULL,
    FOREIGN KEY (id_subcategoria) REFERENCES subcategoria(id_subcategoria),
    nombre VARCHAR(100) NOT NULL,
    monto DECIMAL(12, 2) NOT NULL CHECK (monto >= 0),
    fecha_inicio DATE NOT NULL,
    fecha_final DATE NULL,
    dia_mes INT NOT NULL CHECK (dia_mes BETWEEN 1 AND 28),
    es_vigente BIT NOT NULL DEFAULT 1,
    CONSTRAINT chk_fecha_final CHECK (fecha_final IS NULL OR fecha_final > fecha_inicio)
);

CREATE TABLE transaccion (
    id_transaccion INT PRIMARY KEY IDENTITY (1,1),
    id_subcategoria INT,
    id_obligacion_fija INT,
    id_presupuesto_detalle INT NOT NULL,
    FOREIGN KEY (id_presupuesto_detalle) REFERENCES presupuesto_detalle(id_detalle),
    monto DECIMAL(12, 2) NOT NULL CHECK (monto >= 0),
    metodo_pago VARCHAR(50) NOT NULL CHECK (metodo_pago IN ('efectivo', 'tarjeta debito', 'tarjeta credito', 'transferencia')),
    fecha DATETIME NOT NULL DEFAULT GETDATE(),
    descripcion VARCHAR(255) NULL,
    anio INT NOT NULL,
    mes INT NOT NULL CHECK (mes BETWEEN 1 AND 12)
);

CREATE TABLE obligacion_transaccion(
    id_transaccion INT PRIMARY KEY,
    id_obligacion INT NOT NULL,
    FOREIGN KEY (id_transaccion) REFERENCES transaccion(id_transaccion),
    FOREIGN KEY (id_obligacion) REFERENCES obligacion_fija(id_obligacion)
);
