USE Sistema_bancario;
GO

CREATE PROCEDURE sp_insertar_subcategoria
    @p_id_categoria INT,
    @p_nombre VARCHAR(100),
    @p_descripcion VARCHAR(255),
    @p_es_activa BIT = 1,
    @p_es_sub_defecto BIT = 1
AS
BEGIN
    INSERT INTO subcategoria (id_categoria, nombre, descripcion, es_activa, es_sub_defecto)
    VALUES (@p_id_categoria, @p_nombre, @p_descripcion, @p_es_activa, @p_es_sub_defecto);
END
GO

CREATE PROCEDURE sp_actualizar_subcategoria
    @p_id_subcategoria INT,
    @p_nombre VARCHAR(100),
    @p_descripcion VARCHAR(255),
    @p_es_activa BIT,
    @p_es_sub_defecto BIT
AS
BEGIN
    UPDATE subcategoria
    SET nombre = @p_nombre,
        descripcion = @p_descripcion,
        es_activa = @p_es_activa,
        es_sub_defecto = @p_es_sub_defecto
    WHERE id_subcategoria = @p_id_subcategoria;
END
GO

CREATE PROCEDURE sp_eliminar_subcategoria
    @p_id_subcategoria INT
AS
BEGIN
    DELETE FROM subcategoria
    WHERE id_subcategoria = @p_id_subcategoria;
END
GO

CREATE PROCEDURE sp_consultar_subcategoria
    @p_id_subcategoria INT
AS
BEGIN
    SELECT *
    FROM subcategoria
    WHERE id_subcategoria = @p_id_subcategoria;
END
GO

CREATE PROCEDURE sp_listar_subcategorias
AS
BEGIN
    SELECT *
    FROM subcategoria
    ORDER BY nombre;
END
GO
