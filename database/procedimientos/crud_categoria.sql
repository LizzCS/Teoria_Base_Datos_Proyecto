USE proyecto_tbd;
GO

CREATE PROCEDURE sp_insertar_categoria
    @p_nombre VARCHAR(100),
    @p_descripcion VARCHAR(255),
    @p_tipo_categoria VARCHAR(20),
    @p_id_usuario INT,
    @p_creado_por INT
AS
BEGIN
    INSERT INTO categoria (nombre, descripcion, tipo_categoria)
    VALUES (@p_nombre, @p_descripcion, @p_tipo_categoria);

END
GO

CREATE PROCEDURE sp_actualizar_categoria
    @p_id_categoria INT,
    @p_nombre VARCHAR(100),
    @p_descripcion VARCHAR(255),
    @p_modificado_por INT
AS
BEGIN
    UPDATE categoria
    SET nombre = @p_nombre,
        descripcion = @p_descripcion
    WHERE id_categoria = @p_id_categoria;
END
GO

CREATE PROCEDURE sp_eliminar_categoria
    @p_id_categoria INT
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM subcategoria WHERE id_categoria = @p_id_categoria AND es_activa = 1 AND es_sub_defecto = 0)
    BEGIN
        DELETE FROM categoria WHERE id_categoria = @p_id_categoria;
    END
    ELSE
    BEGIN
        RAISERROR('No se puede eliminar: existen subcategorías activas.',16,1);
    END
END
GO

CREATE PROCEDURE sp_consultar_categoria
    @p_id_categoria INT
AS
BEGIN
    SELECT *
    FROM categoria
    WHERE id_categoria = @p_id_categoria;
END
GO

CREATE PROCEDURE sp_listar_categorias
    @p_id_usuario INT,
    @p_tipo VARCHAR(20) = NULL
AS
BEGIN
    SELECT *
    FROM categoria
    WHERE (@p_tipo IS NULL OR tipo_categoria = @p_tipo);
END
GO
