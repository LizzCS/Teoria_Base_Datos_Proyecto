USE sistema_bancario;
GO

-- INSERTAR
CREATE OR ALTER PROCEDURE sp_insertar_subcategoria
	@id_categoria INT,
	@nombre_subcategoria VARCHAR(300),
	@descripcion VARCHAR(255),
	@es_sub_Defecto BIT,
	@creado_por INT,
	@modificado_por INT

	AS
	BEGIN

	INSERT INTO dbo.subcategoria (id_categoria, nombre, descripcion, subcategoria_por_defecto, creado_por, modificado_por, creado_en, modificado_en)
		VALUES (@id_categoria, @nombre_subcategoria, @descripcion, @es_sub_Defecto, @creado_por, @modificado_por, GETDATE(), GETDATE());
	END
GO

-- ACTUALIZAR
CREATE OR ALTER PROCEDURE  sp_actualizar_subcategoria
	@p_id_subcategoria INT, 
	@p_nombre VARCHAR (100), 
	@p_descripcion VARCHAR (255), 
	@p_modificado_por INT
	AS
	BEGIN

	IF NOT EXISTS (SELECT 1 FROM dbo.subcategoria sb WHERE sb.id_categoria = @p_id_subcategoria)
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
	@p_id_subcategoria INT
	AS
	BEGIN 	
	IF NOT EXISTS (SELECT 1 FROM dbo.subcategoria sb WHERE sb.id_categoria = @p_id_subcategoria)
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

-- CONSULTAR
CREATE OR ALTER PROCEDURE sp_consultar_subcategoria
	@p_id_subcategoria INT
	AS
	BEGIN 
		IF NOT EXISTS (SELECT 1 FROM dbo.subcategoria sb WHERE sb.id_subcategoria = @p_id_subcategoria)
		BEGIN
			RAISERROR('La subcategoría con ID %d no existe.', 16, 1, @p_id_subcategoria);
			RETURN;
		END
		SELECT id_subcategoria, id_categoria, nombre, descripcion, 
		  CASE es_activo 
			WHEN 1 THEN 'Activo' ELSE 'Inactivo' END AS estado
		, CASE subcategoria_por_defecto 
			WHEN 1 THEN 'Es subcategoria por defecto' ELSE 'NO es subcategoria por defecto' END AS estado
		, creado_por, modificado_por, creado_en, modificado_en, creado_en, modificado_en
		FROM subcategoria sb
		WHERE sb.id_subcategoria = @p_id_subcategoria;
	END
GO

-- LISTAR
CREATE OR ALTER PROCEDURE sp_listar_subcategorias_por_categoria
	@p_id_categoria INT
	AS
	BEGIN
		IF NOT EXISTS (SELECT 1 FROM dbo.categoria sb WHERE sb.id_categoria = @p_id_categoria)
		BEGIN
			RAISERROR('La categoria con ID %d no existe.', 16, 1, @p_id_categoria);
			RETURN;
		END
			SELECT c.id_categoria, c.nombre, sb.id_subcategoria, sb.nombre
			FROM categoria c
			INNER JOIN dbo.subcategoria sb 
				ON sb.id_categoria = c.id_categoria; 
	END
GO
