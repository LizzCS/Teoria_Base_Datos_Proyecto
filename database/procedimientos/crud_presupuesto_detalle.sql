USE sistema_bancario;
GO

-- INSERTAR
CREATE PROCEDURE sp_insertar_presupuesto_detalle
    @p_id_presupuesto INT,
    @p_id_subcategoria INT,
    @p_monto_mensual NUMERIC(12,2),
    @p_observaciones VARCHAR(255),
    @p_creado_por INT
AS
BEGIN

    IF NOT EXISTS (
        SELECT 1
        FROM presupuesto
        WHERE presupuesto_id = @p_id_presupuesto
    )
    BEGIN
        RAISERROR('El presupuesto no existe.', 16, 1);
        RETURN;
    END

    IF NOT EXISTS (
        SELECT 1
        FROM subcategoria
        WHERE id_subcategoria = @p_id_subcategoria
    )
    BEGIN
        RAISERROR('La subcategoría no existe.', 16, 1);
        RETURN;
    END

    IF @p_monto_mensual < 0
    BEGIN
        RAISERROR('El monto mensual no puede ser negativo.', 16, 1);
        RETURN;
    END

    INSERT INTO presupuesto_detalle
        (id_presupuesto, id_subcategoria, monto_mensual, observaciones, creado_por, creado_en)
    VALUES
        (@p_id_presupuesto, @p_id_subcategoria, @p_monto_mensual, @p_observaciones, @p_creado_por, GETDATE());

END
GO

-- ACTUALIZAR
CREATE PROCEDURE sp_actualizar_presupuesto_detalle
    @p_id_detalle INT,
    @p_monto_mensual NUMERIC (12,2),
    @p_observaciones VARCHAR(255),
    @p_modificado_por INT
    AS
    BEGIN

    IF NOT EXISTS ( SELECT 1 FROM presupuesto_detalle WHERE id_presupuesto_detalle = @p_id_detalle)
        BEGIN
            RAISERROR('El presupuesto detalle no existe.', 16, 1);
            RETURN;
        END

    UPDATE presupuesto_detalle
    SET monto_mensual = @p_monto_mensual,
        observaciones = @p_observaciones,
        modificado_por = @p_modificado_por,
        modificado_en = GETDATE()
    WHERE id_presupuesto_detalle = @p_id_detalle
    END
GO


---- ELIMINAR
CREATE PROCEDURE sp_eliminar_presupuesto_detalle
    @p_id_detalle INT
    AS
    BEGIN
    IF NOT EXISTS ( SELECT 1 FROM presupuesto_detalle WHERE id_presupuesto_detalle = @p_id_detalle)
        BEGIN
            RAISERROR('El presupuesto detalle no existe.', 16, 1);
            RETURN;
    END
    DELETE FROM presupuesto_detalle
    WHERE id_presupuesto_detalle = @p_id_detalle 
  END
GO


-- CONSULTAR 
CREATE PROCEDURE sp_consultar_presupuesto_detalle
    @p_id_detalle INT
AS
BEGIN

    IF NOT EXISTS (
        SELECT 1
        FROM presupuesto_detalle
        WHERE id_presupuesto_detalle = @p_id_detalle
    )
    BEGIN
        RAISERROR('El detalle con ID %d no existe.',16,1,@p_id_detalle);
        RETURN;
    END

    SELECT 
        d.id_presupuesto_detalle,
        d.id_presupuesto,
        p.nombre_descriptivo AS presupuesto_nombre,
        d.id_subcategoria,
        s.nombre AS subcategoria_nombre,
        s.descripcion AS subcategoria_descripcion,
        c.id_categoria,
        c.nombre AS categoria_nombre,
        d.monto_mensual,
        d.observaciones,
        d.creado_por,
        d.modificado_por,
        d.creado_en,
        d.modificado_en
    FROM presupuesto_detalle d
    INNER JOIN subcategoria s ON d.id_subcategoria = s.id_subcategoria
    INNER JOIN categoria c ON s.id_categoria = c.id_categoria
    INNER JOIN presupuesto p ON d.id_presupuesto = p.presupuesto_id
    WHERE d.id_presupuesto_detalle = @p_id_detalle;

END
GO

---- LISTAR 
CREATE PROCEDURE sp_listar_detalles_presupuesto
    @p_id_presupuesto INT
AS
BEGIN

    IF NOT EXISTS (
        SELECT 1
        FROM presupuesto
        WHERE presupuesto_id = @p_id_presupuesto
    )
    BEGIN
        RAISERROR('El presupuesto con ID %d no existe.',16,1,@p_id_presupuesto);
        RETURN;
    END

    SELECT
        d.id_presupuesto_detalle,
        d.id_subcategoria,
        s.nombre AS subcategoria_nombre,
        s.descripcion AS subcategoria_descripcion,
        d.monto_mensual,
        d.observaciones,
        d.creado_por,
        d.modificado_por,
        d.creado_en,
        d.modificado_en
    FROM presupuesto_detalle d
    INNER JOIN subcategoria s ON d.id_subcategoria = s.id_subcategoria
    WHERE d.id_presupuesto = @p_id_presupuesto
END
GO
