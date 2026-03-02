USE Sistema_bancario;
GO

CREATE PROCEDURE sp_insertar_usuario
    @p_nombre VARCHAR(100),
    @p_apellido VARCHAR(100),
    @p_email VARCHAR(100),
    @p_contrasena VARCHAR(255),
    @p_salario_mensual DECIMAL(10,2)
AS
BEGIN
    INSERT INTO usuario (nombre, apellido, correo_electronico, contrasena, salario_mensual, estado_usuario, fecha_registro)
    VALUES (@p_nombre, @p_apellido, @p_email, @p_contrasena, @p_salario_mensual, 1, GETDATE());
END
GO

CREATE PROCEDURE sp_actualizar_usuario
    @p_id_usuario INT,
    @p_nombre VARCHAR(100),
    @p_apellido VARCHAR(100),
    @p_salario_mensual DECIMAL(10,2)
AS
BEGIN
    UPDATE usuario
    SET nombre = @p_nombre,
        apellido = @p_apellido,
        salario_mensual = @p_salario_mensual
    WHERE usuario_id = @p_id_usuario;
END
GO

CREATE PROCEDURE sp_eliminar_usuario
    @p_id_usuario INT
AS
BEGIN
    UPDATE usuario
    SET estado_usuario = 0
    WHERE usuario_id = @p_id_usuario;
END
GO

CREATE PROCEDURE sp_consultar_usuario
    @p_id_usuario INT
AS
BEGIN
    SELECT usuario_id, nombre, apellido, correo_electronico, contrasena, salario_mensual, estado_usuario, fecha_registro
    FROM usuario
    WHERE usuario_id = @p_id_usuario;
END
GO

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
