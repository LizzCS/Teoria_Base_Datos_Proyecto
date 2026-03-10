USE sistema_bancario;
GO

-- INSERTAR
CREATE OR ALTER PROCEDURE sp_insertar_presupuesto
	@p_id_usuario INT,
	@p_nombre_descriptivo VARCHAR (255),
	@p_mes_inicio tinyint,
    @p_mes_fin tinyint,
	@p_anio_inicio smallint,
    @p_anio_fin smallint
	AS
	BEGIN

	IF NOT EXISTS (
        SELECT 1 
        FROM dbo.usuario 
        WHERE usuario_id = @p_id_usuario
    )
    BEGIN
        RAISERROR('El usuario no existe.',16,1);
        RETURN;
    END

    IF (@p_mes_inicio < 1 OR @p_mes_inicio > 12)
    BEGIN
        RAISERROR('El mes de inicio debe estar entre 1 y 12.',16,1);
        RETURN;
    END

    IF (@p_mes_fin < 1 OR @p_mes_fin > 12)
    BEGIN
        RAISERROR('El mes de fin debe estar entre 1 y 12.',16,1);
        RETURN;
    END

    IF (@p_anio_inicio > @p_anio_fin OR (@p_anio_inicio = @p_anio_fin AND @p_mes_inicio > @p_mes_fin))
    BEGIN
        RAISERROR('El periodo de inicio no puede ser mayor que el periodo final.',16,1);
        RETURN;
    END

    IF EXISTS (
        SELECT 1
        FROM dbo.presupuesto
        WHERE usuario_id = @p_id_usuario
        AND nombre_descriptivo = @p_nombre_descriptivo
    )
    BEGIN
        RAISERROR('Ya existe un presupuesto con ese nombre para el usuario.',16,1);
        RETURN;
    END

	INSERT INTO dbo.presupuesto (usuario_id, nombre_descriptivo, anio_inicio, mes_inicio, anio_fin, mes_fin, fecha_creacion, creado_por, modificado_por, creado_en)
		VALUES (@p_id_usuario, @p_nombre_descriptivo, @p_anio_inicio,@p_mes_inicio,@p_anio_fin, @p_mes_fin, GETDATE() ,@p_id_usuario, @p_id_usuario, GETDATE());
	END
GO


-- ACTUALIZAR
CREATE OR ALTER PROCEDURE sp_actualizar_presupuesto
	@p_id_presupuesto INT,
	@p_nombre VARCHAR (255),
	@p_anio_inicio smallint,
	@p_anio_fin smallint,
	@p_mes_inicio tinyint,
	@p_mes_fin tinyint,
	@p_modificado_por INT
	AS 
	BEGIN

	IF NOT EXISTS (SELECT 1 FROM dbo.presupuesto p WHERE p.presupuesto_id = @p_id_presupuesto)
		BEGIN
			RAISERROR('El presupuesto con ID %d no existe.', 16, 1, @p_id_presupuesto);
			RETURN;
		END

	UPDATE presupuesto
	SET nombre_descriptivo = @p_nombre,
		anio_inicio = @p_anio_inicio,
		anio_fin = @p_anio_fin,
		mes_inicio = @p_mes_inicio,
		mes_fin = @p_mes_fin,
		modificado_por = @p_modificado_por,
		modificado_en = getDATE();
	END
GO

-- ELIMINAR
CREATE OR ALTER PROCEDURE sp_eliminar_presupuesto
    @p_id_presupuesto INT
AS
BEGIN

    IF NOT EXISTS (SELECT 1 FROM dbo.presupuesto WHERE presupuesto_id = @p_id_presupuesto)
    BEGIN
        RAISERROR('El presupuesto con ID %d no existe.',16,1,@p_id_presupuesto);
        RETURN;
    END

    IF EXISTS (
        SELECT 1
        FROM presupuesto p
        INNER JOIN presupuesto_detalle dp
            ON p.presupuesto_id = dp.id_presupuesto
        INNER JOIN transaccion t
            ON t.id_presupuesto_detalle = dp.id_presupuesto_detalle
        WHERE p.presupuesto_id = @p_id_presupuesto
    )
    BEGIN
        RAISERROR('No se puede eliminar el presupuesto porque tiene transacciones asociadas.',16,1);
        RETURN;
    END

    DELETE FROM presupuesto_detalle
    WHERE id_presupuesto = @p_id_presupuesto;

    DELETE FROM presupuesto
    WHERE presupuesto_id = @p_id_presupuesto;

END
GO

-- CONSULTAR
CREATE OR ALTER PROCEDURE sp_consultar_presupuesto
    @p_id_presupuesto INT
AS
BEGIN
    IF NOT EXISTS (
        SELECT 1 
        FROM dbo.presupuesto 
        WHERE presupuesto_id = @p_id_presupuesto
    )
    BEGIN
        RAISERROR('El presupuesto con ID %d no existe.',16,1,@p_id_presupuesto);
        RETURN;
    END

    SELECT 
        presupuesto_id,
        usuario_id,
        nombre_descriptivo,
        anio_inicio,
        mes_inicio,
        anio_fin,
        mes_fin,
        total_ingresos,
        total_gastos,
        total_ahorros,
        creado_por,
        modificado_por,
        creado_en,
        modificado_en
    FROM dbo.presupuesto
    WHERE presupuesto_id = @p_id_presupuesto;
END
GO

-- LISTAR
CREATE OR ALTER PROCEDURE sp_listar_presupuestos_usuario_por_estado
    @p_id_usuario INT,
    @p_estado VARCHAR(20)
AS
BEGIN
    SELECT 
        p.presupuesto_id,
        p.nombre_descriptivo,
        p.anio_inicio,
        p.mes_inicio,
        p.anio_fin,
        p.mes_fin,
        CASE p.estado_presupuesto
            WHEN 1 THEN 'Activo'
            WHEN 2 THEN 'Cerrado'
            WHEN 3 THEN 'Borrador'
        END AS estado_presupuesto,
        p.creado_en
    FROM dbo.presupuesto p
    WHERE p.usuario_id = @p_id_usuario
END
GO
