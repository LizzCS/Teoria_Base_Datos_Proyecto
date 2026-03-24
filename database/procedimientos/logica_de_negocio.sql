USE sistema_bancario;
GO

-- CREAR PRESUPUESTO COMPLETO
CREATE OR ALTER PROCEDURE sp_crear_presupuesto_completo
    @p_id_usuario int,
    @p_nombre varchar(300),
    @p_descripcion varchar(300),
    @p_anio_inicio smallint,
    @p_anio_fin smallint,
    @p_mes_inicio tinyint,
    @p_mes_fin tinyint,
    @p_lista_subcategoria_json nvarchar(max),
    @p_creado_por int
AS
BEGIN
BEGIN TRY
    BEGIN TRANSACTION

    DECLARE @id_presupuesto int

    -- Insertar directamente en lugar de llamar el SP
    INSERT INTO dbo.presupuesto 
        (usuario_id, estado_presupuesto, nombre_descriptivo, descripcion, 
         anio_inicio, mes_inicio, anio_fin, mes_fin, 
         fecha_hora_creacion, creado_por, modificado_por, creado_en)
    VALUES 
        (@p_id_usuario, 1, @p_nombre, @p_descripcion,
         @p_anio_inicio, @p_mes_inicio, @p_anio_fin, @p_mes_fin,
         GETDATE(), @p_id_usuario, @p_id_usuario, GETDATE())

    SET @id_presupuesto = SCOPE_IDENTITY()

    INSERT INTO presupuesto_detalle (id_presupuesto, id_subcategoria, monto_mensual, creado_por, creado_en)
    SELECT 
        @id_presupuesto,
        id_subcategoria,
        monto_mensual,
        @p_creado_por,
        GETDATE()
    FROM OPENJSON(@p_lista_subcategoria_json)
    WITH (
        id_subcategoria int,
        monto_mensual   decimal(12,2)
    )

    COMMIT TRANSACTION
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION
        THROW
    END CATCH
    END
GO
-- REGISTRAR TRANSACCION
CREATE OR ALTER PROCEDURE sp_registrar_transaccion_completa
    @p_id_usuario int,
    @p_id_presupuesto int,
    @p_anio smallint,
    @p_mes tinyint,
    @p_id_subcategoria int,
    @p_tipo varchar(300),
    @p_descripcion varchar(300),
    @p_monto decimal(12,2),
    @p_fecha datetime,
    @p_metodo_pago varchar(300),
    @p_creado_por int
    AS
    BEGIN

    IF (@p_mes NOT BETWEEN 1 AND 12)
    BEGIN
        RAISERROR ('El mes debe estar entre 1 y 12.',16,1)
        RETURN
    END

    IF NOT EXISTS ( SELECT 1 FROM usuario WHERE usuario_id = @p_id_usuario)
    BEGIN
        RAISERROR ('El usuario no existe.',16,1)
        RETURN
    END

    IF NOT EXISTS (SELECT 1 FROM presupuesto WHERE presupuesto_id = @p_id_presupuesto)
    BEGIN
        RAISERROR ('El presupuesto no existe.',16,1)
        RETURN
    END

    IF NOT EXISTS (
        SELECT 1
        FROM presupuesto
        WHERE presupuesto_id = @p_id_presupuesto
            AND @p_anio BETWEEN anio_inicio 
            AND anio_fin
    )
    BEGIN
        RAISERROR ('El anio no esta dentro del periodo del presupuesto.',16,1)
        RETURN
    END

    DECLARE @v_id_presupuesto_detalle int

    SELECT @v_id_presupuesto_detalle = pd.id
    FROM presupuesto_detalle pd
    WHERE pd.id_presupuesto = @p_id_presupuesto
        AND pd.id_subcategoria = @p_id_subcategoria

    IF @v_id_presupuesto_detalle IS NULL
    BEGIN
        RAISERROR ('La subcategoria no pertenece al presupuesto.',16,1)
        RETURN
    END

    EXEC dbo.sp_insertar_transaccion
        @p_id_presupuesto_detalle = @v_id_presupuesto_detalle,
        @p_monto = @p_monto,
        @p_metodo_pago = @p_metodo_pago,
        @p_tipo_transaccion = @p_tipo,
        @p_fecha = @p_fecha,
        @p_descripcion = @p_descripcion,
        @p_anio = @p_anio,
        @p_mes = @p_mes,
        @p_creado_por = @p_creado_por;

    END
GO

--- PROCESAR OBLIGACIONE
CREATE OR ALTER PROCEDURE sp_procesar_obligaciones_mes
    @p_id_usuario int,
    @p_anio smallint,
    @p_mes tinyint,
    @p_id_presupuesto int
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM usuario WHERE usuario_id = @p_id_usuario)
    BEGIN
        RAISERROR('No existe el usuario', 16, 1)
        RETURN
    END

    SELECT 
        o.id_obligacion,
        o.nombre,
        o.monto_fijo_mensual,
        o.dia_del_mes,
        o.fecha_finalizacion,
        CASE 
            WHEN YEAR(o.fecha_finalizacion) = @p_anio 
             AND MONTH(o.fecha_finalizacion) = @p_mes
            THEN 'Se vence este mes.'
            ELSE 'Vigente'
        END AS alerta
    FROM obligacion_fija o
    INNER JOIN subcategoria sc 
        ON sc.id_subcategoria = o.id_subcategoria
    INNER JOIN presupuesto_detalle pd 
        ON pd.id_subcategoria = sc.id_subcategoria
    INNER JOIN presupuesto p 
        ON p.presupuesto_id = pd.id_presupuesto
    WHERE o.esta_vigente = 1
      AND p.usuario_id = @p_id_usuario
      AND p.presupuesto_id = @p_id_presupuesto
END
GO

-- CALCULAR BALANCE
CREATE OR ALTER PROCEDURE sp_calcular_balance_mensual
	@p_id_usuario int,
	@p_id_presupuesto int,
	@p_anio smallint,
	@p_mes tinyint,
	@p_total_ingresos decimal(12,2) output,
	@p_total_gastos decimal (12,2) output,
	@p_total_ahorros decimal (12,2) output,
	@p_balance_final decimal (12,2) output
	AS
	BEGIN

    IF NOT EXISTS (SELECT 1 FROM usuario WHERE usuario_id = @p_id_usuario)
    BEGIN
        RAISERROR('Usuario no existe.',16,1)
        RETURN
    END
    
    IF NOT EXISTS (SELECT 1 FROM presupuesto WHERE presupuesto_id = @p_id_presupuesto)
    BEGIN
        RAISERROR('Presupuesto no existe.',16,1)
        RETURN
    END

    IF (@p_mes NOT BETWEEN 1 AND 12)
    BEGIN
        RAISERROR ('El mes debe estar entre 1 y 12.',16,1)
        RETURN
    END
	
	SELECT 
	@p_total_ingresos = SUM (CASE WHEN t.tipo_transaccion = 'ingreso' THEN t.monto ELSE 0 END),
	@p_total_gastos = SUM (CASE WHEN t.tipo_transaccion = 'gasto' THEN t.monto ELSE 0 END),
	@p_total_ahorros = SUM (CASE WHEN t.tipo_transaccion = 'ahorro' THEN t.monto ELSE 0 END)
	FROM presupuesto p
	INNER JOIN presupuesto_detalle pd
		ON pd.id_presupuesto = p.presupuesto_id
	INNER JOIN transaccion t
		ON t.id_presupuesto_detalle = pd.id
	WHERE p.usuario_id = @p_id_usuario
		AND t.anio_transaccion = @p_anio
		AND t.mes_transaccion = @p_mes
	SET @p_balance_final = @p_total_ingresos - @p_total_gastos - @p_total_ahorros;
	END
GO


-- CALCULAR MONTO EJECUTADO 
CREATE OR ALTER PROCEDURE sp_calcular_monto_ejecutado
	@p_id_subcategoria int,
	@p_id_presupuesto int,
	@p_anio smallint,
	@p_mes tinyint,
	@p_monto_ejecutado decimal(12,2) output
	AS
	BEGIN

    IF NOT EXISTS (SELECT 1 FROM subcategoria WHERE id_subcategoria = @p_id_subcategoria)
    BEGIN
        RAISERROR ('Subcategoria no existe.',16,1)
        RETURN
	END

    IF NOT EXISTS (SELECT 1 FROM presupuesto WHERE presupuesto_id = @p_id_presupuesto)
    BEGIN
        RAISERROR('Presupuesto no existe.',16,1)
        RETURN
    END

    IF (@p_mes NOT BETWEEN 1 AND 12)
    BEGIN
        RAISERROR ('El mes debe estar entre 1 y 12.',16,1)
        RETURN
    END

	SELECT
    @p_monto_ejecutado = SUM(t.monto)
	FROM subcategoria s
	INNER JOIN presupuesto_detalle pd
		ON pd.id_subcategoria = s.id_subcategoria
	INNER JOIN transaccion t
		ON t.id_presupuesto_detalle = pd.id
	WHERE s.id_subcategoria = @p_id_subcategoria
			AND t.anio_transaccion = @p_anio
			AND t.mes_transaccion = @p_mes
			AND pd.id_presupuesto = @p_id_presupuesto
	END
GO

-- CALCULAR PORCENTAJE
CREATE OR ALTER PROCEDURE sp_calcular_porcentaje_ejecucion_mes
    @p_id_subcategoria int,
    @p_id_presupuesto int,
    @p_anio smallint,
    @p_mes tinyint,
    @p_porcentaje_ejecutado decimal(12,2) output
AS
BEGIN

    IF NOT EXISTS (SELECT 1 FROM subcategoria WHERE id_subcategoria = @p_id_subcategoria)
    BEGIN
        RAISERROR ('Subcategoria no existe.',16,1)
        RETURN
	END

    IF NOT EXISTS (SELECT 1 FROM presupuesto WHERE presupuesto_id = @p_id_presupuesto)
    BEGIN
        RAISERROR('Presupuesto no existe.',16,1)
        RETURN
    END

    IF (@p_mes NOT BETWEEN 1 AND 12)
    BEGIN
        RAISERROR ('El mes debe estar entre 1 y 12.',16,1)
        RETURN
    END

    DECLARE @monto_ejecutado DECIMAL(12,2);
    DECLARE @monto_presupuestado DECIMAL(12,2);

    SELECT @monto_ejecutado = dbo.fn_calcular_monto_ejecutado(@p_id_subcategoria, @p_anio, @p_mes);

    SELECT @monto_presupuestado = pd.monto_mensual
    FROM presupuesto_detalle pd
    WHERE pd.id_presupuesto = @p_id_presupuesto
      AND pd.id_subcategoria = @p_id_subcategoria;

    IF (@monto_presupuestado IS NULL OR @monto_presupuestado = 0)
        SET @p_porcentaje_ejecutado = 0;
    ELSE
        SET @p_porcentaje_ejecutado = (@monto_ejecutado / @monto_presupuestado) * 100;
	END
GO

-- CERRAR PRESUPUESTO
CREATE OR ALTER PROCEDURE sp_cerrar_presupuesto
	@p_id_presupuesto int,
	@p_mpodificado_por int
	AS
	BEGIN
	IF NOT EXISTS (SELECT 1 FROM presupuesto p WHERE p.presupuesto_id = @p_id_presupuesto)
    BEGIN
        RAISERROR('El presupuesto no existe.',16,1)
        RETURN
	END
    IF NOT EXISTS (
        SELECT 1 FROM presupuesto p  
        WHERE p.presupuesto_id = @p_id_presupuesto
          AND (
                p.anio_fin < YEAR(GETDATE())
                OR (p.anio_fin = YEAR(GETDATE()) AND p.mes_fin < MONTH(GETDATE()))
              )
    )
    BEGIN
        RAISERROR('El presupuesto aún no ha vencido.',16,1)
        RETURN
    END

	UPDATE presupuesto
	SET estado_presupuesto = 2,
		modificado_por = @p_mpodificado_por,
		modificado_en = GETDATE()
    WHERE presupuesto_id = @p_id_presupuesto;
	END
GO

-- RESUMEN CATEGORIA
CREATE OR ALTER PROCEDURE sp_obtener_resumen_categoria_mes
    @p_id_categoria int,
    @p_id_presupuesto int,
    @p_anio smallint,
    @p_mes tinyint,
    @p_monto_presupuestado decimal(12,2) output,
    @p_monto_ejecutado decimal(12,2) output,
    @p_porcentaje decimal(12,2) output
AS
BEGIN

    IF NOT EXISTS (SELECT 1 FROM categoria WHERE id_categoria = @p_id_categoria)
    BEGIN
        RAISERROR ('Categoria no existe.',16,1)
        RETURN
	END

    IF NOT EXISTS (SELECT 1 FROM presupuesto WHERE presupuesto_id = @p_id_presupuesto)
    BEGIN
        RAISERROR('Presupuesto no existe.',16,1)
        RETURN
    END

    IF (@p_mes NOT BETWEEN 1 AND 12)
    BEGIN
        RAISERROR ('El mes debe estar entre 1 y 12.',16,1)
        RETURN
    END

    SELECT @p_monto_presupuestado = ISNULL(SUM(pd.monto_mensual),0)
    FROM subcategoria s
    INNER JOIN presupuesto_detalle pd
        ON pd.id_subcategoria = s.id_subcategoria
    WHERE s.id_categoria = @p_id_categoria
        AND pd.id_presupuesto = @p_id_presupuesto

    SELECT @p_monto_ejecutado = ISNULL(SUM(t.monto),0)
    FROM subcategoria s
    INNER JOIN presupuesto_detalle pd
        ON pd.id_subcategoria = s.id_subcategoria
    INNER JOIN transaccion t
        ON t.id_presupuesto_detalle = pd.id
    WHERE s.id_categoria = @p_id_categoria
        AND pd.id_presupuesto = @p_id_presupuesto
        AND t.anio_transaccion = @p_anio
        AND t.mes_transaccion = @p_mes

    IF @p_monto_presupuestado = 0
        SET @p_porcentaje = 0
    ELSE
        SET @p_porcentaje = (@p_monto_ejecutado / @p_monto_presupuestado) * 100
    END
GO