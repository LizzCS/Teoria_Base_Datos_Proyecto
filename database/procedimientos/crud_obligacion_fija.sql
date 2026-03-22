USE sistema_bancario;
GO

-- INSERTAR
CREATE OR ALTER PROCEDURE sp_insertar_obligacion
    @p_id_subcategoria int,
    @p_nombre varchar(300),
    @p_descripcion varchar(400),
    @p_monto decimal(12,2),
    @p_dia_vencimiento int,
    @p_fecha_inicio date,
    @p_fecha_fin date,
    @p_creado_por int
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM subcategoria WHERE id_subcategoria = @p_id_subcategoria)
    BEGIN
        RAISERROR('La subcategoria no existe.',16,1);
        RETURN;
    END

    IF (@p_monto < 0)
    BEGIN
        RAISERROR('El monto no puede ser negativo.',16,1);
        RETURN;
    END

    IF  (@p_fecha_fin <= @p_fecha_inicio)
    BEGIN
        RAISERROR('La fecha final debe ser posterior a la fecha de inicio.',16,1);
        RETURN;
    END

    INSERT INTO obligacion_fija
        (id_subcategoria, nombre, descripcion, monto_fijo_mensual, dia_del_mes, fecha_inicio, fecha_finalizacion, esta_vigente, creado_por, modificado_por, creado_en, modificado_en)
    VALUES
        (@p_id_subcategoria, @p_nombre,@p_descripcion, @p_monto, @p_dia_vencimiento, @p_fecha_inicio, @p_fecha_fin, 1, @p_creado_por, @p_creado_por, GETDATE(), GETDATE());
END
GO

-- ACTUALIZAR
CREATE OR ALTER PROCEDURE sp_actualizar_obligacion
    @p_id_obligacion int,
    @p_nombre varchar(300),
    @p_descripcion varchar(400),
    @p_monto decimal(12,2),
    @p_dia_vencimiento tinyint,
    @p_fecha_fin date,
    @p_es_vigente bit,
    @p_modificado_por int
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM obligacion_fija WHERE id_obligacion = @p_id_obligacion)
    BEGIN
        RAISERROR('La obligacion no existe.',16,1);
        RETURN;
    END

    IF (@p_monto < 0)
    BEGIN
        RAISERROR('El monto no puede ser negativo.',16,1);
        RETURN;
    END

    IF  (@p_fecha_fin <= (SELECT fecha_inicio FROM obligacion_fija WHERE id_obligacion = @p_id_obligacion))
    BEGIN
        RAISERROR('La fecha final debe ser posterior a la fecha de inicio.',16,1);
        RETURN;
    END

    UPDATE obligacion_fija
    SET nombre = @p_nombre,
        descripcion = @p_descripcion,
        monto_fijo_mensual = @p_monto,
        dia_del_mes = @p_dia_vencimiento,
        fecha_finalizacion = @p_fecha_fin,
        esta_vigente = @p_es_vigente,
        modificado_por = @p_modificado_por,
        modificado_en = GETDATE()
    WHERE id_obligacion = @p_id_obligacion;
END
GO

-- ELIMINAR
CREATE OR ALTER PROCEDURE sp_eliminar_obligacion
    @p_id_obligacion int
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM obligacion_fija WHERE id_obligacion = @p_id_obligacion)
    BEGIN
        RAISERROR('La obligacion no existe.',16,1);
        RETURN;
    END

    UPDATE obligacion_fija
    SET esta_vigente = 0,
        modificado_en = GETDATE()
    WHERE id_obligacion = @p_id_obligacion;
END
GO

-- CONSULTAR
CREATE OR ALTER PROCEDURE sp_consultar_obligacion
    @p_id_obligacion int
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM obligacion_fija WHERE id_obligacion = @p_id_obligacion)
    BEGIN
        RAISERROR('La obligación no existe.',16,1);
        RETURN;
    END

    SELECT 
        o.id_obligacion,
        o.nombre,
        o.monto_fijo_mensual,
        o.dia_del_mes,
        o.fecha_inicio,
        o.fecha_finalizacion,
        o.esta_vigente,
        o.creado_por,
        o.modificado_por,
        o.creado_en,
        o.modificado_en,
        s.id_subcategoria,
        s.nombre AS subcategoria_nombre,
        c.id_categoria,
        c.nombre AS categoria_nombre
    FROM obligacion_fija o
    INNER JOIN subcategoria s 
        ON o.id_subcategoria = s.id_subcategoria
    INNER JOIN categoria c 
        ON s.id_categoria = c.id_categoria
    WHERE o.id_obligacion = @p_id_obligacion;
END
GO

-- LISTAR
CREATE OR ALTER PROCEDURE sp_listar_obligaciones_usuario
    @p_id_usuario int,
    @p_es_vigente bit
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM usuario WHERE usuario_id = @p_id_usuario)
    BEGIN
        RAISERROR('El usuario no existe.', 16, 1);
        RETURN;
    END

    SELECT 
        o.id_obligacion,
        o.nombre,
        o.descripcion,
        o.monto_fijo_mensual,
        o.dia_del_mes,
        o.fecha_inicio,
        o.fecha_finalizacion,
        CASE o.esta_vigente WHEN 1 THEN 'Activo' ELSE 'Inactivo' END AS estado,
        s.id_subcategoria,
        s.nombre AS subcategoria_nombre,
        c.id_categoria,
        c.nombre AS categoria_nombre,
        o.creado_en
    FROM obligacion_fija o
    LEFT JOIN obligacion_transaccion ot
        ON ot.id_obligacion = o.id_obligacion
    LEFT JOIN transaccion t
        ON t.id_transaccion = ot.id_transaccion
    LEFT JOIN presupuesto_detalle pd
        ON pd.id = t.id_presupuesto_detalle
    LEFT JOIN presupuesto p
        ON p.presupuesto_id = pd.id_presupuesto
        AND p.usuario_id = @p_id_usuario
    INNER JOIN subcategoria s 
        ON o.id_subcategoria = s.id_subcategoria
    INNER JOIN categoria c 
        ON s.id_categoria = c.id_categoria
    WHERE o.esta_vigente = @p_es_vigente
    ORDER BY o.fecha_inicio;
END
GO