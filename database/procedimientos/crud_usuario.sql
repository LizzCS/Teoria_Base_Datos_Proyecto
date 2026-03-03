USE Sistema_bancario;
GO
-- INSERTAR
CREATE PROCEDURE dbo.sp_insertar_usuario
	@nombre VARCHAR(100),
	@apellido VARCHAR(100),
	@correo_electronico VARCHAR(255),
	@contrasena VARCHAR(255),
	@salario_mensual DECIMAL(10, 2),
	@creado_por INT
AS
BEGIN
	INSERT INTO usuario (nombre, apellido, correo_electronico, contrasena, fecha_registro, salario_mensual, estado_usuario, creado_por,creado_en)
	VALUES (@nombre, @apellido, @correo_electronico, @contrasena, GETDATE(), @salario_mensual, 1, @creado_por,GETDATE());
END
GO
    
-- ACTUALIZAR
CREATE PROCEDURE sp_actualizar_usuario
    @p_id_usuario INT,
    @p_nombre VARCHAR(100),
    @p_apellido VARCHAR(100),
    @p_salario_mensual DECIMAL(10,2),
    @p_modificado_por INT
AS
BEGIN
    UPDATE usuario
    SET nombre = @p_nombre,
        apellido = @p_apellido,
        salario_mensual = @p_salario_mensual,
        modificado_por = @p_modificado_por,
        modificado_en = GETDATE()
    WHERE usuario_id = @p_id_usuario;
END
GO

-- ELIMINAR
CREATE PROCEDURE sp_eliminar_usuario
    @p_id_usuario INT,
    @p_modificado_por INT
AS
BEGIN
    UPDATE usuario
    SET estado_usuario = 0,
        modificado_por = @p_modificado_por,
        modificado_en = GETDATE()
    WHERE usuario_id = @p_id_usuario;
END
GO

-- MOSTRAR
CREATE PROCEDURE sp_consultar_usuario
    @p_id_usuario INT
AS
BEGIN
    SELECT usuario_id, nombre, apellido, correo_electronico, contrasena, fecha_registro, salario_mensual, estado_usuario, creado_por, modificado_por ,
        creado_en, modificado_en
    FROM usuario
    WHERE usuario_id = @p_id_usuario;
END
GO

-- LISTAR
CREATE PROCEDURE sp_listar_usuarios
AS
BEGIN
    SELECT usuario_id, nombre, apellido, correo_electronico, salario_mensual,
           CASE estado_usuario WHEN 1 THEN 'Activo' ELSE 'Inactivo' END AS estado,
           fecha_registro
    FROM usuario
    ORDER BY nombre, apellido;
END
GO
