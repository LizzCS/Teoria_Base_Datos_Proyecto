USE sistema_bancario;
GO

-- INSERTAR
CREATE OR ALTER PROCEDURE sp_insertar_subcategoria
    @p_id_categoria int,
    @p_nombre varchar(300),
    @p_descripcion varchar(300),
    @p_es_defecto bit,
    @p_creado_por int
AS
BEGIN

    IF NOT EXISTS (SELECT 1 FROM dbo.categoria WHERE id_categoria = @p_id_categoria)
    BEGIN
        RAISERROR('La categoría con ID %d no existe.', 16, 1, @p_id_categoria);
        RETURN;
    END
 
    INSERT INTO dbo.subcategoria (id_categoria, nombre, descripcion, subcategoria_por_defecto,
        es_activo, creado_por, modificado_por, creado_en, modificado_en)

    VALUES (@p_id_categoria, @p_nombre, @p_descripcion, @p_es_defecto, 1, @p_creado_por,
        @p_creado_por, GETDATE(), GETDATE());
END
GO

-- ACTUALIZAR
CREATE OR ALTER PROCEDURE  sp_actualizar_subcategoria
	@p_id_subcategoria int, 
	@p_nombre varchar(300), 
	@p_descripcion varchar(300), 
	@p_modificado_por int
	AS
	BEGIN

	IF NOT EXISTS (SELECT 1 FROM dbo.subcategoria sb WHERE sb.id_subcategoria = @p_id_subcategoria)
		BEGIN
			RAISERROR('La subcategoría con ID %d no existe.', 16, 1, @p_id_subcategoria);
			RETURN;
		END

	UPDATE dbo.subcategoria
	SET nombre = @p_nombre,
		descripcion = @p_descripcion,
		modificado_por = @p_modificado_por,
		modificado_en = GETDATE()
	WHERE id_subcategoria = @p_id_subcategoria;
	END
GO

-- ELIMINAE
CREATE OR ALTER PROCEDURE sp_eliminar_subcategoria
	@p_id_subcategoria int
	AS
	BEGIN 	
	IF NOT EXISTS (SELECT 1 FROM dbo.subcategoria sb WHERE sb.id_subcategoria = @p_id_subcategoria)
		BEGIN
			RAISERROR('La subcategoría con ID %d no existe.', 16, 1, @p_id_subcategoria);
			RETURN;
		END
	IF EXISTS (SELECT 1 FROM dbo.presupuesto_detalle pd WHERE pd.id_subcategoria = @p_id_subcategoria)
		BEGIN
			RAISERROR('No se puede eliminar la subcategoría con ID %d porque está asociada a un presupuesto detalle.', 16, 1, @p_id_subcategoria);
			RETURN;
		END

	DELETE FROM dbo.subcategoria
	WHERE id_subcategoria = @p_id_subcategoria;
	END
GO


-- CONSULTAR SUBCATEGORIA
CREATE OR ALTER PROCEDURE sp_consultar_subcategoria
    @p_id_subcategoria int
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM dbo.subcategoria WHERE id_subcategoria = @p_id_subcategoria)
    BEGIN
        RAISERROR('La subcategoría con ID %d no existe.', 16, 1, @p_id_subcategoria);
        RETURN;
    END
 
    SELECT
        sb.id_subcategoria,
        sb.id_categoria,
        c.nombre AS nombre_categoria,
        c.tipo_categoria,
        sb.nombre AS nombre_subcategoria,
        sb.descripcion,
        CASE sb.es_activo
            WHEN 1 THEN 'Activo'
            ELSE 'Inactivo'
        END AS estado_activo,
        CASE sb.subcategoria_por_defecto
            WHEN 1 THEN 'Es subcategoría por defecto'
            ELSE 'No es subcategoría por defecto'
        END AS es_por_defecto,
        sb.creado_por,
        sb.modificado_por,
        sb.creado_en,
        sb.modificado_en
    FROM dbo.subcategoria sb
    INNER JOIN dbo.categoria c ON c.id_categoria = sb.id_categoria
    WHERE sb.id_subcategoria = @p_id_subcategoria;
END
GO

-- LISTAR
CREATE OR ALTER PROCEDURE sp_listar_subcategorias_por_categoria
	@p_id_categoria int
	AS
	BEGIN
		IF NOT EXISTS (SELECT 1 FROM dbo.categoria sb WHERE sb.id_categoria = @p_id_categoria)
		BEGIN
			RAISERROR('La categoria con ID %d no existe.', 16, 1, @p_id_categoria);
			RETURN;
		END
        SELECT 
            sb.id_subcategoria,
            sb.nombre,
            sb.descripcion,
            sb.subcategoria_por_defecto,
            sb.es_activo
        FROM subcategoria sb
        WHERE sb.id_categoria = @p_id_categoria
	END
GO

