USE proyecto_tbd;

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
  fecha_creacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
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
);
