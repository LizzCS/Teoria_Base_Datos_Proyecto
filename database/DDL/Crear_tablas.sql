USE sistema_bancario;
GO

CREATE TABLE [usuario] (
  [id] int identity(1,1) primary key,
  [nombre] varchar(300),
  [apellido] varchar(300),
  [correo_electronico] varchar(300),
  [contrasenia] varchar(300),
  [fecha_registro] datetime,
  [salario_mensual_base] decimal(12,2),
  [estado_usuario] bit,
  [creado_por] int,
  [modificado_por] int,
  [creado_en] datetime,
  [modificado_en] datetime
)
GO

CREATE TABLE [presupuesto] (
  [id] int identity(1,1) primary key,
  [usuario_propietario] int,
  [nombre_descriptivo] varchar(300),
  [descripcion] varchar(300),
  [anio_inicio] smallint,
  [mes_inicio] tinyint,
  [anio_fin] smallint,
  [mes_fin] tinyint,
  [total_ingreso] decimal(12,2),
  [total_gasto] decimal(12,2),
  [total_ahorro] decimal(12,2),
  [fecha_hora_creacion] datetime,
  [estado_presupuesto] bit,
  [creado_por] int,
  [modificado_por] int,
  [creado_en] datetime,
  [modificado_en] datetime
)
GO

CREATE TABLE [categoria] (
  [id_categoria] int identity(1,1) primary key,
  [nombre] varchar(300),
  [descripcion] varchar(600),
  [tipo_categoria] varchar(300),
  [creado_por] int,
  [modificado_por] int,
  [creado_en] datetime,
  [modificado_en] datetime
)
GO

CREATE TABLE [subcategoria] (
  [id_subcategoria] int identity(1,1) primary key,
  [id_categoria] int,
  [nombre] varchar(300),
  [descripcion] varchar(600),
  [es_activo] bit,
  [subcategoria_por_defecto] bit,
  [creado_por] int,
  [modificado_por] int,
  [creado_en] datetime,
  [modificado_en] datetime
)
GO

CREATE TABLE [presupuesto_detalle] (
  [id] int identity(1,1) primary key,
  [id_presupuesto] int,
  [id_subcategoria] int,
  [monto_mensual] decimal(12,2),
  [observacion_monto] varchar(300),
  [creado_por] int,
  [modificado_por] int,
  [creado_en] datetime,
  [modificado_en] datetime
)
GO

CREATE TABLE [obligacion_fija] (
  [id] int identity(1,1) primary key,
  [id_subcategoria] int,
  [nombre] varchar(300),
  [descripcion] varchar(400),
  [monto_fijo_mensual] decimal(12,2),
  [dia_del_mes] tinyint,
  [esta_vigente] bit,
  [fecha_inicio] date,
  [fecha_finalizacion] date,
  [creado_por] int,
  [modificado_por] int,
  [creado_en] datetime,
  [modificado_en] datetime
)
GO

CREATE TABLE [transaccion] (
  [id_transaccion] int identity(1,1) primary key,
  [id_presupuesto_detalle] int,
  [anio_transaccion] smallint,
  [mes_transaccion] tinyint,
  [id_obligacion] varchar(30),
  [tipo_transaccion] varchar(300),
  [numero_factura] int,
  [fecha_hora_registro] datetime,
  [creado_por] int,
  [modificado_por] int,
  [creado_en] datetime,
  [modificado_en] datetime
)
GO

CREATE TABLE [obligacion_transaccion] (
  [id_transaccion] int primary key,
  [id_obligacion] int
)
GO

ALTER TABLE [subcategoria] ADD FOREIGN KEY ([id_categoria]) REFERENCES [categoria] ([id_categoria])
GO

ALTER TABLE [presupuesto_detalle] ADD FOREIGN KEY ([id_presupuesto]) REFERENCES [presupuesto] ([id])
GO

ALTER TABLE [presupuesto_detalle] ADD FOREIGN KEY ([id_subcategoria]) REFERENCES [subcategoria] ([id_subcategoria])
GO

ALTER TABLE [presupuesto] ADD FOREIGN KEY ([usuario_propietario]) REFERENCES [usuario] ([id])
GO

ALTER TABLE [obligacion_transaccion] ADD FOREIGN KEY ([id_obligacion]) REFERENCES [obligacion_fija] ([id])
GO

ALTER TABLE [obligacion_transaccion] ADD FOREIGN KEY ([id_transaccion]) REFERENCES [transaccion] ([id_transaccion])
GO

ALTER TABLE [transaccion] ADD FOREIGN KEY ([id_presupuesto_detalle]) REFERENCES [presupuesto_detalle] ([id])
GO

ALTER TABLE [obligacion_fija] ADD FOREIGN KEY ([id_subcategoria]) REFERENCES [subcategoria] ([id_subcategoria])
GO  
