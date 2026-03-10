USE sistema_bancario;
GO

-- INSERTAR
CREATE PROCEDURE sp_insertar_transaccion
    @p_id_presupuesto_detalle INT,
    @p_monto DECIMAL(12,2),
    @p_metodo_pago VARCHAR(300),
    @p_tipo_transaccion VARCHAR(300),
    @p_fecha DATETIME,
    @p_descripcion VARCHAR(300),
    @p_anio smallint,
    @p_mes tinyint,
    @p_creado_por INT
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM presupuesto_detalle WHERE id = @p_id_presupuesto_detalle)
    BEGIN
        RAISERROR('El detalle de presupuesto no existe.',16,1);
        RETURN;
    END

    IF @p_monto < 0
    BEGIN
        RAISERROR('El monto no puede ser negativo.',16,1);
        RETURN;
    END

    IF @p_mes < 1 OR @p_mes > 12
    BEGIN
        RAISERROR('El mes debe estar entre 1 y 12.',16,1);
        RETURN;
    END

    IF @p_metodo_pago NOT IN ('efectivo','tarjeta debito','tarjeta credito','transferencia')
    BEGIN
        RAISERROR('Método de pago inválido.',16,1);
        RETURN;
    END

    INSERT INTO transaccion
        (id_presupuesto_detalle, monto , metodo_pago, tipo_transaccion, fecha_hora_registro, descripcion, anio_transaccion, mes_transaccion, creado_por, modificado_por, creado_en, modificado_en)
    VALUES
        (@p_id_presupuesto_detalle, @p_monto, @p_metodo_pago, @p_tipo_transaccion, @p_fecha, @p_descripcion, @p_anio, @p_mes, @p_creado_por, @p_creado_por, GETDATE(), GETDATE());
END
GO

-- ACTUZALIZAR
CREATE PROCEDURE sp_actualizar_transaccion
    @p_id_transaccion INT,
    @p_monto DECIMAL(12,2),
    @p_metodo_pago VARCHAR(300),
    @p_tipo_transaccion VARCHAR(300),
    @p_fecha DATETIME,
    @p_descripcion VARCHAR(300),
    @p_anio INT,
    @p_mes INT,
    @p_modificado_por INT
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM transaccion WHERE id_transaccion = @p_id_transaccion)
    BEGIN
        RAISERROR('La transacción no existe.',16,1);
        RETURN;
    END

    IF @p_monto < 0
    BEGIN
        RAISERROR('El monto no puede ser negativo.',16,1);
        RETURN;
    END

    IF @p_mes < 1 OR @p_mes > 12
    BEGIN
        RAISERROR('El mes debe estar entre 1 y 12.',16,1);
        RETURN;
    END

    IF @p_metodo_pago NOT IN ('efectivo','tarjeta debito','tarjeta credito','transferencia')
    BEGIN
        RAISERROR('Método de pago inválido.',16,1);
        RETURN;
    END
    UPDATE transaccion
    SET monto = @p_monto,
        metodo_pago = @p_metodo_pago,
        tipo_transaccion = @p_tipo_transaccion,
        fecha_hora_registro = @p_fecha,
        descripcion = @p_descripcion,
        anio_transaccion = @p_anio,
        mes_transaccion = @p_mes,
        modificado_por = @p_modificado_por,
        modificado_en = GETDATE()
    WHERE id_transaccion = @p_id_transaccion;
END
GO

-- ELIMINAR
CREATE PROCEDURE sp_eliminar_transaccion
    @p_id_transaccion INT
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM transaccion WHERE id_transaccion = @p_id_transaccion)
    BEGIN
        RAISERROR('La transacción no existe.',16,1);
        RETURN;
    END

    DELETE FROM transaccion WHERE id_transaccion = @p_id_transaccion;
    END
GO

-- CONSULTAR
CREATE PROCEDURE sp_consultar_transaccion
    @p_id_transaccion INT
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM transaccion WHERE id_transaccion = @p_id_transaccion)
    BEGIN
        RAISERROR('La transacción no existe.',16,1);
        RETURN;
    END

    SELECT 
        t.id_transaccion,
        t.id_presupuesto_detalle,
        t.monto,
        t.metodo_pago,
        t.fecha_hora_registro,
        t.descripcion,
        t.anio_transaccion,
        t.mes_transaccion,
        t.creado_por,
        t.modificado_por,
        t.creado_en,
        t.modificado_en,
        pd.id_presupuesto,
        s.id_subcategoria,
        s.nombre AS subcategoria_nombre,
        c.id_categoria,
        c.nombre AS categoria_nombre
    FROM transaccion t
    INNER JOIN presupuesto_detalle pd 
        ON t.id_presupuesto_detalle = pd.id
    INNER JOIN subcategoria s 
        ON pd.id_subcategoria = s.id_subcategoria
    INNER JOIN categoria c 
        ON s.id_categoria = c.id_categoria
    WHERE t.id_transaccion = @p_id_transaccion;
END
GO

--- LISTAR
CREATE PROCEDURE sp_listar_transacciones_presupuesto
    @p_id_presupuesto INT,
    @p_anio smallint,
    @p_mes tinyint,
    @p_tipo VARCHAR(300)
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM presupuesto WHERE presupuesto_id = @p_id_presupuesto)
    BEGIN
        RAISERROR('El presupuesto no existe.',16,1);
        RETURN;
    END

    SELECT 
        t.id_transaccion,
        t.monto,
        t.tipo_transaccion,
        t.tipo_transaccion,
        t.fecha_hora_registro,
        t,
        t.anio_transaccion,
        t.mes_transaccion,
        pd.id_subcategoria,
        s.nombre AS subcategoria_nombre,
        c.id_categoria,
        c.nombre AS categoria_nombre
    FROM transaccion t
    INNER JOIN presupuesto_detalle pd 
        ON t.id_presupuesto_detalle = pd.id
    INNER JOIN subcategoria s 
        ON pd.id_subcategoria = s.id_subcategoria
    INNER JOIN categoria c 
        ON s.id_categoria = c.id_categoria
    WHERE pd.id_presupuesto = @p_id_presupuesto
END
GO