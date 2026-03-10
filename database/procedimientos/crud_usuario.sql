USE Sistema_bancario;
GO
-- INSERTAR
CREATE OR ALTER PROCEDURE sp_insertar_usuario
	@nombre VARCHAR(300),
	@apellido VARCHAR(300),
	@correo_electronico VARCHAR(300),
	@contrasena VARCHAR(300),
	@salario_mensual DECIMAL(12, 2),
	@creado_por INT
AS
BEGIN
    IF EXISTS (SELECT 1 FROM usuario WHERE correo_electronico = @correo_electronico)
    BEGIN
        RAISERROR('El correo electrónico ya está registrado.', 16, 1);
        RETURN;
    END
    IF (@salario_mensual < 0)
    BEGIN
        RAISERROR('El salario mensual no puede ser negativo.', 16, 1);
        RETURN;
    END
	INSERT INTO usuario (nombre, apellido, correo_electronico, contrasenia, fecha_registro, salario_mensual_base, estado_usuario, creado_por,creado_en)
	VALUES (@nombre, @apellido, @correo_electronico, @contrasena, GETDATE(), @salario_mensual, 1, @creado_por,GETDATE());
END
GO
    
-- ACTUALIZAR
CREATE OR ALTER PROCEDURE sp_actualizar_usuario
    @p_id_usuario INT,
    @p_nombre VARCHAR(300),
    @p_apellido VARCHAR(300),
    @p_salario_mensual DECIMAL(12,2),
    @p_modificado_por INT
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM usuario WHERE usuario_id = @p_id_usuario)
    BEGIN
        RAISERROR('El usuario no existe.', 16, 1);
        RETURN;
    END

    IF EXISTS (SELECT 1 FROM usuario WHERE usuario_id = @p_id_usuario AND estado_usuario = 0)
    BEGIN
        RAISERROR('No se puede actualizar un usuario inactivo.', 16, 1);
        RETURN;
    END
    UPDATE usuario
    SET nombre = @p_nombre,
        apellido = @p_apellido,
        salario_mensual_base = @p_salario_mensual,
        modificado_por = @p_modificado_por,
        modificado_en = GETDATE()
    WHERE usuario_id = @p_id_usuario;
END
GO

-- ELIMINAR
CREATE OR ALTER PROCEDURE sp_eliminar_usuario
    @p_id_usuario INT,
    @p_modificado_por INT
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM usuario WHERE usuario_id = @p_id_usuario)
    BEGIN
        RAISERROR('El usuario no existe.', 16, 1);
        RETURN;
    END
    UPDATE usuario
    SET estado_usuario = 0,
        modificado_por = @p_modificado_por,
        modificado_en = GETDATE()
    WHERE usuario_id = @p_id_usuario;
END
GO

-- MOSTRAR
CREATE OR ALTER PROCEDURE sp_consultar_usuario
    @p_id_usuario INT
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM usuario WHERE usuario_id = @p_id_usuario)
    BEGIN
        RAISERROR('El usuario no existe.', 16, 1);
        RETURN;
    END
    SELECT usuario_id, nombre, apellido, correo_electronico, contrasenia, fecha_registro, salario_mensual_base, estado_usuario, creado_por, modificado_por ,
        creado_en, modificado_en
    FROM usuario
    WHERE usuario_id = @p_id_usuario;
END
GO

-- LISTAR
CREATE OR ALTER PROCEDURE sp_listar_usuarios
AS
BEGIN
    SELECT usuario_id, nombre, apellido, correo_electronico, salario_mensual_base,
           CASE estado_usuario WHEN 1 THEN 'Activo' ELSE 'Inactivo' END AS estado,
           fecha_registro
    FROM usuario
    ORDER BY nombre, apellido;
END
GO