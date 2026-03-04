USE sistema_bancario;
GO

-- INSERTAR
CREATE PROCEDURE sp_insertar_categoria
	@p_nombre VARCHAR (100),
	@p_descripcion VARCHAR (255),
	@p_tipo_categoria VARCHAR (50),
	@p_creado_por INT
	AS
	BEGIN 
		INSERT INTO dbo.categoria (nombre, descripcion, tipo_categoria, creado_por, creado_en)
		VALUES (@p_nombre, @p_descripcion, @p_tipo_categoria, @p_creado_por, GETDATE());
END
GO

-- ACTUALIZAR
CREATE PROCEDURE sp_actualizar_categoria
	@p_id_categoria INT,
	@p_nombre VARCHAR (100),
	@p_descripcion VARCHAR (255),
	@p_modificado_por INT
	AS
	BEGIN
		IF NOT EXISTS (SELECT 1 FROM dbo.categoria c WHERE c.id_categoria = @p_id_categoria)
		BEGIN
			RAISERROR('La categoría con ID %d no existe.', 16, 1, @p_id_categoria);
			RETURN;
		END

		UPDATE dbo.categoria
		SET nombre = @p_nombre,
			descripcion = @p_descripcion,
			modificado_por = @p_modificado_por,
			modificado_en = GETDATE()
		WHERE id_categoria = @p_id_categoria;
	END
GO

-- ELIMINAR
CREATE PROCEDURE sp_eliminar_categoria
	@p_id_categoria INT
	AS
	BEGIN 	
		IF NOT EXISTS (SELECT 1 FROM dbo.categoria c WHERE c.id_categoria = @p_id_categoria)
		BEGIN
			RAISERROR('La categoría con ID %d no existe.', 16, 1, @p_id_categoria);
			RETURN;
		END
		DELETE FROM dbo.categoria
		WHERE id_categoria = @p_id_categoria;
	END
GO

-- CONSULTAR
CREATE PROCEDURE sp_consultar_categoria
	@p_id_categoria INT
	AS
	BEGIN
		IF NOT EXISTS (SELECT 1 FROM dbo.categoria c WHERE c.id_categoria = @p_id_categoria)
		BEGIN
			RAISERROR('La categoría con ID %d no existe.', 16, 1, @p_id_categoria);
			RETURN;
		END
		SELECT id_categoria, nombre, descripcion, tipo_categoria, creado_por, modificado_por, creado_en, modificado_en
		FROM dbo.categoria
		WHERE id_categoria = @p_id_categoria;
	END
GO

-- LISTAR
CREATE PROCEDURE sp_listar_categorias
	@p_id_usuario INT,
	@p_tipo_categoria VARCHAR (20)
	AS
	BEGIN
		IF NOT EXISTS (SELECT 1 FROM dbo.usuario WHERE usuario_id = @p_id_usuario)
		BEGIN
			RAISERROR('El usuario con ID %d no existe.', 16, 1, @p_id_usuario);
			RETURN;
		END
		SELECT c.tipo_categoria, p.usuario_id
		FROM categoria c
		INNER JOIN dbo.subcategoria sb 
			ON c.id_categoria = sb.id_categoria
		INNER JOIN dbo.presupuesto_detalle pd 
			ON sb.id_subcategoria = pd.id_subcategoria
		INNER JOIN dbo.presupuesto p
			ON pd.id_presupuesto = p.presupuesto_id
			WHERE p.usuario_id = @p_id_usuario
	END 
GO
