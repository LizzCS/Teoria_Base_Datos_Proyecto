USE sistema_bancario;
GO

-- INSERTAR
CREATE PROCEDURE sp_insertar_obligacion
    @p_id_usuario INT,
    @p_id_subcategoria INT,
    @p_nombre VARCHAR(300),
    @p_descripcion VARCHAR(400),
    @p_monto DECIMAL(12,2),
    @p_dia_vencimiento INT,
    @p_fecha_inicio DATE,
    @p_fecha_fin DATE,
    @p_creado_por INT
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM usuario WHERE usuario_id = @p_id_usuario)
    BEGIN
        RAISERROR('La subcategoría no existe.',16,1);
        RETURN;
    END
    IF NOT EXISTS (SELECT 1 FROM subcategoria WHERE id_subcategoria = @p_id_subcategoria)
    BEGIN
        RAISERROR('La subcategoría no existe.',16,1);
        RETURN;
    END

    IF @p_monto < 0
    BEGIN
        RAISERROR('El monto no puede ser negativo.',16,1);
        RETURN;
    END

    IF @p_dia_vencimiento < 1 OR @p_dia_vencimiento > 28
    BEGIN
        RAISERROR('El día de vencimiento debe estar entre 1 y 28.',16,1);
        RETURN;
    END

    IF  @p_fecha_fin <= @p_fecha_inicio
    BEGIN
        RAISERROR('La fecha final debe ser posterior a la fecha de inicio.',16,1);
        RETURN;
    END

    INSERT INTO obligacion_fija
        (id_subcategoria, nombre, monto_fijo_mensual, dia_del_mes, fecha_inicio, fecha_finalizacion, esta_vigente, creado_por, modificado_por, creado_en, modificado_en)
    VALUES
        (@p_id_subcategoria, @p_nombre, @p_monto, @p_dia_vencimiento, @p_fecha_inicio, @p_fecha_fin, 1, @p_creado_por, @p_creado_por, GETDATE(), GETDATE());
END
GO

-- ACTUALIZAR
CREATE PROCEDURE sp_actualizar_obligacion
    @p_id_obligacion INT,
    @p_nombre VARCHAR(300),
    @p_descripcion VARCHAR(400),
    @p_monto DECIMAL(12,2),
    @p_dia_vencimiento tinyint,
    @p_fecha_fin DATE,
    @p_es_vigente BIT,
    @p_modificado_por INT
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM obligacion_fija WHERE id_obligacion = @p_id_obligacion)
    BEGIN
        RAISERROR('La obligación no existe.',16,1);
        RETURN;
    END

    IF @p_monto < 0
    BEGIN
        RAISERROR('El monto no puede ser negativo.',16,1);
        RETURN;
    END

    IF @p_dia_vencimiento < 1 OR @p_dia_vencimiento > 28
    BEGIN
        RAISERROR('El día de vencimiento debe estar entre 1 y 28.',16,1);
        RETURN;
    END

    IF @p_fecha_fin IS NOT NULL 
       AND @p_fecha_fin <= (SELECT fecha_inicio FROM obligacion_fija WHERE id_obligacion = @p_id_obligacion)
    BEGIN
        RAISERROR('La fecha final debe ser posterior a la fecha de inicio.',16,1);
        RETURN;
    END

    UPDATE obligacion_fija
    SET nombre = @p_nombre,
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
CREATE PROCEDURE sp_eliminar_obligacion
    @p_id_obligacion INT
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM obligacion_fija WHERE id_obligacion = @p_id_obligacion)
    BEGIN
        RAISERROR('La obligación no existe.',16,1);
        RETURN;
    END

    UPDATE obligacion_fija
    SET esta_vigente = 0,
        modificado_en = GETDATE()
    WHERE id_obligacion = @p_id_obligacion;
END
GO

-- CONSULTAR
CREATE PROCEDURE sp_consultar_obligacion
    @p_id_obligacion INT
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


--- LISTAR
CREATE PROCEDURE sp_listar_obligaciones_usuario
    @p_id_usuario INT,
    @p_es_vigente BIT
AS
BEGIN
    SELECT 
        o.id_obligacion,
        o.nombre,
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
    INNER JOIN subcategoria s 
        ON o.id_subcategoria = s.id_subcategoria
    INNER JOIN categoria c 
        ON s.id_categoria = c.id_categoria
    WHERE (@p_es_vigente IS NULL OR o.esta_vigente = @p_es_vigente)
    ORDER BY o.fecha_inicio;
END
GO
