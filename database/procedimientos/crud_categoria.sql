USE sistema_bancario;
GO

-- INSERTAR
CREATE OR ALTER PROCEDURE sp_insertar_categoria
	@p_nombre varchar (300),
	@p_descripcion varchar (300),
	@p_tipo_categoria varchar (300),
	@p_creado_por int
	AS
	BEGIN
    
    IF @p_tipo_categoria NOT IN ('ahorro','gasto','ingreso')
    BEGIN
        RAISERROR('Tipo de categoria incorrecto.',16,1)
        RETURN;
    END
		INSERT INTO dbo.categoria (nombre, descripcion, tipo_categoria, creado_por, creado_en)
		VALUES (@p_nombre, @p_descripcion, @p_tipo_categoria, @p_creado_por, GETDATE());
    END
GO

-- ACTUALIZAR
CREATE OR ALTER PROCEDURE sp_actualizar_categoria
    @p_id_categoria int,
    @p_nombre varchar(300),
    @p_descripcion varchar(300),
    @p_modificado_por int
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM dbo.categoria WHERE id_categoria = @p_id_categoria)
    BEGIN
        RAISERROR('La categoria con ID %d no existe.', 16, 1, @p_id_categoria)
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
CREATE OR ALTER PROCEDURE sp_eliminar_categoria
    @p_id_categoria int
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM dbo.categoria WHERE id_categoria = @p_id_categoria)
    BEGIN
        RAISERROR('La categoría con ID %d no existe.', 16, 1, @p_id_categoria);
        RETURN;
    END
 
    IF EXISTS (
        SELECT 1
        FROM dbo.subcategoria sb
        INNER JOIN dbo.presupuesto_detalle pd ON pd.id_subcategoria = sb.id_subcategoria
        WHERE sb.id_categoria = @p_id_categoria
    )
    BEGIN
        RAISERROR('No se puede eliminar la categoría con ID %d porque tiene subcategorías en uso en presupuestos.', 16, 1, @p_id_categoria);
        RETURN;
    END
 
    DELETE FROM dbo.subcategoria
    WHERE id_categoria = @p_id_categoria;
 
    DELETE FROM dbo.categoria
    WHERE id_categoria = @p_id_categoria;
END
GO

-- CONSULTAR
CREATE OR ALTER PROCEDURE sp_consultar_categoria
	@p_id_categoria int
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
CREATE OR ALTER PROCEDURE sp_listar_categorias
    @p_id_usuario int,
    @p_tipo_categoria varchar(20)
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM dbo.usuario WHERE usuario_id = @p_id_usuario)
    BEGIN
        RAISERROR('El usuario con ID %d no existe.', 16, 1, @p_id_usuario);
        RETURN;
    END
 
    SELECT
        c.id_categoria,
        c.nombre AS nombre_categoria,
        c.descripcion,
        c.tipo_categoria,
        p.usuario_id
    FROM dbo.categoria c
    INNER JOIN dbo.subcategoria sb
        ON c.id_categoria = sb.id_categoria
    INNER JOIN dbo.presupuesto_detalle pd
        ON sb.id_subcategoria = pd.id_subcategoria
    INNER JOIN dbo.presupuesto p
        ON pd.id_presupuesto = p.presupuesto_id
    WHERE p.usuario_id    = @p_id_usuario
      AND c.tipo_categoria = @p_tipo_categoria 
    ORDER BY c.nombre;
END
GO

CREATE OR ALTER PROCEDURE sp_listar_categorias_general
    @p_tipo_categoria varchar(20)
AS
BEGIN
    SELECT
        c.id_categoria,
        c.nombre AS nombre_categoria,
        c.descripcion,
        c.tipo_categoria
    FROM dbo.categoria c
    WHERE c.tipo_categoria = @p_tipo_categoria
    ORDER BY c.nombre;
END
GO

-- TRIGGER
CREATE OR ALTER TRIGGER trg_subcategoria_defecto_por_categoria
ON categoria
AFTER INSERT
AS
BEGIN
 
    INSERT INTO subcategoria (id_categoria, nombre, descripcion, subcategoria_por_defecto,
        es_activo, creado_por, modificado_por, creado_en, modificado_en)

    SELECT
        i.id_categoria,
        'General','Subcategoría general creada automáticamente para ' + i.nombre,
        1, 
        1, 
        i.creado_por, 
        i.creado_por, 
        GETDATE(), 
        GETDATE()
    FROM inserted i;
    END
GO